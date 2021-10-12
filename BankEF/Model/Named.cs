using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankEF.Model
{
   public class Named:Guided
    {
        private string name = "Noname";
        public string Name { get => name; set => name = !string.IsNullOrEmpty(value) && !string.IsNullOrEmpty(value.Trim()) ? value : "Noname"; }
        public override string ToString() => Name;
    }
}
