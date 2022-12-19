namespace parser
{
    public class Error {
        // Class
        public static string InvalidCharError = "Illegal Character";
        public static string InvalidSyntaxError = "Invalid Syntax";
        public static string ExpectedCharError = "Expected Character";

        // Object
        public string type;
        public string details;

        public Error(string type, string details) {
            this.type = type;
            this.details = details;
        }

        public string repr() {
            string result = $"[Axolotl CLI] Error: {this.type} -> ({this.details})";
            return result;
        }
    }

    public class ErrorMsg {
        // Expected Character Error
        public static string ECE001 = "Expected '\"' (to close string)";
        public static string ECE002 = "Expected '<undesclosed character>'";
        public static string ECE003 = "Expected '-'";
        public static string ECE004 = "Expected character to complete switch";

        // Invalid Syntax Error
        public static string ISE001 = "Expected int, float or string";
        public static string ISE002 = "Expected an identifier";
    }
}