using System;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Travel.Application.TourLists.Commands.CreateTourList;
using Travel.Application.TourLists.Commands.DeleteTourList;
using Travel.Application.TourLists.Commands.UpdateTourList;
using Travel.Application.TourLists.Queries.ExportTours;
using Travel.Application.TourLists.Queries.GetTours;
using Travel.Data.Contexts;
using Travel.Domain.Entities;

namespace Travel.WebApi.Controllers.v1
{    
    public class TourListsController : ApiController
	{
        [HttpGet]
        public async Task<ActionResult<ToursVm>> Get() => await Mediator.Send(new GetToursQuery());

        [HttpGet("{id}")]
        public async Task<FileResult> Get(int id)
        {
            var vm = await Mediator.Send(new ExportToursQuery { ListId = id });

            return File(vm.Content, vm.ContentType, vm.FileName);
        }

        [HttpPost]
        public async Task<ActionResult<int>> Create(CreateTourListCommand command) => await Mediator.Send(command);

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, UpdateTourListCommand command)
        {
            if (id != command.Id)
                return BadRequest();

            await Mediator.Send(command);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            await Mediator.Send(new DeleteTourListCommand { Id = id });

            return NoContent();
        }
    }
}

