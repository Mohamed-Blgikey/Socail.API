using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Socail.BL.Helper
{
    public static class Extentions
    {
        public static void AddPagination(this HttpResponse response,int currentPage,int itemPerPage,int totalItems,int totalPages)
        {
            var paginationHeader = new PaginationHeader(currentPage,itemPerPage,totalItems,totalPages);
            response.Headers.Add("Pagination", JsonConvert.SerializeObject(paginationHeader));
            response.Headers.Add("Access-Control-Expose-Headers", "Pagination");
        }
    }
}
