using BookLibraryManagement.Data;
using BookLibraryManagement.Domains.Models;
using BookLibraryManagement.Models;
using System;
using System.IO;

namespace BookLibraryManagement
{
    class Program
    {
        static void Main(string[] args)
        {

            Library library = new();
            library.Books = new();
            library.BorrowedBooks = new();

            if (!File.Exists(GetFilePath("Books.json")))
            {
                LibraryData.SeedBooks(library, GetFilePath("Books.json"));
            }

            if (!File.Exists(GetFilePath("BorrowedBooks.json")))
            {
                LibraryData.SeedBorrowingInfo(library, GetFilePath("BorrowedBooks.json"));
            }

            Welcome();
            LibraryPL libraryPl = new();
            libraryPl.Reception();

        }

        public static void Welcome()
        {
            Console.WriteLine("Welcome to library App!");
            Console.WriteLine("Please enter your username: ");
            string user = Console.ReadLine();
            Console.Clear();
            Console.WriteLine($"Hello {user}\n"); ;
        }

        public static string GetFilePath(string fileName)
        {
            return Path.Combine(Directory.GetCurrentDirectory().Replace("\\bin\\Debug\\net5.0", ".Data\\Data"), fileName);
        }
    }
}