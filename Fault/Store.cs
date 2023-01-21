using SeraDB;
using System.Text;
using encryption;

namespace back
{
    public static class Store {
        public static string sign = "P@55w0rd";

        public static void init(string filename) {
            new DataBase(filename, new Dictionary<string, object>() {{"__init__", "__init__"}});
        }

        public static byte store(string filename, string key, List<StringBuilder> text) {
            DataBase database = DataBase.load(filename);
            string data = toString(text);
            string encrypt = TwoWay.Encrypt(data, sign);

            try {database.search(key); database.modify(key, encrypt);}
            catch {database.append(key, encrypt);}

            return 0;
        }

        public static List<StringBuilder> load(string filename, string key) {
            DataBase database = DataBase.load(filename);
            string encrypt = (string) database.search(key);
            string decrypt = TwoWay.Decrypt(encrypt, sign);
            return fromString(decrypt);
        }

        public static string toString(List<StringBuilder> text) {
            StringBuilder data = new StringBuilder();
            for (int i = 0; i < text.Count; i++) {
                data.AppendLine(text[i].ToString());
            } return data.ToString();
        }

        public static List<StringBuilder> fromString(string str) {
            string[] lines = str.Split('\n');
            List<StringBuilder> res = new List<StringBuilder>();

            for (int i = 0; i < lines.Length - 1; i++) {
                res.Add(new StringBuilder(lines[i]));
            } return res;
        }
    }
}