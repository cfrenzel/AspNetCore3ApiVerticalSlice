using System;
using System.Threading;
using System.Threading.Tasks;
using System.IO.IsolatedStorage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

using MediatR;
using FluentValidation;
using AutoMapper;

using Banking.Persistence.EFCore;
using Banking.Core.Entities;

namespace Banking.Api.Members
{
    public class Update
    {
        public class Command : IRequest<MemberModel>
        {
            public Int32 Id { get; set; }
            public string GivenName { get; set; }
            public string SurName { get; set; }

            public Command(){}
            
            public Command(Int32 id, string givenName, string surName)
            {
                this.Id = id;
                this.GivenName = givenName;
                this.SurName = surName;
            }
            public class Validator : AbstractValidator<Command>
            {
                public Validator()
                {
                    CascadeMode = CascadeMode.Stop;
                    RuleFor(x => x.GivenName).NotEmpty();
                    RuleFor(x => x.SurName).NotEmpty();
                    RuleFor(x => x.Id).NotEmpty();
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


        public class Handler : IRequestHandler<Command, MemberModel>
        {
            private readonly ApplicationDbContext _db;
            private readonly ILogger<Handler> _log;
            private readonly IMapper _mapper;

            public Handler(ApplicationDbContext db, IMapper mapper, ILogger<Handler> log)//,UserManager<ApplicationUser> userManager)
            {
                _db = db;
                _mapper = mapper;
                _log = log;
            }

            public async Task<MemberModel> Handle(Command command, CancellationToken cancellationToken)
            {
                var member = await _db.Members
                    .Include(x=>x.Accounts)
                    .SingleOrDefaultAsync(x=> x.Id == command.Id);
                if (member == null)
                    return null;

                member.SurName = command.SurName;
                member.GivenName = command.GivenName;
                await _db.SaveChangesAsync();
                return _mapper.Map<MemberModel>(member);
                
            }
        }


    }
}
