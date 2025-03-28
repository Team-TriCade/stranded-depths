using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyTerrainGenerator : MonoBehaviour
{
    public int width = 256; // the width of the terrain in metres
    public int depth = 256; // the depth of the terrain in metres
    public int height = 50; // the maximum height of the terrain in metres

    public float scale = 2f; // the controls the roughness of the terrain. higher values will give rise to a smoother terrain

    // Start is called before the first frame update
    void Start()
    {
        GenerateTerrain();
    }

    void GenerateTerrain(){
        UnityEngine.Terrain terrain = GetComponent<UnityEngine.Terrain>(); // get the terrain component from the GameObject
        TerrainData terrainData = terrain.terrainData; // retrievee the terrain's data

        terrainData.heightmapResolution = width + 1; // set the heightmap resolution aka the grid size for height values
        terrainData.size = new Vector3(width, height, depth); // define the terrain size (x,y,z)

        terrainData.SetHeights(0,0, GenerateHeights(height)); // apply the generated heights

    }

    float[,] GenerateHeights(float terrainHeight){ // generates a 2d array to store height values
        float[,] heights = new float[width, depth]; // create a heightmap array

        for(int x = 0; x<width; x++){
            for(int y=0;y<depth;y++){
                float xCoord = (float)x/width*scale;
                float yCoord = (float)y/width*scale;
                heights[x,y] = Mathf.PerlinNoise(xCoord, yCoord); // get perlin noise value and normalise it
            }
        }
        return heights;
    }

    // Update is called once per frame
    void Update(){
        
    }
}