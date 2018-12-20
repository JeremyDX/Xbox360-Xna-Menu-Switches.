
namespace Xbox_360_Game_Project
{
    class PacketBuilder
    {
	    private byte[] buffer;
	    private int writerIndex;
	    private int oldWriterPosition;

	    public PacketBuilder() {
		    buffer = new byte[16];
	    }

	    public PacketBuilder(int length) {
		    buffer = new byte[length];
	    }

	    public PacketBuilder(byte[] data) {
		    buffer = data;
		    writerIndex = buffer.Length;
	    }

        public int SkipBytes(int skipped)
        {
            int oldPosition = writerIndex;
            writerIndex += skipped;
            return oldPosition;
        }

	    public int WriterIndex() {
		    return writerIndex;
	    }

	    public byte[] Array() {
		    return buffer;
	    }

	    public int Capacity() {
		    return buffer.Length;
	    }

	    public void Reset() {
		    buffer = new byte[16];
		    writerIndex = 0;
	    }

        public int SetIndex(int writerIndex)
        {
            int oldPosition = this.writerIndex;
            this.writerIndex = writerIndex;
            return oldPosition;
        }

	    public PacketBuilder CreatePacket(int id) {
		    ensureBytes(1);
		    buffer[writerIndex++] = (byte)id;
		    return this;
	    }

	    public PacketBuilder CreatePacketTypeByte(int id) {
		    ensureBytes(2);
		    buffer[writerIndex++] = (byte)id;
		    oldWriterPosition = writerIndex;
		    writerIndex++;
		    return this;
	    }

	    public PacketBuilder CreatePacketTypeShort(int id) {
		    ensureBytes(3);
 		    buffer[writerIndex++] = (byte)id;
		    oldWriterPosition = writerIndex;
 		    writerIndex++;
		    writerIndex++;
		    return this;
	    }

	    public void endPacketTypeByte() {
		    buffer[oldWriterPosition] = (byte)(writerIndex - oldWriterPosition - 1);
	    }

	    public void endPacketTypeShort() {
		    buffer[oldWriterPosition] = (byte)((writerIndex - oldWriterPosition - 2) >> 8);
		    buffer[oldWriterPosition + 1] = (byte)(writerIndex - oldWriterPosition - 2);
	    }

        public PacketBuilder AddBoolean(bool value)
        {
            ensureBytes(1);
            buffer[writerIndex++] = value ? (byte) 1 : (byte) 0;
            return this;
        }

        public PacketBuilder AddBoolean(byte value)
        {
            ensureBytes(1);
            buffer[writerIndex++] = value == 0 ? (byte) 0 : (byte) 1;
            return this;
        }

	    public PacketBuilder AddByte(byte value) {
		    ensureBytes(1);
		    buffer[writerIndex++] = value;
		    return this;
	    }

	    public PacketBuilder AddBytes(PacketBuilder builder) {
		    ensureBytes(builder.writerIndex);
		    for (int i = 0; i < builder.writerIndex; i++)
			    buffer[writerIndex++] = builder.buffer[i];
		    return this;
	    }

	    public PacketBuilder AddBytes(byte[] data) {
		    ensureBytes(data.Length);
		    System.Array.Copy(data, 0, buffer, writerIndex, data.Length);
		    writerIndex += data.Length;
		    return this;
	    }

	    public PacketBuilder AddBytes(byte[] data, int start, int end) {
		    ensureBytes(end - start + 1);
		    for (int i = start; i < end; i++)
			    buffer[writerIndex++] = data[i];
		    return this;
	    }

	    public PacketBuilder AddShort(int value) {
		    ensureBytes(2);
		    buffer[writerIndex++] = (byte) (value >> 8);
		    buffer[writerIndex++] = (byte) value;
		    return this;
	    }

	    public PacketBuilder AddInt(int value) {
		    ensureBytes(4);
		    buffer[writerIndex++] = (byte) (value >> 24);
		    buffer[writerIndex++] = (byte) (value >> 16);
		    buffer[writerIndex++] = (byte) (value >> 8);
		    buffer[writerIndex++] = (byte) value;
		    return this;
	    }

	    public PacketBuilder AddLong(long value) {
		    ensureBytes(8);
		    buffer[writerIndex++] = (byte)(int)(value >> 56);
		    buffer[writerIndex++] = (byte)(int)(value >> 48);
		    buffer[writerIndex++] = (byte)(int)(value >> 40);
		    buffer[writerIndex++] = (byte)(int)(value >> 32);
		    buffer[writerIndex++] = (byte)(int)(value >> 24);
		    buffer[writerIndex++] = (byte)(int)(value >> 16);
		    buffer[writerIndex++] = (byte)(int)(value >> 8);
		    buffer[writerIndex++] = (byte)(int)(value);
		    return this;
	    }

	    public PacketBuilder AddString(string s) {
		    char[] data = s.ToCharArray();
		    ensureBytes(data.Length + 1);
		    for (int i = 0; i < data.Length; i++)
			    buffer[writerIndex++] = (byte)data[i];
		    buffer[writerIndex++] = 0;
		    return this;
	    }

       /*
       * Basically it just verify's there is enough room in the buffer.
       */ 
	    public void ensureBytes(int newCapacity) {
		    if ((newCapacity + writerIndex) >= buffer.Length) {
			    byte[] newBuffer = new byte[(newCapacity + writerIndex) * 2];
			    System.Array.Copy(buffer, 0, newBuffer, 0, writerIndex);
			    buffer = newBuffer;
		    }
	    }
    }
}
