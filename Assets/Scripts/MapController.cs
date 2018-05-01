using UnityEngine;
using System.Collections;
using System.IO;

public class MapController : MonoBehaviour
{
    private Vector3[] vertices;
    private Vector3[] normals;
    private Vector2[] uvs;
    private int[] indices;

    void Start()
    {
        LoadMesh();
    }

    void LoadMesh()
    {
        Mesh mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        string filename_packaged = GameController.data_location + "/miami.bdae";

        // -- EXTRACTING --
        ZipUtil.Unzip(filename_packaged, Application.temporaryCachePath + "/map_miami");
        string filename = Application.temporaryCachePath + "/map_miami/batch_data.bin";

        var br = new BinaryReader(File.OpenRead(filename));

        Debug.Log("-- INDICES --");

        int num_faces = 16626 / 2; // CHANGE!!
        int offset_first_vertex = 0;

        vertices = new Vector3[num_faces];
        normals = new Vector3[num_faces];
        uvs = new Vector2[num_faces];

        br.BaseStream.Seek(offset_first_vertex, SeekOrigin.Begin);

        for (int i = 0; i < num_faces; i++)
        {
            float x = ByteHelper.ReadFloat32(br, ByteHelper.ByteOrder.LittleEndian);
            float y = ByteHelper.ReadFloat32(br, ByteHelper.ByteOrder.LittleEndian);
            float z = ByteHelper.ReadFloat32(br, ByteHelper.ByteOrder.LittleEndian);
            vertices[i] = new Vector3(x, y, z);

            float xn = ByteHelper.ReadFloat32(br, ByteHelper.ByteOrder.LittleEndian);
            float yn = ByteHelper.ReadFloat32(br, ByteHelper.ByteOrder.LittleEndian);
            float zn = ByteHelper.ReadFloat32(br, ByteHelper.ByteOrder.LittleEndian);
            normals[i] = new Vector3(xn, yn, zn);

            float xt = ByteHelper.ReadFloat32(br, ByteHelper.ByteOrder.LittleEndian);
            float yt = ByteHelper.ReadFloat32(br, ByteHelper.ByteOrder.LittleEndian);
            uvs[i] = new Vector2(xt, yt);

            //Debug.Log (x + " - " + y + " - " + z);
        }

        Debug.Log("Position: " + br.BaseStream.Position);
        mesh.vertices = vertices;
        mesh.normals = normals;
        mesh.uv = uvs;


        br.BaseStream.Seek(13087148, SeekOrigin.Begin); // 0xc7b1ac

        indices = new int[num_faces];
        for (int i = 0; i < num_faces; i++)
        {
            short a = ByteHelper.ReadInt16(br, ByteHelper.ByteOrder.LittleEndian);
            indices[i] = a;
        }

        mesh.triangles = indices;
    }

    // Update is called once per frame
    void Update()
    {
    }
}