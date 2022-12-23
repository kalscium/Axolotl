using System.Reflection;

namespace axoapi
{
    public class Intrapackage {
        public User user {get;}
        public List<string> cmds {get;}
        public List<string> switches {get;}
        public List<dynamic> values {get;}
        public List<ushort> order {get;}
        public string sign {get;}

        public Intrapackage() {}

        public string advance() {
            if (this.cmds.Count == 0) return "";
            string cmd = this.cmds[0];
            this.cmds.RemoveAt(0);
            return cmd;
        }

        public string run(Dictionary<string, Type> dict) {
            if (this.cmds.Count == 0) return null;
            string cmd = this.advance();

            if (!dict.ContainsKey(cmd))
                return $"[Axolotl] Error: Cmd '{cmd}' not found within program";

            Type type = dict[cmd];
            type.GetMethod("entry").Invoke(null, new object[] {this});

            return null;
        }

        public bool checkSwitch(params string[] swtches) {
            bool isSwitch = true;
            for (int i = 0; i < swtches.Length; i++) {
                if (!this.switches.Contains(swtches[i])) isSwitch = false;
            } return isSwitch;
        }

        public int getSwitch(string swtch) {
            if (!this.checkSwitch(swtch)) return -1;
            int idx = this.switches.IndexOf(swtch);
            return this.order[idx];
        }

        public static List<T2> listTo<T, T2>(List<T> list) {
            List<T2> result = new List<T2>();
            for (int i = 0; i < list.Count; i++) {
                result.Add((dynamic) list[i]);
            } return result;
        }

        /////////////////////
        // SeraLib Stuff
        /////////////////////

        public static uint address = 1003;

        public Intrapackage(axoapi.SeraLib.SeraData data) {
            this.user = data.data[0];
            this.cmds = listTo<dynamic, string>(data.data[1]);
            this.switches = listTo<dynamic, string>(data.data[2]);
            this.values = data.data[3];
            this.order = listTo<dynamic, ushort>(data.data[4]);
            this.sign = data.data[5];
        }
    }

    public class User {
        public string username {get;}
        public Password password {get;}

        public string fullName {get;}
        public byte age {get;}

        public User() {}

        /////////////////////
        // SeraLib Stuff
        /////////////////////

        public static uint address = 1001;

        public User(SeraLib.SeraData data) {
            this.username = data.data[0];
            this.password = data.data[1];
            this.fullName = data.data[2];
            this.age = data.data[3];
        }
    }

    public class Password {
        public string hash {get;}
        public string salt {get;}

        public Password() {}

        /////////////////////
        // SeraLib Stuff
        /////////////////////

        public static uint address = 1002;

        public Password(axoapi.SeraLib.SeraData data) {
            this.hash = data.data[0];
            this.salt = data.data[1];
        }
    }

    public class Compiler {
        public static List<Type> mapped = new List<Type>() {
            typeof(Intrapackage),
            typeof(User),
            typeof(Password),
        };

        public static dynamic parse(string text) {
            axoapi.SeraLib.Parser parser = new axoapi.SeraLib.Parser(text, mapped);
            axoapi.SeraLib.Parser.flush();
            return parser.parse();
        }
    }

    namespace SeraLib
    {
        public static class Symbols {
            public static char at = '­';
            public static char open = '͏';
            public static char close = '⁤';
            public static char com = '⁣';
            public static char raw = '⁠';
            public static char inst = '᠎';
            public static char eof = '⁡';

            public static string toString(char special) {
                Dictionary<char, string> dic = new Dictionary<char, string>() {
                    {Symbols.at, "@"},
                    {Symbols.open, "("},
                    {Symbols.close, ")"},
                    {Symbols.com, ", "},
                    {Symbols.raw, "#"},
                    {Symbols.inst, "&"},
                    {Symbols.eof, "/"},
                }; return dic[special];
            }

            public static bool contains(char other) {
                if (new List<char> {Symbols.at, Symbols.open, Symbols.close, Symbols.com, Symbols.raw, Symbols.eof}.Contains(other)) return true;
                return false;
            }
        }

        public struct SeraData {
            public dynamic data;
            public SeraData(dynamic data) {
                this.data = data;
            }
        }

        public class Parser {
            public static Dictionary<int, dynamic> cache = new Dictionary<int, dynamic>();
            public string text;
            public int pos = -1;
            public char currentChar;
            public Dictionary<uint, Type> dest;

            public Parser(string text, Dictionary<uint, Type> dest) {
                this.text = text;
                this.dest = dest;
                this.advance();
            }

            public Parser(string text, List<Type> list) {
                this.text = text;

                this.dest = new Dictionary<uint, Type>();
                for (int i = 0; i < list.Count; i++) {
                    Type type = list[i];
                    this.dest[(uint) type.GetField("address").GetValue(null)] = type;
                } this.advance();
            }

            public void advance() {
                if (this.pos == this.text.Length - 1) {
                    this.currentChar = Symbols.eof;
                } else {
                    this.pos++;
                    this.currentChar = this.text[pos];
                }
            }

            public dynamic parse() {
                return this.expr();
            }

            dynamic expr() {
                if (this.currentChar == Symbols.raw) return this.getRaw();
                uint address = this.address();
                int instance = this.instance();

                if (cache.ContainsKey(instance)) return cache[instance];

                List<dynamic> data = this.list();

                if (address == SeraGenerics.List.address) return data;
                if (address == SeraGenerics.Null.address) return null;
                if (address == SeraGenerics.Number.address) return SeraGenerics.Number.parse(data[1], data[0]);

                dynamic result = Activator.CreateInstance(this.dest[address], new object[] {new SeraData(data)});
                cache[instance] = result;

                return result;
            }

            List<dynamic> list() {
                if (this.currentChar != Symbols.open) {
                    this.error();
                } this.advance();

                List<dynamic> data = new List<dynamic>();

                if (this.currentChar == Symbols.close) {
                    this.advance();
                    return data;
                } data.Add(this.expr());

                while (this.currentChar == Symbols.com) {
                    this.advance();
                    data.Add(this.expr());
                }

                if (this.currentChar != Symbols.close) this.error();
                this.advance();
                return data;
            }

            uint address() {
                if (this.currentChar != Symbols.at) {
                    this.error();
                } this.advance();

                string result = "";
                while (new List<char>() {'0', '1', '2', '3', '4', '5', '6', '7', '8', '9'}.Contains(this.currentChar)) {
                    result += this.currentChar;
                    this.advance();
                } return uint.Parse(result);
            }

            int instance() {
                if (this.currentChar != Symbols.inst) {
                    this.error();
                } this.advance();

                string result = "";
                while (new List<char>() {'0', '1', '2', '3', '4', '5', '6', '7', '8', '9'}.Contains(this.currentChar)) {
                    result += this.currentChar;
                    this.advance();
                } return int.Parse(result);
            }

            string getRaw() {
                if (this.currentChar != Symbols.raw) {
                    this.error();
                } this.advance();

                string result = "";
                while (!Symbols.contains(this.currentChar)) {
                    result += this.currentChar;
                    this.advance();
                } return result;
            }

            public void error() {
                throw new Exception("[SeraLib Error] Could not parse data");
            }

            public static void flush() {
                cache = new Dictionary<int, dynamic>();
            }
        }

        public class SeraBall: List<dynamic> {
            public static List<int> cache = new List<int>();
            public uint address;
            public dynamic instance;

            public SeraBall(dynamic instance) {
                this.instance = instance;
                this.address = (uint) instance.GetType().GetField("address").GetValue(null);
            }

            public string compile() {
                string result = "";
                result += this.getAddress();
                result += this.getInstance();

                return result;
            }

            string getAddress() {
                return $"{Symbols.at}{this.address}";
            }

            string getInstance() {
                int hashCode = this.instance.GetHashCode();

                if (!cache.Contains(hashCode)) {
                    cache.Add(hashCode);
                    return $"{Symbols.inst}{cache.IndexOf(hashCode)}{this.list()}";
                } return $"{Symbols.inst}{cache.IndexOf(hashCode)}";
            }

            string list() {
                string result = $"{Symbols.open}";
                if (this.Count > 0) result += this.getNested(0);

                for (int i = 1; i < this.Count; i++) {
                    result += $"{Symbols.com}{this.getNested(i)}";
                } result += Symbols.close;

                return result;
            }

            string getNested(int index) {
                if (this[index] is string) return $"{Symbols.raw}{this[index]}";
                if (this[index] is List<dynamic>) return new SeraGenerics.List(this[index]).seralib().compile();
                if (this[index] is null) return new SeraGenerics.Null().seralib().compile();
                if (SeraGenerics.Number.map.ContainsValue(this[index].GetType())) return new SeraGenerics.Number(this[index]).seralib().compile();
                if (this[index] is not SeraData) try {return this[index].seralib().compile();} catch {throw new Exception("[SeraLib Error] Cannot compile unknown object");}
                return this[index].data.seralib().compile();
            }

            public static void flush() {
                cache = new List<int>();
            }
        }

        public class SeraGenerics {
            public static Dictionary<uint, Type> dest = new Dictionary<uint, Type>() {
                {List.address, typeof(List)}
            };

            public class List {
                public List<dynamic> list;

                public static uint address = 1;

                public List(List<dynamic> list) {
                    this.list = list;
                }

                public SeraBall seralib() {
                    SeraBall result = new SeraBall(this);
                    for (int i = 0; i < this.list.Count; i++)
                        result.Add(this.list[i]);
                    return result;
                }
            }

            public class Null {
                public string data = "<null>";
                public static uint address = 2;

                public Null() {}
                public Null(SeraData data) {}

                public SeraBall seralib() {
                    return new SeraBall(this) {this.data};
                }
            }

            public class Number {
                public dynamic number;
                public string type;
                public static uint address = 3;
                public static Dictionary<string, Type> map = new Dictionary<string, Type> {
                    {"sb", typeof(sbyte)},
                    {"b", typeof(byte)},
                    {"s", typeof(short)},
                    {"us", typeof(ushort)},
                    {"i", typeof(int)},
                    {"ui", typeof(uint)},
                    {"l", typeof(long)},
                    {"ul", typeof(ulong)},
                    {"ni", typeof(nint)},
                    {"nui", typeof(nuint)},
                };

                public Number(dynamic number) {
                    this.number = number;
                    this.type = map.FirstOrDefault(x => x.Value == number.GetType()).Key;
                }

                public SeraBall seralib() {
                    return new SeraBall(this) {this.number.ToString(), type};
                }

                public static dynamic parse(string name, string number) {
                    Type type = map[name];
                    System.Collections.Generic.IEnumerable<MethodInfo> methods = type.GetMethods().
                    Where(x => x.Name == "Parse");

                    foreach (MethodInfo i in methods) {
                        try {return i.Invoke(null, new object[] {number});}
                        catch {};
                    } throw new Exception("[CS Error] Could not parse number data");
                }
            }
        }
    }
}