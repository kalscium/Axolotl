using System.Text;

namespace back
{
    public class Entry {
        public static string sign = "What the fuck";
        public short index;
        public string title;
        public string description;
        public List<StringBuilder> text;

        public Entry(string title=null, string description="") {
            this.index = (short) (DateTime.Now - new DateTime(DateTime.Now.Year, 1, 1)).TotalDays;
            this.title = title is not null ? title: DateTime.Now.DayOfWeek.ToString();
            this.description = description;
            this.text = new List<StringBuilder>() {new StringBuilder()};
        }

        public new string ToString() {
            StringBuilder data = new StringBuilder();
            for (int i = 0; i < this.text.Count; i++) data.AppendLine(this.text[i].ToString());
            return encryption.TwoWay.Encrypt(data.ToString(), sign);
        }

        public static List<StringBuilder> FromString(string text) {
            text = encryption.TwoWay.Decrypt(text, sign);
            string[] lines = text.Split('\n');
            List<StringBuilder> res = new List<StringBuilder>();
            for (int i = 0; i < lines.Length - 1; i++) {
                res.Add(new StringBuilder(lines[i]));
            } return res;
        }

        public Entry(SeraDB.Entry entry) {}


        public object SeraDB {
            get {
                return new object[] {
                    this.index,
                    this.title,
                    this.description,
                    this.ToString(),
                };
            } set {
                object[] list = (object[]) value;
                this.index = (short) list[0];
                this.title = (string) list[1];
                this.description = (string) list[2];
                this.text = FromString((string) list[3]);
            }
        }
    }
}