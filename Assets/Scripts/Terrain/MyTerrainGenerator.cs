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


        //random offset for river to create a different pattern every time
        float riverOffset = Random.Range(0f,100f);
        
        // Generate a base perlin noise terrain
        for(int x = 0; x<width; x++){
            for(int y=0;y<depth;y++){
                float xCoord = (float)x/width*scale;
                float yCoord = (float)y/width*scale;
                float noise = Mathf.PerlinNoise(xCoord, yCoord); // get perlin noise value
                 
                //create a curved river path using perlin noise 
                float riverPath = Mathf.PerlinNoise(y*0.004f+riverOffset, 0) * width; // y*n, more the n, the more the curves

                //control river width dynamically using another perlin noise function
                float riverWidth = Mathf.PerlinNoise(y*0.1f, riverOffset) * 20f + 10f; // we can also play with these values for the thickness

                if(Mathf.Abs(x-riverPath) < riverWidth){ // lower the terrain near the river
                    float blendFactor = Mathf.Abs(x-riverPath)/riverWidth;
                    noise *= Mathf.Lerp(0.2f, 1f, blendFactor); // make the riverbed lower

                }
                Debug.Log($"River Path at {y}: {riverPath}, Width: {riverWidth}");
                heights[x,y] = noise;

            }
        }

        // // Carve the river's path
        // for(int x = 0; x < width; x++){
        //     int riverY = (int)(depth * (Mathf.PerlinNoise(x*0.02f, 0) * 0.02f)); // a slight curve

        //     for (int offset = -3; offset <= 3; offset++){ // adjust river width
        //         int y = riverY + offset;
        //         if(y >= 0 && y < depth){
        //             heights[x,y] *= 0.02f; // lower the height to create a riverbed
        //         }

        //     }
        // }
        return heights;
    }

    // Update is called once per frame
    void Update(){
        

    }
}
