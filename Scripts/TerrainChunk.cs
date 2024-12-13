using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class TerrainChunk : MonoBehaviour
{
    public Vector3[] verticies;
    int[] triangles;
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void CreateTerrainChunk(int vertexDistance, int chunkSize, Vector2 chunkPos, Material material, int seed, Vector2 offset, float height, float scale, float islandscale, int octaves, float persistance, float lacunarity, float falloffStart, float falloffEnd, bool lodBoundaryNegativeX, bool lodBoundaryPositiveX, bool lodBoundaryNegativeZ, bool lodBoundaryPositiveZ)
    {
        int terrainSide = (chunkSize / vertexDistance);
        float[,] noiseMap = Noise.GenerateNoiseMap(chunkSize + 1, chunkSize + 1, seed, scale, octaves, persistance, lacunarity, offset);
        float[,] IslandMap = Noise.GenerateNoiseMap(chunkSize + 1, chunkSize + 1, seed + 1, islandscale, 1, persistance, lacunarity, offset);
        //float[,] falloffMap = FalloffGenerator.GenerateFalloffMap(new Vector2Int(xSize + 1, zSize + 1), falloffStart, falloffEnd);

        verticies = new Vector3[(chunkSize + 1) * (chunkSize + 1)];

        MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();

        meshRenderer.material = material;

        Mesh mesh = new Mesh();
        meshFilter.mesh = mesh;

        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

        int i = 0;

        for (int z = 0; z <= terrainSide; z++)
        {
            for (int x = 0; x <= terrainSide; x++)
            {
                int x2 = (x * vertexDistance) + (int)(chunkSize * chunkPos.x);
                int z2 = (z * vertexDistance) + (int)(chunkSize * chunkPos.y);
                float m = IslandMap[x, z];

                float y = noiseMap[x, z] * (1 - Mathf.SmoothStep(1, 0, Mathf.InverseLerp(falloffStart, falloffEnd, Mathf.Clamp(m, 0, 1)))) * height;

                float y2 = Perlin.Fbm(x2 / scale, z2 / scale, octaves) * height;
                //float y2 = (Mathf.Sin(x2 / scale) + Mathf.Sin(z2 / scale)) * height;

                verticies[i] = new Vector3(x2, y2, z2);
                
                i++;
            }
        }

        if (terrainSide > 1)
        {
            if (lodBoundaryNegativeX)
            {
                int x = 0;
                for (int z = 1; z <= terrainSide; z += 2)
                {
                    int j = z * (terrainSide + 1) + x;
                    float sum = verticies[j - terrainSide - 1].y + verticies[j + terrainSide + 1].y;
                    float average = sum / 2;
                    verticies[j].y = average;
                }
            }
            if (lodBoundaryPositiveX)
            {
                int x = terrainSide;
                for (int z = 1; z <= terrainSide; z += 2)
                {
                    int j = z * (terrainSide + 1) + x;
                    float sum = verticies[j - terrainSide - 1].y + verticies[j + terrainSide + 1].y;
                    float average = sum / 2;
                    verticies[j].y = average;
                }
            }
            if (lodBoundaryNegativeZ)
            {
                int z = 0;
                for (int x = 1; x <= terrainSide; x += 2)
                {
                    int j = z * (terrainSide + 1) + x;
                    float sum = verticies[j - 1].y + verticies[j + 1].y;
                    float average = sum / 2;
                    verticies[j].y = average;
                }
            }
            if (lodBoundaryPositiveZ)
            {
                int z = terrainSide;
                for (int x = 1; x <= terrainSide; x += 2)
                {
                    int j = z * (terrainSide + 1) + x;
                    float sum = verticies[j - 1].y + verticies[j + 1].y;
                    float average = sum / 2;
                    verticies[j].y = average;
                }
            }
        }
        

        triangles = new int[terrainSide * terrainSide * 6];

        int vert = 0;
        int tris = 0;

        for (int z = 0; z < terrainSide; z++)
        {
            for (int x = 0; x < terrainSide; x++)
            {
                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = vert + terrainSide + 1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + terrainSide + 1;
                triangles[tris + 5] = vert + terrainSide + 2;

                vert++;
                tris += 6;
            }
            vert++;
        }
        mesh.Clear();

        mesh.vertices = verticies;
        mesh.triangles = triangles;

        mesh.RecalculateNormals();
    }
}

