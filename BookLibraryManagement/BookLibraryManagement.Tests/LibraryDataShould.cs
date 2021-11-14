using BookLibraryManagement.Data;
using BookLibraryManagement.Domains.Models;
using BookLibraryManagement.Models;
using BookLibraryManagement.Services;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace BookLibraryManagement.Tests
{
    [Category("DataSeed")]
    public class LibraryDataShould
    {
        readonly string BooksPath = Path.Combine(Directory.GetCurrentDirectory().Replace("\\BookLibraryManagement.Tests\\bin\\Debug\\net5.0", "\\BookLibraryManagement.Data\\Data"), "Books.json");
        readonly string BorrowedBooksPath = Path.Combine(Directory.GetCurrentDirectory().Replace("\\BookLibraryManagement.Tests\\bin\\Debug\\net5.0", "\\BookLibraryManagement.Data\\Data"), "BorrowedBooks.json");


        [Test]
        public void Should_CreateAnInitialBookFileIfNoFileExists()
        {
            //Arrange
            Library library = new();
            library.Books = new();

            //Act 
            if (!File.Exists(BooksPath))
            {
                LibraryData.SeedBooks(library, BooksPath);
            }
            //Assert
            Assert.That(File.Exists(BooksPath));
        }

        [Test]
        public void Should_CreateAnInitialBorrowedBookFileIfNoFileExists()
        {
            //Arrange
            Library library = new();
            library.BorrowedBooks = new();

            //Act 
            if (!File.Exists(BorrowedBooksPath))
            {
                LibraryData.SeedBorrowingInfo(library, BorrowedBooksPath);
            }
            //Assert
            Assert.That(File.Exists(BorrowedBooksPath));
        }

        [Test]
        public void Should_InitialSeedOfBooksCreateListWithBooks()
        {
            //Arrange
            Library library = new();
            library.Books = new();

            //Act 
            if (!File.Exists(BooksPath))
            {
                LibraryData.SeedBooks(library, BooksPath);
            }

            library.Books = BookService.GetAllBooks(BooksPath);

            //string BookString = File.ReadAllText(BooksPath);
            //library.Books = JsonSerializer.Deserialize<List<Book>>(BookString);

            //Assert
            Assert.That(library.Books.Count, Is.EqualTo(4));
            Assert.That(library.Books, Is.Not.Empty);
            Assert.That(library.Books, Is.Not.Null);
        }

        [Test]
        public void Should_InitialSeedOfBorrowedBooksCreateListWithBooks()
        {
            //Arrange
            Library library = new();
            library.BorrowedBooks = new();

            //Act 
            if (!File.Exists(BorrowedBooksPath))
            {
                LibraryData.SeedBorrowingInfo(library, BorrowedBooksPath); 
            }

            library.BorrowedBooks = BookService.GetAllBorrowedBookData(BorrowedBooksPath);

            //Assert
            Assert.That(library.BorrowedBooks.Count, Is.EqualTo(1));
            Assert.That(library.BorrowedBooks, Is.Not.Empty);
            Assert.That(library.BorrowedBooks, Is.Not.Null);
        }




    }
}