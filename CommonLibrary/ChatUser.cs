using System.ComponentModel.DataAnnotations;

namespace CommonLibrary;

public class ChatUser
{
	public int Id { get; set; }
	public string Name { get; set; }
	public string Login { get; set; }
}