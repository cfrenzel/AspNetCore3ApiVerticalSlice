using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

using MediatR;
using FluentValidation;
using AutoMapper;

using Banking.Persistence.EFCore;
using Banking.Core.Entities;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Banking.Api.Accounts
{
    public class Transfer
    {
        public class Command : IRequest<Response>
        {
            public Int32 SourceAccountId { get; set; }
            public Int32 BeneficiaryAccountId { get; set; }
            public Decimal TransferAmount { get; set; }

            public Command(){}

            public class Validator : AbstractValidator<Command>
            {
                public Validator()
                {
                    CascadeMode = CascadeMode.Stop;
                    RuleFor(x => x.SourceAccountId).NotEmpty();
                    RuleFor(x => x.BeneficiaryAccountId).NotEmpty();
                    RuleFor(x => x.TransferAmount).GreaterThan(0);
                    RuleFor(x => x.TransferAmount).Must(x => Decimal.Round(x, 2) == x);//no more than 2 decimal places
                }
            }
        }

        public class Response
        {
            public Int32 SourceAccountId { get; set; }
            public Decimal SourceAccountOriginalBalance { get; set; }
            public Decimal SourceAccountBalance { get; set; }
            public Int32 BeneficiaryAccountId { get; set; }
            public Decimal TransferAmount { get; set; }
            public DateTime TransferedAtUtc { get; set; }
        }


        public class Handler : IRequestHandler<Command, Response>
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

            public async Task<Response> Handle(Command command, CancellationToken cancellationToken)
            {
                var sourceAccount = await _db.Accounts.SingleOrDefaultAsync(x => x.Id == command.SourceAccountId);
                var beneficiiaryAccount = await _db.Accounts.SingleOrDefaultAsync(x => x.Id == command.BeneficiaryAccountId);

                if(sourceAccount == null || beneficiiaryAccount == null)
                    ///TODO: convert these with custom exception handlers in middleware or return ActionResult directly
                    throw new InvalidOperationException("Not Found");

                if (!sourceAccount.CurrencyCode.Equals(beneficiiaryAccount.CurrencyCode))
                    throw new InvalidOperationException("Cannot currently transfer between accounts of different currency");

                if (sourceAccount.Balance < command.TransferAmount)
                    throw new InvalidOperationException("Insufficient Funds");

                var origBalance = sourceAccount.Balance;
                sourceAccount.Balance -= command.TransferAmount;
                beneficiiaryAccount.Balance += command.TransferAmount;
                
                //we have optimistic concurrency on the accounts; so if their balance has changed
                //since we read the value, the update will throw a concurency excepton
                await _db.SaveChangesAsync();
                
                return new Response()
                {
                    SourceAccountId = sourceAccount.Id,
                    SourceAccountOriginalBalance = origBalance,
                    SourceAccountBalance = sourceAccount.Balance,
                    BeneficiaryAccountId = beneficiiaryAccount.Id,
                    TransferAmount = command.TransferAmount,
                    TransferedAtUtc = DateTime.UtcNow
                };
            }
        }


    }
}
