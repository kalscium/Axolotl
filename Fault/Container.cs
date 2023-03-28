using SeraDB;

namespace back
{
    public static class Container {
        public static Entry search(DataBase database, string path) {
            try {
                return (Entry) database.search(path);
            } catch {
                Console.WriteLine("[Fault] Cannot find entry at path");
            } return null;
        }

        public static string path = $"{DateTime.Now.Year}.{getTerm()}.{(DateTime.Today - new DateTime(DateTime.Today.Year, 1, 1)).Days}";

        public static string getTerm() {
            DateTime currentDate = DateTime.Now;
            return currentDate switch
            {
                DateTime d when d.Month == 1 && d.Day >= 1 && d.Day <= 30 => "PreHolidays",
                DateTime d when (d.Month == 1 && d.Day >= 31) || d.Month == 2 || d.Month == 3 || (d.Month == 4 && d.Day <= 4) => "Term1",
                DateTime d when (d.Month == 4 && d.Day <= 24) => "PostTerm1Holidays",
                DateTime d when d.Month == 4 || d.Month == 5 || (d.Month == 6 && d.Day <= 23) => "Term2",
                DateTime d when d.Month == 6 || (d.Month == 7 && d.Day < 10) => "PostTerm2Holidays",
                DateTime d when d.Month == 7 || d.Month == 8 || (d.Month == 9 && d.Day <= 15) => "Term3",
                DateTime d when d.Month == 9 || (d.Month == 10 && d.Day < 2) => "PostTerm3Holidays",
                DateTime d when d.Month == 10 || d.Month == 11 || (d.Month == 12 && d.Day <= 20) => "Term4",
                DateTime d when d.Month == 12 && d.Day >= 21 => "PostSchoolHolidays",
                _ => "Not within term"
            };
        }

        public static void save(DataBase database, Entry entry) {
            try {
                database.modify(path, entry);
            } catch {
                database.append(path, entry);
            }
        }
    }
}