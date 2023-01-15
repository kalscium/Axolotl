namespace parser
{
    public class ParseResult {
        // Static
        public Error error = null;
        public object node = null;
        public uint advanceCount = 0;

        public void registerAdvance(object advance) {
            this.advanceCount++;
        }

        public object register(ParseResult res) {
            this.advanceCount += res.advanceCount;
            if (res.error is not null) this.error = res.error;
            return res.node;
        }

        public ParseResult success(object node) {
            this.node = node;
            return this;
        }

        public ParseResult failure(Error error) {
            if (this.error is null || this.advanceCount == 0)  {
                this.error = error;
            } return this;
        }
    }
}