using System.Text;

namespace Xbox_360_Game_Project
{
    public class Packet 
    {
	    public short ID;
	    private byte[] BUFFER;
	    private int readerIndex = 0;

	    public Packet(int id, byte[] buffer) {
		    ID = (short)id;
		    BUFFER = buffer;
	    }

	    public void SkipBytes(int skipped) {
		    readerIndex += skipped;
	    }

	    public void SetReaderIndex(int newIndex) {
		    readerIndex = newIndex;
	    }

	    public int ReadableBytes() {
		    return BUFFER.Length - readerIndex;
	    }

	    public int Capacity() {
		    return BUFFER.Length;
	    }

	    public byte[] Array() {
		    return BUFFER;
	    }

	    public int ReaderIndex() {
		    return readerIndex;
	    }

        public bool ReadBoolean()
        {
            return ReadByte() == 1;
        }

	    public byte ReadByte() {
		    return BUFFER[readerIndex++];
	    }

	    public int ReadShort() {
		    return (ReadByte() << 8) | ReadByte();
	    }

	    public int ReadInt() {
		    return (ReadByte() << 24) | (ReadByte() << 16) | (ReadByte() << 8) | ReadByte();
	    }

	    public long ReadLong() {
            return (ReadByte() << 56) | (ReadByte() << 48) | (ReadByte() << 40) | (ReadByte() << 32) | (ReadByte() << 24) | (ReadByte() << 16) | (ReadByte() << 8) | ReadByte();
	    }

	    public string ReadString() {
		    StringBuilder sb = new StringBuilder();
		    byte b;
		    while((b = BUFFER[readerIndex++]) != 0) {
			    sb.Append((char)b);
		    }
		    return sb.ToString();
	    }
    }
}