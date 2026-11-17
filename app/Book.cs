using System;
using System.Collections.Generic;
using System.Text;

namespace App
{
    internal class Book
    {
        public string Name { get; set; }
        public string Aother { get; set; }
        public Book(string Name, string Aother)
        {
            this.Name = Name;
            this.Aother = Aother;
        }
    }
}
