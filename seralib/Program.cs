namespace SeraLib
{
    public class Program {
        public static void Main(string[] args) {
            Child child = new Child("Bob", 5);
            Person Martha = new Person("Martha", 19, new List<dynamic>() {"Basketball", "gun"}, child);
            Person Dave = new Person("Dave", 21, new List<dynamic>() {"programmer", "dev"}, child, wife: Martha);

            string output = Dave.seralib().compile();
            SeraBall.flush();
            File.WriteAllText("Person.slo", output);

            List<Type> map = new List<Type>() {
                typeof(Person),
                typeof(Child),
            };

            string input = File.ReadAllText("Person.slo");
            Parser parser = new Parser(input, map);
            Person Dave2 = parser.parse();
            Parser.flush();

            Dave2.greet();
            Person Martha2 = Dave2.wife;
            Martha2.greet();

            Martha2.child.age = 8;
            Martha2.child.greet();
            Dave2.child.greet();
        }
    }

    public class Person {
        public string name;
        public int age;
        public List<dynamic> skills;
        public Child child;
        public string middleName;
        public Person wife;

        public Person(string name, int age, List<dynamic> skills, Child child, string middleName=null, Person wife=null) {
            this.name = name;
            this.age = age;
            this.skills = skills;
            this.child = child;
            this.middleName = middleName;
            this.wife = wife;
        }

        public void greet() {
            System.Console.WriteLine($"Hello, my name is {this.name} {this.middleName} and I am {this.age} years old! Skill: {this.skills[0]}");
        }

        public static uint address = 1001;

        public Person(SeraData data) {
            this.name = data.data[0];
            this.age = data.data[1];
            this.skills = data.data[2];
            this.child = data.data[3];
            this.middleName = data.data[4];
            this.wife = data.data[5];
        }

        public SeraBall seralib() {
            return new SeraBall(this) {
                this.name,
                this.age,
                this.skills,
                this.child,
                this.middleName,
                this.wife,
            };
        }
    }

    public class Child {
        public string name;
        public int age;
        public List<dynamic> children = new List<dynamic>();

        public Child(string name, int age) {
            this.name = name;
            this.age = age;
        }

        public void greet() {
            System.Console.WriteLine($"I am a {this.age} year old child called {this.name}!");
        }

        public static uint address = 1002;

        public Child(SeraData data) {
            this.name = data.data[0];
            this.age = data.data[1];
            this.children = data.data[2];
        }

        public SeraBall seralib() {
            return new SeraBall(this) {
                this.name,
                this.age,
                this.children,
            };
        }
    }
}