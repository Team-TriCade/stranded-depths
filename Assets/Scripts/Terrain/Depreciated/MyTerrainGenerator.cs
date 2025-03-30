using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyTerrainGenerator : MonoBehaviour
{
    public int width = 256;
    public int depth = 256;
    public int height = 50;
    public float scale = 2f;
    public float riverOffset;

    private Terrain terrain;
    private TerrainData terrainData;
    public Vector2 chunkCoord;

    void Start()
    {
        terrain = GetComponent<Terrain>();
        if(terrain == null){
            Debug.LogError("Terrain component missing on object!");
            return;
        }
        terrainData = terrain.terrainData;
        GenerateTerrain();
    }

    public void Initialize(Vector2 coord)
    {
        this.chunkCoord = coord;
        if (terrainData == null) {
            Debug.LogError("TerrainData is null! Make sure it's assigned before calling GenerateTerrain.");
            return;
        }
        GenerateTerrain();
    }

    void GenerateTerrain()
    {
        terrainData.heightmapResolution = width + 1;
        terrainData.size = new Vector3(width, height, depth);
        terrainData.SetHeights(0, 0, GenerateHeights());
    }

    float[,] GenerateHeights()
    {
        float[,] heights = new float[width, depth];
        riverOffset = Random.Range(0f, 100f);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < depth; y++)
            {
                float worldX = (chunkCoord.x * width + x) / 100f;
                float worldY = (chunkCoord.y * depth + y) / 100f;
                float noise = Mathf.PerlinNoise(worldX * scale, worldY * scale);

                // River logic
                float riverPath = Mathf.PerlinNoise(worldY * 0.004f + riverOffset, 0) * width;
                float riverWidth = Mathf.PerlinNoise(worldY * 0.1f, riverOffset) * 20f + 10f;

                if (Mathf.Abs(x - riverPath) < riverWidth)
                {
                    float blendFactor = Mathf.Abs(x - riverPath) / riverWidth;
                    noise *= Mathf.Lerp(0.2f, 1f, blendFactor);
                }

                heights[x, y] = noise;
            }
        }
        return heights;
    }
}
