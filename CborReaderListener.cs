namespace Assets.DiverseWorlds.Cbor {
    public interface CborReaderListener {
        void OnInteger(uint value, int sign);
        void OnLong(ulong value, int sign);
        void OnBytes(byte[] value);
        void OnString(string value);
        void OnArray(int size);
        void OnMap(int size);
        void OnTag(uint tag);
        void OnSpecial(uint code);
    }
}
