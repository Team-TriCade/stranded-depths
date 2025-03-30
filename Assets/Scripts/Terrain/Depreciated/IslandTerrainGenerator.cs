using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IslandTerrainGenerator: MonoBehaviour{
    public int width = 256;
    public int depth = 256;
    public int height = 59;
    public float scale = 10f;
    public float falloffStrength = 3f;

    private Terrain terrain;
    private TerrainData terrainData;
    
  

    // called at the start of every frame
    void Start(){
        terrain = GetComponent<Terrain>();
        terrainData = terrain.terrainData;
        GenerateTerrain();
    }

    void GenerateTerrain(){
        terrainData.heightmapResolution = width + 1;
        terrainData.size = new Vector3(width, height, depth);
        terrainData.SetHeights(0,0, GenerateHeights());
    }

    float[,] GenerateHeights(){
        float[,] heights = new float[depth, width];


        // iterate through each point in the terrain
        for(int x = 0; x < width; x++){
            for(int y = 0; y < depth; y++){
                float xCoord = (float)x/width*scale;
                float yCoord = (float)y/depth*scale;
                float noise = Mathf.PerlinNoise(xCoord, yCoord); // 0..1

                float falloff = CalculateFalloff(x,y);

                // set the border terrain to 0
                if(falloff>=1f){
                  heights[y,x] = 0f;
                }
                else{
                  heights[y,x] = Mathf.Clamp01(noise * (1-falloff)); // ensure that the edges are zero
                }
            }
        }
        return heights;
    }

    float CalculateFalloff(int x, int y){
        float centerX = width/2f;
        float centerY = depth/2f;

        float minDistance = Mathf.Min(width, depth) /2f; // radius of the island
        // normalise x and y to a -1..1 range
        float dx = (float)(x-centerX)/centerX;
        float dy = (float)(y-centerY)/centerY;

        // radial distance
        float distance = Mathf.Sqrt(dx*dx + dy*dy); // distance formula

        
        return Mathf.Clamp01(Mathf.Pow(distance, falloffStrength)*2f);
    }
}
