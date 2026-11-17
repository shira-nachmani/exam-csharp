using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace App
{
    internal class MakeOutput
    {
        public int ReportDay { get; set; }
        public List<string[]> Rows  { get; set; }
        public List<string> Fines { get; set; }
        public List<string> Flagged { get; set; }
        public List<string> Error { get; set; }

        public MakeOutput(int reportDay, List<string[]> rows)
        {
            ReportDay = reportDay;
            Rows = rows;
        }

        //public bool IsOkLone(string[] line)
        //{
        //    for (int i = 0; i < line.Length; i++)
        //    {
                  
        //    }


        //    //שורה שאין בה בדיוק שישה שדות,
        //    //שורה שיש בה שדה ריק חוץ מיום ההחזרה,
        //    ////שורה שיש בה יום שאינו מספר,
        //    ////שורה שיום ההחזרה בה לפני יום ההשאלה,
        //    ////ושורה שסוג החברה בה לא מוכר.
        //    //45
        //}
    }
}
