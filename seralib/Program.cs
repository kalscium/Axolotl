namespace SeraDB
{
    public class Program {

        public static void Main(string[] args) {
            Nerd Ethan = new Nerd("Ethan Ma", 12, new string[] {"Programming", "Beating Children", null});
            Nerd Bob = new Nerd("Bob Smith", 13, new string[] {"Eating", "Minecraft"});
            Bob.friend = Ethan;
            Ethan.friend = Bob;

            Dictionary<string, object> dict = new Dictionary<string, object> {
                {"Ethan", Ethan},
                {"Bob", Bob},
                {"msg", "Ethan is really cool"}
            };

            SeraDB.DataBase database = new SeraDB.DataBase("SeraDB.sldb", dict);
            Nerd Ethan2 = (Nerd) database.search("Ethan");
            Nerd Bob2 = (Nerd) database.search("Bob");
            Ethan2.greet();

            database.append("Simp", new Nerd("Joe Stevens", 14, new string[] {"Nothing"}));
            database.append("Harry", new Nerd("Harry Potter", 10, new string[] {"Cool Rich Kid"}));
            database.modify("Simp", new Nerd("Joe Smith", 16, new string[] {"Basketball", "Being Tall"}));
            database.modify("Harry", new Nerd("Harry Stevens", 12, new string[] {"Something"}));
            database.remove("msg");

            database.refactor();
        }
    }

    public class Nerd {
        public string name;
        public int age;
        public string[] skills;
        public Nerd friend;

        public Nerd(string name, int age, string[] skills) {
            this.name = name;
            this.age = age;
            this.skills = skills;
        }

        public void greet() {
            System.Console.WriteLine($"Hello, I am a {this.age} year old, my name is {this.name} and my friend is {this.friend.name}!");
        }

        public static uint address = 2001;

        public Nerd(SeraDB.Entry entry) {}

        public dynamic SeraDB {
            get {return new object[] {
                this.name,
                this.age,
                this.skills,
                this.friend,
            };}
            set {
                this.name = value[0];
                this.age = value[1];
                this.skills = ((object[]) value[2]).Select(o => (string) o).ToArray();
                this.friend = value[3];
            }
        }
    }
}