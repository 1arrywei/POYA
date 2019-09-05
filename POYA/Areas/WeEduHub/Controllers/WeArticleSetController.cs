using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using POYA.Areas.iEduHub.Models;
using POYA.Data;

namespace POYA.Areas.WeEduHub.Controllers
{
    [Area("WeEduHub")]
    public class WeArticleSetController : Controller
    {
        private readonly ApplicationDbContext _context;

        public WeArticleSetController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: WeEduHub/WeArticleSet
        public async Task<IActionResult> Index()
        {
            return View(await _context.WeArticleSet.ToListAsync());
        }

        // GET: WeEduHub/WeArticleSet/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var weArticleSet = await _context.WeArticleSet
                .FirstOrDefaultAsync(m => m.Id == id);
            if (weArticleSet == null)
            {
                return NotFound();
            }

            return View(weArticleSet);
        }

        // GET: WeEduHub/WeArticleSet/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: WeEduHub/WeArticleSet/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UserId,Name,Description,DOCreating")] WeArticleSet weArticleSet)
        {
            if (ModelState.IsValid)
            {
                weArticleSet.Id = Guid.NewGuid();
                _context.Add(weArticleSet);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(weArticleSet);
        }

        // GET: WeEduHub/WeArticleSet/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var weArticleSet = await _context.WeArticleSet.FindAsync(id);
            if (weArticleSet == null)
            {
                return NotFound();
            }
            return View(weArticleSet);
        }

        // POST: WeEduHub/WeArticleSet/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,UserId,Name,Description,DOCreating")] WeArticleSet weArticleSet)
        {
            if (id != weArticleSet.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(weArticleSet);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WeArticleSetExists(weArticleSet.Id))
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
            return View(weArticleSet);
        }

        // GET: WeEduHub/WeArticleSet/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var weArticleSet = await _context.WeArticleSet
                .FirstOrDefaultAsync(m => m.Id == id);
            if (weArticleSet == null)
            {
                return NotFound();
            }

            return View(weArticleSet);
        }

        // POST: WeEduHub/WeArticleSet/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var weArticleSet = await _context.WeArticleSet.FindAsync(id);
            _context.WeArticleSet.Remove(weArticleSet);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool WeArticleSetExists(Guid id)
        {
            return _context.WeArticleSet.Any(e => e.Id == id);
        }
    }
}
