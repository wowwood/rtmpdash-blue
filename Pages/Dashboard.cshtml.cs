using System;
using System.Linq;
using LinqToDB;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RTMPDash.Backend.Database;

namespace RTMPDash.Pages;

public class DashboardModel : PageModel {
	public void OnGet() { }

	public void OnPost() {
		if (!Request.HasFormContentType || string.IsNullOrWhiteSpace(Request.Form["action"]) || string.IsNullOrWhiteSpace(HttpContext.Session.GetString("authenticatedUser")))
			return;

		using var db   = new Database.DbConn();
		var       user = db.Users.FirstOrDefault(p => p.Username == HttpContext.Session.GetString("authenticatedUser"));

		if (Request.Form["action"] == "password_change") {
			var newPass = Request.Form["pass"];
			user!.Password = newPass.ToString().Sha256();
			db.Update(user);
			Response.Redirect("/Logout");
		}

		if (Request.Form["action"] == "chaturl_set") {
			user!.ChatUrl = Request.Form["value"];
			db.Update(user);
			Response.Redirect("/Dashboard");
		}

		if (Request.Form["action"] == "announceurl_set") {
			user!.AnnouncementUrl = Request.Form["value"];
			db.Update(user);
			Response.Redirect("/Dashboard");
		}

		if (user!.AllowRestream) {
			if (Request.Form["action"] == "restream_urls_set") {
				user!.RestreamUrls = Request.Form["value"];
				db.Update(user);
				Response.Redirect("/Dashboard");
			}

			if (Request.Form["action"] == "restream_targets_set") {
				var newtgts = Request.Form["value"].ToString().Trim();
				if (newtgts.Contains("localhost") || newtgts.Contains("127.0.0.1") || newtgts.Contains(user.StreamKey))
					return;

				user!.RestreamTargets = newtgts;
				db.Update(user);
				Response.Redirect("/Dashboard");
			}
		}

		if (Request.Form["action"] == "pronoun_subj_set") {
			var target = string.IsNullOrWhiteSpace(Request.Form["value"]) ? "they" : Request.Form["value"].ToString();
			user!.PronounSubject = target.ToLowerInvariant();
			db.Update(user);
			Response.Redirect("/Dashboard");
		}

		if (Request.Form["action"] == "pronoun_poss_set") {
			var target = string.IsNullOrWhiteSpace(Request.Form["value"]) ? "their" : Request.Form["value"].ToString();
			user!.PronounPossessive = target.ToLowerInvariant();
			db.Update(user);
			Response.Redirect("/Dashboard");
		}

		if (Request.Form["action"] == "pronoun_plural") {
			user!.PronounPlural = true;
			db.Update(user);
			Response.Redirect("/Dashboard");
		}

		if (Request.Form["action"] == "pronoun_singular") {
			user!.PronounPlural = false;
			db.Update(user);
			Response.Redirect("/Dashboard");
		}

		if (Request.Form["action"] == "streamkey_reset") {
			user!.StreamKey = Guid.NewGuid().ToString();
			db.Update(user);
		}

		if (Request.Form["action"] == "private_toggle") {
			if (user.IsPrivate) {
				user!.IsPrivate = false;
			}
			else {
				user!.PrivateAccessKey = Guid.NewGuid().ToString();
				user!.IsPrivate        = true;
			}

			db.Update(user);
		}
	}
}
