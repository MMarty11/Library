using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biblioteca.Models
{
    internal class Book
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string Isbn { get; set; }
        public string Year { get; set; }


        public bool IsAvaiable { get; set; } = true;

        // Navigazione (se usi EF)
        public ICollection<Loan> Loans { get; set; } = new List<Loan>();
    }
}
