#nullable disable

namespace Domain.Model
{
    public partial class Deposit : Account
    {
        public override string ToString() => base.ToString() + " (Deposit)";
    }
}
