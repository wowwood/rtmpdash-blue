using System;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RTMPDash.Backend.Database;

namespace RTMPDash.Pages;

public class LoginModel : PageModel {
	public void OnPost() {
		if (!Request.HasFormContentType || string.IsNullOrWhiteSpace(Request.Form["user"]) || string.IsNullOrWhiteSpace(Request.Form["pass"]))
			return;

		using var db   = new Database.DbConn();
		var       user = db.Users.FirstOrDefault(p => p.Username == Request.Form["user"].ToString());

		if (user == null)
			return;

		if (user.Password != Request.Form["pass"].ToString().Sha256())
			return;

		HttpContext.Session.SetString("authenticatedUser", user.Username);
	}
}

public static class StringExtensions {
	public static string Sha256(this string rawData) {
		// Create a SHA256
		using var sha256Hash = SHA256.Create();

		// ComputeHash - returns byte array
		var bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

		// Convert byte array to a string
		var builder = new StringBuilder();
		for (var i = 0; i < bytes.Length; i++)
			builder.Append(bytes[i].ToString("x2"));

		return builder.ToString();
	}

	public static string FirstCharToUpper(this string input) => input switch {
		null => throw new ArgumentNullException(nameof(input)),
		""   => throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input)),
		_    => input.First().ToString().ToUpper() + input.Substring(1)
	};

	public static string Base64Encode(this string plainText) {
		var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
		return Convert.ToBase64String(plainTextBytes);
	}

	public static string UrlEncode(this string plainText) => HttpUtility.UrlEncode(plainText);

	public static string Delimit(this string input, int max) => input.PadRight(max, ' ').Substring(0, max).TrimEnd();

	public static string Bash(this string cmd) {
		var escapedArgs = cmd.Replace("\"", "\\\"");

		var process = new Process {
			StartInfo = new ProcessStartInfo {
				FileName               = "/bin/bash",
				Arguments              = $"-c \"{escapedArgs}\"",
				RedirectStandardOutput = true,
				UseShellExecute        = false,
				CreateNoWindow         = true
			}
		};
		process.Start();
		var result = process.StandardOutput.ReadToEnd();
		process.WaitForExit();
		return result;
	}
}
