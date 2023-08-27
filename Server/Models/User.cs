using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Models;

public class User
{
	public User()
	{
		//Messages = new HashSet<Message>();
	}

	public int Id { get; set; }
	[StringLength(100)]
	public string Name { get; set; }
	[StringLength(100)]
	public string Login { get; set; }
	[StringLength(100)]
	public string Password { get; set; }

	//public ICollection<Message> Messages { get; set; }
}
