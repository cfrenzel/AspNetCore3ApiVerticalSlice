using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

using MediatR;
using FluentValidation;
using AutoMapper;
using AutoMapper.QueryableExtensions;

using Banking.Persistence.EFCore;
using Banking.Core.Entities;

namespace Banking.Api.Members
{
    public class List
    {
        public class Query : IRequest<List<MemberModel>>
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


        public class MemberModel
        {
            public Int32 Id { get; set; }
            public string GivenName { get; set; }
            public string SurName { get; set; }
            public Int32 InstitutionId { get; set; }
            public List<AccountModel> Accounts { get; set; } = new List<AccountModel>();
            public DateTime CreatedAtUtc { get; set; }
        }

        public class AccountModel
        {
            public Int32 Id { get; set; }
            public Decimal Balance { get; set; }
            public string CurrencyCode { get; set; } = "USD";
        }


        public class Mapping : Profile
        {
            public Mapping()
            {
                CreateMap<Member, MemberModel>();
                CreateMap<Account,AccountModel>();
            }
        }

        public class Handler : IRequestHandler<Query, List<MemberModel>>
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

            public async Task<List<MemberModel>> Handle(Query query, CancellationToken cancellationToken)
            {
                return await _db.Members
                    .Include(x=>x.Accounts)
                    .ProjectTo<MemberModel>(_mapper.ConfigurationProvider)
                    .Paginate(query.PageNumber, query.PageSize)
                    .ToListAsync();
            }
        }
 
    
    
    
    }
}

