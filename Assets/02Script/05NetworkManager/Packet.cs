using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;


public enum PacketType : byte
{
    Unknown = 0x00,
    PlayerInit = 0x01,
    PlayerUpdate = 0x02,
    MonsterUpdate = 0x03,
    WorldUpdate = 0x04,
    TrapUpdate = 0x05,
}

public enum AnimType : byte
{
    Idle = 0x00,
    Run = 0x01,
    Jump = 0x02,
    Attack = 0x03,
    Dash = 0x04,
    Hit = 0x05,
    Dead = 0x06,
    Skill1 = 0x07,
    Skill2 = 0x08
}

[StructLayout(LayoutKind.Sequential, Pack = 4)]
public struct PacketHeader
{
    public PacketType Type;   // 패킷의 타입 (예: 메시지, 명령 등)
    public byte playerCount;  // 플레이어 숫자 (월드 브로드 캐스트)
    public byte bossActed;      // 보스 행동 여부
    public byte padding;       // reserved for future use
    public uint Length;       // 패킷의 총 데이터 길이

    public PacketHeader(PacketType type = PacketType.Unknown, uint length = 0)
    {
        Type = type;
        playerCount = 0;
        bossActed = 0;
		padding = 0;
        Length = length;
    }
}

public class Packet
{
    public PacketHeader Header;
    public List<byte> Data; // 패킷 데이터
    private int readPos;

    public static int HEADER_SIZE = 8;

    public Packet()
    {
        Data = new List<byte>();
        readPos = 0;
    }

    public Packet(PacketHeader header, List<byte> data)
    {
        Header = header;
        Data = data;
        readPos = 0;
    }
    public void Write(byte value)
    {
        Data.Add(value);
        Header.Length = (uint)(Data.Count + HEADER_SIZE);
    }

    public void Write(int value)
    {
        byte[] rawData = BitConverter.GetBytes(value);
        Data.AddRange(rawData);
        Header.Length = (uint)(Data.Count + HEADER_SIZE);
    }

    public void Write(float value)
    {
        byte[] rawData = BitConverter.GetBytes(value);
        Data.AddRange(rawData);
        Header.Length = (uint)(Data.Count + HEADER_SIZE);
    }

    public void Write(ushort value)
    {
        byte[] rawData = BitConverter.GetBytes(value);
        Data.AddRange(rawData);
        Header.Length = (uint)(Data.Count + HEADER_SIZE);
    }

    public void WriteString(string value)
    {
        ushort strLength = (ushort)value.Length;
        Write(strLength);
        Data.AddRange(Encoding.UTF8.GetBytes(value));
        Header.Length = (uint)(Data.Count + HEADER_SIZE);
    }
    public byte ReadByte()
    {
        if (readPos + sizeof(byte) > Data.Count)
            throw new InvalidOperationException("Lack of packet data!");

        byte value = Data[readPos];
        readPos += sizeof(byte);
        return value;
    }

    public int ReadInt()
    {
        if (readPos + sizeof(int) > Data.Count)
            throw new InvalidOperationException("Lack of packet data!");

        int value = BitConverter.ToInt32(Data.ToArray(), readPos);
        readPos += sizeof(int);
        return value;
    }

    public float ReadFloat()
    {
        if (readPos + sizeof(float) > Data.Count)
            throw new InvalidOperationException("Lack of packet data!");

        float value = BitConverter.ToSingle(Data.ToArray(), readPos);
        readPos += sizeof(float);
        return value;
    }

    public ushort ReadUShort()
    {
        if (readPos + sizeof(ushort) > Data.Count)
            throw new InvalidOperationException("Lack of packet data!");

        ushort value = BitConverter.ToUInt16(Data.ToArray(), readPos);
        readPos += sizeof(ushort);
        return value;
    }

    public string ReadString()
    {
        ushort strLength = ReadUShort();
        if (readPos + strLength > Data.Count)
            throw new InvalidOperationException("Lack of packet data!");

        string value = Encoding.UTF8.GetString(Data.GetRange(readPos, strLength).ToArray());
        readPos += strLength;
        return value;
    }

    // Serialize packet to a byte array
    public byte[] Serialize()
    {
        int totalSize = HEADER_SIZE + Data.Count;
        byte[] buffer = new byte[totalSize];

        Buffer.BlockCopy(GetBytes(Header), 0, buffer, 0, HEADER_SIZE); // 헤더 복사
        if (Data.Count > 0)
            Buffer.BlockCopy(Data.ToArray(), 0, buffer, HEADER_SIZE, Data.Count); // 데이터 복사

        return buffer;
    }


    public static Packet Deserialize(byte[] buffer)
    {
        if (buffer.Length < HEADER_SIZE)
            throw new InvalidOperationException("Buffer too small!");

        PacketHeader header = FromBytes(buffer);
        byte[] data = buffer[HEADER_SIZE..]; // 리스트 대신 배열 그대로 사용

        return new Packet(header, new List<byte>(data)); // 배열을 직접 List<byte>로 변환
    }


    private static byte[] GetBytes(PacketHeader value)
    {
        // Get the exact size of the struct including padding
        int size = HEADER_SIZE;
        byte[] bytes = new byte[size];

        // Marshal the structure to a pointer in memory
        IntPtr ptr = Marshal.AllocHGlobal(size);
        try
        {
            // Copy the structure to memory
            Marshal.StructureToPtr(value, ptr, false);

            // Copy from memory to byte array (with padding)
            Marshal.Copy(ptr, bytes, 0, size);
        }
        finally
        {
            // Clean up memory
            Marshal.FreeHGlobal(ptr);
        }

        return bytes;
    }

    public static PacketHeader FromBytes(byte[] bytes)
    {
        // Ensure that the byte array is large enough for the header
        if (bytes.Length < HEADER_SIZE)
            throw new InvalidOperationException("Byte array too small to deserialize PacketHeader.");

        // Allocate memory for the struct
        IntPtr ptr = Marshal.AllocHGlobal(bytes.Length);
        try
        {
            // Copy the bytes into the allocated memory
            Marshal.Copy(bytes, 0, ptr, bytes.Length);

            // Marshal the data from the pointer back to a PacketHeader structure
            PacketHeader header = Marshal.PtrToStructure<PacketHeader>(ptr);
            return header;
        }
        finally
        {
            // Clean up allocated memory
            Marshal.FreeHGlobal(ptr);
        }
    }

}
