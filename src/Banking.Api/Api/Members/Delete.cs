using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using MediatR;
using FluentValidation;

using Banking.Persistence.EFCore;
using Banking.Core.Entities;


namespace Banking.Api.Members
{
    public class Delete
    {
        public class Command : IRequest<Int32?>
        {
            public Int32 Id { get; set; }

            public Command(Int32 id)
            {
                this.Id = id;
            }
            public class Validator : AbstractValidator<Command>
            {
                public Validator()
                { 
                    RuleFor(x => x.Id).NotEmpty();
                }
            }
        }


        public class Handler : IRequestHandler<Command, Int32?>
        {
            private readonly ApplicationDbContext _db;
            private readonly ILogger<Handler> _log;

            public Handler(ApplicationDbContext db, ILogger<Handler> log)//,UserManager<ApplicationUser> userManager)
            {
                _db = db;
                _log = log;
            }

            public async Task<Int32?> Handle(Command command, CancellationToken cancellationToken)
            {
                var member = await _db.Members.Include(x=>x.Accounts).SingleOrDefaultAsync(x=> x.Id == command.Id);
                if (member == null)
                    return null;

                _db.Members.Remove(member);
                await _db.SaveChangesAsync();
                return member.Id;
            }
        }


    }
}
