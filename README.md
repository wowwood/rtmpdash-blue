# RTMPDash - blue

This project is heavily based on the similarly named project by [Laura](https://zotan.pw/). Thanks for helping me deploy it properly!

You can find the original [here](https://git.ztn.sh/zotan/rtmpdash)

[Production instance](https://stream.whatthe.blue)

Dependancies:
dotnet-sdk-7.0
dotnet-sdk-8.0

# Stats

To get the stats you have to install vnstat (to monitor the traffic) and vnstati (to make the images). Run stats.sh at whatever interval you'd like (mine is every 15 mins)

<details>
  <summary># nginx Config</summary>


```angular2html
load_module /usr/lib/nginx/modules/ngx_rtmp_module.so; # load our rtmp module
worker_processes 1;

user www-data;

rtmp_auto_push on;
rtmp_auto_push_reconnect 1s;
rtmp_socket_dir /var/sock;

events {
    worker_connections 12800;
}

rtmp {
 log_format rtmp '[$time_local] $command -> $app with key "$name" - $bytes_received bytes received ($session_readable_time)';
 server {
  listen 1935;
#  listen [::]:1935;

  access_log /var/log/nginx/rtmp_access.log rtmp;

  max_message 32M;
  ping 1m;
  ping_timeout 10s;
  drop_idle_publisher 10s;
  notify_method get;

  application ingress {
   live on;

   allow play 127.0.0.1;
   allow play 193.3.165.48;
   deny play all;

   notify_relay_redirect on;

   on_publish http://127.0.0.1:60001/api/authenticate;

   hls on;
   hls_path /tmp/hls/;
   hls_continuous off;
   hls_fragment 1s;
   hls_fragment_naming system;
   hls_playlist_length 10s;
   hls_allow_client_cache enabled;
   hls_keys on;
   hls_key_path /tmp/keys/;
   hls_key_url https://cdn.stream.whatthe.blue/keys/;
   hls_fragments_per_key 10;

   dash on;
   dash_path /tmp/dash;
   dash_fragment 1s;
   dash_playlist_length 10s;
  }
 }
}

http {
  types_hash_max_size 4096;
  server_names_hash_bucket_size 128;
  include mime.types;
  default_type application/octet-stream;

  ssl_session_timeout 10m;

  #resolver 213.133.100.100;
  ssl_stapling on;
  ssl_stapling_verify on;

  sendfile on;
  tcp_nodelay on;
  tcp_nopush on;
  keepalive_timeout 120;
  client_max_body_size 100G;
  gzip on;
  gzip_types application/atom+xml application/geo+json application/javascript application/x-javascript application/json application/ld+json application/manifest+json application/rdf+xml application/rss+xml application/vnd.ms-fontobject application/wasm application/x-web-app-manifest+json application/xhtml+xml application/xml font/eot font/otf font/ttf image/bmp image/svg+xml text/cache-manifest text/calendar text/css text/javascript text/markdown text/plain text/xml text/vcard text/vnd.rim.location.xloc text/vtt text/x-component text/x-cross-domain-policy;
  underscores_in_headers on;

  access_log off;

  types {
   application/vnd.apple.mpegurl m3u8;
   video/mp2t ts;
  }

  server {
    listen 127.0.0.1:8083;
    listen [::1]:8083;
    rtmp_stat all;
  }

  server {
    server_name stats.stream.whatthe.blue;

    root /var/www/stats;


    listen 443 ssl; # managed by Certbot
    ssl_certificate /etc/letsencrypt/live/stream.whatthe.blue/fullchain.pem; # managed by Certbot
    ssl_certificate_key /etc/letsencrypt/live/stream.whatthe.blue/privkey.pem; # managed by Certbot
    include /etc/letsencrypt/options-ssl-nginx.conf; # managed by Certbot
    ssl_dhparam /etc/letsencrypt/ssl-dhparams.pem; # managed by Certbot


}


  server {
    server_name live.on.stream.whatthe.blue;

    root /var/www/liveon;



    listen 443 ssl; # managed by Certbot
    ssl_certificate /etc/letsencrypt/live/stream.whatthe.blue/fullchain.pem; # managed by Certbot
    ssl_certificate_key /etc/letsencrypt/live/stream.whatthe.blue/privkey.pem; # managed by Certbot
    include /etc/letsencrypt/options-ssl-nginx.conf; # managed by Certbot
    ssl_dhparam /etc/letsencrypt/ssl-dhparams.pem; # managed by Certbot

}

  server {
    server_name player.stream.whatthe.blue;

    listen [::]:443 ssl;
    listen 443 ssl;
    ssl_certificate /etc/letsencrypt/live/stream.whatthe.blue/fullchain.pem; # managed by Certbot
    ssl_certificate_key /etc/letsencrypt/live/stream.whatthe.blue/privkey.pem; # managed by Certbot
    include /etc/letsencrypt/options-ssl-nginx.conf;
    ssl_dhparam /etc/letsencrypt/ssl-dhparams.pem;

    root /var/www/player;

    location = /favicon.ico {
      #ignore rewrite rules
    }

    location ^~ /lib/ {
      #ignore rewrite rules
    }

    location = /watch.html {
      #ignore rewrite rules
    }

    location / {
      rewrite / /watch.html last;
    }



}



  server {
    server_name cdn.stream.whatthe.blue;

    listen [::]:443 ssl;
    listen 443 ssl;
    ssl_certificate /etc/letsencrypt/live/stream.whatthe.blue/fullchain.pem; # managed by Certbot
    ssl_certificate_key /etc/letsencrypt/live/stream.whatthe.blue/privkey.pem; # managed by Certbot
    include /etc/letsencrypt/options-ssl-nginx.conf;
    ssl_dhparam /etc/letsencrypt/ssl-dhparams.pem;

    location /hls/ {
      alias /tmp/hls/;

      add_header 'Cache-Control' 'public,immutable';
      add_header 'Access-Control-Allow-Origin' '*' always;
      add_header 'Access-Control-Expose-Headers' 'Content-Length';

      # allow CORS preflight requests
      if ($request_method = 'OPTIONS') {
        add_header 'Access-Control-Allow-Origin' '*';
        add_header 'Access-Control-Max-Age' 1728000;
        add_header 'Content-Type' 'text/plain charset=UTF-8';
        add_header 'Content-Length' 0;
        return 204;
      }
    }

    location /dash/ {
      alias /tmp/dash/;
      add_header 'Cache-Control' 'public,immutable';
      add_header 'Access-Control-Allow-Origin' '*' always;
      add_header 'Access-Control-Expose-Headers' 'Content-Length';
    }

    location /keys/ {
      alias /tmp/keys/;
      add_header 'Cache-Control' 'public,immutable';
      add_header 'Access-Control-Allow-Origin' '*' always;
      add_header 'Access-Control-Expose-Headers' 'Content-Length';
    }


}



  server {
    server_name stream.whatthe.blue;

    listen [::]:443 ssl;
    listen 443 ssl;
    ssl_certificate /etc/letsencrypt/live/stream.whatthe.blue/fullchain.pem; # managed by Certbot
    ssl_certificate_key /etc/letsencrypt/live/stream.whatthe.blue/privkey.pem; # managed by Certbot
    include /etc/letsencrypt/options-ssl-nginx.conf;
    ssl_dhparam /etc/letsencrypt/ssl-dhparams.pem;


    location / {
      proxy_pass      http://127.0.0.1:60001;

      proxy_http_version 1.1;

      proxy_redirect off;
      proxy_set_header Host $host;
      proxy_set_header X-Real-IP $remote_addr;
      proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
      proxy_set_header X-Forwarded-Host $server_name;
    }



}

  server {
    listen 80;
    listen [::]:80;
    server_name stream.whatthe.blue;

    if ($host = stream.whatthe.blue) {
        return 301 https://$host$request_uri;
    }

    return 404;






}


  server {
    if ($host = stats.stream.whatthe.blue) {
        return 301 https://$host$request_uri;
    } # managed by Certbot


    server_name stats.stream.whatthe.blue;
    listen 80;
    return 404; # managed by Certbot


}





  server {
    if ($host = live.on.stream.whatthe.blue) {
        return 301 https://$host$request_uri;
    } # managed by Certbot


    server_name live.on.stream.whatthe.blue;
    listen 80;
    return 404; # managed by Certbot


}}
```

</details>


<details>
  <summary># system.d unit</summary>

```
[Unit]
Description=RTMPDash
Wants=network-online.target
After=network-online.target

[Service]
User=rtmpdash
Group=rtmpdash
WorkingDirectory=/opt/rtmpdash
Environment=ASPNETCORE_URLS='http://127.0.0.1:60001'
Environment=ASPNETCORE_ENVIRONMENT=Production
ExecStart=/usr/bin/dotnet run -c Release --no-launch-profile
Type=simple
TimeoutStopSec=20

Restart=on-failure
KillMode=control-group

[Install]
WantedBy=multi-user.target
```
</summary>