namespace SeraLib
{
    public class Program {
        public static void Main(string[] args) {
            Person Dave = new Person("Dave", 21, new List<dynamic> {"programmer", "child"}, new Child("Bob", 5));
            string output = Dave.seralib().compile();
            File.WriteAllText("Person.slo", output);

            Dictionary<uint, Type> des = new Dictionary<uint, Type>() {
                {1001, typeof(Person)},
                {1002, typeof(Child)},
            };

            string input = File.ReadAllText("Person.slo");
            Parser parser = new Parser(input, des);
            Person Dave2 = parser.parse();
            Dave2.greet();
            Dave2.child.greet();
        }
    }

    public class Person {
        public string name;
        public int age;
        public List<dynamic> skills;
        public Child child;
        public string middleName;

        public Person(string name, int age, List<dynamic> skills, Child child, string middleName=null) {
            this.name = name;
            this.age = age;
            this.skills = skills;
            this.child = child;
            this.middleName = middleName;
        }

        public void greet() {
            System.Console.WriteLine($"Hello, my name is {this.name} {this.middleName} and I am {this.age} years old! Skill: {this.skills[0]}");
        }

        public static uint address = 1001;

        public Person(SeraData data) {
            this.name = data.data[0];
            this.age = (int) data.data[1];
            this.skills = data.data[2];
            this.child = data.data[3];
            this.middleName = data.data[4];
        }

        public SeraBall seralib() {
            return new SeraBall(Person.address) {
                this.name,
                (double) this.age,
                this.skills,
                this.child,
                this.middleName,
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
            this.age = (int) data.data[1];
            this.children = data.data[2];
        }

        public SeraBall seralib() {
            return new SeraBall(Child.address) {
                this.name,
                (double) this.age,
                this.children,
            };
        }
    }
}