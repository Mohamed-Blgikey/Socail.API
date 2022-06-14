using Socail.DAL.Extend;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Socail.DAL.Entity
{
    public class Message
    {
        public int Id { get; set; }
        public string SenderId { get; set; }
        public ApplicationUser Sender { get; set; }

        public string ResipientId { get; set; }
        public ApplicationUser Resipient { get; set; }

        public string Content { get; set; }
        public bool IsRead { get; set; }
        public DateTime? DateRead { get; set; }
        public DateTime messageSent { get; set; }
        public bool SenderDeleted { get; set; }
        public bool RecipientDeleted { get; set; }

    }
}
