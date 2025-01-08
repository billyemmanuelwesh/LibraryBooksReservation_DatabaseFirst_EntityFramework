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
    public class LivresController : Controller
    {
        private readonly LocalDBMSSQLLocalDBContext _context;
        public String[] MesCategories = new String[] { "", "Enfant(-14 ans)", "Adulte(+18 ans)", "Jeunesse (+14 ans)" };

        public LivresController(LocalDBMSSQLLocalDBContext context)
        {
            _context = context;
        }

        // GET: Livres
        public async Task<IActionResult> Index()
        {
            ViewBag.MesCategories = MesCategories; // pour afficher : Enfant, Adulte, Jenesse à la place de 1, 2, 3 ect...
            return _context.Livre != null ? 
                          View(await _context.Livre.ToListAsync()) :
                          Problem("Entity set 'LocalDBMSSQLLocalDBContext.Livre'  is null.");
        }

        // GET: Livres/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            ViewBag.MesCategories = MesCategories;

            if (id == null || _context.Livre == null)
            {
                return NotFound();
            }

            var livre = await _context.Livre
                .FirstOrDefaultAsync(m => m.Numero == id);
            if (livre == null)
            {
                return NotFound();
            }

            return View(livre);
        }

        // GET: Livres/Create
        public IActionResult Create()
        {
            ViewBag.MesCategories = MesCategories;

            return View();
        }

        // POST: Livres/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Titre,Auteur,Categories")] Livre livre)
        {
            ViewBag.MesCategories = MesCategories;

            livre.Numero = _context.Livre.Max(x => x.Numero) + 1;
            if (ModelState.IsValid)
            {
                _context.Add(livre);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(livre);
        }

        // GET: Livres/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            ViewBag.MesCategories = MesCategories;

            if (id == null || _context.Livre == null)
            {
                return NotFound();
            }

            var livre = await _context.Livre.FindAsync(id);
            if (livre == null)
            {
                return NotFound();
            }
            return View(livre);
        }

        // POST: Livres/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Numero,Titre,Auteur,Categories")] Livre livre)
        {
            ViewBag.MesCategories = MesCategories;

            if (id != livre.Numero)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(livre);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LivreExists(livre.Numero))
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
            return View(livre);
        }

        // GET: Livres/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {

            if (id == null || _context.Livre == null)
            {
                return NotFound();
            }

            var livre = await _context.Livre
                .FirstOrDefaultAsync(m => m.Numero == id);
            if (livre == null)
            {
                return NotFound();
            }

            return View(livre);
        }

        // POST: Livres/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Livre == null)
            {
                return Problem("Entity set 'LocalDBMSSQLLocalDBContext.Livre'  is null.");
            }
            var livre = await _context.Livre.FindAsync(id);
            if (livre != null)
            {
                var prets = _context.Pret.Where(p => p.NumLivre == id);
                if (prets.Count() == 0)
                {
                    _context.Livre.Remove(livre);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["Message"] = "Impossible de supprimer ce Livre car un pret est associé";
                    return View(livre);
                }
            }
            return View();
        }

            private bool LivreExists(int id)
        {
          return (_context.Livre?.Any(e => e.Numero == id)).GetValueOrDefault();
        }
    }
}
