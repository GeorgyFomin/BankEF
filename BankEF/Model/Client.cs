using System;
using System.Collections.Generic;

#nullable disable

namespace BankEF.Model
{
    public partial class Client : Named
    {
        public Guid DepartmentId { get; set; }
        private ICollection<Deposit> deposits = new HashSet<Deposit>();
        public virtual ICollection<Deposit> Deposits
        {
            get => deposits; set
            {
                deposits = value ?? new HashSet<Deposit>(); foreach (Deposit deposit in deposits)
                {
                    deposit.ClientId = Id;
                }
            }
        }
        private ICollection<Loan> loans = new HashSet<Loan>();
        public virtual ICollection<Loan> Loans
        {
            get => loans; set
            {
                loans = value ?? new HashSet<Loan>(); foreach (Loan loan in loans)
                {
                    loan.ClientId = Id;
                }
            }
        }
    }
}
