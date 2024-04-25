using System;
using System.Linq;
using LinqToDB;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RTMPDash.Backend.Database;
using RTMPDash.Backend.Database.Tables;

namespace RTMPDash.Pages;

public class RegisterModel : PageModel {
	public void OnPost() {
		if (!Request.HasFormContentType
		 || string.IsNullOrWhiteSpace(Request.Form["user"])
		 || string.IsNullOrWhiteSpace(Request.Form["pass"])
		 || string.IsNullOrWhiteSpace(Request.Form["code"]))
			return;

		using var db = new Database.DbConn();
		if (!db.Invites.Any(p => p.Code == Request.Form["code"]))
			return;

		var newUser = Request.Form["user"].ToString().ToLowerInvariant();
		var user    = db.Users.FirstOrDefault(p => p.Username == newUser);
		if (user != null) {
			//user already exists
			Response.Redirect("/Register?e=user_exists");
			return;
		}

		if (db.Users.Any(p => p.StreamKey == newUser || p.PrivateAccessKey == newUser)) {
			//user invalid
			Response.Redirect("/Register?e=user_invalid");
			return;
		}

		if (newUser is "register" or "login" or "logout" or "privacy" or "stats" or "index" or "error" or "dashboard" or "credits" or "admin" or "content") {
			// user invalid
			Response.Redirect("/Register?e=user_invalid");
			return;
		}

		user = new User {
			Username          = Request.Form["user"].ToString(),
			Password          = Request.Form["pass"].ToString().Sha256(),
			StreamKey         = Guid.NewGuid().ToString(),
			PronounSubject    = "they",
			PronounPossessive = "their",
			PronounPlural     = true,
			AllowRestream     = true
		};

		db.Insert(user);

		db.Delete(db.Invites.First(p => p.Code == Request.Form["code"]));

		HttpContext.Session.SetString("authenticatedUser", user.Username);
	}
}
