namespace parser
{
    public class RTResult {
        public object value;
        public Error error;

        public RTResult() {
            this.reset();
        }

        public void reset() {
            this.value = null;
            this.error = null;
        }

        public object register(RTResult res) {
            if (res.error is not null) return res;
            return res.value;
        }

        public RTResult success(object value) {
            this.reset();
            this.value = value;
            return this;
        }

        public RTResult failure(Error error) {
            this.reset();
            this.error = error;
            return this;
        }
    }
}