using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Banking.Core.Entities
{
    public class Account
    {
        public Int32 Id { get; set; }
        
        public Decimal Balance { get; set; }
   
        public string CurrencyCode { get; set; } = "USD";

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

        public virtual Int32 MemberId { get; set; }

        [Timestamp, ConcurrencyCheck]
        public byte[] ConcurrencyStamp { get; private set; }

        private Account(){}

        public Account(Int32 id, Decimal balance, string currencyCode)
        {
            this.Id = id;
            this.Balance = balance;
            this.CurrencyCode = currencyCode;
        }

    }
}
