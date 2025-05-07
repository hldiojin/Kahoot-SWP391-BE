using Microsoft.EntityFrameworkCore;
using Repository.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Paging
{
    public class Paging
    {
        public static async Task<PagedResult<T>> GetPagedResultAsync<T>(IQueryable<T> query, int pageNumber, int pageSize)
        {
            var result = new PagedResult<T>();

            result.TotalCount = await query.CountAsync();
            result.PageNumber = pageNumber;
            result.PageSize = pageSize;
            result.Items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

            return result;
        }
    }
}
