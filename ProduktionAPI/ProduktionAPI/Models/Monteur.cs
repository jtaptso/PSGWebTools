namespace ProduktionAPI.Models
{
    public class Monteur
    {
        public string Name { get; set; }
        public int Code { get; set; }
        public int KeyNummer { get; set; }
        public string Brand { get; set; }
        public string UserTyp { get; set; }
        public bool UserExists { get; set; } = false;
    }
}
