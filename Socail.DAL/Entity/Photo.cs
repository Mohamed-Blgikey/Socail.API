using Socail.DAL.Extend;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Socail.DAL.Entity
{
    public class Photo
    {
        public int Id { get; set; }
        public string Url { get; set; }    
        public string Description { get; set; }    
        public DateTime DateAdded { get; set; }    
        public bool IsMain { get; set; }

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
    }
}
