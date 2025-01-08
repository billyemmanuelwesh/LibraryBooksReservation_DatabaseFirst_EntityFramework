using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GPLCC_2.Models;

namespace GPLCC_2.Controllers
{
    public class RetardsController : Controller
    {
        private readonly LocalDBMSSQLLocalDBContext _context;

        public RetardsController(LocalDBMSSQLLocalDBContext context)
        {
            _context = context;
        }

        // GET: Retards
        public async Task<IActionResult> Index()
        {

            _context.RemoveRange(_context.Retard); // on vide les retards
            _context.SaveChanges();

            // selection des prets où la dateFin est plus petit que la date d'aujourd'hui
            var ListeDesPretsEnRetard = _context.Pret.Where(x => x.DateFin.Value.Date < DateTime.Now.Date);

            // pour chaque Pret en retard
            foreach (Pret p in ListeDesPretsEnRetard)
            {
                Retard r = new Retard();
                r.NumPretNavigation = p;
                if(p.DateFin != null)
                    r.NbJoursRetard = DateTime.Now.Subtract((DateTime)p.DateFin).Days;

                _context.Add(r);
            }
            _context.SaveChanges();

            ; return _context.Retard != null ? 
                          View(await _context.Retard.Include(x => x.NumPretNavigation)
                          .Include(p => p.NumPretNavigation.NumMembreNavigation)
                          .Include(z => z.NumPretNavigation.NumLivreNavigation).ToListAsync()) :
                          Problem("Entity set 'LocalDBMSSQLLocalDBContext.Retard'  is null.");
        }

        // GET: Retards/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Retard == null)
            {
                return NotFound();
            }

            var retard = await _context.Retard
                .Include(p => p.NumPretNavigation)
                .FirstOrDefaultAsync(m => m.NumPret == id);
            if (retard == null)
            {
                return NotFound();
            }

            return View(retard);
        }

        // GET: Retards/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Retards/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("NbJoursRetard,NumPret")] Retard retard)
        {
            if (ModelState.IsValid)
            {
                _context.Add(retard);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(retard);
        }

        // GET: Retards/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Retard == null)
            {
                return NotFound();
            }

            var retard = await _context.Retard.FindAsync(id);
            if (retard == null)
            {
                return NotFound();
            }
            return View(retard);
        }

        // POST: Retards/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("NbJoursRetard,NumPret")] Retard retard)
        {
            if (id != retard.NumPret)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(retard);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RetardExists(retard.NumPret))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(retard);
        }

        // GET: Retards/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Retard == null)
            {
                return NotFound();
            }

            var retard = await _context.Retard
                .FirstOrDefaultAsync(m => m.NumPret == id);
            if (retard == null)
            {
                return NotFound();
            }

            return View(retard);
        }

        // POST: Retards/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Retard == null)
            {
                return Problem("Entity set 'LocalDBMSSQLLocalDBContext.Retard'  is null.");
            }
            var retard = await _context.Retard.FindAsync(id);
            if (retard != null)
            {
                _context.Retard.Remove(retard);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RetardExists(int id)
        {
          return (_context.Retard?.Any(e => e.NumPret == id)).GetValueOrDefault();
        }
    }
}
