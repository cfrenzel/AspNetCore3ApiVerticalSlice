using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

using MediatR;
using FluentValidation;
using AutoMapper;

using Banking.Persistence.EFCore;
using Banking.Core.Entities;

namespace Banking.Api.Members
{
    public class Create
    {
        public class Command : IRequest<MemberModel>
        {
            public Int32 Id { get; set; }
            public string GivenName { get; set; }
            public string SurName { get; set; }
            public Int32 InstitutionId { get; set; }

            public Command(){}

            public class Validator : AbstractValidator<Command>
            {
                public Validator()
                {
                    CascadeMode = CascadeMode.Stop;
                    RuleFor(x => x.GivenName).NotEmpty();
                    RuleFor(x => x.SurName).NotEmpty();
                    RuleFor(x => x.InstitutionId).NotEmpty();
                    RuleFor(x => x.Id).GreaterThan(1000);
                }
            }
        }

        public class MemberModel
        {
            public Int32 Id { get; set; }
            public string GivenName { get; set; }
            public string SurName { get; set; }
            public Int32 InstitutionId { get; set; }
            public DateTime CreatedAtUtc { get; set; }
        }

        public class Mapping : Profile
        {
            public Mapping()
            {
                CreateMap<Member, MemberModel>();
            }
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
                var inst = _db.Institutions.Find(command.InstitutionId);


                var newMember = new Member(command.Id, command.GivenName, command.SurName, inst);
                _db.Members.Add(newMember);
                await _db.SaveChangesAsync();
                return _mapper.Map<MemberModel>(newMember);
            }
        }


    }
}
