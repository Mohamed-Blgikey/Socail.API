using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Socail.BL.Helper
{
    public class PaginationHeader
    {
        public int CurrentPage { get; set; }
        public int ItemPerPage { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
        public PaginationHeader(int currentPage ,int itemsPerPage,int totalItems,int totalPages)
        {
            this.CurrentPage= currentPage;
            this.ItemPerPage= itemsPerPage;
            this.TotalItems= totalItems;
            this.TotalPages= totalPages;
        }
    }
}
