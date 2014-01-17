using System.Collections.Generic;
using System.IO;
using Assets.DiverseWorlds.Cbor.Exception;

namespace Assets.DiverseWorlds.Cbor.Buffer {
    
    public class CborInputChunks : CborInput {
        private readonly List<MemoryStream> chunks = new List<MemoryStream>();
        public event InputHandler InputEvent;

        public void AddChunk(byte[] chunk) {
            if(chunk.Length == 0) {
                return;
            }
            RemoveReadedChunks();
            chunks.Add(new MemoryStream(chunk, false));
            InputEvent();
        }

        public void RemoveReadedChunks() {
            for (int i = 0; i < chunks.Count; i++) {
                if (chunks[0].Length - chunks[0].Position == 0) {
                    chunks.RemoveAt(0);
                }
                else break;
            }
        }

        public bool HasBytes(int count) {
            long collected = 0;
            foreach(var memoryStream in chunks) {
                collected += memoryStream.Length - memoryStream.Position;
                if(collected >= count) {
                    return true;
                }
            }
            return false;
        }

        public int GetByte() {
            foreach(var memoryStream in chunks) {
                if(memoryStream.Length - memoryStream.Position > 0) {
                    return memoryStream.ReadByte();
                }
            }

            throw new CborException("buffer underflow");
        }

        public uint GetInt8() {
            return (uint) GetByte();
        }

        public uint GetInt16() {
            //return BitConverter.ToUInt16(GetBytes(2), 0);
            return ((uint) GetByte() << 8) | ((uint) GetByte());
        }

        public uint GetInt32() {
            //return BitConverter.ToUInt32(GetBytes(4), 0);
            return ((uint)GetByte() << 24) | ((uint)GetByte() << 16) | ((uint)GetByte() << 8) | ((uint)GetByte());
        }

        public ulong GetInt64() {
            return ((ulong) GetByte() << 56) | ((ulong) GetByte() << 48) | ((ulong) GetByte() << 40) | ((ulong) GetByte() << 32) | ((ulong) GetByte() << 24) | ((ulong) GetByte() << 16) | ((ulong) GetByte() << 8) | ((ulong) GetByte());
            //return BitConverter.ToUInt64(GetBytes(8), 0);
        }

        public byte[] GetBytes(int count) {
            byte[] data = new byte[count];
            int collected = 0;
            foreach(var chunk in chunks) {
                int size = (int) (chunk.Length - chunk.Position);
                if(size == 0) {
                    continue;
                }

                if(size > count - collected) {
                    size = count - collected;
                }

                chunk.Read(data, collected, size);
                collected += size;
                if(collected >= count) {
                    return data;
                }
            }

            throw new CborException("buffer underflow");
        }
    }
}
