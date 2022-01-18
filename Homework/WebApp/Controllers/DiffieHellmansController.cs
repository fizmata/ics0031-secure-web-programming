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
using Humanizer;

namespace WebApp.Controllers
{
    [Authorize]
    public class DiffieHellmansController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DiffieHellmansController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: DiffieHellmans
        public async Task<IActionResult> Index()
        {
            var applicationDbContext =
                _context.DiffieHellmans
                    .Where(e => e.UserId == User.GetUserId());
            
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: DiffieHellmans/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var diffieHellman = await _context.DiffieHellmans
                .Include(e => e.User)
                .FirstOrDefaultAsync(m => m.Id == id && m.UserId == User.GetUserId());
            if (diffieHellman == null)
            {
                return NotFound();
            }

            return View(diffieHellman);
        }

        // GET: DiffieHellmans/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: DiffieHellmans/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DiffieHellman diffieHellman)
        {
            diffieHellman.UserId = User.GetUserId();
            
            if (diffieHellman.PrivateKeyA > 1)
            {
                var (prime, primitive, serverSecret, ourPartial, theirPartial, sharedOur) = PrimeCrypto.GenerateDH(0, 0,diffieHellman.PrivateKeyA);

                diffieHellman.PrivateKeyB = serverSecret;
                diffieHellman.P = prime;
                diffieHellman.G = primitive;
                diffieHellman.PublicKeyA = theirPartial;
                diffieHellman.PublicKeyB = ourPartial;
                diffieHellman.SymmetricKey = sharedOur;
            }
            else
            {
                return View(diffieHellman);
            }

            if (!ModelState.IsValid) return View(diffieHellman);
            _context.Add(diffieHellman);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: DiffieHellmans/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var diffieHellman = await _context.DiffieHellmans.FirstOrDefaultAsync(e => e.UserId == User.GetUserId() && e.Id == id);
            if (diffieHellman == null)
            {
                return NotFound();
            }
            return View(diffieHellman);
        }

        // POST: DiffieHellmans/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, DiffieHellman diffieHellman)
        {
            if (id != diffieHellman.Id)
            {
                return NotFound();
            }
            
            var isOwner = await _context.DiffieHellmans.AnyAsync(e => e.Id == id && e.UserId == User.GetUserId());
            if (isOwner == false)
            {
                return NotFound();
            }
            
            diffieHellman.UserId = User.GetUserId();

            if (!ModelState.IsValid) return View(diffieHellman);
            try
            {
                _context.Update(diffieHellman);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DiffieHellmanExists(diffieHellman.Id))
                {
                    return NotFound();
                }

                throw;
            }
            return RedirectToAction(nameof(Index));
            //ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", diffieHellman.UserId);
        }

        // GET: DiffieHellmans/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var diffieHellman = await _context.DiffieHellmans
                .Include(e => e.User)
                .FirstOrDefaultAsync(m => m.Id == id && m.UserId == User.GetUserId());
            if (diffieHellman == null)
            {
                return NotFound();
            }

            return View(diffieHellman);
        }

        // POST: DiffieHellmans/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var diffieHellman = await _context.DiffieHellmans.FirstOrDefaultAsync(e => e.Id == id && e.UserId == User.GetUserId());
            _context.DiffieHellmans.Remove(diffieHellman);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DiffieHellmanExists(int id)
        {
            return _context.DiffieHellmans.Any(e => e.Id == id);
        }
    }
}
