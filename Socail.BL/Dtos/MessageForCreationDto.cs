using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Socail.BL.Dtos
{
    public class MessageForCreationDto
    {
        public MessageForCreationDto()
        {
            messageSent = DateTime.Now;
        }
        public string SenderId { get; set; }

        public string ResipientId { get; set; }

        public string Content { get; set; }
        public DateTime messageSent { get; set; }

    }
}
