using System.ComponentModel.DataAnnotations;

namespace CommonLibrary;

public class LoginResponse
{
	public bool IsLoggedIn { get; set; }
	public ChatUser? User { get; set; }
}