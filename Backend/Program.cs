using System;
using System.Threading;
using LinqToDB.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace RTMPDash.Backend;

public class Program {
	public const string SiteName               = "stream.whatthe.blue";
	public const string IngressDomain          = "rtmp://stream.whatthe.blue";
	public const string RtmpStatsUrl           = "http://127.0.0.1:8083";
	public const string RootDomain             = "https://stream.whatthe.blue";
	public const string PlayerDomain           = "https://player.stream.whatthe.blue";
	public const string FragmentDomain         = "https://cdn.stream.whatthe.blue";
	public const string StatsDomain            = "https://stats.stream.whatthe.blue";
	public const string PrivacyEmail           = "stream-privacy@whatthe.blue";
	public const string CopyrightEmail         = "stream-copyright@whatthe.blue";
	public const string AbuseEmail             = "stream-abuse@whatthe.blue";
	public const string ServiceAnnouncementUrl = "https://t.me/genderisntreal";
	public const string ServiceStatusUrl       = "https://status.stream.whatthe.blue";

	public const string ContactInfo =
		"<a href=\"https://t.me/genderisntreal\" target=\"_blank\">Telegram</a>.";

	public static void Main(string[] args) {
		DataConnection.DefaultSettings = new Database.Database.Settings();
		ThreadPool.SetMinThreads(100, 100);
		Migrations.RunMigrations();
		CreateHostBuilder(args).Build().Run();
	}

	public static IHostBuilder CreateHostBuilder(string[] args) => Host.CreateDefaultBuilder(args).ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
}

public static class TimeExtensions {
	public static TimeSpan StripMilliseconds(this TimeSpan time) => new(time.Days, time.Hours, time.Minutes, time.Seconds);
}
