using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RTMPDash.Pages; 

public class ProfileModel : PageModel {
	public new string User { get; set; }

	public void OnGet(string user) {
		User = user;
	}
}