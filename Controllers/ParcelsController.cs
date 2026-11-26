using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ParcelTrack.Data;
using ParcelTrack.Models;
using ParcelTrack.Services;

namespace ParcelTrack.Controllers
{
    // The parcel pages: list, details, add, edit, delete.
    // Visitors may look. Staff and Admin add and edit; only the Admin deletes.
    public class ParcelsController : Controller
    {
        private readonly AppDbContext _db;
        private readonly IDeliveryFeeCalculator _feeCalculator;

        public ParcelsController(AppDbContext db, IDeliveryFeeCalculator feeCalculator)
        {
            _db = db;
            _feeCalculator = feeCalculator;
        }

        // The two dropdowns: every branch A to Z, and the four statuses
        private void LoadChoices()
        {
            List<Branch> branches = _db.Branches.OrderBy(branch => branch.Name).ToList();
            ViewBag.Branches = new SelectList(branches, "Id", "Name");
            string[] statuses = { "Received", "In transit", "Ready for pickup", "Delivered" };
            ViewBag.Statuses = new SelectList(statuses);
        }

        // GET /parcels: every parcel, sorted by tracking code, each with its branch
        [HttpGet("parcels")]
        public IActionResult Index()
        {
            ViewData["Title"] = "Parcels";
            List<Parcel> parcels = _db.Parcels
                .Include(parcel => parcel.Branch)
                .OrderBy(parcel => parcel.TrackingCode)
                .ToList();
            return View(parcels);
        }

        // GET /parcels/2: one parcel. /parcels/99 is a 404.
        [HttpGet("parcels/{id:int}")]
        public IActionResult Details(int id)
        {
            Parcel? parcel = _db.Parcels
                .Include(each_parcel => each_parcel.Branch)
                .FirstOrDefault(each_parcel => each_parcel.Id == id);
            if (parcel == null)
            {
                return NotFound();
            }
            ViewData["Title"] = parcel.TrackingCode;
            ViewData["DeliveryFee"] = _feeCalculator.CalculateFee(parcel.WeightKg, parcel.Branch?.Parish);
            return View(parcel);
        }

        // GET /parcels/add: an empty form. Staff or Admin only.
        [Authorize(Policy = "CanManageParcels")]
        [HttpGet("parcels/add")]
        public IActionResult Add()
        {
            ViewData["Title"] = "Add a parcel";
            LoadChoices();
            return View(new Parcel());
        }

        // POST /parcels/add: save the new parcel, or show the form again with the messages
        [Authorize(Policy = "CanManageParcels")]
        [HttpPost("parcels/add")]
        [ValidateAntiForgeryToken]
        public IActionResult Add(Parcel new_parcel)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Title"] = "Add a parcel";
                LoadChoices();
                return View(new_parcel);
            }

            _db.Parcels.Add(new_parcel);
            try
            {
                _db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                // The unique index on TrackingCode said no
                ModelState.AddModelError("TrackingCode", "Another parcel already has that tracking code");
                ViewData["Title"] = "Add a parcel";
                LoadChoices();
                return View(new_parcel);
            }

            TempData["Message"] = $"{new_parcel.TrackingCode} for {new_parcel.Recipient} was added";
            return RedirectToAction("Index");
        }

        // GET /parcels/edit/2: the form, filled in
        [Authorize(Policy = "CanManageParcels")]
        [HttpGet("parcels/edit/{id:int}")]
        public IActionResult Edit(int id)
        {
            Parcel? parcel = _db.Parcels.Find(id);
            if (parcel == null)
            {
                return NotFound();
            }
            ViewData["Title"] = "Edit a parcel";
            LoadChoices();
            return View(parcel);
        }

        // POST /parcels/edit/2: save the changes to the same row
        [Authorize(Policy = "CanManageParcels")]
        [HttpPost("parcels/edit/{id:int}")]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Parcel changed_parcel)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Title"] = "Edit a parcel";
                LoadChoices();
                return View(changed_parcel);
            }

            Parcel? parcel = _db.Parcels.Find(id);
            if (parcel == null)
            {
                TempData["Message"] = "That parcel was deleted before you saved";
                return RedirectToAction("Index");
            }
            parcel.TrackingCode = changed_parcel.TrackingCode;
            parcel.Recipient = changed_parcel.Recipient;
            parcel.WeightKg = changed_parcel.WeightKg;
            parcel.Status = changed_parcel.Status;
            parcel.BranchId = changed_parcel.BranchId;

            try
            {
                _db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("TrackingCode", "Another parcel already has that tracking code");
                ViewData["Title"] = "Edit a parcel";
                LoadChoices();
                return View(changed_parcel);
            }

            TempData["Message"] = $"Saved {parcel.TrackingCode}";
            return RedirectToAction("Index");
        }

        // GET /parcels/delete/2: ask first. Admin only.
        [Authorize(Roles = "Admin")]
        [HttpGet("parcels/delete/{id:int}")]
        public IActionResult Delete(int id)
        {
            Parcel? parcel = _db.Parcels.Find(id);
            if (parcel == null)
            {
                return NotFound();
            }
            ViewData["Title"] = "Delete a parcel";
            return View(parcel);
        }

        // POST /parcels/delete/2: the Yes button
        [Authorize(Roles = "Admin")]
        [HttpPost("parcels/delete/{id:int}")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            Parcel? parcel = _db.Parcels.Find(id);
            if (parcel == null)
            {
                TempData["Message"] = "That parcel was already deleted";
                return RedirectToAction("Index");
            }
            _db.Parcels.Remove(parcel);
            _db.SaveChanges();
            TempData["Message"] = $"{parcel.TrackingCode} was deleted";
            return RedirectToAction("Index");
        }
    }
}
