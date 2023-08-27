using System.ComponentModel.DataAnnotations;

namespace CommonLibrary;

public class GetMessagesRequest
{
	public ChatUser? From { get; set; }
	public ChatUser? To { get; set; }
	public int? AfterId { get; set; }
}