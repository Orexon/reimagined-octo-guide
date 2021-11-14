using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookLibraryManagement.Models
{
    public class Book
    {
        public int ISBN { get; set; }
        public string Name { get; set; }
        public string Author { get; set; }
        public string Category { get; set; }
        public string Language { get; set; }
        public int PublicationDate { get; set; }

        public override string ToString()
        {
            return $"ISBN:{ISBN}\nName:{Name}\nAuthor:{Author}\nCategory:{Category}\nLangueage:{Language}\nReleaseDate:{PublicationDate}";
        }
    }
}
