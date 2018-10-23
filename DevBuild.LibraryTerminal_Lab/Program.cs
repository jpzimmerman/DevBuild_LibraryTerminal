using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevBuild.Utilities;

namespace DevBuild.LibraryTerminal_Lab
{
    class Program
    {

        public static List<BookRecord> libraryBooks = new List<BookRecord>();
        public static List<BookRecord> checkedOutBooks = new List<BookRecord>();
        public static string[] menuOptions = { "Display Books", "Search Books (by Title, Author, or Genre)", "Sort books (by Title, Author, or Genre)", "Check out Book", "Return Book", "Add Book" };


        //we'll use this Dictionary to dynamically attach numbers to each of our menu options, so our user can select an option by entering its number
        public static Dictionary<string, int> menuOptionsIndices = new Dictionary<string, int>();

        static void Main(string[] args)
        {    
            libraryBooks.Add(new BookRecord("To Kill a Mockingbird", "Harper Lee", 3, checkedOut: false));
            libraryBooks.Add(new BookRecord("The Dragon Reborn", "Robert Jordan", 6, checkedOut: true));
            libraryBooks.Add(new BookRecord("Manhunter", "Thomas Harris", 10, checkedOut: false));
            libraryBooks.Add(new BookRecord("Computer Science Distilled", "Wladston Ferreira Filho", 25, checkedOut: false));
            libraryBooks.Add(new BookRecord("Ready Player One", "Ernest Cline", 7, checkedOut: true));
            libraryBooks.Add(new BookRecord("Brave New World", "Aldous Huxley", 12, checkedOut: false));
            libraryBooks.Add(new BookRecord("The Prize", "Irving Wallace", 8, checkedOut: false));
            libraryBooks.Add(new BookRecord("Stranger in a Strange Land", "Robert A. Heinlein", 6, checkedOut: true));
            libraryBooks.Add(new BookRecord("The Gods Themselves", "Isaac Asimov", 9, checkedOut: false));
            libraryBooks.Add(new BookRecord("A Scanner Darkly", "Philip K. Dick", 14, checkedOut: false));
            libraryBooks.Add(new BookRecord("Eldest", "Christopher Paolini", 4, checkedOut: true));
            libraryBooks.Add(new BookRecord("The Future of the Mind", "Michio Kaku", 11, checkedOut: false));

            string userResponse = "";
            uint userInput_Numeric;

            var menuIndex = 1;
            foreach (string s in menuOptions)
            {
                menuOptionsIndices.Add(s, menuIndex++);
            }

            //libraryBooks.Sort((x, y) => x.Author.CompareTo(y.Author));
            libraryBooks = libraryBooks.OrderBy(x => x.Title).ToList<BookRecord>();

            while (true)
            {
                userResponse = "";
                userInput_Numeric = 0;
                DisplayMenuOptions(menuOptionsIndices);
                while (!uint.TryParse(userResponse, out userInput_Numeric) || userInput_Numeric < 1)
                {
                    userResponse = "";
                    UserInput.PromptUntilValidEntry("Please select a menu option. ", ref userResponse, InformationType.Numeric);
                }

                switch ((int)userInput_Numeric)
                {
                    case 1:         //currently assigned to Display Books
                        {
                            DisplayBooks(libraryBooks, showCheckedOutBooks: true);
                            break;
                        }
                    case 2:
                        {
                            DisplayMenuOptions(LibraryHelpers.sortOrSearchBooksOptions);
                            List<BookRecord> searchResults = LibraryHelpers.SearchForBook(libraryBooks);
                            DisplayBooks(searchResults, showCheckedOutBooks: true);
                            break;
                        }
                    case 3:         //currently assigned to Sort Books
                        {
                            DisplayMenuOptions(LibraryHelpers.sortOrSearchBooksOptions);
                            LibraryHelpers.SortBooks(ref libraryBooks);
                            break;
                        }
                    case 4:         //currently assigned to Check Out Book
                        {
                            CheckOutBook(libraryBooks);
                            break;
                        }
                    case 5:         //currently assigned to Return Book
                        {
                            if (checkedOutBooks.Count > 0)
                            {
                                ReturnBook(libraryBooks);
                            }
                            else { Console.WriteLine("\nYou don't appear to have any library books checked out.\n"); }
                            break;
                        }
                    case 6:         //currently assigned to Add Book
                        {
                            LibraryHelpers.AddBook(ref libraryBooks);
                            break;
                        }
                }
            }
        }

        public static void DisplayMenuOptions(Dictionary<string, int> options)
        {
            foreach (KeyValuePair<string, int> s in options)
            {
                Console.WriteLine($"{s.Value}.) {s.Key}");
            }
        }

        public static void DisplayMenuOptions(string[] options)
        {
            Console.WriteLine("");
            for (int i = 0; i < options.Length; i++)
            {
                Console.WriteLine((i + 1) + ".) " + options[i]);
            }
            Console.WriteLine("");
        }

        public static void DisplayBooks(IList<BookRecord> bookList, [Optional]bool showCheckedOutBooks, [Optional]bool menuMode)
        {
            //let's print a header for our book list -- Title, author, number of available copies for each book
            Console.WriteLine("\n");
            Console.WriteLine((menuMode? "    " : "") + "Title".PadRight(30) + " " + "Author".PadRight(25) + " " +  "Available?".PadRight(2));
            Console.WriteLine((menuMode? "    " : "") + "===========================".PadRight(30) + " " + "======================".PadRight(25) + " " + "============".PadRight(2));
            var menuIndex = 1;

            //for every book, let's print a title, author, whether or not the book is available at the library, and how many copies we have if so
            foreach (BookRecord c in bookList)
            {
                if (!showCheckedOutBooks & c.CheckedOut) { continue;}
                Console.WriteLine(  (menuMode? $"{menuIndex++}.) " : "") + c.Title.PadRight(30) + " " + c.Author.PadRight(25) + " " +
                                    (!c.CheckedOut ?    $"Yes, Available Copies: {c.AvailableCopies} " : $"No, expected available: {c.ExpectedAvailabilityDate.ToShortDateString()}").PadRight(2));
            }
            Console.WriteLine("\n");
            return;
        }

        public static void CheckOutBook(IList<BookRecord> bookList)
        {
            string userResponse = "";
            uint userSelection_Numeric = 0;
            List<BookRecord> availableBooks = bookList.Where(x => x.CheckedOut == false).ToList<BookRecord>();

            //let's assume for this call that the user doesn't want to see books they can't check out, and can get the projected availability date from the Display Books option
            DisplayBooks(availableBooks, showCheckedOutBooks: false, menuMode: true);

            while (!uint.TryParse(userResponse, out userSelection_Numeric) || userSelection_Numeric < 1 || (userSelection_Numeric > availableBooks.Count))
            {
                userResponse = "";
                UserInput.PromptUntilValidEntry($"Please select a book, 1-{availableBooks.Count}: ", ref userResponse);
            }

            //find the requested book's index in the master list, then decrement the number of copies of this book we show available. 
            //if this number gets to zero, this book is all checked out.
            var masterListIndex = bookList.IndexOf(availableBooks[(int)userSelection_Numeric - 1]);

            if (!checkedOutBooks.Contains(bookList[masterListIndex]))
            {
                bookList[masterListIndex].AvailableCopies--;
                bookList[masterListIndex].ExpectedAvailabilityDate = DateTime.Now.AddDays(14);
                checkedOutBooks.Add(bookList[masterListIndex]);
                Console.WriteLine($"{bookList[masterListIndex].Title} successfully checked out. This book is due back to the library on or before {bookList[masterListIndex].ExpectedAvailabilityDate.ToShortDateString()}.\n");
            }
            else        //if we already have this book, no sense in checking out another copy to us
            {
                Console.WriteLine("It looks like you've already checked out a copy of this book.\n");
            }
        }

        public static void ReturnBook(IList<BookRecord> bookList)
        {
            string userResponse = "";
            uint userSelection_Numeric = 0;

            DisplayBooks(checkedOutBooks, showCheckedOutBooks: true, menuMode: true);
            while (!uint.TryParse(userResponse, out userSelection_Numeric) || userSelection_Numeric < 1 || (userSelection_Numeric > checkedOutBooks.Count))
            {
                userResponse = "";
                UserInput.PromptUntilValidEntry($"Please select a book, 1-{checkedOutBooks.Count}: ", ref userResponse);
            }
            var masterListIndex = bookList.IndexOf(checkedOutBooks[(int)(userSelection_Numeric - 1)]);
            if (bookList.Contains(checkedOutBooks[(int)(userSelection_Numeric - 1)]))
            {
                bookList[masterListIndex].AvailableCopies++;
                bookList[masterListIndex].ExpectedAvailabilityDate = DateTime.Now;
                checkedOutBooks.RemoveAt((int)(userSelection_Numeric - 1));
            }
        }
    }
}
