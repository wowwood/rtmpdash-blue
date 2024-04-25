# RTMPDash

[Production instance](https://chaos.stream)

## Installation

- Adjust all the `const`'s in Program.cs
- Install [nginx-mod-rtmp](https://git.zotan.services/zotan/nginx-mod-rtmp) (arch
  package: `aur/nginx-mod-rtmp-zotanmew-git`)
- Configure nginx-mod-rtmp, example config below
- Install and start `redis` for persistent sessions
- Start RTMPDash, example systemd unit below
- The admin user is output to stdout on first startup, if RTMPdash started with the systemd unit it will be in the
  journal

## Further setup

- Customize privacy policy to your environment if you plan on hosting it publicly
- Set up player, example code below

### nginx-mod-rtmp example config

```
load_module /usr/lib/nginx/modules/ngx_rtmp_module.so; # load our rtmp module
worker_processes 1; # VERY IMPORTANT, otherwise nginx-mod-rtmp breaks

rtmp {
	log_format rtmp '[$time_local] $command -> $app with key "$name" - $bytes_received bytes received ($session_readable_time)';
	server {
		listen [::]:1935;

		access_log /var/log/nginx/rtmp_access.log rtmp;

		max_message 32M;
		ping 1m;
		ping_timeout 10s;
		drop_idle_publisher 10s;
		notify_method get;

		application ingress {
			live on;

			allow play 127.0.0.1;
			deny play all;

			notify_relay_redirect on;

			on_publish http://localhost:60001/api/authenticate;

			hls on;
			hls_path /mnt/ssd_data/hls/src;
			hls_continuous off;
			hls_fragment 1s;
			hls_fragment_naming system;
			hls_playlist_length 60s;
			hls_allow_client_cache enabled;
		}
	}
}
```

### RTMPdash example systemd unit (development)

```
[Unit]
Description=RTMPDash
Wants=network-online.target
After=network-online.target

[Service]
User=rtmpdash
Group=rtmpdash
WorkingDirectory=/opt/rtmpdash
Environment=ASPNETCORE_URLS='http://*:60001'
Environment=ASPNETCORE_ENVIRONMENT=Development
ExecStart=/usr/bin/dotnet watch run --no-launch-profile
Type=simple
TimeoutStopSec=20

# Lets built in updater work well.
Restart=on-failure
KillMode=control-group

[Install]
WantedBy=multi-user.target
```

## RTMPdash example systemd unit (production)

```
[Unit]
Description=RTMPDash
Wants=network-online.target
After=network-online.target

[Service]
User=rtmpdash
Group=rtmpdash
WorkingDirectory=/opt/rtmpdash
Environment=ASPNETCORE_URLS='http://*:60001'
Environment=ASPNETCORE_ENVIRONMENT=Production
ExecStart=/usr/bin/dotnet run -c Release --no-launch-profile
Type=simple
TimeoutStopSec=20

# Lets built in updater work well.
Restart=on-failure
KillMode=control-group

[Install]
WantedBy=multi-user.target
```

### VideoJS Player example code

```
<!DOCTYPE html>
<html>
  <head>
    <meta charset='utf-8'>
    <meta http-equiv="X-UA-Compatible" content="chrome=1">
    <link href="/lib/video-js.min.css" rel="stylesheet" />
    <title>Player - chaos.stream</title>
    <style>
      html, body, #wrapper {
        margin: 0;
        padding: 0;
        width: 100%;
        height: 100%;
      }
    </style>
  </head>
  <body>
    <div id="wrapper">
    <video id='hls-player' class="video-js vjs-default-skin vjs-big-play-centered" controls autoplay>
      <source type="application/x-mpegURL">
    </video>
    </div>
    <script src="/lib/video.min.js"></script>
    <script>
      document.querySelector('source').src = 'https://cdn.chaos.stream/hls/src/' + window.location.pathname.substring(1) + '.m3u8';
      document.getElementById("hls-player").addEventListener("error",function(event){
        console.log('HTML5 video error, reloading page in 10sec');
        setTimeout(function () { location.reload(true); }, 10000);
      });

      var player = videojs('hls-player', {
        liveui: true,
        html5: {
          vhs: {
            blacklistDuration: Infinity,
            maxPlaylistRetries: 5,
            experimentalBufferBasedABR: true,
          },
        },
      });

      player.ready(function() {
        let player = this

        player.on("error", function(){
          console.log('VJS Error. Treating as offline.');
          player.poster('https://live.on.chaos.stream/lib/offline.png');
          player.errorDisplay.hide();
        });

        player.on("ended", function(){
          console.log('Stream ended. Redirecting to profile.');
          document.location = 'https://chaos.stream/' + window.location.pathname.substring(1);
        });
      });

      player.fill(true);
    </script>
  </body>
</html>
```

### accompanying nginx config for VideoJS Player

```
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
```
