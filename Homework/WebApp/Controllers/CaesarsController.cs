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
    public class CaesarsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CaesarsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Caesars
        public async Task<IActionResult> Index()
        {
            var applicationDbContext =
                _context.Caesars
                    .Where(e => e.UserId == User.GetUserId());
            
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Caesars/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var caesar = await _context.Caesars
                .Include(e => e.User)
                .FirstOrDefaultAsync(m => m.Id == id && m.UserId == User.GetUserId());
            if (caesar == null)
            {
                return NotFound();
            }

            return View(caesar);
        }

        // GET: Caesars/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Caesars/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Caesar caesar)
        {
            caesar.UserId = User.GetUserId();

            if (caesar.PlainText is not null && caesar.CipherText is null)
            {
                 caesar.CipherText = ConsoleApp.Program.CesarCypher(caesar.Key, caesar.PlainText, ConsoleApp.Program.OperationType.Encrypt);
            }
            else if (caesar.PlainText is null && caesar.CipherText is not null) 
            {
                caesar.PlainText = ConsoleApp.Program.CesarCypher(caesar.Key, caesar.CipherText, ConsoleApp.Program.OperationType.Decrypt); 
            }
            else
            {
                return View(caesar);
            }
            
            if (!ModelState.IsValid) return View(caesar);
            _context.Add(caesar);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Caesars/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var caesar = await _context.Caesars.FirstOrDefaultAsync(e => e.UserId == User.GetUserId() && e.Id == id);
            if (caesar == null)
            {
                return NotFound();
            }
            return View(caesar);
        }

        // POST: Caesars/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Caesar caesar)
        {
            if (id != caesar.Id)
            {
                return NotFound();
            }
            
            var isOwner = await _context.Caesars.AnyAsync(e => e.Id == id && e.UserId == User.GetUserId());
            if (isOwner == false)
            {
                return NotFound();
            }
            
            caesar.UserId = User.GetUserId();

            if (!ModelState.IsValid) return View(caesar);
            try
            {
                _context.Update(caesar);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CaesarExists(caesar.Id))
                {
                    return NotFound();
                }

                throw;
            }
            return RedirectToAction(nameof(Index));
            //ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", caesar.UserId);
        }

        // GET: Caesars/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var caesar = await _context.Caesars
                .Include(e => e.User)
                .FirstOrDefaultAsync(m => m.Id == id && m.UserId == User.GetUserId());
            if (caesar == null)
            {
                return NotFound();
            }

            return View(caesar);
        }

        // POST: Caesars/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var caesar = await _context.Caesars.FirstOrDefaultAsync(e => e.Id == id && e.UserId == User.GetUserId());
            _context.Caesars.Remove(caesar);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CaesarExists(int id)
        {
            return _context.Caesars.Any(e => e.Id == id);
        }
    }
}
