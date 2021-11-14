using BookLibraryManagement.Data;
using BookLibraryManagement.Domains.Models;
using BookLibraryManagement.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace BookLibraryManagement.Services
{
    public class BookService
    {
        public static void SaveBooksData(List<Book> books, string booksPath)
        {
            string json = JsonSerializer.Serialize(books);
            File.WriteAllText(booksPath, json);
        }

        public static void SaveBorrowData(List<BorrowedBook> borrowedBooks, string borrowedBooksPath)
        {
            string json = JsonSerializer.Serialize(borrowedBooks);
            File.WriteAllText(borrowedBooksPath, json);
        }

        public static List<Book> GetAllBooks(string booksPath)
        {
            Library library = new();
            
            string BookString = File.ReadAllText(booksPath);
            library.Books = JsonSerializer.Deserialize<List<Book>>(BookString);
            return library.Books;
        }

        public static List<BorrowedBook> GetAllBorrowedBookData(string borrowedBooksPath)
        {
            Library library = new();
            string borrowedBooksString = File.ReadAllText(borrowedBooksPath);
            library.BorrowedBooks = JsonSerializer.Deserialize<List<BorrowedBook>>(borrowedBooksString);
            return library.BorrowedBooks;
        }

        public static BorrowedBook GetBorrowedBookByISBN(int ISBN, string borrowedBooksPath)
        {
            List<BorrowedBook> borrowedBooks = GetAllBorrowedBookData(borrowedBooksPath);
            return borrowedBooks.Where(x => x.ISBN == ISBN).FirstOrDefault();
        }

        public static bool AddBook(Book book, string booksPath)
        {
            List<Book> books = GetAllBooks(booksPath);
            bool isDone;
            try
            {
                books.Add(book);
                SaveBooksData(books, booksPath);
                isDone = true;
            }
            catch (Exception e)
            {
                isDone = false;
                Console.WriteLine($"Something went wrong: {e}");
            }
            return isDone;
        }

        public static bool BorrowBook(BorrowedBook book, string borrowedBooksPath)
        {
            List<BorrowedBook> borrowedBooks = GetAllBorrowedBookData(borrowedBooksPath);
            bool isDone;
            try
            {
                borrowedBooks.Add(book);
                SaveBorrowData(borrowedBooks, borrowedBooksPath);
                Console.WriteLine("Book borrowed successfully..");
                isDone = true;
            }
            catch (Exception e)
            {
                isDone = false;
                Console.WriteLine($"Something went wrong: {e}");
            }
            return isDone;
        }

        public static bool ReturnBorrowedBook(BorrowedBook book, string booksPath, string borrowedBooksPath)
        {
            bool returned;
            List<Book> books = GetAllBooks(booksPath);
            List<BorrowedBook> borrowedBooks = GetAllBorrowedBookData(borrowedBooksPath);
            try
            {
                books.Add(new Book
                {
                    ISBN = book.ISBN,
                    Name = book.Name,
                    Author = book.Author,
                    Category = book.Category,
                    Language = book.Language,
                    PublicationDate = book.PublicationDate
                });
                SaveBooksData(books, booksPath);

                BorrowedBook b = borrowedBooks.Find(x => x.ISBN == book.ISBN);
                borrowedBooks.Remove(b);
                SaveBorrowData(borrowedBooks, borrowedBooksPath);
                Console.WriteLine("Book added successfully..");
                returned = true;
            }
            catch (Exception e)
            {
                returned = false;
                Console.WriteLine($"Something went wrong: {e}");
            }
            return returned;
        }

        public static bool RemoveBook(int ISBN, string booksPath)
        {
            bool removed = false;
            List<Book> books = GetAllBooks(booksPath);
            try
            {
                Book removeBook = books.Find(x => x.ISBN == ISBN);
                books.Remove(removeBook);
                SaveBooksData(books, booksPath);
                removed = true;
            }
            catch (Exception e)
            {
                removed = false;
                Console.WriteLine($"Something went wrong: {e}");
            }
            return removed;
        }

        public static List<Book> GetAllBooksByAuthor(string author, string booksPath)
        {
            List<Book> books = GetAllBooks(booksPath);
            return books.Where(x => x.Author == author).ToList();
        }

        public static List<Book> GetAllBooksByCategory(string category, string booksPath)
        {
            List<Book> books = GetAllBooks(booksPath);
            return books.Where(x => x.Category == category).ToList();
        }

        public static List<Book> GetAllBooksByLanguage(string language, string booksPath)
        {
            List<Book> books = GetAllBooks(booksPath);
            return books.Where(x => x.Language == language).ToList();
        }

        public static List<Book> GetBooksByISBN(int ISBN, string booksPath)
        {
            List<Book> books = GetAllBooks(booksPath);
            return books.Where(x => x.ISBN == ISBN).ToList();
        }

        public static List<Book> GetAllBooksByName(string name, string booksPath)
        {
            List<Book> books = GetAllBooks(booksPath);
            return books.Where(x => x.Name == name).ToList();
        }

        public static bool BookValidation(int ISBN, string booksPath)
        {
            List<Book> books = GetAllBooks(booksPath);
            return books.Where(x => x.ISBN == ISBN).Any();
        }

        public static bool BorrowValidation(string userName, string booksPath)
        {
            List<BorrowedBook> borrowedBooks = GetAllBorrowedBookData(booksPath);
            int borrowLimitReached = borrowedBooks.Where(x => x.UserName == userName).ToList().Count;
            if(borrowLimitReached < 3)
                return false;
            else return true;
        }
    }
}
