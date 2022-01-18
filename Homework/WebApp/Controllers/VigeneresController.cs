using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DAL;
using Domain;
using Microsoft.AspNetCore.Authorization;
using WebApp.Helpers;
using ConsoleApp;

namespace WebApp.Controllers
{
    [Authorize]
    public class VigeneresController : Controller
    {
        private readonly ApplicationDbContext _context;

        public VigeneresController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Vigeneres
        public async Task<IActionResult> Index()
        {
            var applicationDbContext =
                _context.Vigeneres
                    .Where(e => e.UserId == User.GetUserId());
            
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Vigeneres/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vigenere = await _context.Vigeneres
                .Include(e => e.User)
                .FirstOrDefaultAsync(m => m.Id == id && m.UserId == User.GetUserId());
            if (vigenere == null)
            {
                return NotFound();
            }

            return View(vigenere);
        }

        // GET: Vigeneres/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Vigeneres/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Vigenere vigenere)
        {
            vigenere.UserId = User.GetUserId();

            if (vigenere.PlainText is not null && vigenere.CipherText is null)
            {
                vigenere.CipherText = ConsoleApp.Program.Cypher( vigenere.Key, vigenere.PlainText, ConsoleApp.Program.OperationType.Encrypt);
            }
            else if (vigenere.PlainText is null && vigenere.CipherText is not null)
            {
                vigenere.PlainText = ConsoleApp.Program.Cypher(vigenere.Key, vigenere.CipherText,
                    ConsoleApp.Program.OperationType.Decrypt);
            }
            else
            {
                return View(vigenere);
            }
            
            if (!ModelState.IsValid) return View(vigenere);
            _context.Add(vigenere);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Vigeneres/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vigenere = await _context.Vigeneres.FirstOrDefaultAsync(e => e.UserId == User.GetUserId() && e.Id == id);
            if (vigenere == null)
            {
                return NotFound();
            }
            return View(vigenere);
        }

        // POST: Vigeneres/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Vigenere vigenere)
        {
            if (id != vigenere.Id)
            {
                return NotFound();
            }
            
            var isOwner = await _context.Vigeneres.AnyAsync(e => e.Id == id && e.UserId == User.GetUserId());
            if (isOwner == false)
            {
                return NotFound();
            }
            
            vigenere.UserId = User.GetUserId();

            if (!ModelState.IsValid) return View(vigenere);
            try
            {
                _context.Update(vigenere);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VigenereExists(vigenere.Id))
                {
                    return NotFound();
                }

                throw;
            }
            return RedirectToAction(nameof(Index));
            //ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", vigenere.UserId);
        }

        // GET: Vigeneres/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vigenere = await _context.Vigeneres
                .Include(e => e.User)
                .FirstOrDefaultAsync(m => m.Id == id && m.UserId == User.GetUserId());
            if (vigenere == null)
            {
                return NotFound();
            }

            return View(vigenere);
        }

        // POST: Vigeneres/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var vigenere = await _context.Vigeneres.FirstOrDefaultAsync(e => e.Id == id && e.UserId == User.GetUserId());
            _context.Vigeneres.Remove(vigenere);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VigenereExists(int id)
        {
            return _context.Vigeneres.Any(e => e.Id == id);
        }
    }
}
