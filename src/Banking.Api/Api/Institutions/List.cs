using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

using MediatR;
using FluentValidation;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

using Banking.Persistence.EFCore;
using Banking.Core.Entities;


namespace Banking.Api.Institutions
{
    public class List
    {
        public class Query : IRequest<List<InstitutionModel>>
        {
            public Int32? PageNumber { get; set; } = 1;
            public Int32? PageSize { get; set; } = 50;

            public Query() { }

            public class Validator : AbstractValidator<Query>
            {
                public Validator()
                {
                    CascadeMode = CascadeMode.Stop;
                    RuleFor(x => x.PageNumber).GreaterThan(0);
                    RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
                }
            }

        }


        public class InstitutionModel
        {
            public Int32 Id { get; set; }
            public string Name { get; set; }
        }

        public class Mapping : Profile
        {
            public Mapping()
            {
                CreateMap<Institution, InstitutionModel>();
            }
        }

        public class Handler : IRequestHandler<Query, List<InstitutionModel>>
        {
            private readonly ApplicationDbContext _db;
            private readonly ILogger<Handler> _log;
            private readonly IMapper _mapper;

            public Handler(ApplicationDbContext db, IMapper mapper, ILogger<Handler> log)
            {
                _db = db;
                _mapper = mapper;
                _log = log;
            }

            public async Task<List<InstitutionModel>> Handle(Query query, CancellationToken cancellationToken)
            {
                return await _db.Institutions
                    .ProjectTo<InstitutionModel>(_mapper.ConfigurationProvider)
                    .Paginate(query.PageNumber, query.PageSize)
                    .ToListAsync();
            }
        }




    }
}

