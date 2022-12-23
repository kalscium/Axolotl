namespace touch
{
    public class Touch {
        public static void Main(string[] args) {
            if (args.Length > 0) {
                File.Create(args[0]);
            } else {
                System.Console.WriteLine("[]");
            }
        }
    }
}