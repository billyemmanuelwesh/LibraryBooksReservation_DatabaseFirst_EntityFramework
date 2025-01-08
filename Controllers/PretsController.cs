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
    public class PretsController : Controller
    {
        private readonly LocalDBMSSQLLocalDBContext _context;

        public PretsController(LocalDBMSSQLLocalDBContext context)
        {
            _context = context;
        }

        // GET: Prets
        public async Task<IActionResult> Index()
        {
            var localDBMSSQLLocalDBContext = _context.Pret.Include(p => p.NumLivreNavigation).Include(p => p.NumMembreNavigation);
            return View(await localDBMSSQLLocalDBContext.ToListAsync());
        }

        // GET: Prets/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Pret == null)
            {
                return NotFound();
            }

            var pret = await _context.Pret
                .Include(p => p.NumLivreNavigation)
                .Include(p => p.NumMembreNavigation)
                .FirstOrDefaultAsync(m => m.Numero == id);
            if (pret == null)
            {
                return NotFound();
            }

            return View(pret);
        }

        // GET: Prets/Create
        public IActionResult Create()
        {
            // on n'affiche pas un livre deja emprumter
            var reqListeAutorise = _context.Livre.Where(x => x.Pret.Count() == 0);
            ViewData["NumLivre"] = new SelectList(reqListeAutorise, "Numero", "Affichage");
           
            // a plus de 3 livres n'affiche pas le membre
            var reqMembreAutorise = _context.Membre.Where(x => x.Pret.Count() < 3);
            ViewData["NumMembre"] = new SelectList(reqMembreAutorise, "Numero", "Affichage");

            return View();
        }

        // POST: Prets/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DateDebut,DateFin,NumMembre,NumLivre")] Pret pret)
        {
            pret.Numero = _context.Pret.Max(x => x.Numero) + 1;
            pret.DateFin = pret.DateDebut.AddDays(7); // la durée d'un prêt est de 7 jours
            if (ModelState.IsValid)
            {

                _context.Add(pret);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }



            // on n'affiche pas un livre deja emprumter
            var reqListeAutorise = _context.Livre.Where(x => x.Pret.Count() == 0);
            ViewData["NumLivre"] = new SelectList(reqListeAutorise, "Numero", "Affichage");

            // a plus de 3 livres n'affiche pas le membre
            var reqMembreAutorise = _context.Membre.Where(x => x.Pret.Count() < 3);
            ViewData["NumMembre"] = new SelectList(reqMembreAutorise, "Numero", "Affichage");
            return View(pret);
        }

        // GET: Prets/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Pret == null)
            {
                return NotFound();
            }

            var pret = await _context.Pret.FindAsync(id);
            if (pret == null)
            {
                return NotFound();
            }

            // on n'affiche pas un livre deja emprumter
            var reqListeAutorise = _context.Livre.Where(x => x.Pret.Count() == 0 || (x.Pret != null && x.Pret.First().NumLivre == pret.NumLivre));
            ViewData["NumLivre"] = new SelectList(reqListeAutorise, "Numero", "Affichage");

            // a plus de 3 livres n'affiche pas le membre
            var reqMembreAutorise = _context.Membre.Where(x => x.Pret.Count() < 3 || (x.Pret != null && x.Pret.First().NumMembre == pret.NumMembre));
            ViewData["NumMembre"] = new SelectList(reqMembreAutorise, "Numero", "Affichage");
            return View(pret);
        }

        // POST: Prets/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("DateDebut,DateFin,NumMembre,NumLivre,Numero")] Pret pret)
        {
            //pret.DateFin = pret.DateDebut.AddDays(7); // la durée d'un prêt est de 7 jours
            if (id != pret.Numero)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(pret);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PretExists(pret.Numero))
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

            // on n'affiche pas un livre deja emprumter
            var reqListeAutorise = _context.Livre.Where(x => x.Pret.Count() == 0);
            ViewData["NumLivre"] = new SelectList(reqListeAutorise, "Numero", "Affichage");

            // a plus de 3 livres n'affiche pas le membre
            var reqMembreAutorise = _context.Membre.Where(x => x.Pret.Count() < 3);
            ViewData["NumMembre"] = new SelectList(reqMembreAutorise, "Numero", "Affichage");
            return View(pret);
        }

        // GET: Prets/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Pret == null)
            {
                return NotFound();
            }

            var pret = await _context.Pret
                .Include(p => p.NumLivreNavigation)
                .Include(p => p.NumMembreNavigation)
                .FirstOrDefaultAsync(m => m.Numero == id);
            if (pret == null)
            {
                return NotFound();
            }

            return View(pret);
        }

        // POST: Prets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Pret == null)
            {
                return Problem("Entity set 'LocalDBMSSQLLocalDBContext.Pret'  is null.");
            }
            var pret = await _context.Pret.FindAsync(id);
            if (pret != null)
            {
                var retard = await _context.Retard.FirstOrDefaultAsync(x => x.NumPret == id);
                if (retard != null)
                {
                    _context.Retard.Remove(retard);
                }
                _context.Pret.Remove(pret);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PretExists(int id)
        {
          return (_context.Pret?.Any(e => e.Numero == id)).GetValueOrDefault();
        }
    }
}
