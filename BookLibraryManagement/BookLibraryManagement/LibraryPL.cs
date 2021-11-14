using BookLibraryManagement.Domains.Models;
using BookLibraryManagement.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BookLibraryManagement.Models
{
    public class LibraryPL
    {
        static readonly string booksPath = Program.GetFilePath("Books.json");
        static readonly string borrowedBooksPath = Program.GetFilePath("BorrowedBooks.json");

        public void Reception()
        {
            bool correct;
            do
            {
                Console.WriteLine("Please press the number of the desired action:\n 1) Book List\n 2) Add a book\n 3) Take a book\n 4) Return a book\n 5) DeleteBook \n 6) Exit");
                bool isParsable = int.TryParse(Console.ReadLine(), out int action);
                if (isParsable)
                {
                    correct = true;
                    switch (action)
                    {
                        case 1:
                            DisplayBooks();
                            break;
                        case 2:
                            AddBook();
                            break;
                        case 3:
                            BorrowBook();
                            break;
                        case 4:
                            ReturnBook();
                            break;
                        case 5:
                            DeleteBook();
                            break;
                        case 6:
                            Bye();
                            break;
                        default:
                            correct = false;
                            Console.WriteLine("Wrong Key press. Try again.");
                            break;
                    }
                }
                else
                {
                    correct = false;
                    Console.WriteLine("Wrong Key press. Try again.");
                }
            } while (!correct);
        }

        private static void Bye()
        {
            Console.Clear();
            Console.WriteLine("See you next time!");
            Environment.Exit(0);
        }

        private void AddBook()
        {
            Console.Clear();
            Console.WriteLine("Add");
            Book book = new();
            Console.WriteLine("Enter new book details");
            bool loop = true;
            do
            {
                Console.WriteLine("Enter ISBN: ");
                bool choice = int.TryParse(Console.ReadLine(), out int selection);
                if (choice)
                {
                    if (!BookService.BookValidation(selection, booksPath))
                    {
                        book.ISBN = selection;
                        loop = false;
                    }
                    else
                        Console.WriteLine("Such ISBN already exists. Try again.");
                }
                else
                {
                    Console.WriteLine("Wrong Key press. Try again.");
                }
            } while (loop);

            Console.WriteLine("Enter Book name: ");
            book.Name = Console.ReadLine();
            Console.WriteLine("Enter Author: ");
            book.Author = Console.ReadLine();
            Console.WriteLine("Enter Category of the book: ");
            book.Category = Console.ReadLine();
            Console.WriteLine("Enter Book Language: ");
            book.Language = Console.ReadLine();
            bool loop2 = true;
            do
            {
                Console.WriteLine("Enter Years of publication: ");
                bool successful = int.TryParse(Console.ReadLine(), out int years);
                if (successful)
                {
                    book.PublicationDate = years;
                    loop2 = false;
                }
                else
                    Console.WriteLine("Wrong Key press. Try again.");
            } while (loop2);

            bool added = BookService.AddBook(book, booksPath);
            Console.WriteLine("--------------------------------------------------\n");
            if (added)
                ExitLoop();
        }

        private void BorrowBook()
        {
            Console.Clear();
            Console.WriteLine("Take book");
            Console.WriteLine("To borrow a book please enter your UserName:");

            string userName = Console.ReadLine();
            bool borrowLimitReached = BookService.BorrowValidation(userName, booksPath);
            if (borrowLimitReached)
            {
                Console.Clear();
                Console.WriteLine("Sorry, can only borrow 3 books per person at once.\n");
                Reception();
            }

            bool loop = true;
            do
            {
                Console.WriteLine("Enter ISBN of the book you wish to borrow:");
                bool choice = int.TryParse(Console.ReadLine(), out int booksId);
                if (choice)
                {

                    List<Book> found = BookService.GetBooksByISBN(booksId, booksPath);
                    if (found.Count > 0)
                    {
                        BorrowedBook borrowedBook = new();
                        borrowedBook.TakenWhen = DateTime.UtcNow;

                        bool looping = true;
                        do
                        {
                            Console.WriteLine("What period do you wish to borrow the book for:\n1) 10 minutes 2) 30 days 3) Custom period");
                            bool next = int.TryParse(Console.ReadLine(), out int selection);
                            if (next)
                            {
                                if (selection == 1)
                                {
                                    borrowedBook.ReturnBy = borrowedBook.TakenWhen.AddMinutes(10);
                                    looping = false;
                                }
                                else if (selection == 2)
                                {
                                    borrowedBook.ReturnBy = borrowedBook.TakenWhen.AddDays(30);
                                    looping = false;
                                }
                                else if (selection == 3)
                                {
                                    DateTime time = GetDateTime();
                                    DateTime dateTime = new();
                                    borrowedBook.ReturnBy = dateTime.AddTicks(time.Ticks);
                                    looping = false;
                                }
                                else
                                    Console.WriteLine("Wrong Key press. Try again.");
                            }
                            else
                            {
                                Console.WriteLine("Wrong Key press. Try again.");
                            }
                        } while (looping);

                        borrowedBook.UserName = userName;
                        borrowedBook.ISBN = booksId;
                        borrowedBook.Name = found.First().Name;
                        borrowedBook.Language = found.First().Language;
                        borrowedBook.Author = found.First().Author;
                        borrowedBook.Category = found.First().Category;
                        borrowedBook.PublicationDate = found.First().PublicationDate;

                        BookService.BorrowBook(borrowedBook, borrowedBooksPath);
                        BookService.RemoveBook(booksId, booksPath);
                        loop = false;
                    }
                    else
                    {
                        bool loop2 = true;
                        do
                        {
                            Console.WriteLine("Such ISBN is not part of available books. \n1) Try again \n2) Go to main menu");
                            bool next = int.TryParse(Console.ReadLine(), out int selection);
                            if (next)
                            {
                                if (selection == 1)
                                {
                                    Console.Clear();
                                    BorrowBook();
                                }
                                else if (selection == 2)
                                {
                                    Console.Clear();
                                    Reception();
                                }
                                else
                                {
                                    Console.WriteLine("Wrong Key press. Try again.");
                                }
                            }
                            else
                            {
                                Console.WriteLine("Wrong Key press. Try again.");
                            }
                        } while (loop2);
                    }
                }
                else
                {
                    Console.WriteLine("Wrong Key press. Try again.");
                }
            } while (loop);
        }

        private void ReturnBook()
        {
            Console.Clear();
            Console.WriteLine("Return a book");
            bool looping = true;
            do
            {
                Console.WriteLine("To return a book please enter ISBN of the book you are returning:");
                bool next = int.TryParse(Console.ReadLine(), out int ISBN);
                if (next)
                {
                    BorrowedBook borrowedBook = BookService.GetBorrowedBookByISBN(ISBN,borrowedBooksPath);
                    if (borrowedBook is not null)
                    {
                        if (borrowedBook.ReturnBy.CompareTo(DateTime.UtcNow) < 0)
                        {
                            Console.WriteLine("You are late with the return.");
                            Console.WriteLine("It's said that Rome wasn't build in a day,but you surely took your time reading that book.");
                        }
                        else
                        {
                            Console.WriteLine("Returned on time!");
                        }
                        BookService.ReturnBorrowedBook(borrowedBook, booksPath,borrowedBooksPath);
                        Console.WriteLine("Book returned");
                        ExitLoop();
                    }
                    else
                    {
                        Console.WriteLine("Book with such ISBN does not exist in borrowed book list...");
                        bool loop2 = true;
                        do
                        {
                            Console.WriteLine("\n1) Try again \n2) Go to main menu");
                            bool nextstep = int.TryParse(Console.ReadLine(), out int selection);
                            if (nextstep)
                            {
                                if (selection == 1)
                                {
                                    Console.Clear();
                                    ReturnBook();
                                }
                                else if (selection == 2)
                                {
                                    Console.Clear();
                                    Reception();
                                }
                                else
                                {
                                    Console.WriteLine("Wrong Key press. Try again.");
                                }
                            }
                            else
                            {
                                Console.WriteLine("Wrong Key press. Try again.");
                            }
                        } while (loop2);
                    }
                }
                else
                {
                    bool loop2 = true;
                    do
                    {
                        Console.WriteLine("Wrong input. ISBN must be numeric. \n1) Try again \n2) Go to main menu");
                        bool nextstep = int.TryParse(Console.ReadLine(), out int selection);
                        if (nextstep)
                        {
                            if (selection == 1)
                            {
                                Console.Clear();
                                ReturnBook();
                            }
                            else if (selection == 2)
                            {
                                Console.Clear();
                                Reception();
                            }
                            else
                            {
                                Console.WriteLine("Wrong Key press. Try again.");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Wrong Key press. Try again.");
                        }
                    } while (loop2);
                }
            } while (looping);
        }

        private void DeleteBook()
        {
            Console.Clear();
            Console.WriteLine("Delete book");
            Console.WriteLine("Enter ISBN of the book you wish to delete");

            bool loop = true;
            do
            {
                Console.WriteLine("ISBN:");
                bool choice = int.TryParse(Console.ReadLine(), out int selection);
                if (choice)
                {
                    if (BookService.BookValidation(selection,booksPath))
                    {
                        bool deleted = BookService.RemoveBook(selection,booksPath);
                        if (deleted)
                            Console.WriteLine("Book Deleted successfully..");
                        loop = false;
                    }
                    else
                    {
                        Console.WriteLine("Book with such ISBN does not exist.");
                        bool loop2 = true;
                        do
                        {
                            Console.WriteLine("\nDo you wish to: \n1) Try entering ISBN again?\n2) Go to Main menu");
                            bool y = int.TryParse(Console.ReadLine(), out int action);
                            if (y)
                            {
                                if (action == 1)
                                    loop2 = false;
                                else if (action == 2)
                                {
                                    Console.Clear();
                                    Reception();
                                }
                                else
                                    Console.WriteLine("Wrong input. Try again.");
                            }
                            else
                            {
                                Console.WriteLine("Wrong input. Try again.");
                            }
                        } while (loop2);
                    }
                }
                else
                {
                    bool loop3 = true;
                    do
                    {
                        Console.WriteLine("Invalid ISBN. Must be numeric.\nDo you wish to: \n1) Try entering ISBN again?\n2) Go to Main menu");
                        bool y = int.TryParse(Console.ReadLine(), out int action);
                        if (y)
                        {
                            if (action == 1)
                                loop3 = false;
                            else if (action == 2)
                                Reception();
                            else
                                Console.WriteLine("Wrong input. Try again.");
                        }
                        else
                        {
                            Console.WriteLine("Wrong input. Try again.");
                        }
                    } while (loop3);
                }
            } while (loop);
        }

        private DateTime GetDateTime()
        {
            Console.WriteLine("Enter Date when you wish to return the book in \"dd/MM/yyyy\" format.");
            string line = Console.ReadLine();
            DateTime dt;

            while (!DateTime.TryParseExact(line, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dt))
            {
                Console.WriteLine("Invalid date, please retry.");
                line = Console.ReadLine();
            }
            if (dt.CompareTo(DateTime.UtcNow) <= 0)
            {
                Console.WriteLine("Custom book return period must be atleast 1 day from today!");
                GetDateTime();
            }
            if (dt.CompareTo(DateTime.UtcNow.AddDays(61)) > 0)
            {
                Console.WriteLine("Can't take book for more than 2 months.");
                GetDateTime();
            }
            return dt;
        }

        private void ExitLoop()
        {
            bool loop = true;
            do
            {
                Console.WriteLine("Choose your next action:\n 1)Go to main Menu \n 2)Exit");
                bool choice = int.TryParse(Console.ReadLine(), out int selection);
                if (choice)
                {
                    if (selection == 1)
                    {
                        Console.Clear();
                        Reception();
                    }
                    else if (selection == 2)
                        Bye();
                    else
                    {
                        Console.WriteLine("Wrong Key press. Try again.");
                    }
                }
                else
                {
                    Console.WriteLine("Wrong Key press. Try again.");
                }
            } while (loop);
        }

        private static void ListAll(IEnumerable<Book> List)
        {
            var count = 1;
            foreach (Book book in List)
            {
                Console.WriteLine($"{count}) {book}\n");
                count++;
            }
            Console.WriteLine("----------------------------------------------------------------");
        }

        private void DisplayBooks()
        {
            Console.Clear();
            bool correct = false;
            do
            {
                Console.WriteLine("Please select how you want to filter Books\n");
                Console.WriteLine("Filter by: \n1) All available books\n2) Author\n3) Category\n4) Language\n5) ISBN\n6) Book name\n7) Taken books \n8) Go to main menu.");
                bool selected = int.TryParse(Console.ReadLine(), out int filterBy);
                if (selected)
                {
                    switch (filterBy)
                    {
                        case 1:
                            {
                                Console.Clear();
                                Console.WriteLine("\n1)See all available books");
                                Console.WriteLine("List of books:\n");
                                ListAll(BookService.GetAllBooks(booksPath));
                                ExitLoop();
                            }
                            break;
                        case 2:
                            {
                                Console.Clear();
                                Console.WriteLine($"\nEnter Author you wish to filter by:");
                                List<Book> subList = BookService.GetAllBooksByAuthor(Console.ReadLine(),booksPath);
                                Console.WriteLine($"{subList.Count} books match your search criteria:");
                                ListAll(subList);
                                ExitLoop();
                            }
                            break;
                        case 3:
                            {
                                Console.Clear();
                                Console.WriteLine("\nEnter Category you wish to filter by:");
                                List<Book> subList = BookService.GetAllBooksByCategory(Console.ReadLine(), booksPath);
                                Console.WriteLine($"{subList.Count} books match your search criteria:");
                                ListAll(subList);
                                ExitLoop();
                            }
                            break;
                        case 4:
                            {
                                Console.Clear();
                                Console.WriteLine("\nEnter Language you wish to filter by:");
                                List<Book> subList = BookService.GetAllBooksByLanguage(Console.ReadLine(), booksPath);
                                Console.WriteLine($"{subList.Count} books match your search criteria:");
                                ListAll(subList);
                                ExitLoop();
                            }
                            break;
                        case 5:
                            {
                                Console.Clear();
                                Console.WriteLine("\nEnter ISBN you wish to filter by:");
                                bool works;
                                do
                                {
                                    bool number = int.TryParse(Console.ReadLine(), out int ISBN);
                                    if (number)
                                    {
                                        List<Book> subList = BookService.GetBooksByISBN(ISBN, booksPath);
                                        Console.WriteLine($"{subList.Count} books match your search criteria:");
                                        ListAll(subList);
                                        works = true;
                                    }
                                    else
                                    {
                                        works = false;
                                        Console.WriteLine("Wrong Key press. Try again.");
                                    }
                                } while (!works);
                                ExitLoop();
                            }
                            break;
                        case 6:
                            {
                                Console.Clear();
                                Console.WriteLine("\nEnter Book name you wish to filter by:");
                                string userInput = Console.ReadLine();
                                List<Book> subList = BookService.GetAllBooksByName(userInput, booksPath);
                                Console.WriteLine($"{subList.Count} books match your search criteria:");
                                ListAll(subList);
                                ExitLoop();
                            }
                            break;
                        case 7:
                            {
                                var count = 1;
                                Console.Clear();
                                Console.WriteLine("\nBorrowed Books:");
                                List<BorrowedBook> borrowedBooks = BookService.GetAllBorrowedBookData(borrowedBooksPath);
                                Console.WriteLine($"{borrowedBooks.Count} books have been borrowed:");
                                foreach (BorrowedBook borrowed in borrowedBooks)
                                {
                                    Console.WriteLine($"{count}) {borrowed}\n");
                                    count++;
                                }
                                Console.WriteLine("----------------------------------------------------------------");
                                ExitLoop();
                            }
                            break;
                        case 8:
                            {
                                Console.Clear();
                                Reception();
                            }
                            break;
                        default:
                            correct = false;
                            Console.WriteLine("Wrong Key press. Try again.");
                            break;
                    }
                }
                else
                {
                    correct = false;
                    Console.WriteLine("Wrong Key press. Try again.");
                }
            } while (!correct);
        }
    }
}