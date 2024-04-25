using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Xml.Serialization;

namespace RTMPDash.Backend;

public static class StreamUtils {
	private static readonly XmlSerializer Serializer = new(typeof(StatsObject));

	public static bool IsLive(string user, StatsObject stats) => stats.Server.Applications.First(p => p.Name == "ingress").MethodLive.Streams.Any(p => p.Name == user);

	public static bool IsLive(string user, string target, StatsObject stats) => stats.Server.Applications.First(p => p.Name == "ingress")
																					 .MethodLive.Streams
																					 .Any(p => p.Name == user && p.Clients.Any(c => c.Address == target.Replace("rtmp://", "")));

	public static long GetClientTime(string user, StatsObject stats) =>
		long.Parse(stats.Server.Applications.First(p => p.Name == "ingress").MethodLive.Streams.First(p => p.Name == user).Time);

	public static int CountLiveRestreams(string user, StatsObject stats) {
		var db     = new Database.Database.DbConn();
		var dbUser = db.Users.First(p => p.Username == user);
		return dbUser.RestreamTargets.Split(",")
					 .Count(target => stats.Server.Applications.First(p => p.Name == "ingress")
									       .MethodLive.Streams.Any(p => p.Name == user && p.Clients.Any(c => c.Address == target.Replace("rtmp://", ""))));
	}

	public static int CountLiveRestreams(string user, string privateAccesskey, StatsObject stats) {
		var db     = new Database.Database.DbConn();
		var dbUser = db.Users.First(p => p.Username == user);
		return dbUser.RestreamTargets.Split(",")
					 .Count(target => stats.Server.Applications.First(p => p.Name == "ingress")
									       .MethodLive.Streams.Any(p => p.Name == privateAccesskey && p.Clients.Any(c => c.Address == target.Replace("rtmp://", ""))));
	}

	public static List<string> ListLiveUsers() => GetStatsObject().Server.Applications.First(p => p.Name == "ingress").MethodLive.Streams.Select(p => p.Name).ToList();

	public static StatsObject GetStatsObject() {
		var obj = (StatsObject)Serializer.Deserialize(new HttpClient().GetStreamAsync(Program.RtmpStatsUrl).Result);
		return obj;
	}
}

[XmlRoot(ElementName = "rtmp")]
public class StatsObject {
	[XmlElement(ElementName = "nginx_version", IsNullable = true)]
	public string NginxVersion { get; set; }

	[XmlElement(ElementName = "nginx_rtmp_version", IsNullable = true)]
	public string NginxRtmpVersion { get; set; }

	[XmlElement(ElementName = "server", IsNullable = true)]    public Server Server              { get; set; }
	[XmlElement(ElementName = "built", IsNullable = true)]     public string Built               { get; set; }
	[XmlElement(ElementName = "pid", IsNullable = true)]       public string Pid                 { get; set; }
	[XmlElement(ElementName = "uptime", IsNullable = true)]    public string Uptime              { get; set; }
	[XmlElement(ElementName = "naccepted", IsNullable = true)] public string AcceptedConnections { get; set; }
	[XmlElement(ElementName = "bw_in", IsNullable = true)]     public string BwIn                { get; set; }
	[XmlElement(ElementName = "bytes_in", IsNullable = true)]  public string BytesIn             { get; set; }
	[XmlElement(ElementName = "bw_out", IsNullable = true)]    public string BwOut               { get; set; }
	[XmlElement(ElementName = "bytes_out", IsNullable = true)] public string BytesOut            { get; set; }
}

[XmlRoot(ElementName = "server", IsNullable = true)]
public class Server {
	[XmlElement(ElementName = "application", IsNullable = true)] public List<Application> Applications { get; set; }
}

[XmlRoot(ElementName = "application", IsNullable = true)]
public class Application {
	[XmlElement(ElementName = "live", IsNullable = true)] public MethodLive MethodLive { get; set; }
	[XmlElement(ElementName = "name", IsNullable = true)] public string     Name       { get; set; }
}

[XmlRoot(ElementName = "live", IsNullable = true)]
public class MethodLive {
	[XmlElement(ElementName = "stream", IsNullable = true)]   public List<Stream> Streams   { get; set; }
	[XmlElement(ElementName = "nclients", IsNullable = true)] public string       NoClients { get; set; }
}

[XmlRoot(ElementName = "stream", IsNullable = true)]
public class Stream {
	[XmlElement(ElementName = "client", IsNullable = true)]     public List<Client> Clients    { get; set; }
	[XmlElement(ElementName = "meta", IsNullable = true)]       public Meta         Meta       { get; set; }
	[XmlElement(ElementName = "name", IsNullable = true)]       public string       Name       { get; set; }
	[XmlElement(ElementName = "time", IsNullable = true)]       public string       Time       { get; set; }
	[XmlElement(ElementName = "bw_in", IsNullable = true)]      public string       BwIn       { get; set; }
	[XmlElement(ElementName = "bytes_in", IsNullable = true)]   public string       BytesIn    { get; set; }
	[XmlElement(ElementName = "bw_out", IsNullable = true)]     public string       BwOut      { get; set; }
	[XmlElement(ElementName = "bytes_out", IsNullable = true)]  public string       BytesOut   { get; set; }
	[XmlElement(ElementName = "bw_audio", IsNullable = true)]   public string       BwAudio    { get; set; }
	[XmlElement(ElementName = "bw_video", IsNullable = true)]   public string       BwVideo    { get; set; }
	[XmlElement(ElementName = "bw_data", IsNullable = true)]    public string       BwData     { get; set; }
	[XmlElement(ElementName = "nclients", IsNullable = true)]   public string       NoClients  { get; set; }
	[XmlElement(ElementName = "active", IsNullable = true)]     public object       Active     { get; set; }
	[XmlElement(ElementName = "publishing", IsNullable = true)] public object       Publishing { get; set; }
}

[XmlRoot(ElementName = "client", IsNullable = true)]
public class Client {
	[XmlElement(ElementName = "id", IsNullable = true)]         public string Id         { get; set; }
	[XmlElement(ElementName = "address", IsNullable = true)]    public string Address    { get; set; }
	[XmlElement(ElementName = "port", IsNullable = true)]       public string Port       { get; set; }
	[XmlElement(ElementName = "time", IsNullable = true)]       public string Time       { get; set; }
	[XmlElement(ElementName = "flashver", IsNullable = true)]   public string FlashVer   { get; set; }
	[XmlElement(ElementName = "swfurl", IsNullable = true)]     public string SwfUrl     { get; set; }
	[XmlElement(ElementName = "bytes_in", IsNullable = true)]   public string BytesIn    { get; set; }
	[XmlElement(ElementName = "bytes_out", IsNullable = true)]  public string BytesOut   { get; set; }
	[XmlElement(ElementName = "dropped", IsNullable = true)]    public string Dropped    { get; set; }
	[XmlElement(ElementName = "avsync", IsNullable = true)]     public string AvSync     { get; set; }
	[XmlElement(ElementName = "timestamp", IsNullable = true)]  public string Timestamp  { get; set; }
	[XmlElement(ElementName = "active", IsNullable = true)]     public object Active     { get; set; }
	[XmlElement(ElementName = "publishing", IsNullable = true)] public object Publishing { get; set; }
}

[XmlRoot(ElementName = "meta", IsNullable = true)]
public class Meta {
	[XmlElement(ElementName = "video", IsNullable = true)] public Video Video { get; set; }
	[XmlElement(ElementName = "audio", IsNullable = true)] public Audio Audio { get; set; }
}

[XmlRoot(ElementName = "video", IsNullable = true)]
public class Video {
	[XmlElement(ElementName = "width", IsNullable = true)]      public string Width     { get; set; }
	[XmlElement(ElementName = "height", IsNullable = true)]     public string Height    { get; set; }
	[XmlElement(ElementName = "frame_rate", IsNullable = true)] public string FrameRate { get; set; }
	[XmlElement(ElementName = "codec", IsNullable = true)]      public string Codec     { get; set; }
	[XmlElement(ElementName = "profile", IsNullable = true)]    public string Profile   { get; set; }
	[XmlElement(ElementName = "compat", IsNullable = true)]     public string Compat    { get; set; }
	[XmlElement(ElementName = "level", IsNullable = true)]      public string Level     { get; set; }
}

[XmlRoot(ElementName = "audio", IsNullable = true)]
public class Audio {
	[XmlElement(ElementName = "codec", IsNullable = true)]       public string Codec      { get; set; }
	[XmlElement(ElementName = "profile", IsNullable = true)]     public string Profile    { get; set; }
	[XmlElement(ElementName = "channels", IsNullable = true)]    public string Channels   { get; set; }
	[XmlElement(ElementName = "sample_rate", IsNullable = true)] public string SampleRate { get; set; }
}
