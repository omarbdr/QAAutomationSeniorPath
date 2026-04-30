using Microsoft.VisualBasic;

namespace Library.Domain
{
    public class BookRecord
    {
        public string ISBN { get; set; }
        public string Title { get; set; }
        public int Year { get; set; }
        public decimal Rating { get; set; }
        public string Author { get; set; }
        public int Id { get; set; }
        public bool OnLoan { get; set; }
        public DateTime DueDate { get; set; } = DateTime.Now.AddDays(7); // Establece la fecha de vencimiento a 7 días a partir de la fecha actual

        //Validation fields
        public bool IsValid { get; set; }
        public string Error { get; set; }
    }
}
