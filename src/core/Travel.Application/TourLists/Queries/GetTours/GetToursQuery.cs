using System;
using AutoMapper;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Travel.Application.Common.Interfaces;
using Travel.Application.Dtos.Tour;
using AutoMapper.QueryableExtensions;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Travel.Application.TourLists.Queries.GetTours
{
	public class GetToursQuery : IRequest<ToursVm> { }

    public class GetToursQueryHandler : IRequestHandler<GetToursQuery, ToursVm>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;

        public GetToursQueryHandler(IApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ToursVm> Handle(GetToursQuery request, CancellationToken cancellationToken)
        {
            return new ToursVm
            {
                Lists = await _context.TourLists
                .ProjectTo<TourListDto>(_mapper.ConfigurationProvider)
                .OrderBy(t => t.City)
                .ToListAsync(cancellationToken)
            };
        }
    }
}

