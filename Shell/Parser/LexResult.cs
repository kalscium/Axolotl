namespace parser
{
    public class LexResult {
        public Error error = null;
        public object tok = null;
        public bool found = false;

        public object register(LexResult res) {
            if (res is LexResult) {
                if (res.error is not null) this.error = res.error;
                return res.tok;
            } throw new Exception("Registered non-LexResult object");
        }

        public LexResult success(object node) {
            this.tok = node;
            return this;
        }

        public LexResult failure(Error error) {
            if (this.error is null) this.error = error;
            return this;
        }
    }
}