using UnityEngine;
using System.Collections;
using System.IO;
using System;


public class ByteHelper
{
    // ------ HELPER METHODS ------

    public enum ByteOrder : int
    {
        LittleEndian,
        BigEndian
    }

    public static byte[] ReadBytes(BinaryReader reader, int fieldSize, ByteOrder byteOrder)
    {
        byte[] bytes = new byte[fieldSize];
        if (byteOrder == ByteOrder.LittleEndian)
            return reader.ReadBytes(fieldSize);
        else
        {
            for (int i = fieldSize - 1; i > -1; i--)
                bytes[i] = reader.ReadByte();
            return bytes;
        }
    }

    public static short ReadInt16(BinaryReader reader, ByteOrder byteOrder)
    {
        if (byteOrder == ByteOrder.LittleEndian)
        {
            return reader.ReadInt16();
        }
        else // Big-Endian
        {
            return BitConverter.ToInt16(ReadBytes(reader, 2, ByteOrder.BigEndian), 0);
        }
    }

    public static ushort ReadUInt16(BinaryReader reader, ByteOrder byteOrder)
    {
        if (byteOrder == ByteOrder.LittleEndian)
        {
            return reader.ReadUInt16();
        }
        else // Big-Endian
        {
            return BitConverter.ToUInt16(ReadBytes(reader, 2, ByteOrder.BigEndian), 0);
        }
    }

    public static int ReadInt32(BinaryReader reader, ByteOrder byteOrder)
    {
        if (byteOrder == ByteOrder.LittleEndian)
        {
            return reader.ReadInt32();
        }
        else // Big-Endian
        {
            return BitConverter.ToInt32(ReadBytes(reader, 4, ByteOrder.BigEndian), 0);
        }
    }

    public static uint ReadUInt32(BinaryReader reader, ByteOrder byteOrder)
    {
        if (byteOrder == ByteOrder.LittleEndian)
        {
            return reader.ReadUInt32();
        }
        else // Big-Endian
        {
            return BitConverter.ToUInt32(ReadBytes(reader, 4, ByteOrder.BigEndian), 0);
        }
    }

    public static float ReadFloat32(BinaryReader reader, ByteOrder byteOrder)
    {
        if (byteOrder == ByteOrder.LittleEndian)
        {
            return reader.ReadSingle();
        }
        else // Big-Endian
        {
            return BitConverter.ToSingle(ReadBytes(reader, 4, ByteOrder.BigEndian), 0);
        }
    }

    public static string ReadString(BinaryReader reader, int characters, ByteOrder byteOrder)
    {
        return System.Text.Encoding.ASCII.GetString(ReadBytes(reader, characters, byteOrder));
    }

    public static long GetStartOffset(BinaryReader reader)
    {
        reader.BaseStream.Seek(0, SeekOrigin.Begin);
        int count = 0;
        while (count < 3)
        {
            if (ReadInt16(reader, ByteOrder.LittleEndian) == 11312)
            {
                // 12332 is "30 2C" in HEX (little endian)
                count++;
                Debug.Log(count);
                Debug.Log(reader.BaseStream.Position);
            }
        }
        return reader.BaseStream.Position - 6;
    }
}