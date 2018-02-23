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
    public class HorsesController : Controller {
        private readonly BegEFCoreContext _context;

        public HorsesController(BegEFCoreContext context) {
            _context = context;
        }

        // GET: Horses
        public async Task<IActionResult> Index(int? page) {
            int pageSize = 5;
            var horses = from h in _context.Horse orderby h.HorseId select h;
            return View(await PagedList<Horse>.CreateAsync(horses.AsNoTracking(), page ?? 1, pageSize));
            //return View(await _context.Horse.ToListAsync());
        }

        // GET: Horses/Details/5
        public async Task<IActionResult> Details(int? id) {
            if (id == null) {
                return NotFound();
            }

            var horse = await _context.Horse
                .SingleOrDefaultAsync(m => m.HorseId == id);
            if (horse == null) {
                return NotFound();
            }

            return View(horse);
        }

        // GET: Horses/Create
        public IActionResult Create() {
            return View();
        }

        // POST: Horses/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("HorseId,Name,Breed,Height,Value")] Horse horse) {
            if (ModelState.IsValid) {
                _context.Add(horse);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(horse);
        }

        // GET: Horses/Edit/5
        public async Task<IActionResult> Edit(int? id) {
            if (id == null) {
                return NotFound();
            }

            var horse = await _context.Horse.SingleOrDefaultAsync(m => m.HorseId == id);
            if (horse == null) {
                return NotFound();
            }
            return View(horse);
        }

        // POST: Horses/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("HorseId,Name,Breed,Height,Value")] Horse horse) {
            if (id != horse.HorseId) {
                return NotFound();
            }

            if (ModelState.IsValid) {
                try {
                    _context.Update(horse);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException) {
                    if (!HorseExists(horse.HorseId)) {
                        return NotFound();
                    }
                    else {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(horse);
        }

        // GET: Horses/Delete/5
        public async Task<IActionResult> Delete(int? id) {
            if (id == null) {
                return NotFound();
            }

            var horse = await _context.Horse
                .SingleOrDefaultAsync(m => m.HorseId == id);
            if (horse == null) {
                return NotFound();
            }

            return View(horse);
        }

        // POST: Horses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id) {
            var horse = await _context.Horse.SingleOrDefaultAsync(m => m.HorseId == id);
            _context.Horse.Remove(horse);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool HorseExists(int id) {
            return _context.Horse.Any(e => e.HorseId == id);
        }
    }
}
