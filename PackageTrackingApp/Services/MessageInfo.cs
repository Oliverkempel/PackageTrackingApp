using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PackageTrackingApp.Models
{
    public class MessageInfo
    {
        public string Id { get; set; }
        public string To { get; set; }
        public string From { get; set; }
        public string Subject { get; set; }
        public string Date { get; set; }

        public MessageInfo(string id)
        {
            Id = id;
        }
    }
}