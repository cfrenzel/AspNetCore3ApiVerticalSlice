using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MediatR;
using FluentValidation;

using Banking.Persistence.EFCore;
using Banking.Core.Entities;
using AutoMapper;

namespace Banking.Api.Institutions
{
    public class Create
    {
        public class Command : IRequest<InstitutionModel>
        {
            public Int32 Id { get; set; }
            public string Name { get; set; }

            public Command() { }

            public class Validator : AbstractValidator<Command>
            {
                public Validator()
                {
                    CascadeMode = CascadeMode.Stop;
                    RuleFor(x => x.Name).NotEmpty();
                    RuleFor(x => x.Id).GreaterThan(1000);
                }
            }
        }

        public class InstitutionModel
        {
            public Int32 Id { get; set; }
            public string Name { get; set; }
            public DateTime CreatedAtUtc { get; set; }
        }

        public class Mapping : Profile
        {
            public Mapping()
            {
                CreateMap<Institution, InstitutionModel>();
            }
        }

        public class Handler : IRequestHandler<Command, InstitutionModel>
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

            public async Task<InstitutionModel> Handle(Command command, CancellationToken cancellationToken)
            {
                var newInstitution = new Institution(command.Id, command.Name);
                _db.Institutions.Add(newInstitution);
                await _db.SaveChangesAsync();
                
                return _mapper.Map<InstitutionModel>(newInstitution);
            }
        }


    }
}
