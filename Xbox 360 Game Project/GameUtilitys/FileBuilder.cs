using System.Text;

namespace Xbox_360_Game_Project
{
    class FileBuilder
    {

	    private byte[] buffer;
	    private int readerIndex = 0;
	    private int writerIndex = 0;

        public FileBuilder(int length)
        {
            buffer = new byte[length];
        }

        public FileBuilder(byte[] buffer)
        {
            this.buffer = buffer;
        }

        public void ResizeStorage(int size)
        {
            buffer = new byte[size];
            readerIndex = 0;
            writerIndex = 0;
        }

	    public void SkipBytes(int skipped) {
		    writerIndex += skipped;
	    }

	    public int ReadableBytes() {
		    return (buffer.Length - readerIndex);
        }

        public int Capacity()
        {
            return buffer.Length;
        }

        public byte[] Array()
        {
            return buffer;
        }

	    public int ReaderIndex() {
		    return readerIndex;
	    }

	    public int WriterIndex() {
		    return writerIndex;
	    }
	
	    public int WriteLength() {
		    int size = writerIndex;
		    writerIndex = 0;
		    WriteShort(size);
		    return size;	
	    }

	    public void WriteByte(byte data) {
		    buffer[writerIndex++] = data;
	    }
	
	    public void WriteShort(int data) {
		    buffer[writerIndex++] = (byte)(data >> 8);
		    buffer[writerIndex++] = (byte)(data);
	    }

	    public void WriteInt(int data) {
		    buffer[writerIndex++] = (byte)(data >> 24);
		    buffer[writerIndex++] = (byte)(data >> 16);
		    buffer[writerIndex++] = (byte)(data >> 8);
		    buffer[writerIndex++] = (byte)(data);
	    }

	    public void WriteLong(long data) {
		    buffer[writerIndex++] = (byte)(data >> 56);
		    buffer[writerIndex++] = (byte)(data >> 48);
		    buffer[writerIndex++] = (byte)(data >> 40);
		    buffer[writerIndex++] = (byte)(data >> 32);
		    buffer[writerIndex++] = (byte)(data >> 24);
		    buffer[writerIndex++] = (byte)(data >> 16);
		    buffer[writerIndex++] = (byte)(data >> 8);
		    buffer[writerIndex++] = (byte)(data);
	    }

	    public void WriteString(string s) {
		    WriteString(s.ToCharArray());
	    }

	    public void WriteString(char[] data) {
		    for (int i = 0; i < data.Length; i++)
			    buffer[writerIndex++] = (byte)data[i];
		    buffer[writerIndex++] = 0;
	    }

	    public int ReadByte() {
		    return buffer[readerIndex++];
	    }

	    public int ReadShort() {
		    return (ReadByte() << 8) | ReadByte();
	    }

        public int ReadInt() {
		    return (ReadByte() << 24) | (ReadByte() << 16) | (ReadByte() << 8) | ReadByte();
        }

        public long ReadLong() {
            return (ReadByte() << 56) | (ReadByte() << 48) | (ReadByte() << 40) | (ReadByte() << 32) |
            (ReadByte() << 24) | (ReadByte() << 16) | (ReadByte() << 8) | ReadByte();
	    }

	    public string ReadString() {
            StringBuilder sb = new StringBuilder();
            byte b;
            while ((b = buffer[readerIndex++]) != 0)
            {
                sb.Append((char)b);
            }
            return sb.ToString();
	    }

    }
}
