using System;

namespace Domain.Model
{
    public class Account : Ided
    {
        public const decimal MinSize = .01m;
        public int Number { get; private set; }
        public virtual Client Client { get; set; }
        private decimal size = MinSize;
        public decimal Size { get => size; set => size = value < MinSize ? MinSize : value; }
        public double Rate { get; set; }
        public bool Cap { get; set; }
        public Account() => Number = Math.Abs(GetHashCode());
        public override string ToString() => (Client != null ? Client.ToString() : string.Empty) + ";Account №" + $"{Number};Size {Size:C2};Rate {Rate:g4};Cap {Cap}";
    }
}
