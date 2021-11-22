using System.Collections.Generic;

#nullable disable

namespace Domain.Model
{
    public partial class Department : Named
    {
        private ICollection<Client> clients = new HashSet<Client>();
        public virtual ICollection<Client> Clients
        {
            get => clients; set
            {
                clients = value ?? new HashSet<Client>(); 
                foreach (Client client in clients)
                {
                    client.Department = this;
                }
            }
        }
        public override string ToString() => "Department " + Name;
    }
}
