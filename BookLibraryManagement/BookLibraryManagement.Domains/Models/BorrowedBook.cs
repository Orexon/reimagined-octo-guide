using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookLibraryManagement.Models
{
    public class BorrowedBook : Book
    {
        public string UserName { get; set; }
        public DateTime TakenWhen { get; set; }
        public DateTime ReturnBy { get; set; }

        public override string ToString()
        {
            return $"Username:{UserName}\nBook:{Name}\nISBN:{ISBN}\nTakenAt:{TakenWhen}\nReturnBy:{ReturnBy}";
        }
    }
}