using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Socail.BL.Dtos
{
    public class MessageToReturnDto
    {
        public int Id { get; set; }
        public string SenderId { get; set; }
        public string SenderFullName { get; set; }
        public string SenderPhotoName { get; set; }

        public string ResipientId { get; set; }
        public string ResipientFullName { get; set; }
        public string ResipientPhotoName { get; set; }

        public string Content { get; set; }
        public bool IsRead { get; set; }
        public DateTime DateRead { get; set; }
        public DateTime MessageSent { get; set; }
    }
}
