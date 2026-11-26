using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ParcelTrack.Data;
using ParcelTrack.Filters;
using ParcelTrack.Models;
using ParcelTrack.Services;

namespace ParcelTrack.Controllers.Api
{
    [ApiController]
    [Route("api/parcels")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ParcelsApiController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IDeliveryFeeCalculator _feeCalculator;

        public ParcelsApiController(AppDbContext db, IDeliveryFeeCalculator feeCalculator)
        {
            _db = db;
            _feeCalculator = feeCalculator;
        }

        private ParcelDto ToDto(Parcel parcel)
        {
            ParcelDto dto = new ParcelDto();
            dto.Id = parcel.Id;
            dto.TrackingCode = parcel.TrackingCode;
            dto.Recipient = parcel.Recipient;
            dto.WeightKg = parcel.WeightKg;
            dto.Status = parcel.Status;
            dto.BranchId = parcel.BranchId;
            if (parcel.Branch != null)
            {
                dto.BranchName = parcel.Branch.Name;
            }
            dto.DeliveryFee = _feeCalculator.CalculateFee(parcel.WeightKg, parcel.Branch?.Parish);
            return dto;
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult GetAll()
        {
            List<Parcel> parcels = _db.Parcels
                .Include(parcel => parcel.Branch)
                .OrderBy(parcel => parcel.TrackingCode)
                .ToList();

            List<ParcelDto> result = new List<ParcelDto>();
            foreach (Parcel parcel in parcels)
            {
                result.Add(ToDto(parcel));
            }
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet("{id:int}")]
        public IActionResult GetOne(int id)
        {
            Parcel? parcel = _db.Parcels
                .Include(each_parcel => each_parcel.Branch)
                .FirstOrDefault(each_parcel => each_parcel.Id == id);
            if (parcel == null)
            {
                return NotFound();
            }
            return Ok(ToDto(parcel));
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "CanManageParcels")]
        [RequireClient]
        [HttpPost]
        public IActionResult Create(ParcelInput input)
        {
            Parcel parcel = new Parcel();
            parcel.TrackingCode = input.TrackingCode;
            parcel.Recipient = input.Recipient;
            parcel.WeightKg = input.WeightKg;
            parcel.Status = input.Status;
            parcel.BranchId = input.BranchId;

            _db.Parcels.Add(parcel);
            try
            {
                _db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                return Conflict(new { message = "Another parcel already has that tracking code, or there is no such branch" });
            }

            _db.Entry(parcel).Reference(saved_parcel => saved_parcel.Branch).Load();

            return CreatedAtAction(nameof(GetOne), new { id = parcel.Id }, ToDto(parcel));
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "CanManageParcels")]
        [RequireClient]
        [HttpPut("{id:int}")]
        public IActionResult Update(int id, ParcelInput input)
        {
            Parcel? parcel = _db.Parcels.Find(id);
            if (parcel == null)
            {
                return NotFound();
            }
            parcel.TrackingCode = input.TrackingCode;
            parcel.Recipient = input.Recipient;
            parcel.WeightKg = input.WeightKg;
            parcel.Status = input.Status;
            parcel.BranchId = input.BranchId;

            try
            {
                _db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                return Conflict(new { message = "Another parcel already has that tracking code, or there is no such branch" });
            }
            return NoContent();
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        [RequireClient]
        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            Parcel? parcel = _db.Parcels.Find(id);
            if (parcel == null)
            {
                return NotFound();
            }
            _db.Parcels.Remove(parcel);
            _db.SaveChanges();
            return NoContent();
        }
    }
}
