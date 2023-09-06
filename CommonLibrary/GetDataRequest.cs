using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibrary
{
    public class GetDataRequest
    {
        public ChatUser? From { get; set; }
        public int? AfterId { get; set; }
    }
}
