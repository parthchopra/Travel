using System;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Travel.Application.TourPackages.Commands.CreateTourPackage;
using Travel.Application.TourPackages.Commands.DeleteTourPackage;
using Travel.Application.TourPackages.Commands.UpdateTourPackage;
using Travel.Application.TourPackages.Commands.UpdateTourPackageDetail;
using Travel.Data.Contexts;
using Travel.Domain.Entities;

namespace Travel.WebApi.Controllers.v1
{    
    public class TourPackagesController : ApiController
    {
        [HttpPost("{id}")]
        public async Task<ActionResult<int>> Create([FromBody] CreateTourPackageCommand command) => await Mediator.Send(command);

        [HttpPut("{id}")]
        public async Task<ActionResult> Update([FromRoute]int id, [FromBody] UpdateTourPackageCommand command)
        {
            if (id != command.Id)
                return BadRequest();

            await Mediator.Send(command);

            return NoContent();
        }

        [HttpPut("[action]")]
        public async Task<ActionResult> UpdateItemDetails(int id, UpdateTourPackageDetailCommand command)
        {
            if (id != command.Id)
                return BadRequest();

            await Mediator.Send(command);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            await Mediator.Send(new DeleteTourPackageCommand { Id = id });

            return NoContent();
        }
    }
}

