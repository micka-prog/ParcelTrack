using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ParcelTrack.Data;
using ParcelTrack.Models;

namespace ParcelTrack.Controllers
{
    // One branch and the parcels going to it: the link walked the other way.
    public class BranchesController : Controller
    {
        private readonly AppDbContext _db;

        public BranchesController(AppDbContext db)
        {
            _db = db;
        }

        // GET /branches/5: the branch, with its parcels (Include). /branches/99 is a 404.
        [HttpGet("branches/{id:int}")]
        public IActionResult Details(int id)
        {
            Branch? branch = _db.Branches
                .Include(each_branch => each_branch.Parcels)
                .FirstOrDefault(each_branch => each_branch.Id == id);
            if (branch == null)
            {
                return NotFound();
            }
            ViewData["Title"] = branch.Name;
            return View(branch);
        }
    }
}
