using System;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;

namespace Travel.WebApi.Controllers
{
	[ApiController]
    [Route("api/[controller]")]
    public abstract class ApiController : ControllerBase
	{
        private IMediator _mediator = default!;
        protected IMediator Mediator
        {
            get
            {
                if (_mediator != null)
                    return _mediator;
                else
                {
                    var mediator = HttpContext.RequestServices.GetService<IMediator>();
                    if (mediator == null)
                        throw new ArgumentNullException(nameof(mediator));

                    return mediator;
                }
            }
        }
    }
}

