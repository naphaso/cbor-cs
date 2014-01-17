using System;

namespace Assets.DiverseWorlds.Cbor.Buffer {
    class CborInputMoveBuffer : CborInput {
        public event InputHandler InputEvent;
        private byte[] buffer;
        private int readIndex;
        private int writeIndex;

        public CborInputMoveBuffer(int size) {
            this.buffer = new byte[size];
            readIndex = 0;
            writeIndex = 0;
        }

        public void AddChunk(byte[] chunk) {
            if(buffer.Length - writeIndex < chunk.Length) {
                if(writeIndex - readIndex != 0) {
                    Array.Copy(buffer, readIndex, buffer, 0, writeIndex - readIndex);
                    writeIndex = writeIndex - readIndex;
                    readIndex = 0;
                } else {
                    readIndex = 0;
                    writeIndex = 0;
                }
            }

            Array.Copy(chunk, 0, buffer, writeIndex, chunk.Length);
            writeIndex += chunk.Length;
            InputEvent();
        }

        public bool HasBytes(int count) {
            return count <= writeIndex - readIndex;
        }

        public int GetByte() {
            return buffer[readIndex++];
        }

        public uint GetInt8() {
            return buffer[readIndex++];
        }

        public uint GetInt16() {
            readIndex += 2;
            return ((uint)buffer[readIndex - 2] << 8) | ((uint)buffer[readIndex - 1]);
        }

        public uint GetInt32() {
            readIndex += 4;
            return ((uint)buffer[readIndex - 4] << 24) | ((uint)buffer[readIndex - 3] << 16) | ((uint)buffer[readIndex - 2] << 8) | ((uint)buffer[readIndex - 1]);
        }

        public ulong GetInt64() {
            readIndex += 8;
            return ((ulong)buffer[readIndex - 8] << 56) | ((ulong)buffer[readIndex - 7] << 48) | ((ulong)buffer[readIndex - 6] << 40) | ((ulong)buffer[readIndex - 5] << 32) | ((ulong)buffer[readIndex - 4] << 24) | ((ulong)buffer[readIndex - 3] << 16) | ((ulong)buffer[readIndex - 2] << 8) | ((ulong)buffer[readIndex - 1]);
        }

        public byte[] GetBytes(int count) {
            byte[] data = new byte[count];
            Array.Copy(buffer, readIndex, data, 0, count);
            readIndex += count;
            return data;
        }
    }
}
