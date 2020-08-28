using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;

namespace Banking.Core.Entities
{
    public class Member
    {
        public Int32 Id { get; set; }
        public string GivenName { get; set; }
        public string SurName { get; set; }

    
        public Int32 InstitutionId  { get; set; }
        public Institution Institution { get; set; }

        public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

        private Member() {}
        public Member(int id, string givenName, string surName, Institution inst)
        {
            this.Id = id;
            this.GivenName = givenName;
            this.SurName = surName;
            this.Institution = inst;
        }

        public void AddAccount(Account account)
        {
            this.Accounts.Add(account);
        }

    }
}
