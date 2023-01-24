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

        public static string create(DataBase database, string path, Entry entry) {
            string loc = $"{path}.{(DateTime.Today - new DateTime(DateTime.Today.Year, 1, 1)).Days}";
            database.append(loc, entry);
            return loc;
        }

        public static string create(DataBase database, Entry entry) => create(database, $"{DateTime.Now.Year}.{getTerm()}", entry);

        public static void remove(DataBase database, string path, bool abs=true) {
            if (abs) {database.remove(path); return;};
            remove(database, $"{DateTime.Now.Year}.{getTerm()}.{(DateTime.Today - new DateTime(DateTime.Today.Year, 1, 1)).Days}");
        }

        public static string modify(DataBase database, string path, Entry entry) {
            string loc  = $"{path}.{(DateTime.Today - new DateTime(DateTime.Today.Year, 1, 1)).Days}";
            database.modify(loc, entry);
            return loc;
        }

        public static string modify(DataBase database, Entry entry) => modify(database, $"{DateTime.Now.Year}.{getTerm()}", entry);

        public static string getTerm() {
            DateTime currentDate = DateTime.Now;
            return currentDate switch
            {
                DateTime d when d.Month == 1 && d.Day >= 1 && d.Day <= 30 => "PreHolidays",
                DateTime d when (d.Month == 1 && d.Day >= 31) || d.Month == 2 || (d.Month == 3 && d.Day <= 25) => "Term1",
                DateTime d when (d.Month == 4 && d.Day >= 26) || d.Month == 5 || d.Month == 6 || (d.Month == 7 && d.Day <= 10) => "Term2",
                DateTime d when (d.Month == 7 && d.Day >= 11) || d.Month == 8 || d.Month == 9 || (d.Month == 10 && d.Day <= 2) => "Term3",
                DateTime d when (d.Month == 10 && d.Day >= 3) || d.Month == 11 || (d.Month == 12 && d.Day <= 20) => "Term4",
                DateTime d when d.Month == 12 && d.Day >= 21 => "PostHolidays",
                _ => "Not within term"
            };
        }

        public static string change(DataBase database, Entry entry) {
            try {
                return modify(database, entry);
            } catch {
                return create(database, entry);
            }
        }

        public static string change(DataBase database, string path,  Entry entry) {
            try {
                return modify(database, entry);
            } catch {
                return create(database, entry);
            }
        }
    }
}