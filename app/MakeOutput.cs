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
        public List<string> TopBooks { get; set; }
        public List<string> AtRisk { get; set; }
        public int TotalFine { get; private set; }

        public MakeOutput(int reportDay, List<string[]> rows)
        {
            ReportDay = reportDay;
            Rows = rows;
            Fines = new List<string>();
            Flagged = new List<string>();
            Error = new List<string>();
            TopBooks = new List<string>();
            AtRisk = new List<string>();
            TotalFine = 0;
        }



        public int FineForStudent(string[]student)
        {
            int loanDay = int.Parse(student[4]);
            int endDay = string.IsNullOrWhiteSpace(student[5])
                ? ReportDay
                : int.Parse(student[5]);
            int days = endDay - loanDay;
            if (days <= 14)
                return 0;
            return Math.Min(30, days - 14);
        }

        public int FineForGuest(string[] guest)
        {
            int loanDay = int.Parse(guest[4]);
            int endDay = string.IsNullOrWhiteSpace(guest[5])
                ? ReportDay
                : int.Parse(guest[5]);
            int days = endDay - loanDay;
            return (int)Math.Ceiling(days * 2.5);
        }

        public bool IsOkLone(string[] line)
        {
            if (line.Length != 6) return false;
            for (int i = 0; i < line.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(line[i]) && i != 5) return false;
            }
            if (!int.TryParse(line[4], out int loanDay)) return false;
            if (!string.IsNullOrWhiteSpace(line[5]) &&
                (!int.TryParse(line[5], out int returnDay) || returnDay < loanDay))
                return false;

            return true;
        }

        public void ClassifyRows()
        {
            Dictionary<string, int> fineByReader = new Dictionary<string, int>();
            Dictionary<string, string> nameByReader = new Dictionary<string, string>();
            List<string> fineOrder = new List<string>();
            HashSet<string> flaggedReaders = new HashSet<string>();
            List<string> flaggedOrder = new List<string>();
            Dictionary<string, int> bookCounts = new Dictionary<string, int>();
            Dictionary<string, int> finedLoansByReader = new Dictionary<string, int>();
            Dictionary<string, string> atRiskNames = new Dictionary<string, string>();

            foreach (string[] row in Rows)
            {
                if (!IsOkLone(row))
                {
                    Error.Add(string.Join(",", row));
                    continue;
                }

                if (!Enum.TryParse(row[2], true, out EnumType type))
                {
                    Error.Add(string.Join(",", row));
                    continue;
                }

                if (!bookCounts.ContainsKey(row[3]))
                    bookCounts[row[3]] = 0;
                bookCounts[row[3]]++;

                int endDay = string.IsNullOrWhiteSpace(row[5])
                    ? ReportDay
                    : int.Parse(row[5]);
                int days = endDay - int.Parse(row[4]);

                if (type == EnumType.teacher)
                {
                    if (days > 60)
                    {
                        if (flaggedReaders.Add(row[0]))
                            flaggedOrder.Add(row[0]);

                        if (!fineByReader.ContainsKey(row[0]))
                        {
                            fineByReader[row[0]] = 0;
                            fineOrder.Add(row[0]);
                            nameByReader[row[0]] = row[1];
                        }
                    }
                    continue;
                }

                int fine = type == EnumType.guest
                    ? FineForGuest(row)
                    : FineForStudent(row);

                if (fine > 0)
                {
                    if (!fineByReader.ContainsKey(row[0]))
                    {
                        fineByReader[row[0]] = 0;
                        fineOrder.Add(row[0]);
                        nameByReader[row[0]] = row[1];
                    }
                    fineByReader[row[0]] += fine;
                    TotalFine += fine;

                    if (!finedLoansByReader.ContainsKey(row[0]))
                    {
                        finedLoansByReader[row[0]] = 0;
                        atRiskNames[row[0]] = row[1];
                    }
                    finedLoansByReader[row[0]]++;
                }
            }

            foreach (string readerId in fineOrder)
                Fines.Add($"{readerId},{nameByReader[readerId]},{fineByReader[readerId]}");

            foreach (string readerId in flaggedOrder)
                Flagged.Add($"{readerId},{nameByReader[readerId]}");

            List<KeyValuePair<string, int>> sortedBooks = new List<KeyValuePair<string, int>>(bookCounts);
            sortedBooks.Sort((first, second) =>
            {
                int countComparison = second.Value.CompareTo(first.Value);
                return countComparison != 0
                    ? countComparison
                    : StringComparer.Ordinal.Compare(first.Key, second.Key);
            });

            int booksToAdd = Math.Min(3, sortedBooks.Count);
            for (int i = 0; i < booksToAdd; i++)
                TopBooks.Add($"{sortedBooks[i].Key},{sortedBooks[i].Value}");

            foreach (KeyValuePair<string, int> reader in finedLoansByReader)
            {
                if (reader.Value > 2)
                    AtRisk.Add($"{reader.Key},{atRiskNames[reader.Key]}");
            }
        }
    }
}
