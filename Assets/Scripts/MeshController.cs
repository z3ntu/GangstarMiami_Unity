using UnityEngine;
using System.Collections;
using System.IO;
using System;

public class MeshController : MonoBehaviour
{
    private Vector3[] vertices;
    private Vector3[] normals;
    private Vector2[] uvs;
    private int[] indices;

    public string modelname;

    void Start()
    {
        if (modelname != "")
        {
            LoadMesh();
        }
        else
        {
            Debug.Log("Please enter a modelname!");
        }
    }

    void LoadMesh()
    {
        Mesh mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        //string filename = "/home/luca/Downloads/hex2obj/car_delivery.bdae_FILES/little_endian_quantized.bdae";
        //string filename = "/home/luca/Downloads/hex2obj/car_ambulance.bdae_FILES/little_endian_quantized.bdae";
        //string filename_packaged = "/home/luca/Downloads/hex2obj/car_ambulance.bdae";
        string filename_packaged = GameController.data_location + "/" + modelname;
        //mesh.subMeshCount = 6;

        // -- EXTRACTING --
        ZipUtil.Unzip(filename_packaged, Application.temporaryCachePath + "/test_" + modelname);
        string filename = Application.temporaryCachePath + "/test_" + modelname + "/little_endian_quantized.bdae";

        var br = new BinaryReader(File.OpenRead(filename));

        Debug.Log("find 302C-302C-302C & go back 8 bytes and read address");
        //br.BaseStream.Seek (long.Parse("9E0", System.Globalization.NumberStyles.HexNumber), SeekOrigin.Begin);
        br.BaseStream.Seek(ByteHelper.GetStartOffset(br) - 8, SeekOrigin.Begin);
        int offset = ByteHelper.ReadInt32(br, ByteHelper.ByteOrder.LittleEndian);
        Debug.Log(offset);

        Debug.Log("jump to that address which is where the model name offset is");
        br.BaseStream.Seek(offset, SeekOrigin.Begin);

        Debug.Log("go back 12 bytes to get number of submeshes");
        //Debug.Log ("Position: " + br.BaseStream.Position);
        br.BaseStream.Seek(-12, SeekOrigin.Current);
        //Debug.Log ("Position: " + br.BaseStream.Position);
        short nrOfSubmeshes = ByteHelper.ReadInt16(br, ByteHelper.ByteOrder.LittleEndian);
        Debug.Log(nrOfSubmeshes);

        Debug.Log("0c984C - offset to model name");
        br.BaseStream.Seek(10, SeekOrigin.Current);
        int offset_model_name = ByteHelper.ReadInt32(br, ByteHelper.ByteOrder.LittleEndian);
        Debug.Log(offset_model_name);

        Debug.Log("0x9850 - length of first vertex block");
        int length_first_vertex = ByteHelper.ReadInt32(br, ByteHelper.ByteOrder.LittleEndian);
        Debug.Log(length_first_vertex);

        Debug.Log("0x9854 - offset to first vertex block");
        int offset_first_vertex = ByteHelper.ReadInt32(br, ByteHelper.ByteOrder.LittleEndian);
        Debug.Log(offset_first_vertex);

        Debug.Log("0x9858 - length of first face indices data (divide by 2 to get num faces)");
        int length_first_face_indices = ByteHelper.ReadInt32(br, ByteHelper.ByteOrder.LittleEndian);
        int num_faces = length_first_face_indices / 2;
        Debug.Log(length_first_face_indices);
        Debug.Log(num_faces);

        Debug.Log("0x985C - offset to first face indices");
        int offset_first_face_indices = ByteHelper.ReadInt32(br, ByteHelper.ByteOrder.LittleEndian);
        Debug.Log(offset_first_face_indices);

        Debug.Log("--- VERTICES ---");

        vertices = new Vector3[num_faces];
        normals = new Vector3[num_faces];
        uvs = new Vector2[num_faces];

        br.BaseStream.Seek(offset_first_vertex, SeekOrigin.Begin);

        for (int i = 0; i < num_faces; i++)
        {
            float x = ByteHelper.ReadFloat32(br, ByteHelper.ByteOrder.LittleEndian);
            float y = ByteHelper.ReadFloat32(br, ByteHelper.ByteOrder.LittleEndian);
            float z = ByteHelper.ReadFloat32(br, ByteHelper.ByteOrder.LittleEndian);
            vertices[i] = new Vector3(x / 100, y / 100, z / 100);

            float xn = ByteHelper.ReadFloat32(br, ByteHelper.ByteOrder.LittleEndian);
            float yn = ByteHelper.ReadFloat32(br, ByteHelper.ByteOrder.LittleEndian);
            float zn = ByteHelper.ReadFloat32(br, ByteHelper.ByteOrder.LittleEndian);
            normals[i] = new Vector3(xn, yn, zn);

            float xt = ByteHelper.ReadFloat32(br, ByteHelper.ByteOrder.LittleEndian);
            float yt = ByteHelper.ReadFloat32(br, ByteHelper.ByteOrder.LittleEndian);
            uvs[i] = new Vector2(xt, -yt);

            //Debug.Log (x + " - " + y + " - " + z);
        }

        Debug.Log("Position: " + br.BaseStream.Position);
        mesh.vertices = vertices;
        mesh.normals = normals;
        mesh.uv = uvs;

        Debug.Log("-- INDICES --");

        indices = new int[num_faces];
        for (int i = 0; i < num_faces; i++)
        {
            short a = ByteHelper.ReadInt16(br, ByteHelper.ByteOrder.LittleEndian);
            indices[i] = a;
        }

        mesh.triangles = indices;

        Directory.CreateDirectory("/tmp/meshcontroller");
        string verticesText = "";
        foreach (Vector3 vertice in vertices)
        {
            verticesText += vertice.x + "," + vertice.y + "," + vertice.z + Environment.NewLine;
        }

        File.WriteAllText("/tmp/meshcontroller/vertices", verticesText);

        string normalsText = "";
        foreach (Vector3 normal in normals)
        {
            normalsText += normal.x + "," + normal.y + "," + normal.z + Environment.NewLine;
        }

        File.WriteAllText("/tmp/meshcontroller/normals", normalsText);

        string uvsText = "";
        foreach (Vector3 uv in uvs)
        {
            uvsText += uv.x + "," + uv.y + "," + uv.z + Environment.NewLine;
        }

        File.WriteAllText("/tmp/meshcontroller/uvs", uvsText);

        string indicesText = "";
        foreach (int indice in indices)
        {
            indicesText += indice + Environment.NewLine;
        }

        File.WriteAllText("/tmp/meshcontroller/indices", indicesText);

//        mesh.RecalculateNormals();
//        mesh.RecalculateBounds();
    }
}