#nullable disable

namespace Domain.Model
{
    public partial class Loan : Account
    {
        public override string ToString() => base.ToString() + " (Loan)";
    }
}
