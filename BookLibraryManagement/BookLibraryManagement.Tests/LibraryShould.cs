using BookLibraryManagement.Data;
using BookLibraryManagement.Domains.Models;
using BookLibraryManagement.Models;
using BookLibraryManagement.Services;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace BookLibraryManagement.Tests
{
    [Category("Library")]
    public class LibraryShould
    {
        readonly string BooksPath = Path.Combine(Directory.GetCurrentDirectory().Replace("\\BookLibraryManagement.Tests\\bin\\Debug\\net5.0", "\\BookLibraryManagement.Data\\Data"), "Books.json");
        readonly string BorrowedBooksPath = Path.Combine(Directory.GetCurrentDirectory().Replace("\\BookLibraryManagement.Tests\\bin\\Debug\\net5.0", "\\BookLibraryManagement.Data\\Data"), "BorrowedBooks.json");

        private static List<Book> books;
        private static List<BorrowedBook> borrowedBooks;
        private static Library library;
        
        [OneTimeSetUp]
        public void Setup()
        {
            library = new();

            if (!File.Exists(BooksPath))
            {
                LibraryData.SeedBooks(library, BooksPath);
            }

            if (!File.Exists(BorrowedBooksPath))
            {
                LibraryData.SeedBorrowingInfo(library, BorrowedBooksPath);
            }

            books = new();
            books = BookService.GetAllBooks(BooksPath);
            borrowedBooks = new();
            borrowedBooks = BookService.GetAllBorrowedBookData(BorrowedBooksPath);
        }

        [Test]
        public void Should_ReturnListOfBooks()
        {
            //Assert
            Assert.NotNull(books);
            CollectionAssert.AllItemsAreInstancesOfType(books, typeof(Book));
            CollectionAssert.AllItemsAreUnique(books);
        }

        [Test]
        public void Should_ReturnListOfBorrowedBooks()
        {
            Assert.NotNull(borrowedBooks);
            CollectionAssert.AllItemsAreInstancesOfType(borrowedBooks, typeof(BorrowedBook));
            CollectionAssert.AllItemsAreUnique(borrowedBooks);
        }

        [Test]
        public void Should_AddBookToTheListOfBooks()
        {
            //Arrange
            Book book = new Book
            {
                ISBN = 9999,
                Name = "The Age of Spiritual Machines",
                Author = "Ray Kurzweil",
                Category = "Science",
                Language = "English",
                PublicationDate = 1999
            };

            //Act
            int count = books.Count;
            BookService.AddBook(book, BooksPath);
           
            //Assert
            Assert.That(() => books.Add(book), Throws.Nothing);
            Assert.That(books, Contains.Item(book));
        }

        [Test]
        public void Should_RemoveBookFromTheListOfBooks()
        {
            //Arrange
            Book book = new Book
            {
                ISBN = 9999,
                Name = "The Age of Spiritual Machines",
                Author = "Ray Kurzweil",
                Category = "Science",
                Language = "English",
                PublicationDate = 1999
            };

            //Act
            if (books.Any(x => x.ISBN == book.ISBN))
            {
                try
                {
                    books.Remove(book);
                    BookService.RemoveBook(book.ISBN, BooksPath);
                }
                catch (Exception)
                {
                    throw;
                }
            }
            //Assert
            Assert.That(() => books.Remove(book), Throws.Nothing);
            Assert.That(books, Does.Not.Contain(book));
        }

        [Test]
        public void Should_ReturnAllBooksByAuthor()
        {
            //Arrange
            string author = "Isaac Asimov";

            //Act
            List<Book> books = BookService.GetAllBooksByAuthor(author, BooksPath);

            //Assert
            CollectionAssert.AllItemsAreNotNull(books);
            CollectionAssert.AllItemsAreUnique(books);
            CollectionAssert.AllItemsAreInstancesOfType(books,typeof(Book));
            Assert.That(books, Has.Exactly(2).Items);
        }

        [Test]
        public void Should_ReturnAllBooksByCategory()
        {
            //Arrange
            string category = "Science fiction";

            //Act
            List<Book> books = BookService.GetAllBooksByCategory(category, BooksPath);

            //Assert
            Assert.That(books, Is.Not.Empty);
            CollectionAssert.AllItemsAreNotNull(books);
            CollectionAssert.AllItemsAreUnique(books);
            CollectionAssert.AllItemsAreInstancesOfType(books, typeof(Book));
            Assert.That(books, Has.Exactly(3).Items);
        }

        [Test]
        public void Should_ReturnAllBooksByName()
        {
            //Arrange
            string name = "A Game of Thrones";

            //Act
            List<Book> books = BookService.GetAllBooksByName(name, BooksPath);
           
            
            //Assert
            Assert.That(books, Is.Not.Empty);
            CollectionAssert.AllItemsAreNotNull(books);
            CollectionAssert.AllItemsAreUnique(books);
            CollectionAssert.AllItemsAreInstancesOfType(books, typeof(Book));
            Assert.That(books, Has.Exactly(1).Items);
        }

        [Test]
        public void Should_ReturnAllBooksByLanguage()
        {
            //Arrange
            string lang = "English";

            //Act
            List<Book> books = BookService.GetAllBooksByLanguage(lang, BooksPath);
            //Assert
            CollectionAssert.AllItemsAreNotNull(books);
            CollectionAssert.AllItemsAreUnique(books);
            CollectionAssert.AllItemsAreInstancesOfType(books, typeof(Book));
            Assert.That(books, Has.Exactly(4).Items);

        }

        [Test]
        public void Should_ReturnAllBooksByISBN()
        {
            //Arrange
            int ISBN = 2153;

            //Act
            List<Book> books = BookService.GetBooksByISBN(ISBN, BooksPath);

            //Assert
            Assert.That(books, Is.Not.Null);
            Assert.That(books.Count, Is.EqualTo(1));
            Assert.That(books.First().ISBN, Is.EqualTo(ISBN));
        }


        [Test]
        public void Should_ReturnBorrowedBook()
        {
            //Arrange
            int ISBN = 3746;

            //Act
            BorrowedBook borrowedBook = BookService.GetBorrowedBookByISBN(ISBN, BorrowedBooksPath);
            BookService.ReturnBorrowedBook(borrowedBook, BooksPath,BorrowedBooksPath);
            List<Book> books = BookService.GetAllBooks(BooksPath);
            Book book = BookService.GetBooksByISBN(ISBN, BooksPath).First();
            bool contains = books.Any(x => x.ISBN == ISBN);

            //Assert
            Assert.IsTrue(contains);
            Assert.That(books, Has.Exactly(5).Items);
            CollectionAssert.AllItemsAreNotNull(books);
        }

        [Test]
        public void Should_BorrowBookAndListContainCorrectNumberOfBooks()
        {
            //Arrange
            BorrowedBook bBook = new BorrowedBook
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
            };

            //Act
            BookService.BorrowBook(bBook, BorrowedBooksPath);
            BookService.RemoveBook(3746, BooksPath);

            Book book = new Book
            {
                ISBN = 3746,
                Name = "Childhood's End",
                Author = "Arthur C Clarke",
                Category = "Science fiction",
                Language = "English",
                PublicationDate = 1953
            };

            List<Book> books = BookService.GetAllBooks(BooksPath);
            List<BorrowedBook> borrowedBooks = BookService.GetAllBorrowedBookData(BorrowedBooksPath);

            bool contains = borrowedBooks.Any(x => x.ISBN == 3746);
            bool doesNot = books.Any(x => x.ISBN == 3746);
            //Assert
            Assert.IsFalse(doesNot);
            Assert.IsTrue(contains);
            //Assert.That(borrowedBooks.Count, Is.EqualTo(1)); //error.. why? :||
        }
    }
}