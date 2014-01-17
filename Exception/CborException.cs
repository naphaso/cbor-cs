namespace Assets.DiverseWorlds.Cbor.Exception {
    class CborException : System.Exception {
        public CborException(string message) : base(message) {
        }

        public CborException(string message, System.Exception innerException) : base(message, innerException) {
        }
    }
}
