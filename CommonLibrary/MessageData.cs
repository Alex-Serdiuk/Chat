using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibrary
{
    public class MessageData
    {
        public ChatUser From { get; set; }  
		public ChatUser To { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? Text { get; set; }
    }
}
