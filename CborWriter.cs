using System;
using System.IO;
using System.Text;

namespace Assets.DiverseWorlds.Cbor {
    public class CborWriter : IDisposable {
        private readonly Stream output;

        public CborWriter(Stream output) {
            this.output = output;
        }

        private void WriteTypeAndValue(int majorType, uint value) {
            majorType <<= 5;
            if(value < 24) {
                output.WriteByte((byte) (majorType | value));
            } else if(value < 256) {
                output.WriteByte((byte) (majorType | 24));
                output.WriteByte((byte) value);
            } else if(value < 65536) {
                output.WriteByte((byte) (majorType | 25));
                output.WriteByte((byte) (value >> 8));
                output.WriteByte((byte) (value));
            } else {
                output.WriteByte((byte) (majorType | 26));
                output.WriteByte((byte) (value >> 24));
                output.WriteByte((byte) (value >> 16));
                output.WriteByte((byte) (value >> 8));
                output.WriteByte((byte) value);
            }
        }

        private void WriteTypeAndValue(int majorType, ulong value) {
            majorType <<= 5;
            if(value < 24UL) {
                output.WriteByte((byte)(majorType | (int)value));
            } else if (value < 256UL) {
                output.WriteByte((byte)(majorType | 24));
                output.WriteByte((byte)value);
            } else if (value < 65536UL) {
                output.WriteByte((byte)(majorType | 25));
                output.WriteByte((byte)(value >> 8));
                output.WriteByte((byte)(value));
            } else if(value < 4294967295UL) {
                output.WriteByte((byte) (majorType | 26));
                output.WriteByte((byte) (value >> 24));
                output.WriteByte((byte) (value >> 16));
                output.WriteByte((byte) (value >> 8));
                output.WriteByte((byte) value);
            } else {
                output.WriteByte((byte) (majorType | 27));
                output.WriteByte((byte) (value >> 56));
                output.WriteByte((byte) (value >> 48));
                output.WriteByte((byte) (value >> 40));
                output.WriteByte((byte) (value >> 32));
                output.WriteByte((byte) (value >> 24));
                output.WriteByte((byte) (value >> 16));
                output.WriteByte((byte) (value >> 8));
                output.WriteByte((byte) value);
            }
        }

        public void Write(int value) {
            if(value < 0) {
                WriteTypeAndValue(1, (uint) -value);
            } else {
                WriteTypeAndValue(0, (uint) value);
            }
        }

        public void Write(uint value) {
            WriteTypeAndValue(0, value);
        }

        public void Write(long value) {
            if(value < 0L) {
                WriteTypeAndValue(1, (ulong) -value);
            } else {
                WriteTypeAndValue(0, (ulong) value);
            }
        }

        public void Write(ulong value) {
            WriteTypeAndValue(0, value);
        }

        public void Write(byte[] value) {
            WriteTypeAndValue(2, (uint) value.Length);
            output.Write(value, 0, value.Length);
        }

        public void Write(byte[] value, int offset, int limit) {
            WriteTypeAndValue(2, (uint)limit);
            output.Write(value, offset, limit);
        }

        public void Write(string value) {
            byte[] data = Encoding.UTF8.GetBytes(value);
            WriteTypeAndValue(3, (uint) data.Length);
            output.Write(data, 0, data.Length);
        }

        public void WriteArray(int size) {
            WriteTypeAndValue(4, (uint) size);
        }

        public void WriteMap(int size) {
            WriteTypeAndValue(5, (uint) size);
        }

        public void WriteTag(uint tag) {
            WriteTypeAndValue(6, tag);
        }

        public void writeSpecial(uint code) {
            WriteTypeAndValue(7, code);
        }

        public void Dispose() {
            
        }
    }
}
