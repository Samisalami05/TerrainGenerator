using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterChunk : MonoBehaviour
{
    public Vector3[] waterVerticies;
    int[] waterTriangles;
    

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void CreateWaterChunk(int vertexDistance, int chunkSize, Vector2 chunkPos, Material waterMaterial, float waterLevel)
    {
        int WaterSide = (chunkSize / vertexDistance);

        waterVerticies = new Vector3[(chunkSize + 1) * (chunkSize + 1)];


        MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();

        meshRenderer.material = waterMaterial;

        Mesh mesh = new Mesh();
        meshFilter.mesh = mesh;

        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

        int i = 0;
        for (int z = 0; z <= WaterSide; z++)
        {
            for (int x = 0; x <= WaterSide; x++)
            {
                waterVerticies[i] = new Vector3((x * vertexDistance) + (chunkSize * chunkPos.x), waterLevel, (z * vertexDistance) + (chunkSize * chunkPos.y));

                i++;
            }
        }

        waterTriangles = new int[WaterSide * WaterSide * 6];

        int vert = 0;
        int tris = 0;
        
        for (int z = 0; z < WaterSide; z++)
        {
            for (int x = 0; x < WaterSide; x++)
            {
                waterTriangles[tris + 0] = vert + 0;
                waterTriangles[tris + 1] = vert + WaterSide + 1;
                waterTriangles[tris + 2] = vert + 1;
                waterTriangles[tris + 3] = vert + 1;
                waterTriangles[tris + 4] = vert + WaterSide + 1;
                waterTriangles[tris + 5] = vert + WaterSide + 2;

                vert++;
                tris += 6;
            }
            vert++;
        }
        mesh.Clear();
        
        mesh.vertices = waterVerticies;
        mesh.triangles = waterTriangles;

        mesh.RecalculateNormals();
    }
}
