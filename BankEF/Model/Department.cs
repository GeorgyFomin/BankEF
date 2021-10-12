using System;
using System.Collections.Generic;

#nullable disable

namespace BankEF.Model
{
    public partial class Department : Named
    {
        private ICollection<Client> clients = new HashSet<Client>();
        public virtual ICollection<Client> Clients
        {
            get => clients; set
            {
                clients = value ?? new HashSet<Client>(); foreach (Client client in clients)
                {
                    client.DepartmentId = Id;
                }
            }
        }
    }
}
