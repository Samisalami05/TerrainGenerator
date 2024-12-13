using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;
using static UnityEditor.PlayerSettings;

[RequireComponent(typeof(MeshFilter))]
public class MeshGenerator : MonoBehaviour
{

    Mesh mesh;
    Mesh Watermesh;
    GameObject Water;

    Vector3[] verticies;
    int[] triangles;
    public Material terrainMaterial;

    public Vector3[] waterVerticies;
    int[] waterTriangles;
    public Material waterMaterial;

    public int xSize = 10;
    public int zSize = 10;

    public int WaterxSize = 10;
    public int WaterzSize = 10;

    public Vector2 offset;

    public float scale = 10;
    public int speedX = 0;
    public int speedZ = 0;
    public float height = 4;

    public float islandscale = 10;

    public int octaves;
    public float persistance;
    public float lacunarity;

    public float falloffStart;
    public float falloffEnd;

    public int seed;

    public GameObject grass;
    List<GameObject> GrassObjects = new List<GameObject>();

    public float minGrassHeight = 0f;
    public float maxGrassHeight = 20f;
    public int grassCount = 4;
    public float grassRandomizationScale = 20f;
    public float grassYoffset = 0f;

    bool wow = false;
    bool hasToUpdate = true;

    public int vertexDistance = 1;
    public int chunkSize = 16;
    public int WaterSize = 2;
    public float waterLevel = 0;

    public int renderDistance = 8;

    

    Dictionary<Vector2, GameObject> WaterChunks = new Dictionary<Vector2, GameObject>();
    Dictionary<Vector2, GameObject> TerrainChunks = new Dictionary<Vector2, GameObject>();

    void Start()
    {
        wow = true;

        Water = new GameObject("Water");
        Water.transform.parent = transform;
        MeshFilter WaterFilter = Water.AddComponent<MeshFilter>();
        MeshRenderer Waterrenderer = Water.AddComponent<MeshRenderer>();

        Waterrenderer.material = waterMaterial;

        mesh = new Mesh();
        Watermesh = new Mesh();

        GetComponent<MeshFilter>().mesh = mesh;
        WaterFilter.mesh = Watermesh;

        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        Watermesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        OnValidate();
    }
    private void OnValidate()
    {
        if (wow == false)
        {
            return;
        }
        hasToUpdate = true;



    }
    private void Update()
    {

        if (hasToUpdate)
        {
            //Createshape();
            //CreateWater();
            foreach (GameObject water in WaterChunks.Values)
            {
                Destroy(water);
            }
            WaterChunks.Clear();
            foreach (GameObject terrain in TerrainChunks.Values)
            {
                Destroy(terrain);
            }
            TerrainChunks.Clear();

            
            //Createshape();

            UpdateMesh();
            hasToUpdate = false;
        }
        GenerateWater();
    }
    void SpawnWaterChunkObject(Vector2 chunkPos, int vertexDistance)
    {
        GameObject water = new GameObject("chunk x: " + chunkPos.x + " z: " + chunkPos.y);
        WaterChunk chunk = water.AddComponent<WaterChunk>();
        chunk.CreateWaterChunk(vertexDistance, chunkSize, chunkPos, waterMaterial, waterLevel);
        WaterChunks.Add(chunkPos, water);
    }
    void SpawnTerrainChunkObject(Vector2 chunkPos, int vertexDistance, bool lodBoundaryNegativeX, bool lodBoundaryPositiveX, bool lodBoundaryNegativeZ, bool lodBoundaryPositiveZ)
    {
        GameObject terrain = new GameObject("Terrain chunk x: " + chunkPos.x + " z: " + chunkPos.y + " Lod: " + Unity.Mathematics.math.tzcnt(vertexDistance));
        TerrainChunk chunk = terrain.AddComponent<TerrainChunk>();
        chunk.CreateTerrainChunk(vertexDistance, chunkSize, chunkPos, terrainMaterial, seed, offset, height, scale, islandscale, octaves, persistance, lacunarity, falloffStart, falloffEnd, lodBoundaryNegativeX, lodBoundaryPositiveX, lodBoundaryNegativeZ, lodBoundaryPositiveZ);
        TerrainChunks.Add(chunkPos, terrain);
    }

    bool TryGenerate(Vector2 pos, int vertexDistance, bool lodBoundaryNegativeX, bool lodBoundaryPositiveX, bool lodBoundaryNegativeZ, bool lodBoundaryPositiveZ)
    {
        if (TerrainChunks.ContainsKey(pos))
        {
            return false;
        }
        //SpawnWaterChunkObject(pos, vertexDistance);
        SpawnTerrainChunkObject(pos, vertexDistance, lodBoundaryNegativeX, lodBoundaryPositiveX, lodBoundaryNegativeZ, lodBoundaryPositiveZ);
        return true;
    }
    void GenerateWater()
    {
        Vector2 pos = new Vector2(0,0);

        int distance = 0;

        while (true)
        {
            Vector2 renderpos = new Vector2(distance, distance);
            int lodWidth = 4;
            int vertexDistance = 1 << (distance / lodWidth);
            bool od = distance % lodWidth == lodWidth - 1;
            if (vertexDistance > chunkSize)
            {
                break;
            }
            if (TryGenerate(renderpos + pos, vertexDistance, distance == 0 && od, true && od, distance == 0 && od, true && od))
            {
                return;
            }

            while (renderpos.x > -distance)
            {
                renderpos.x--;
                bool lodBoundaryPositiveX = false;
                if (renderpos.x == distance) 
                {
                    lodBoundaryPositiveX = true;
                }
                bool lodBoundaryNegativeX = false;
                if (renderpos.x == -distance)
                {
                    lodBoundaryNegativeX = true;
                }
                if (TryGenerate(renderpos + pos, vertexDistance, lodBoundaryNegativeX && od, lodBoundaryPositiveX && od, false && od, true && od))
                {
                    return;
                }
                
            }
            while (renderpos.y > -distance)
            {
                renderpos.y--;
                bool lodBoundaryPositiveZ = false;
                if (renderpos.y == distance)
                {
                    lodBoundaryPositiveZ = true;
                }
                bool lodBoundaryNegativeZ = false;
                if (renderpos.y == -distance)
                {
                    lodBoundaryNegativeZ = true;
                }
                if (TryGenerate(renderpos + pos, vertexDistance, true && od, false && od, lodBoundaryNegativeZ && od, lodBoundaryPositiveZ && od))
                {
                    return;
                }
            }
            while (renderpos.x < distance)
            {
                renderpos.x++;
                bool lodBoundaryPositiveX = false;
                if (renderpos.x == distance)
                {
                    lodBoundaryPositiveX = true;
                }
                bool lodBoundaryNegativeX = false;
                if (renderpos.x == -distance)
                {
                    lodBoundaryNegativeX = true;
                }
                if (TryGenerate(renderpos + pos, vertexDistance, lodBoundaryNegativeX && od, lodBoundaryPositiveX && od, true && od, false && od))
                {
                    return;
                }
            }
            while (renderpos.y < distance - 1)
            {
                renderpos.y++;
                bool lodBoundaryPositiveZ = false;
                if (renderpos.y == distance)
                {
                    lodBoundaryPositiveZ = true;
                }
                bool lodBoundaryNegativeZ = false;
                if (renderpos.y == -distance)
                {
                    lodBoundaryNegativeZ = true;
                }
                if (TryGenerate(renderpos + pos, vertexDistance, false && od, true && od, lodBoundaryNegativeZ && od, lodBoundaryPositiveZ && od))
                {
                    return;
                }
            }
            distance++;
        }

    }

    void Createshape()
    {
        float[,] noiseMap = Noise.GenerateNoiseMap(xSize + 1, zSize + 1, seed, scale, octaves, persistance, lacunarity, offset);
        float[,] IslandMap = Noise.GenerateNoiseMap(xSize + 1, zSize + 1, seed + 1, islandscale, 1, persistance, lacunarity, offset);
        float[,] falloffMap = FalloffGenerator.GenerateFalloffMap(new Vector2Int(xSize + 1, zSize + 1), falloffStart, falloffEnd);

        verticies = new Vector3[(xSize + 1) * (zSize + 1)];

        for (int t = 0; t < GrassObjects.Count; t++)
        {
            Destroy(GrassObjects[t]);
        }

        int i = 0;

        for (int z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                float m = IslandMap[x, z];

                float y = noiseMap[x, z] * (1 - Mathf.SmoothStep(1, 0, Mathf.InverseLerp(falloffStart, falloffEnd, Mathf.Clamp(m, 0, 1)))) * height;

                verticies[i] = new Vector3(x, y, z);

                i++;
            }
        }

        for (int z = 0; z < zSize; z++)
        {
            for (int x = 0; x < xSize; x++)
            {
                float[] floats = new float[4];
                for (int r = 0; r < 4; r++)
                {
                    i = (xSize + 1) * (z + r / 2) + (x + r % 2);
                    floats[r] = verticies[i].y;
                }

                for (int c = 0; c < grassCount; c++)
                {
                    for (int r = 0; r < grassCount; r++)
                    {
                        float grassHeight;
                        if (r < (grassCount - c - 1))
                        {
                            grassHeight = Mathf.Lerp(Mathf.Lerp(floats[0], floats[1], (float)r / grassCount), floats[2], (float)c / grassCount);
                        }
                        else
                        {
                            grassHeight = Mathf.Lerp(floats[1], Mathf.Lerp(floats[2], floats[3], (float)r / grassCount), (float)c / grassCount);
                        }

                        float RandomX = Random.Range(-0.2f, 0.2f);
                        float RandomZ = Random.Range(-0.2f, 0.2f);
                        float RandomRotation = Random.Range(0f, 90f);

                        if (grassHeight > minGrassHeight + (Mathf.Abs(RandomX) * grassRandomizationScale) && grassHeight < maxGrassHeight + (Mathf.Abs(RandomZ) * grassRandomizationScale))
                        {
                            GrassObjects.Add(Instantiate(grass, new Vector3((x + ((float)r / grassCount)), grassHeight + grassYoffset, z + ((float)c / grassCount)), Quaternion.Euler(0, RandomRotation, 0)));
                        }
                    }
                }
            }
        }

        triangles = new int[xSize * zSize * 6];

        int vert = 0;
        int tris = 0;

        for (int z = 0; z < zSize; z++)
        {
            for (int x = 0; x < xSize; x++)
            {
                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = vert + xSize + 1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + xSize + 1;
                triangles[tris + 5] = vert + xSize + 2;

                vert++;
                tris += 6;
            }
            vert++;
        }
    }
    void SpawnWater(int WaterSize)
    {
        int WaterSide = (chunkSize / vertexDistance);
        waterVerticies = new Vector3[((WaterSide + 1 ) * WaterSize) * ((WaterSide + 1) * WaterSize)];
        
        for (int z = 0; z < WaterSize; z++)
        {
            for (int x = 0; x < WaterSize; x++)
            {
                Vector2 chunkposition = new Vector2(x, z);
                GameObject water = new GameObject("chunk x: " + x + " z: " + z);
                WaterChunk chunk =  water.AddComponent<WaterChunk>();
                chunk.CreateWaterChunk(vertexDistance, chunkSize, chunkposition, waterMaterial, waterLevel);
                WaterChunks.Add(chunkposition, water);
            }
        }

    }

    void CreateWaterChunk(int vertexDistance, int chunkSize, Vector3[] waterVerticies, Vector2 chunkPos)
    {
        int WaterSide = (chunkSize / vertexDistance);

        waterVerticies = new Vector3[(chunkSize + 1) * (chunkSize + 1)];
        int i = 0;
        for (int z = 0; z <= WaterSide; z++)
        {
            for (int x = 0; x <= WaterSide; x++)
            {
                waterVerticies[i] = new Vector3((x * vertexDistance) + (chunkSize * chunkPos.x), 0, (z * vertexDistance) + (chunkSize * chunkPos.y));

                i++;
            }
        }

        waterTriangles = new int[((WaterSide + (chunkSize * (int)chunkPos.x)) * (WaterSide + (chunkSize * (int)chunkPos.y))) * 6];

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
    }
    void UpdateMesh()
    {
        mesh.Clear();
        Watermesh.Clear();

        mesh.vertices = verticies;
        mesh.triangles = triangles;
        //mesh.colors = colors;

        Watermesh.vertices = waterVerticies;
        Watermesh.triangles = waterTriangles;

        mesh.RecalculateNormals();
        Watermesh.RecalculateNormals();
    }
}
