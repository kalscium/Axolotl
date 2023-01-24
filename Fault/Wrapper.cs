using SeraDB;
using System.Text;

namespace back
{
    public class Wrapper {
        public DataBase database;
        public string title;
        public string description;
        public Entry entry;

        public Wrapper(string dbname, string title=null, string description="") {
            if (File.Exists(dbname)) this.database = DataBase.load(dbname);
            else this.database = new DataBase(dbname, new Dictionary<string, object> {{"__init__", "__init__"}});
            this.title = title;
            this.description = description;

            try {
                this.entry = (Entry) this.database.search(Container.path);
            } catch {
                this.entry = new Entry(title, description);
            }
        }

        public byte save(List<StringBuilder> lines) {
            this.entry.text = lines;
            Container.save(this.database, this.entry);
            return 0;
        }

        public void edit() => new front.Front(this.entry.text, this.save);
    }
}