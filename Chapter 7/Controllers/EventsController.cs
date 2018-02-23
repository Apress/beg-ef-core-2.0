using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EquineTracker.Models;
using EquineTracker.Helpers;

namespace EquineTracker.Controllers {
    public class EventsController : Controller {
        private readonly BegEFCoreContext _context;
        public int PageSize = 4;
        public EventsController(BegEFCoreContext context) {
            _context = context;
        }

        // GET: Events
        public async Task<IActionResult> Index(string sortOrder, string searchString, string currentFilter, int page = 1) {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["LocationSortParam"] = sortOrder == "loc" ? "loc_desc" : "loc";
            ViewData["DateSortParam"] = string.IsNullOrEmpty(sortOrder) ? "date_desc" : "";
            if (searchString == null) {
                searchString = currentFilter;
            }
            ViewData["CurrentFilter"] = searchString;
            //ViewData["SearchParam"] = searchString;

            EventListViewModel model = new EventListViewModel();
            var events = _context.Event.Include(x => x.Location);
            // Check if searchString is null, if not search the events for the name of the event
            if (!string.IsNullOrEmpty(searchString)) {
                events = events.Where(n => n.Name.Contains(searchString)).Include(x => x.Location);
            }
            switch (sortOrder) {
                case "loc_desc":
                    model = new EventListViewModel {
                        Events = events.Include(x => x.Location)
                            .OrderByDescending(e => e.Location)
                            .Skip((page - 1) * PageSize)
                            .Take(PageSize)
                    };
                    break;
                case "loc":
                    model = new EventListViewModel {
                        Events = events.Include(x => x.Location)
                        .OrderBy(e => e.Location)
                        .Skip((page - 1) * PageSize)
                        .Take(PageSize),
                    };
                    break;
                case "date_desc":
                    model = new EventListViewModel {
                        Events = events.Include(x => x.Location)
                        .OrderByDescending(e => e.EventDate)
                        .Skip((page - 1) * PageSize)
                        .Take(PageSize),
                    };
                    break;
                default:
                    model = new EventListViewModel {
                        Events = events.Include(x => x.Location)
                        .OrderBy(e => e.EventDate)
                        .Skip((page - 1) * PageSize)
                        .Take(PageSize),
                    };
                    break;
            }
            model.PagingInfo = new PagingInfo {
                CurrentPage = page,
                ObjectsPerPage = PageSize,
                TotalObjects = events.ToList().Count()
            };
            return View(model);
        }

        // GET: Events/Details/5
        public async Task<IActionResult> Details(int? id) {
            if (id == null) {
                return NotFound();
            }

            var @event = await _context.Event
                .Include(x => x.Location)
                .SingleOrDefaultAsync(m => m.EventId == id);
            if (@event == null) {
                return NotFound();
            }

            return View(@event);
        }

        // GET: Events/Create
        public IActionResult Create() {
            ViewData["LocationId"] = new SelectList(_context.Location, "LocationId", "Name");
            return View();
        }

        // POST: Events/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EventId,Name,LocationId,Description,EventDate")] Event @event) {
            if (ModelState.IsValid) {
                _context.Add(@event);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["LocationId"] = new SelectList(_context.Location, "LocationId", "Name", @event.LocationId);
            return View(@event);
        }

        // GET: Events/Edit/5
        public async Task<IActionResult> Edit(int? id) {
            if (id == null) {
                return NotFound();
            }

            var @event = await _context.Event.SingleOrDefaultAsync(m => m.EventId == id);
            if (@event == null) {
                return NotFound();
            }
            ViewData["LocationId"] = new SelectList(_context.Location, "LocationId", "Name", @event.LocationId);
            return View(@event);
        }

        // POST: Events/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EventId,Name,LocationId,Description,EventDate")] Event @event) {
            if (id != @event.EventId) {
                return NotFound();
            }

            if (ModelState.IsValid) {
                try {
                    _context.Update(@event);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException) {
                    if (!EventExists(@event.EventId)) {
                        return NotFound();
                    }
                    else {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["LocationId"] = new SelectList(_context.Location, "LocationId", "Name", @event.LocationId);
            return View(@event);
        }

        // GET: Events/Delete/5
        public async Task<IActionResult> Delete(int? id) {
            if (id == null) {
                return NotFound();
            }

            var @event = await _context.Event
                .Include(x => x.Location)
                .SingleOrDefaultAsync(m => m.EventId == id);
            if (@event == null) {
                return NotFound();
            }

            return View(@event);
        }

        // POST: Events/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id) {
            var @event = await _context.Event.SingleOrDefaultAsync(m => m.EventId == id);
            _context.Event.Remove(@event);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EventExists(int id) {
            return _context.Event.Any(e => e.EventId == id);
        }

        // "Get" Events by LocationID
        public List<Event> GetEventsByLocation(int locationId) {
            List<Event> lEvent = _context.Event.Include(x => x.Location).Where(l => l.LocationId == locationId).OrderBy(d => d.EventDate).ToList();

            return lEvent;
        }

        // GET: Events by LocationID
        public ViewResult EventByLocation(int locationId) {
            List<Event> lEvent = GetEventsByLocation(locationId);
            return View(lEvent);
        }
    }
}
