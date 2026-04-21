namespace Library.Domain
{
    public class BookRecord
    {
        public string ISBN { get; set; }
        public string title { get; set; }
        public int year { get; set; }
        public decimal rating { get; set; }
        public string Autor { get; set; }
        public bool esValido { get; set; }
        public string error { get; set; }

    }
}
