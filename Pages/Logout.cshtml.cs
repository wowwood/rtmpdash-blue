using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RTMPDash.Pages; 

public class LogoutModel : PageModel {
	public void OnGet() {
		HttpContext.Session.Clear();
	}
}