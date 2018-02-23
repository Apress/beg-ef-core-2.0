using System;

namespace EquineTracker.Helpers {
    public class PagingInfo {
        public int TotalObjects { get; set; }
        public int ObjectsPerPage { get; set; }
        public int CurrentPage { get; set; }

        public int TotalPages {
            get {
                return (int)Math.Ceiling((decimal)TotalObjects / ObjectsPerPage);
            }
        }

        public bool HasPreviousPage {
            get {
                return (CurrentPage > 1);
            }
        }

        public bool HasNextPage {
            get {
                return (CurrentPage < TotalPages);
            }
        }
    }
}
