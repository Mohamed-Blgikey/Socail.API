using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Socail.BL.Helper
{
    public class UserParams
    {
        private const int maxPageSize = 50;
        public int PageNumber { get; set; } = 1;

        private int pageSize = 10;

        public int PageSize
        {
            get { return pageSize; }
            set { pageSize = (value > 50) ? maxPageSize : value; }
        }

        public string? UserId { get; set; }
        public int? Gender { get; set; }
        public bool Likees { get; set; } = false;
        public bool Likers { get; set; } = false;

    }
}
