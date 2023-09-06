using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibrary
{
    public class GetDataResponse
    {
        public List<ChatUser> users { get; set; }
        public List<MessageData> Messages { get; set; }
    }
}
