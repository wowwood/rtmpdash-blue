using System.Linq;
using Microsoft.AspNetCore.Mvc;
using RTMPDash.Backend.Database;

namespace RTMPDash.Controllers;

[ApiController, Route("/api/authenticate")]
public class RtmpAuthController : ControllerBase {
	[HttpGet]
	public string Get() {
		var db = new Database.DbConn();
		if (!db.Users.Any(p => p.StreamKey == Request.Query["name"])) {
			Response.StatusCode = 403;
			return "unauthorized";
		}

		var user = db.Users.FirstOrDefault(p => p.StreamKey == Request.Query["name"]);

		Response.Headers.Add("x-rtmp-user", user!.IsPrivate ? user!.PrivateAccessKey : user!.Username);

		if (user.AllowRestream && !string.IsNullOrWhiteSpace(user.RestreamTargets))
			Response.Headers.Add("x-rtmp-target", user.RestreamTargets);

		Response.StatusCode = 302;
		return "authorized as " + user!.Username;
	}
}