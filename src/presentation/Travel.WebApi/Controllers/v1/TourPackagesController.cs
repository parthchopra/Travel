using System;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Travel.Data.Contexts;
using Travel.Domain.Entities;

namespace Travel.WebApi.Controllers.v1
{
    [ApiController]
    [Route("api/[controller]")]
    public class TourPackagesController : ApiController
    {
        private readonly TravelDbContext _context;

        public TourPackagesController(TravelDbContext context, IMediator mediator) : base(mediator)
        {
            _context = context;
        }


        [HttpPost]
        public async Task<ActionResult<int>> Create([FromBody] TourPackage tourPackage)
        {
            await _context.TourPackages.AddAsync
              (tourPackage);
            await _context.SaveChangesAsync();
            return Ok(tourPackage);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var tourPackage = await
            _context.TourPackages.SingleOrDefaultAsync(tp => tp.Id == id);
            if (tourPackage == null)
            {
                return NotFound();
            }
            _context.TourPackages.Remove(tourPackage);
            await _context.SaveChangesAsync();
            return Ok(tourPackage);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult>Update([FromRoute] int id, [FromBody] TourPackage tourPackage)
        {
            _context.Update(tourPackage);
            await _context.SaveChangesAsync();
            return Ok(tourPackage);
        }
    }
}

