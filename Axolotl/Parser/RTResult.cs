namespace parser
{
    public class RTResult {
        public dynamic value;
        public Error error;

        public RTResult() {
            this.reset();
        }

        public void reset() {
            this.value = null;
            this.error = null;
        }

        public dynamic register(RTResult res) {
            if (res.error is not null) return res;
            return res.value;
        }

        public RTResult success(dynamic value) {
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