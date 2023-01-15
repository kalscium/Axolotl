using System.Reflection;

#pragma warning disable CS8600, CS8603, CS8602, CS8601, CS8618, CS8625

namespace SeraDB
{
    public static class Symbols {
        public static char at = '@';
        public static char raw = '#';
        public static char dsh = '-';
        public static char idx = 'i';
        public static char com = ',';
        public static char obj = '$';
        public static char type = 't';
        public static char list = '&';
        public static char none = '^';
        public static char eof = 'ꝏ';
    }

    public struct SeraData {
        public object data;
        public SeraData(object data) {
            this.data = data;
        }
    }

    public class Parser {
        public Nodes.Index index;
        public string fileLoc;
        private Dictionary<int, object> cache = new Dictionary<int, object>();
        public Dictionary<int, object> objCache = new Dictionary<int, object>();

        public Parser(string fileLoc) {
            this.fileLoc = fileLoc;
            this.index = new X(this, this.getLineValue(1), new Nodes.Line("1", "1")).index();
        }

        public object getRaw(string key) {
            Nodes.Line lines = this.index.search(key);
            return this.visit(lines);
        }

        public object visit(Nodes.Line line) {
            string value = this.getLineValue(line);
            return new X(this, value, line).stmt();
        }

        public List<uint> refvisit(Nodes.Line line) {
            string value = this.getLineValue(line);
            return new Z(this, value, line).stmt();
        }

        public string getLineValue(uint idx) {
            using (StreamReader reader = new StreamReader(this.fileLoc)) {
                for (uint i = 1; i < idx; i++) {
                    reader.ReadLine();
                } string line = reader.ReadLine();
                return line;
            }
        }

        public string getLineValue(Nodes.Line lines) {
            using (StreamReader reader = new StreamReader(this.fileLoc)) {
                string line = "";
                uint total = 1;
                for (int i = 0; i < lines.lines.Count; i++) {
                    if (lines.lines[0] == lines.lines[1] && i > 0) continue;
                    for (uint a = total; a < lines.lines[i]; a++) {
                        reader.ReadLine();
                        total++;
                    } line += reader.ReadLine(); total++;
                } return line;
            }
        }

        public class Z: X {
            private List<uint> dep = new List<uint>();

            public Z(Parser parser, string text, Nodes.Line line): base(parser, text, line) {
                this.addLine(this.line);
            }

            public new List<uint> stmt() {
                char type = this.currentChar;
                if (type == Symbols.eof) this.error("Statement does not contain value type");
                if (type == Symbols.obj) this.obj();
                if (type == Symbols.list) this.list();
                if (type == Symbols.idx) this.index();
                if (type == Symbols.at) this.getAt();

                return this.dep;
            }

            public void addLine(Nodes.Line line) {
                if (line.lines[0] == line.lines[1]) {
                    this.dep.Add(line.lines[0]);
                    return;
                } for (int i = 0; i < line.lines.Count; i++) {
                    this.dep.Add(line.lines[i]);
                }
            }

            public new void getAt() => this.dep.AddRange(this.parser.refvisit(this.at()));

            public new void index() {
                if (this.currentChar != Symbols.idx) this.error("Index Search Statement does not contain statement type");
                this.advance();
                this.getString();
            }

            public new void obj() {
                if (this.currentChar != Symbols.obj) this.error("Object Statement does not contain statement type");
                this.advance();
                this.advance();
                this.getString();
                while (this.currentChar == Symbols.com) {
                    this.advance();
                    if (this.currentChar == ' ') this.advance();
                    this.stmt();
                }
            }

            public new void list() {
                if (this.currentChar != Symbols.list) this.error("List Statement does not contain statement type");
                this.advance();

                if (this.currentChar == Symbols.eof) return;

                this.stmt();
                while (this.currentChar == Symbols.com) {
                    this.advance();
                    if (this.currentChar == ' ') this.advance();
                    this.stmt();
                }
            }
        }

        public class X {
            public string text = "";
            public int pos = -1;
            public char currentChar;
            public Parser parser;
            public Nodes.Line line;

            public X(Parser parser, string text, Nodes.Line line) {
                this.parser = parser;
                this.text = text;
                this.line = line;
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

            public object stmt() {
                char type = this.currentChar;
                if (type == Symbols.eof) this.error("Statement does not contain value type");
                if (type == Symbols.idx) return this.getIndex();
                if (type == Symbols.raw) return this.getRaw();
                if (type == Symbols.obj) return this.obj();
                if (type == Symbols.at) return this.getAt();
                if (type == Symbols.list) return this.list();
                if (type == Symbols.none) return null;

                return this.error("Statement has invalid type");
            }

            public object[] list() {
                if (this.currentChar != Symbols.list) this.error("List Statement does not contain statement type");
                List<object> list = new List<object>();
                this.advance();

                if (this.currentChar == Symbols.eof) return new object[0];

                list.Add(this.stmt());
                while (this.currentChar == Symbols.com) {
                    this.advance();
                    if (this.currentChar == ' ') this.advance();
                    list.Add(this.stmt());
                } return list.ToArray();
            }

            public object getAt() => this.parser.visit(this.at());

            public Nodes.Object obj() {
                int hashCode = this.line.GetHashCode();
                if (this.parser.cache.ContainsKey(hashCode)) return (Nodes.Object) this.parser.cache[hashCode];
                if (this.currentChar != Symbols.obj) this.error("Object Statement does not contain statement type");

                Nodes.Object obj = new Nodes.Object(this.parser);
                this.parser.cache.Add(hashCode, obj);
                this.advance();

                obj.type = this.objType();
                while (this.currentChar == Symbols.com) {
                    this.advance();
                    if (this.currentChar == ' ') this.advance();
                    obj.raw.Add(this.stmt());
                } return obj;
            }

            public object getIndex() {
                if (this.currentChar != Symbols.idx) this.error("Index Search Statement does not contain statement type");
                this.advance();
                Nodes.Line lines = this.parser.index.search(this.getString());
                if (this.parser.cache.ContainsKey(lines.GetHashCode())) return this.parser.cache[lines.GetHashCode()];
                object output = this.parser.visit(lines);
                return output;
            }

            public string getString() {
                char[] alphabet = {'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z','A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '.', '_'};
                string res = "";

                while (alphabet.Contains(this.currentChar)) {
                    res += this.currentChar;
                    this.advance();
                } return res;
            }

            public string objType() {
                if (this.currentChar != Symbols.type) this.error("Object Type does not contain statement type");
                this.advance();
                return this.getString();
            }

            public Nodes.Index index() {
                if (this.currentChar != Symbols.idx) this.error("Index does not contain index type");
                Nodes.Index index = new Nodes.Index();
                this.advance();

                Nodes.Index.Entry getEntry() {
                    Nodes.Index.Entry entry = new Nodes.Index.Entry();
                    entry.key = this.getString();
                    entry.line = this.at();

                    return entry;
                } index.entries.Add(getEntry());

                while (this.currentChar == Symbols.com) {
                    this.advance();
                    if (this.currentChar == ' ') this.advance();
                    index.entries.Add(getEntry());
                }

                return index;
            }

            public string getRaw() {
                if (this.currentChar != Symbols.raw) this.error("Raw Data does not contain raw data type");
                string value = "";
                this.advance();
                
                while (this.currentChar != Symbols.eof) {
                    value += this.currentChar;
                    this.advance();
                } return value;
            }

            public Nodes.Line at() {
                if (this.currentChar != Symbols.at) this.error("Line Definition does not contain 'at' type");
                this.advance();

                string getNum() {
                    char[] nums = {'0', '1', '2', '3', '4', '5', '6', '7', '8', '9'};
                    string num = "";

                    while (nums.Contains(this.currentChar)) {
                        num += this.currentChar;
                        this.advance();
                    } return num;
                } string first = getNum();

                if (this.currentChar != Symbols.dsh) return new Nodes.Line(first, first);
                this.advance();
                return new Nodes.Line(first, getNum());
            }

            public byte error(string msg) => throw new Exception($"[SeraDB X] Error: {msg}");
        }
    }

    public class Compiler {
        public Dictionary<string, object> map;
        public Nodes.Index index = new Nodes.Index();
        public uint free = 2;

        public Compiler(Dictionary<string, object> map) {
            this.map = map;
        }

        public Nodes.Line advance() {
            string free = this.free.ToString();
            Nodes.Line line = new Nodes.Line(free, free);
            this.free++;
            return line;
        }

        public string init() {
            string output = "";
            for (int i = 0; i < this.map.Keys.Count; i++) {
                Nodes.Index.Entry entry = new Nodes.Index.Entry();
                entry.key = this.map.Keys.ToArray()[i];
                entry.line = this.advance();

                this.index.entries.Add(entry);
                output += '\n' + this.expr(this.map[entry.key]);
            } return idx(this.index) + output;
        }

        public string expr(object obj) => obj switch {
            object o when o.GetType().IsArray => this.array(obj),
            string => raw((string) obj),
            null => Symbols.none.ToString(),
            _ => this.obj(obj),
        };

        public string array(object obj) {
            object[] list = (object[]) obj;
            string[] contents = new string[list.Length];
            string depends = "";
 
            for (int i = 0; i < list.Length; i++) {
                if (this.map.Values.Contains(list[i])) {
                    contents[i] = this.getIndex(list[i]);
                    continue;
                } depends += '\n' + this.expr(list[i]);
                contents[i] = at(this.advance());
            } return Symbols.list + mkList(contents) + depends;
        }

        public string obj(object obj) {
            object[] list; 
            try { list = (object[]) obj.GetType().GetProperty("SeraDB").GetValue(obj);
            } catch {
                try {list = new object[] {safeMethod("ToString", obj.GetType(), obj, new object[0])};}
                catch {list = new object[0];}
            }

            string[] contents = new string[list.Length + 1];
            string depends = "";

            contents[0] = getType(obj);
            for (int i = 0; i < list.Length; i++) {
                if (this.map.Values.Contains(list[i])) {
                    contents[i + 1] = this.getIndex(list[i]);
                    continue;
                } contents[i + 1] = at(this.advance());
                depends += '\n' + this.expr(list[i]);
            } return Symbols.obj + mkList(contents) + depends;
        }

        public static object safeMethod(string name, Type type, object obj, object[] args) {
            System.Collections.Generic.IEnumerable<MethodInfo> methods = type.GetMethods().
            Where(x => x.Name == name);

            foreach (MethodInfo i in methods) {
                try {return i.Invoke(obj, args);}
                catch {};
            } throw new Exception($"[SeraDB Obj] Error: Could not find corresponding method for '{name}'");
        }

        public string getIndex(object obj) {
            string[] keys = this.map.Keys.ToArray();
            for (int i = 0; i < keys.Length; i++) {
                if (this.map[keys[i]] == obj) return Symbols.idx + keys[i];
            } return error("Cannot find key of this object").ToString();
        }

        public static string getType(object obj) {
            return Symbols.type + obj.GetType().FullName; // Experimental
            // try {
            //     return Symbols.type + (string) obj.GetType().GetField("SeraDB_address").GetValue(null);
            // } catch {
            //     return error("Cannot compile an unknown object").ToString();
            // }
        }

        public static string raw(string str) => Symbols.raw + str;

        public static string idx(Nodes.Index index) {
            string[] items = new string[index.entries.Count];
            for (int i = 0; i < index.entries.Count; i++) {
                string key = index.entries[i].key;
                Nodes.Line line = index.entries[i].line;

                items[i] = key + at(line);
            } return Symbols.idx + mkList(items);
        }

        public static string mkList(string[] list) {
            string output = "";
            for (int i = 0; i < list.Length; i++) {
                output += i == list.Length - 1 ? list[i]: list[i] + Symbols.com;
            } return output;
        }

        public static string at(Nodes.Line line) => line.lines[0] != line.lines[1] ? Symbols.at + line.lines[0].ToString() + "-" + line.lines[line.lines.Count - 1]: Symbols.at + line.lines[0].ToString();

        public static byte error(string msg) => throw new Exception($"[SeraDB Compiler] Error: {msg}");
    }

    public class DataBase {
        public string filename;
        private Parser parser;
        private Nodes.Index index;
        private uint fline;

        public static DataBase load(string filename) {
            DataBase database = new DataBase(Entry.DataBase);
            database.filename = filename;
            database.parser = new Parser(database.filename);
            database.index = database.parser.index;
            database.fline = uint.Parse(database.readLastLine());

            return database;
        }

        public DataBase(string filename, Dictionary<string, object> map) {
            Compiler compiler = new Compiler(map);
            string data = compiler.init();
            this.fline = compiler.free;

            data += $"\n{this.fline}";
            File.WriteAllText(filename, data);

            this.filename = filename;
            this.parser = new Parser(this.filename);
            this.index = this.parser.index;
        }
        
        public DataBase(Entry entry) {}

        public object search(string key) {
            Nodes.Line lines = this.parser.index.search(key);
            object obj = this.parser.visit(lines);

            return obj switch {
                Nodes.Object => ((Nodes.Object) obj).generate(),
                string => obj,
                object[] => Nodes.Object.getNested((object[]) obj, new Dictionary<int, object>()),
                _ => null,
            };
        }

        public void refactor() {
            Dictionary<string, object> map = new Dictionary<string, object>();
            for (int i = 0; i< this.index.entries.Count; i++) {
                string key = this.index.entries[i].key;
                object obj = this.search(this.index.entries[i].key);
                map.Add(key, obj);
            }

            File.Delete(this.filename);

            Compiler compiler = new Compiler(new Dictionary<string, object>(map));
            string data = compiler.init();
            this.fline = compiler.free;

            data += $"\n{this.fline}";
            File.WriteAllText(this.filename, data);

            this.parser = new Parser(this.filename);
            this.index = this.parser.index;
        }

        public void modify(string key, object obj) {
            this.remove(key);
            this.append(key, obj);
        }

        public void remove(string key) {
            Nodes.Line obj = this.index.search(key);

            this.index.remove(key);
            this.parser.visit(obj);

            List<uint> lines = this.parser.refvisit(obj);
            lines.Insert(0, 1);
            lines.Add(this.fline);

            string[] replaces = new string[lines.Count];
            for (int i = 0; i < replaces.Length; i++) replaces[i] = "";
            replaces[replaces.Length - 1] = this.fline.ToString();
            replaces[0] = Compiler.idx(this.index);

            this.edit(lines, replaces);
        }

        public void append(string key, object obj) {
            Compiler compiler = new Compiler(new Dictionary<string, object> {{key, obj}});
            compiler.free = this.fline;

            string output = "";
            for (int i = 0; i < compiler.map.Keys.Count; i++) {
                Nodes.Index.Entry entry = new Nodes.Index.Entry();
                entry.key = compiler.map.Keys.ToArray()[i];
                entry.line = compiler.advance();

                this.index.entries.Add(entry);
                output += i == 0 ? compiler.expr(compiler.map[entry.key]): '\n' + compiler.expr(compiler.map[entry.key]);
            } this.edit(new uint[] {1, this.fline, this.fline + 1}, new string[] {Compiler.idx(this.index), output, compiler.free.ToString()});
            this.fline = compiler.free;
        }

        public object rawSearch(string key) => this.parser.getRaw(key);

        private void edit(uint[] idx, string[] replace) {
            using (StreamReader reader = new StreamReader(this.filename))
            using (StreamWriter writer = new StreamWriter(this.filename + ".tmp")) {
                string output = "";
                string line = "";
                for (uint i = 1; true; i++) {
                    line = reader.ReadLine();
                    if (idx.Contains(i)) line = replace[Array.IndexOf(idx, i)];
                    if (line is null) break;
                    if (i == 1) writer.Write(line);
                    else writer.Write('\n' + line);
                    output += line + "\n";
                }
            } File.Delete(this.filename);
            File.Move(this.filename + ".tmp", this.filename);
        }

        private void edit(List<uint> idx, string[] replace) {
            using (StreamReader reader = new StreamReader(this.filename))
            using (StreamWriter writer = new StreamWriter(this.filename + ".tmp")) {
                string line = "";
                for (uint i = 1; true; i++) {
                    line = reader.ReadLine();
                    if (idx.Contains(i)) line = replace[idx.IndexOf(i)];
                    if (line is null) break;
                    if (i == 1) writer.Write(line);
                    else writer.Write('\n' + line);
                }
            } File.Delete(this.filename);
            File.Move(this.filename + ".tmp", this.filename);
        }

        private string readLastLine() {
            using (StreamReader reader = new StreamReader(this.filename)) {
                string line;
                string past = "";
                for (uint i = 1; true; i++) {
                    line = reader.ReadLine();
                    if (line is null) break;
                    past = line;
                } return past;
            }
        }
    }
}

namespace SeraDB {
    public enum Entry {
        ObjectGeneration,
        DataBase,
    }

    namespace Nodes
{
    public class Object {
        public string type = "";
        public List<object> raw = new List<object>();
        private Dictionary<int, object> cache;
        private Parser parser;

        public Object(Parser parser) {
            this.parser = parser;
            this.cache = this.parser.objCache;
        }

        public object generate() {
            int hashCode = this.GetHashCode();
            if (this.cache.ContainsKey(hashCode)) return this.cache[hashCode];

            Type type = Type.GetType(this.type);
            if (type is null) throw new Exception($"[SeraDB Object] Object Type '{this.type}' not found in current context");

            object obj;
            try {obj = Activator.CreateInstance(type, new object[] {Entry.ObjectGeneration});}
            catch {return Compiler.safeMethod("Parse", type, null, getNested(this.raw.ToArray(), this.cache));}

            this.cache[hashCode] = obj;
            obj.GetType().GetProperty("SeraDB").SetValue(obj, getNested(this.raw.ToArray(), this.cache));

            return obj;
        }

        public static object[] getNested(object[] raw, Dictionary<int, object> cache) {
            object[] res = new object[raw.Length];
            for (int i = 0; i < raw.Length; i++) {
                object data = raw[i];
                if (data is Nodes.Object) {
                    Nodes.Object dat = (Nodes.Object) data;
                    int hashCode = dat.GetHashCode();
                    if (cache.ContainsKey(hashCode)) res[i] = cache[hashCode];
                    else {
                        dat.cache = cache;
                        res[i] = dat.generate();
                    }
                } else if (data is object[]) res[i] = getNested((object[]) data, cache);
                else res[i] = data;
            } return res;
        }
    }

    public class Line {
        public List<uint> lines;

        public Line(string rstart, string rend) {
            uint start = uint.Parse(rstart);
            uint end = uint.Parse(rend);
            uint dif = end - start;

            this.lines = new List<uint> {start};
            for (uint i = 1; i < dif; i++) {
                this.lines.Add(start + i);
            } this.lines.Add(end);
        }

        public uint avg() {
            uint output = 0;
            for (int i = 0; i < this.lines.Count; i++) {
                output += this.lines[i];
            } return output / (uint) this.lines.Count;
        }
    }

    public class Index {
        public struct Entry {
            public string key;
            public Nodes.Line line;
        }

        public List<Entry> entries = new List<Entry>();
        // public List<Line> accessed = new List<Line>();

        public Nodes.Line search(string key) {
            for (int i = 0; i < entries.Count; i++) {
                if (entries[i].key == key) {
                    // this.accessed.Add(entries[i].line);
                    return entries[i].line;
                }
            } throw new Exception($"[SeraDB Index] Object '{key}' not found in index");
        }

        public void remove(string key) {
            for (int i = 0; i < entries.Count; i++) {
                if (entries[i].key == key) {
                    this.entries.RemoveAt(i);
                    break;
                }
            }
        }
    }
}}
