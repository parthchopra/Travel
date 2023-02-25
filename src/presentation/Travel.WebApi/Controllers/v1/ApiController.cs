using System;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Travel.WebApi.Controllers.v1
{
	//[ApiVersion("1.0")]
	[ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    public abstract class ApiController : ControllerBase
	{
        protected readonly IMediator _mediator;

        public ApiController(IMediator mediator)
        {
            _mediator = mediator;
        }
    }
}

