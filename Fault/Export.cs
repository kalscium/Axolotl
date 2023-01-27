using SeraDB;

namespace back
{
    public static class Export {
        public static void export(DataBase database) {
            string dir = $"({database.filename})";
            Directory.CreateDirectory(dir);
            Entry[] entries = pull(database);
            string index = "===( Index )===\n";
            string last = "";

            for (int i = 0; i < entries.Length; i++) {
                string term = database.index.entries[i].key.Split('.')[1];
                if (last != term) { last = term; index += $"\n===( {term} )===\n"; };
                index += $"{entries[i].index}\u2502 {entries[i].title} : {entries[i].description}\n";
                string path = database.index.entries[i].key;
                path = path.Substring(0, path.LastIndexOf(".")).Replace(".", "/");
                path = Path.Combine(dir, path);
                Directory.CreateDirectory(path);
                entry(path, entries[i]);
            } File.WriteAllText(Path.Combine(dir, "Index.idx"), index);
        }

        public static Entry[] pull(DataBase database) {
            SeraDB.Nodes.Index index = database.index;
            Entry[] res = new Entry[index.entries.Count];
            for (int i = 0; i < index.entries.Count; i++) res[i] = (Entry) database.search(index.entries[i].key);
            return res;
        }

        public static void entry(string path, Entry entry) {
            string filename = $"{entry.index}; {entry.title} - {entry.date.Replace('/', ' ')}";
            string header = $"{entry.date} - {entry.day}\n\n";
            File.WriteAllText(Path.Combine(path, filename), $"{header}{entry.toString()}");
        }

        public static DataBase getDataBase(string filename) {
            if (File.Exists(filename)) return DataBase.load(filename);
            System.Console.WriteLine("[Fault] Error: Cannot export non-existant log");
            return null;
        }
    }
}