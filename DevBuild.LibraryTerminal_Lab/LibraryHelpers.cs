using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevBuild.Utilities;

namespace DevBuild.LibraryTerminal_Lab
{
    class LibraryHelpers
    {
        public enum BookData { Title = 0, Author = 1, Genre = 2 }
        public static string[] sortOrSearchBooksOptions = { "By Title", "By Author", "By Genre" };

        /// <summary>
        /// This method allows a user to search the library for books by (as of this writing) title, author, or genre
        /// </summary>
        /// <param name="searchTermType">Allows a user to search specifically by one piece of book data -- author, title, or genre</param>
        /// <param name="searchTerm">Search terms to find among authors, book titles, or genres</param>
        /// <returns>Returns a list of BookRecords corresponding to books that match the input search terms</returns>
        public static List<BookRecord> SearchForBook(List<BookRecord> bookList)
        {
            List<BookRecord> results = new List<BookRecord>();

            uint userSelection_Numeric = 0;
            string userResponse = "";

            while (!uint.TryParse(userResponse, out userSelection_Numeric) || userSelection_Numeric < 1 || userSelection_Numeric > sortOrSearchBooksOptions.Length)
            {
                userResponse = "";
                UserInput.PromptUntilValidEntry($"Please select an option, 1-{sortOrSearchBooksOptions.Length}: ", ref userResponse, InformationType.Numeric);
            }

            userResponse = "";
            UserInput.PromptUntilValidEntry("Enter some search terms: ", ref userResponse);

            switch ((BookData)(userSelection_Numeric - 1))
            {
                case BookData.Author:
                    {
                        results = bookList.Where<BookRecord>(x => x.Author.ToLower().Contains(userResponse.ToLower())).ToList<BookRecord>();
                        break;
                    }
                case BookData.Title:
                    {
                        results = bookList.Where<BookRecord>(x => x.Title.ToLower().Contains(userResponse.ToLower())).ToList<BookRecord>();
                        break;
                    }
                case BookData.Genre:
                    {
                        //bookList = bookList.OrderBy(x => x.Genre).ToList<BookRecord>();
                        break;
                    }
            }
            return results;
        }

        public static void SortBooks(ref List<BookRecord> bookList)
        {
            uint userSelection_Numeric = 0;
            string userResponse = "";

            while (!uint.TryParse(userResponse, out userSelection_Numeric) || userSelection_Numeric < 1 || userSelection_Numeric > sortOrSearchBooksOptions.Length)
            {
                userResponse = "";
                UserInput.PromptUntilValidEntry($"Please select an option, 1-{sortOrSearchBooksOptions.Length}: ", ref userResponse, InformationType.Numeric);
            }

            switch ((BookData)(userSelection_Numeric - 1))
            {
                case BookData.Author:
                    {
                        bookList = bookList.OrderBy(x => x.Author).ToList<BookRecord>();
                        break;
                    }
                case BookData.Title:
                    {
                        //let's remove "A" and "The From a book title if present, then reappend them to book titles once they're sorted
                        List<string[]> stringRefs = new List<string[]>();

                        foreach (BookRecord bookRecord in bookList)
                        {
                            if (bookRecord.Title.StartsWith("A ") || bookRecord.Title.StartsWith("The "))
                            {
                                string[] tmp = bookRecord.Title.Split(new char[] { ' ' }, 2);
                                stringRefs.Add(tmp);
                                bookRecord.Title = tmp[1];   
                            }
                        }
                        bookList = bookList.OrderBy(x => x.Title).ToList<BookRecord>();

                        //now let's find those titles that began with "A" and "The", and reappend their articles
                        foreach (string[] s in stringRefs)
                        {
                            //book title we're looking for will contain the rest of the string, without "A" or "The"
                            BookRecord j = bookList.Find(x => x.Title.Contains(s[1]));
                            j.Title = s[0] + " " + s[1];
                        }

                        break;
                    }
                case BookData.Genre:
                    {
                        bookList = bookList.OrderBy(x => x.Genre).ToList<BookRecord>();
                        break;
                    }
            }
        }

        public static void AddBook(ref List<BookRecord> bookList)
        {
            string userResponse = "";
            string titleEntry = "", authorEntry = "";
            uint availableCopies = 0;
            BookRecord addedBook;

            UserInput.PromptUntilValidEntry("Please enter book's title: ", ref titleEntry);
            UserInput.PromptUntilValidEntry("Please enter book's author: ", ref authorEntry);

            while (!uint.TryParse(userResponse, out availableCopies) || availableCopies < 0)
            {
                UserInput.PromptUntilValidEntry("How many copies of this book do we have? ", ref userResponse, InformationType.Numeric);
            }

            addedBook = new BookRecord(titleEntry, authorEntry, availableCopies, false);
            bookList.Add(addedBook);

            Console.WriteLine("\nBook successfully added.\n");
            return;
        }
    }
}
