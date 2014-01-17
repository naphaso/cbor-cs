namespace Assets.DiverseWorlds.Cbor.Buffer {
    public delegate void InputHandler();
    public interface CborInput {
        event InputHandler InputEvent;
        void AddChunk(byte[] chunk);
        bool HasBytes(int count);
        int GetByte();
        uint GetInt8();
        uint GetInt16();
        uint GetInt32();
        ulong GetInt64();
        byte[] GetBytes(int count);
    }
}
