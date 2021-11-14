using BookLibraryManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookLibraryManagement.Domains.Models
{
    public class Library
    {
        public List<Book> Books { get; set; }
        public List<BorrowedBook> BorrowedBooks { get; set; }
    }
}
