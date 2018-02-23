using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace EquineTracker.Helpers {
    public class PagedList<T> : List<T> {
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }

        public PagedList(List<T> items, int count, int currentPage, int pageSize) {
            CurrentPage = currentPage;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);

            this.AddRange(items);
        }

        public bool HasPreviousPage {
            get { return (CurrentPage > 1); }
        }

        public bool HasNextPage {
            get { return (CurrentPage < TotalPages); }
        }

        public static async Task<PagedList<T>> CreateAsync(IQueryable<T> source, int pageIndex, int pageSize) {
            var count = await source.CountAsync();
            var items = await source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PagedList<T>(items, count, pageIndex, pageSize);
        }
    }
}
