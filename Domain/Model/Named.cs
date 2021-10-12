namespace Domain.Model
{
    public class Named:Guided
    {
        private string name = "Noname";
        public string Name { get => name; set => name = !string.IsNullOrEmpty(value) && !string.IsNullOrEmpty(value.Trim()) ? value : "Noname"; }
        public override string ToString() => Name;
    }
}
