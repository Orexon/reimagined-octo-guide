using BookLibraryManagement.Domains.Models;
using BookLibraryManagement.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace BookLibraryManagement.Data
{
    public class LibraryData
    {
        //static readonly string BooksPath = Path.Combine(Directory.GetCurrentDirectory().Replace("\\bin\\Debug\\net5.0", ".Data\\Data"), "Books.json");
        //static readonly string BorrowedBooksPath = Path.Combine(Directory.GetCurrentDirectory().Replace("\\bin\\Debug\\net5.0", ".Data\\Data"), "BorrowedBooks.json");

        
        public static void SeedBooks(Library library, string path)
        {
            library.Books.Add(new Book
            {
                ISBN = 2153,
                Name = "Dune",
                Author = "Frank Herbert",
                Category = "Science fiction",
                Language = "English",
                PublicationDate = 1965
            });

            library.Books.Add(new Book
            {
                ISBN = 2533,
                Name = "Foundation",
                Author = "Isaac Asimov",
                Category = "Science fiction",
                Language = "English",
                PublicationDate = 1942
            });
            library.Books.Add(new Book
            {
                ISBN = 3743,
                Name = "The Second Foundation",
                Author = "Isaac Asimov",
                Category = "Science fiction",
                Language = "English",
                PublicationDate = 1953
            });
            library.Books.Add(new Book
            {
                ISBN = 3755,
                Name = "A Game of Thrones",
                Author = "George R. R. Martin",
                Category = "Fantasy",
                Language = "English",
                PublicationDate = 1996
            });

            string json = JsonSerializer.Serialize(library.Books);
            File.WriteAllText(path, json);
        }

        public static void SeedBorrowingInfo(Library library, string path)
        {
            library.BorrowedBooks.Add(new BorrowedBook
            {
                UserName = "Admin",
                ReturnBy = new DateTime(2021, 11, 16, 00, 00, 00),
                TakenWhen = new DateTime(2021, 11, 11, 00, 00, 00),
                ISBN = 3746,
                Name = "Childhood's End",
                Author = "Arthur C Clarke",
                Category = "Science fiction",
                Language = "English",
                PublicationDate = 1953
            });

            string json2 = JsonSerializer.Serialize(library.BorrowedBooks);
            File.WriteAllText(path, json2);
        }
    }
}
