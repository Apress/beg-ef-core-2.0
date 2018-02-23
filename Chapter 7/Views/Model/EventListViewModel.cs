using System.Collections.Generic;
using EquineTracker.Helpers;

namespace EquineTracker.Models {
    public class EventListViewModel {
        public IEnumerable<Event> Events { get; set; }
        public PagingInfo PagingInfo { get; set; }
    }
}