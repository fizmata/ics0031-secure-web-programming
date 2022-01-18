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
    public class RsasController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RsasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Rsas
        public async Task<IActionResult> Index()
        {
            var applicationDbContext =
                _context.Rsas
                    .Where(e => e.UserId == User.GetUserId());
            
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Rsas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rsa = await _context.Rsas
                .Include(e => e.User)
                .FirstOrDefaultAsync(m => m.Id == id && m.UserId == User.GetUserId());
            if (rsa == null)
            {
                return NotFound();
            }

            return View(rsa);
        }

        // GET: Rsas/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Rsas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Rsa rsa)
        {
            rsa.UserId = User.GetUserId();
            
            if (rsa.PlainText is not null && rsa.CipherText is null)
            {
                if (rsa.N > 1 && rsa.E > 1)
                {
                    rsa.CipherText = PrimeCrypto.RsaEnc(rsa.PlainText, (int)rsa.E, (int)rsa.N);
                }
                else
                {
                    var rsaArr = PrimeCrypto.GenerateRSA();
                    rsa.N = rsaArr[0];
                    rsa.E = rsaArr[1];
                    rsa.D = rsaArr[2];
                    rsa.CipherText = PrimeCrypto.RsaEnc(rsa.PlainText, (int)rsa.E, (int)rsa.N);
                }
            }
            else if (rsa.PlainText is null && rsa.CipherText is not null)
            {
                if (rsa.N > 1 && rsa.D > 1)
                {
                    rsa.PlainText = PrimeCrypto.RsaDec(rsa.CipherText
                        .Replace(" ", string.Empty), (int)rsa.D, (int)rsa.N);
                }
                else
                {
                    return View(rsa);
                }
            }
            else
            {
                return View(rsa);
            }

            if (!ModelState.IsValid) return View(rsa);
            _context.Add(rsa);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Rsas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rsa = await _context.Rsas.FirstOrDefaultAsync(e => e.UserId == User.GetUserId() && e.Id == id);
            if (rsa == null)
            {
                return NotFound();
            }
            return View(rsa);
        }

        // POST: Rsas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Rsa rsa)
        {
            if (id != rsa.Id)
            {
                return NotFound();
            }
            
            var isOwner = await _context.Rsas.AnyAsync(e => e.Id == id && e.UserId == User.GetUserId());
            if (isOwner == false)
            {
                return NotFound();
            }
            
            rsa.UserId = User.GetUserId();

            if (!ModelState.IsValid) return View(rsa);
            try
            {
                _context.Update(rsa);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RsaExists(rsa.Id))
                {
                    return NotFound();
                }

                throw;
            }
            return RedirectToAction(nameof(Index));
            //ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", rsa.UserId);
        }

        // GET: Rsas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rsa = await _context.Rsas
                .Include(e => e.User)
                .FirstOrDefaultAsync(m => m.Id == id && m.UserId == User.GetUserId());
            if (rsa == null)
            {
                return NotFound();
            }

            return View(rsa);
        }

        // POST: Rsas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var rsa = await _context.Rsas.FirstOrDefaultAsync(e => e.Id == id && e.UserId == User.GetUserId());
            _context.Rsas.Remove(rsa);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RsaExists(int id)
        {
            return _context.Rsas.Any(e => e.Id == id);
        }
    }
}
