using SeraDB;
using System.Text;

namespace back
{
    public class Wrapper {
        public DataBase database;
        public string title;
        public string description;
        public Entry entry;
        public front.Front front;

        public Wrapper(string dbname, string title=null, string description="") {
            if (File.Exists(dbname)) this.database = DataBase.load(dbname);
            else this.database = new DataBase(dbname, new Dictionary<string, object> {{"__init__", "__init__"}});
            this.title = title;
            this.description = description;

            try {
                this.entry = (Entry) this.database.search(Container.path);
            } catch {
                this.entry = new Entry(title, description);
            } this.front = new front.Front(this.entry.text, this.save);
        }

        public byte save() {
            Container.save(this.database, this.entry);
            return 0;
        }

        public void edit() {
            new Thread(() => Console.CancelKeyPress += onExit).Start();
            this.front.run();
        }

        public void onExit(object sender, ConsoleCancelEventArgs e) {
            this.save();
            this.database.remove("__init__");
            this.database.refactor();
            Console.Clear();
            Environment.Exit(0);
        }
    }
}