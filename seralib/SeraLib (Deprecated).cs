using System.Reflection;

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

    public enum Entry {
        Parser
    }

    public struct SeraData {
        public object data;
        public SeraData(object data) {
            this.data = data;
        }
    }

    public class Parser {
        public string text;
        public int pos = -1;
        public char currentChar;
        public Dictionary<uint, Type> dest;
        public Dictionary<int, object> cache = new Dictionary<int, object>();

        public Parser(string text, Dictionary<uint, Type> dest) {
            this.text = text;
            this.dest = dest;
            this.advance();
        }

        public Parser(string text, Type[] list) {
            this.text = text;

            this.dest = new Dictionary<uint, Type>();
            for (int i = 0; i < list.Length; i++) {
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

        public object parse() {
            return this.expr();
        }

        object expr() {
            if (this.currentChar == Symbols.raw) return this.getRaw();
            uint address = this.address();
            int instance = this.instance();

            if (this.cache.ContainsKey(instance)) return this.cache[instance];

            List<object> data = this.list();

            if (address == SeraGenerics.List.address) return data;
            if (address == SeraGenerics.Null.address) return null;
            if (address == SeraGenerics.Number.address) return SeraGenerics.Number.parse((string) data[1], (string) data[0]);

            object result = Activator.CreateInstance(this.dest[address], new object[] {Entry.Parser, data});
            this.cache[instance] = result;

            return result;
        }

        List<object> list() {
            if (this.currentChar != Symbols.open) {
                this.error();
            } this.advance();

            List<object> data = new List<object>();

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
            while (new char[] {'0', '1', '2', '3', '4', '5', '6', '7', '8', '9'}.Contains(this.currentChar)) {
                result += this.currentChar;
                this.advance();
            } return uint.Parse(result);
        }

        int instance() {
            if (this.currentChar != Symbols.inst) {
                this.error();
            } this.advance();

            string result = "";
            while (new char[] {'0', '1', '2', '3', '4', '5', '6', '7', '8', '9'}.Contains(this.currentChar)) {
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
    }

    public class SeraBall: List<object> {
        public List<int> cache = new List<int>();
        public uint address;
        public object instance;

        public SeraBall(object instance) {
            this.instance = instance;
            this.address = (uint) instance.GetType().GetField("address").GetValue(null);
        }

        public string compile(List<int> cache) {
            this.cache = cache;
            string result = "";
            result += this.getAddress();
            result += this.getInstance();

            return result;
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

            if (!this.cache.Contains(hashCode)) {
                this.cache.Add(hashCode);
                return $"{Symbols.inst}{this.cache.IndexOf(hashCode)}{this.list()}";
            } return $"{Symbols.inst}{this.cache.IndexOf(hashCode)}";
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
            Func<object, string> compile = (object obj) => ((SeraBall) obj.GetType().GetMethod("seralib").Invoke(this[index], new object[0])).compile(this.cache);

            if (this[index] is string) return $"{Symbols.raw}{this[index]}";
            if (this[index] is List<object>) return new SeraGenerics.List((List<object>) this[index]).seralib().compile(this.cache);
            if (this[index] is null) return new SeraGenerics.Null().seralib().compile(this.cache);
            if (SeraGenerics.Number.map.ContainsValue(this[index].GetType())) return new SeraGenerics.Number(this[index]).seralib().compile(this.cache);
            if (this[index] is not SeraData) try {return compile(this[index]);} catch {throw new Exception("[SeraLib Error] Cannot compile unknown object");}
            return compile(((SeraData) this[index]).data);
        }
    }
 
    public class SeraGenerics {
        public static Dictionary<uint, Type> dest = new Dictionary<uint, Type>() {
            {List.address, typeof(List)}
        };

        public class List {
            public List<object> list;

            public static uint address = 1;

            public List(List<object> list) {
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
            public object number;
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

            public Number(object number) {
                this.number = number;
                this.type = map.FirstOrDefault(x => x.Value == number.GetType()).Key;
            }

            public SeraBall seralib() {
                return new SeraBall(this) {this.number.ToString(), type};
            }

            public static object parse(string name, string number) {
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
