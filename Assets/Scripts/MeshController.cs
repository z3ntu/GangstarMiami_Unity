using UnityEngine;
using System.Collections;
using System.IO;
using System;

public class MeshController : MonoBehaviour {

	private Vector3[] vertices;
	private Vector3[] normals;
	private Vector2[] uvs;
	private int[] indices;

	void Start () {
		Mesh mesh = new Mesh();
		GetComponent<MeshFilter>().mesh = mesh;

		//mesh.subMeshCount = 6;

		var br = new BinaryReader(File.OpenRead("/home/luca/Downloads/hex2obj/car_delivery.bdae_FILES/little_endian_quantized.bdae"));

		Debug.Log ("go back 8 bytes and read address");
		br.BaseStream.Seek (long.Parse("9E0", System.Globalization.NumberStyles.HexNumber), SeekOrigin.Begin);
		int offset = ReadInt32 (br, ByteOrder.LittleEndian);
		Debug.Log (offset);

		Debug.Log ("jump to that address which is where the model name offset is");
		br.BaseStream.Seek (offset, SeekOrigin.Begin);

		Debug.Log ("go back 12 bytes to get number of submeshes");
		//Debug.Log ("Position: " + br.BaseStream.Position);
		br.BaseStream.Seek (-(12), SeekOrigin.Current);
		//Debug.Log ("Position: " + br.BaseStream.Position);
		short nrOfSubmeshes = ReadInt16 (br, ByteOrder.LittleEndian);
		Debug.Log (nrOfSubmeshes);

		Debug.Log ("0c984C - offset to model name");
		br.BaseStream.Seek (10, SeekOrigin.Current);
		int offset_model_name = ReadInt32 (br, ByteOrder.LittleEndian);
		Debug.Log (offset_model_name);

		Debug.Log ("0x9850 - length of first vertex block");
		int length_first_vertex = ReadInt32 (br, ByteOrder.LittleEndian);
		Debug.Log (length_first_vertex);

		Debug.Log ("0x9854 - offset to first vertex block");
		int offset_first_vertex = ReadInt32 (br, ByteOrder.LittleEndian);
		Debug.Log (offset_first_vertex);

		Debug.Log ("0x9858 - length of first face indices data (divide by 2 to get num faces)");
		int length_first_face_indices = ReadInt32 (br, ByteOrder.LittleEndian);
		int num_faces = length_first_face_indices / 2;
		Debug.Log (length_first_face_indices);
		Debug.Log (num_faces);

		Debug.Log ("0x985C - offset to first face indices");
		int offset_first_face_indices = ReadInt32 (br, ByteOrder.LittleEndian);
		Debug.Log (offset_first_face_indices);

		Debug.Log ("--- VERTICES ---");

		vertices = new Vector3[num_faces];
		normals = new Vector3[num_faces];
		uvs = new Vector2[num_faces];

		br.BaseStream.Seek (offset_first_vertex, SeekOrigin.Begin);

		for (int i = 0; i < num_faces; i++) {
			float x = ReadFloat32 (br, ByteOrder.LittleEndian);
			float y = ReadFloat32 (br, ByteOrder.LittleEndian);
			float z = ReadFloat32 (br, ByteOrder.LittleEndian);
			vertices [i] = new Vector3 (x, y, z);

			float xn = ReadFloat32 (br, ByteOrder.LittleEndian);
			float yn = ReadFloat32 (br, ByteOrder.LittleEndian);
			float zn = ReadFloat32 (br, ByteOrder.LittleEndian);
			normals [i] = new Vector3 (xn, yn, zn);

			float xt = ReadFloat32 (br, ByteOrder.LittleEndian);
			float yt = ReadFloat32 (br, ByteOrder.LittleEndian);
			uvs [i] = new Vector2 (xt, yt);

			//Debug.Log (x + " - " + y + " - " + z);
		}
		Debug.Log ("Position: " + br.BaseStream.Position);
		mesh.vertices = vertices;
		mesh.normals = normals;
		mesh.uv = uvs;

		Debug.Log ("-- INDICES --");

		indices = new int[num_faces];
		for (int i = 0; i < num_faces; i++) {
			short a = ReadInt16 (br, ByteOrder.LittleEndian);
			indices [i] = a;
		}

		mesh.triangles = indices;

		mesh.RecalculateNormals();
		mesh.RecalculateBounds ();
		mesh.Optimize();
	}

	// ------ HELPER METHODS ------

	private enum ByteOrder : int
	{
		LittleEndian,
		BigEndian
	}

	static byte[] ReadBytes(BinaryReader reader, int fieldSize, ByteOrder byteOrder)
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

	static short ReadInt16(BinaryReader reader, ByteOrder byteOrder)
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

	static ushort ReadUInt16(BinaryReader reader, ByteOrder byteOrder)
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

	static int ReadInt32(BinaryReader reader, ByteOrder byteOrder)
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

	static uint ReadUInt32(BinaryReader reader, ByteOrder byteOrder)
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

	static float ReadFloat32(BinaryReader reader, ByteOrder byteOrder)
	{
		if (byteOrder == ByteOrder.LittleEndian)
		{
			return reader.ReadSingle ();
		}
		else // Big-Endian
		{
			return BitConverter.ToSingle(ReadBytes(reader, 4, ByteOrder.BigEndian), 0);
		}
	}

	static string ReadString(BinaryReader reader, int characters, ByteOrder byteOrder)
	{
		return System.Text.Encoding.ASCII.GetString(ReadBytes (reader, characters, byteOrder));
	}

}
