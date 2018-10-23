using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace DevBuild.LibraryTerminal_Lab
{
    public enum Genre { Unknown, Fiction, NonFiction, Adventure, Fantasy, Horror, SciFi, Mystery, Romance, OprahsBookClub, SelfHelp, Philosophy, Hobby }

    class BookRecord
    {
        public string   Title { get; set; }
        public string   Author { get; set; }
        public Genre    Genre { get; set; }
        
        public bool     CheckedOut { get; set; } = false;
        public DateTime ExpectedAvailabilityDate { get; set; }      //if book is checked out, let's set this to expected time + 2 hours 
                                                                    //(you know, allow time for manual check-in)

        private uint    availableCopies;

        public BookRecord() { }

        public BookRecord(string bookTitle, string author, uint availableCopies, bool checkedOut, [Optional]DateTime expectedDate)
        {
            Title = bookTitle;
            Author = author;
            AvailableCopies = availableCopies;
            CheckedOut = checkedOut;
            ExpectedAvailabilityDate = DateTime.Now.AddDays(14);
        }

        public void ReturnBook()
        {
            CheckedOut = false;
        }

        public void CheckOut(out DateTime dueDate)
        {
            dueDate = DateTime.Now.AddDays(14);
            ExpectedAvailabilityDate = dueDate;
        }

        public uint AvailableCopies
        {
            get { return availableCopies; }
            set
            {
                availableCopies = value;
                if (availableCopies <= 0) { availableCopies = 0; CheckedOut = true; }
            }
        }
    }
}
