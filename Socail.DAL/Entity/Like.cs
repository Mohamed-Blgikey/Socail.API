using Socail.DAL.Extend;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Socail.DAL.Entity
{
    public class Like
    {
        public string LikerId { get; set; }
        public ApplicationUser Liker { get; set; }

        public string LikeeId { get; set; }
        public ApplicationUser Likee { get; set; }
    }
}
