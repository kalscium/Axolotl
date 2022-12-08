using System.Reflection;

namespace SeraLib
{
    public static class Symbols {
        public static char at = '­';
        public static char open = '͏';
        public static char close = '⁤';
        public static char com = '⁣';
        public static char raw = '⁠';
        public static char eof = '⁡';

        public static string toString(char special) {
            Dictionary<char, string> dic = new Dictionary<char, string>() {
                {Symbols.at, "@"},
                {Symbols.open, "("},
                {Symbols.close, ")"},
                {Symbols.com, ", "},
                {Symbols.raw, "#"},
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
        public string text;
        public int pos = -1;
        public char currentChar;
        public Dictionary<uint, Type> dest;

        public Parser(string text, Dictionary<uint, Type> dest) {
            this.text = text;
            this.dest = dest;
            this.advance();
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
            List<dynamic> data = this.list();

            if (address == SeraGenerics.List.address) return data;

            return Activator.CreateInstance(this.dest[address], new object[] {new SeraData(data)});
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

    public class SeraBall: List<dynamic> {
        public uint address;

        public SeraBall(uint address) {
            this.address = address;
        }

        public string compile() {
            string result = "";
            result += this.getAddress();
            result += this.list();

            return result;
        }

        string getAddress() {
            return $"{Symbols.at}{this.address}";
        }

        string list() {
            string result = $"{Symbols.open}";
            result += this.getNested(0);

            for (int i = 1; i < this.Count; i++) {
                result += $"{Symbols.com}{this.getNested(i)}";
            } result += Symbols.close;

            return result;
        }

        string getNested(int index) {
            if (this[index] is string) return $"{Symbols.raw}{this[index]}";
            if (this[index] is List<dynamic>) return new SeraGenerics.List(this[index]).seralib().compile();
            if (this[index] is not SeraData) throw new Exception("[SeraLib Error] Cannot compile unknown object");
            return this[index].data.seralib().compile();
        }
    }

    public class SeraGenerics {
        public static Dictionary<uint, Type> dest = new Dictionary<uint, Type>() {
            {List.address, typeof(List)}
        };

        public class List {
            public List<dynamic> list;

            public static uint address = 2341;

            public List(List<dynamic> list) {
                this.list = list;
            }

            public SeraBall seralib() {
                SeraBall result = new SeraBall(List.address);
                for (int i = 0; i < this.list.Count; i++)
                    result.Add(this.list[i]);
                return result;
            }
        }
    }
}