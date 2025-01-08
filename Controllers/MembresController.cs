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
    public class MembresController : Controller
    {
        private readonly LocalDBMSSQLLocalDBContext _context;

        public MembresController(LocalDBMSSQLLocalDBContext context)
        {
            _context = context;
        }

        // GET: Membres
        public async Task<IActionResult> Index()
        {
              return _context.Membre != null ? 
                          View(await _context.Membre.ToListAsync()) :
                          Problem("Entity set 'LocalDBMSSQLLocalDBContext.Membre'  is null.");
        }

        // GET: Membres/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Membre == null)
            {
                return NotFound();
            }

            var membre = await _context.Membre
                .FirstOrDefaultAsync(m => m.Numero == id);
            if (membre == null)
            {
                return NotFound();
            }

            return View(membre);
        }

        // GET: Membres/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Membres/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Nom,Prenom")] Membre membre)
        {
            membre.Numero = _context.Membre.Max(x => x.Numero) + 1;
            if (ModelState.IsValid)
            {
                _context.Add(membre);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(membre);
        }

        // GET: Membres/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Membre == null)
            {
                return NotFound();
            }

            var membre = await _context.Membre.FindAsync(id);
            if (membre == null)
            {
                return NotFound();
            }
            return View(membre);
        }

        // POST: Membres/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Numero,Nom,Prenom")] Membre membre)
        {
            if (id != membre.Numero)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(membre);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MembreExists(membre.Numero))
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
            return View(membre);
        }

        // GET: Membres/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Membre == null)
            {
                return NotFound();
            }

            var membre = await _context.Membre
                .FirstOrDefaultAsync(m => m.Numero == id);
            if (membre == null)
            {
                return NotFound();
            }

            return View(membre);
        }

        // POST: Membres/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Membre == null)
            {
                return Problem("Entity set 'LocalDBMSSQLLocalDBContext.Membre'  is null.");
            }
            var membre = await _context.Membre.FindAsync(id);
            if (membre != null)
            {
                var prets = _context.Pret.Where(p => p.NumMembre == id);
                if (prets.Count() == 0)
                {
                    _context.Membre.Remove(membre);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["Message"] = "Impossible de supprimer ce membre car un pret est associé";
                    return View(membre);
                }
            }
            return View();
        }

        private bool MembreExists(int id)
        {
          return (_context.Membre?.Any(e => e.Numero == id)).GetValueOrDefault();
        }
    }
}
