using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankEF.Model
{
    public class Account : Guided
    {
        public const decimal MinSize = .01m;
        public int Number { get; private set; }
        public Guid ClientId { get; set; }
        private decimal size = MinSize;
        public decimal Size { get => size; set => size = value < MinSize ? MinSize : value; }
        public double Rate { get; set; }
        public bool Cap { get; set; }
        public Account() => Number = GetHashCode();

    }
}
