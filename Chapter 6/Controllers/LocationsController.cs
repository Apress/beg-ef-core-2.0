using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EquineTracker.Models;

namespace EquineTracker.Controllers {
    public class LocationsController : Controller {
        private readonly BegEFCoreContext _context;

        public LocationsController(BegEFCoreContext context) {
            _context = context;
        }

        public ViewResult DbFkError() {
            return View();
        }

        // GET: Locations
        public async Task<IActionResult> Index() {
            return View(await _context.Location.ToListAsync());
        }

        // GET: Locations/Details/5
        public async Task<IActionResult> Details(int? id) {
            if (id == null) {
                return NotFound();
            }

            var location = await _context.Location
                .SingleOrDefaultAsync(m => m.LocationId == id);
            if (location == null) {
                return NotFound();
            }

            return View(location);
        }

        // GET: Locations/Create
        public IActionResult Create() {
            return View();
        }

        // POST: Locations/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LocationId,Name,StreetAddress,City,State,ZipCode")] Location location) {
            if (ModelState.IsValid) {
                _context.Add(location);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(location);
        }

        // GET: Locations/Edit/5
        public async Task<IActionResult> Edit(int? id) {
            if (id == null) {
                return NotFound();
            }

            var location = await _context.Location.SingleOrDefaultAsync(m => m.LocationId == id);
            if (location == null) {
                return NotFound();
            }
            return View(location);
        }

        // POST: Locations/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("LocationId,Name,StreetAddress,City,State,ZipCode")] Location location) {
            if (id != location.LocationId) {
                return NotFound();
            }

            if (ModelState.IsValid) {
                try {
                    _context.Update(location);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException) {
                    if (!LocationExists(location.LocationId)) {
                        return NotFound();
                    }
                    else {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(location);
        }

        // GET: Locations/Delete/5
        public async Task<IActionResult> Delete(int? id) {
            if (id == null) {
                return NotFound();
            }

            var location = await _context.Location
                .SingleOrDefaultAsync(m => m.LocationId == id);
            if (location == null) {
                return NotFound();
            }

            return View(location);
        }

        // POST: Locations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id) {
            EventsController ec = new EventsController(_context);
            List<Event> lEvent = ec.GetEventsByLocation(id);
            if(lEvent.Count != 0) {
                return View(nameof(DbFkError));
            }

            var location = await _context.Location.SingleOrDefaultAsync(m => m.LocationId == id);
            _context.Location.Remove(location);
            try {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException) {
                return View(nameof(DbFkError));
            }

            return RedirectToAction(nameof(Index));
        }

        private bool LocationExists(int id) {
            return _context.Location.Any(e => e.LocationId == id);
        }
    }
}
