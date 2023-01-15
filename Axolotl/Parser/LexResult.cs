namespace parser
{
    public class LexResult {
        public Error error = null;
        public object tok = null;
        public object cache = null;
        public bool found = false;

        public object register(LexResult res) {
            if (res.error is not null) this.error = res.error;
            this.cache = res.cache;
            return res.tok;
        }

        public LexResult success(object node) {
            this.tok = node;
            return this;
        }

        public LexResult success(object node, object cache) {
            this.tok = node;
            this.cache = cache;
            return this;
        }

        public LexResult failure(Error error) {
            if (this.error is null) this.error = error;
            return this;
        }
    }
}