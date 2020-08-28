//using System;
//using System.Threading;
//using System.Threading.Tasks;
//using Microsoft.Extensions.Logging;
//using MediatR;
//using FluentValidation;

//using Banking.Persistence.EFCore;
//using Banking.Core.Entities;
//using System.IO.IsolatedStorage;
//using Microsoft.EntityFrameworkCore;

//namespace Banking.Application.Accounts
//{
//    public class CreateAccount
//    {
//        public class Command : IRequest<Account>
//        {
//            public Int32 Id { get; set; }

//            public Decimal Balance { get; set; }

//            public string CurrencyCode { get; set; }

//            public Int32 MemberId { get; set; }
            
//            public class Validator : AbstractValidator<Command>
//            {
//                public Validator()
//                {
//                    CascadeMode = CascadeMode.Stop;
//                    RuleFor(x => x.Balance).GreaterThan(0M);
//                    RuleFor(x => x.CurrencyCode).NotEmpty().Must(x=>x == "USD");
//                    RuleFor(x => x.MemberId).NotEmpty();
//                    RuleFor(x => x.Id).GreaterThan(1000);
//                }
//            }
//        }


//        public class Handler : IRequestHandler<Command, Account>
//        {
//            private readonly ApplicationDbContext _db;
//            private readonly ILogger<Handler> _log;

//            public Handler(ApplicationDbContext db, ILogger<Handler> log)//,UserManager<ApplicationUser> userManager)
//            {
//                _db = db;
//                _log = log;
//            }

//            public async Task<Account> Handle(Command command, CancellationToken cancellationToken)
//            {
//                var member = await _db.Members.Include(x=>x.Accounts).SingleOrDefaultAsync(x=> x.Id == command.MemberId);
//                var newAccount = new Account(command.Id, command.Balance, command.CurrencyCode);
//                member.AddAccount(newAccount);
//                await _db.SaveChangesAsync();
//                return newAccount;
                
//            }
//        }


//    }
//}
