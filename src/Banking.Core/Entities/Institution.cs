using System;
using System.Runtime.InteropServices.ComTypes;

namespace Banking.Core.Entities
{
    public class Institution
    {
        public Int32 Id { get; set; }
        
        public string Name { get; set; }

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        
        private Institution(){}
        
        public Institution(int id, string name)
        {
            if (id < 1)
                throw new ArgumentException("Id must be positive");
            
            this.Id = id;

            if (String.IsNullOrEmpty(name))
                throw new ArgumentException("Name is requiired");

            this.Name = name;
        }

      

    }
}
