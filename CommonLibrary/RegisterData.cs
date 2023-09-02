using System.ComponentModel.DataAnnotations;

namespace CommonLibrary;

public class RegisterData
{
	public string? Name { get; set; }
	public string? Login { get; set; }
	public string? Password { get; set; }
}