using System;
using System.Collections.Generic;
using System.Text;

namespace App
{
    internal class Lone
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public Book Book { get; set; }
        public EnumType Type { get; set; }
        public DateTime LoneDate { get; set; }
        public DateTime ReportDate { get; set; }

        public Lone(int userId, string name, Book book, EnumType type, DateTime loneDate, DateTime reportDate)
        {
            UserId=userId;
            Name=name;
            Type=type;
            LoneDate=loneDate;
            ReportDate=reportDate;
        }


    }
}
