using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Models;

public class Message
{
	public int Id { get; set; }
	[ForeignKey("FromUserId")]
	public virtual User From { get; set; }
	[ForeignKey("ToUserId")]
	public virtual User To { get; set; }
	public DateTime CreatedAt { get; set; }
	[StringLength(5000)]
	public string Text { get; set; }
}
