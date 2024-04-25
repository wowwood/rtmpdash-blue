using System;
using System.Linq;
using LinqToDB;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RTMPDash.Backend.Database;
using RTMPDash.Backend.Database.Tables;

namespace RTMPDash.Pages;

public class AdminModel : PageModel {
	public void OnGet() { }

	public void OnPost() {
		if (string.IsNullOrEmpty(HttpContext.Session.GetString("authenticatedUser"))
			|| !new Database.DbConn().Users.First(p => p.Username == HttpContext.Session.GetString("authenticatedUser")).IsAdmin)
			return;

		if (!Request.HasFormContentType || string.IsNullOrWhiteSpace(Request.Form["action"]))
			return;

		var db = new Database.DbConn();

		if (Request.Form["action"] == "invite_generate")
			db.Insert(new Invite { Code = Guid.NewGuid().ToString() });

		if (Request.Form["action"] == "invite_revoke")
			db.Delete(db.Invites.First(p => p.Code == Request.Form["target"]));

		if (Request.Form["action"] == "restream_allow") {
			var user = db.Users.First(p => p.Username == Request.Form["target"]);
			user.AllowRestream = true;
			db.Update(user);
		}

		if (Request.Form["action"] == "restream_revoke") {
			var user = db.Users.First(p => p.Username == Request.Form["target"]);
			user.AllowRestream = false;
			db.Update(user);
		}

		if (Request.Form["action"] == "admin_grant") {
			var user = db.Users.First(p => p.Username == Request.Form["target"]);
			user.IsAdmin = true;
			db.Update(user);
		}

		if (Request.Form["action"] == "admin_revoke") {
			var user = db.Users.First(p => p.Username == Request.Form["target"]);
			user.IsAdmin = false;
			db.Update(user);
		}

		if (Request.Form["action"] == "user_delete") {
			var user = db.Users.First(p => p.Username == Request.Form["target"]);
			db.Delete(user);
			Response.Redirect("/Admin");
		}

		if (Request.Form["action"] == "user_setpass") {
			var user = db.Users.First(p => p.Username == Request.Form["target"]);
			user.Password = Request.Form["value"].ToString().Sha256();
			db.Update(user);
			Response.Redirect("/Admin");
		}
	}
}
