using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

using MediatR;
using FluentValidation;
using AutoMapper.QueryableExtensions;

using Banking.Persistence.EFCore;
using Banking.Core.Entities;
using AutoMapper;
using System.Security.Cryptography.X509Certificates;

namespace Banking.Api.Institutions
{
    public class Find
    {
        public class Query : IRequest<InstitutionModel>
        {
            public Int32 Id { get; set; }

            public Query() { }
            public Query(Int32 id)
            {
                Id = id;
            }
            public class Validator : AbstractValidator<Query>
            {
                public Validator()
                {
                    CascadeMode = CascadeMode.Stop;
                    RuleFor(x => x.Id).NotEmpty();
                }
            }
        }

        public class InstitutionModel
        {
            public Int32 Id { get; set; }
            public string Name { get; set; }
            public DateTime CreatedAtUtc { get; set; }
        }

 
        public class Handler : IRequestHandler<Query, InstitutionModel>
        {
            private readonly ApplicationDbContext _db;
            private readonly ILogger<Handler> _log;
            private readonly IMapper _mapper;

            public Handler(ApplicationDbContext db, IMapper mapper, ILogger<Handler> log){
                _db = db;
                _mapper = mapper;
                _log = log;
            }

            public async Task<InstitutionModel> Handle(Query query, CancellationToken cancellationToken)
            {
                return await _db.Institutions
                    .ProjectTo<InstitutionModel>(_mapper.ConfigurationProvider)
                    .SingleOrDefaultAsync(x => x.Id == query.Id);
            }
        }
 
    
    
    
    }
}

