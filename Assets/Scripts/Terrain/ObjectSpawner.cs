using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    public GameObject[] objects;
    //public Terrain terrain;
    public int objectCount = 470; // numebr of objects to spawn
    // Start is called before the first frame update

    private MyTerrainGenerator terrainGenerator;
    
    void Start()
    {
        terrainGenerator = FindObjectOfType<MyTerrainGenerator>(); 
        if(terrainGenerator == null){
          Debug.LogError("Parent script now found");
          return;
        }
        SpawnObjects();
    }


    bool IsInRiver(Vector3 position){
            float riverOffset = terrainGenerator.riverOffset;
            float width = terrainGenerator.width;
            float y = position.z;
            float riverPath = Mathf.PerlinNoise(y*0.004f+riverOffset, 0) * width; // y*n, more the n, the more the curves
            float riverWidth = Mathf.PerlinNoise(y*0.1f, riverOffset) * 20f + 10f; // we can also play with these values for the thickness

            return Mathf.Abs(position.x - riverPath) < riverWidth;
    }

    void SpawnObjects(){

        UnityEngine.Terrain terrain = GetComponent<UnityEngine.Terrain>(); // get the terrain component from the GameObject
        TerrainData terrainData = terrain.terrainData; // retrievee the terrain's data

        for(int i = 0; i < objectCount; i++){
            //Pick a random prefab
            GameObject prefab = objects[Random.Range(0, objects.Length)];
            Vector3 position;
            int attempts = 0;

            do {
                // get random position within terrain
                float x = Random.Range(0, terrainData.size.x);
                float z = Random.Range(0, terrainData.size.z);
                float y = terrain.SampleHeight(new Vector3(x,0,z)); // the height at (x,z)

                position = new Vector3(x,y,z);
                attempts++;
            } while(IsInRiver(position) && attempts < 10);

            //Spawn the objects
            Instantiate(prefab, position, Quaternion.Euler(0, Random.Range(0,360), 0)); // spawn them at different rotations around the y axis(horizontally) 
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
