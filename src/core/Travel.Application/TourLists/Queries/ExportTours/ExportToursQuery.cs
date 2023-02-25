using System;
using AutoMapper;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Travel.Application.Common.Interfaces;
using System.Linq;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace Travel.Application.TourLists.Queries.ExportTours
{
	public class ExportToursQuery : IRequest<ExportToursVm>
	{
        public int ListId { get; set; }
    }

    public class ExportToursQueryHandler : IRequestHandler<ExportToursQuery, ExportToursVm>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ICsvFileBuilder _fileBuilder;

        public ExportToursQueryHandler(IApplicationDbContext context, IMapper mapper, ICsvFileBuilder fileBuilder)
        {
            _context = context;
            _mapper = mapper;
            _fileBuilder = fileBuilder;
        }

        public async Task<ExportToursVm> Handle(ExportToursQuery request, CancellationToken cancellationToken)
        {
            var vm = new ExportToursVm();

            var records = await _context.TourPackages
              .Where(t => t.ListId == request.ListId)
              .ProjectTo<TourPackageRecord>(_mapper.ConfigurationProvider)
              .ToListAsync(cancellationToken);

            vm.Content = _fileBuilder.BuildTourPackagesFile(records);
            vm.ContentType = "text/csv";
            vm.FileName = "TourPackages.csv";

            return await Task.FromResult(vm);
        }
    }
}

