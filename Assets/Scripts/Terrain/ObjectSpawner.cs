using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    public GameObject[] objects;
    public int objectCount = 470; // numebr of objects to spawn
    // Start is called before the first frame update
    void Start()
    {
        SpawnObjects();
    }

    void SpawnObjects(){
        UnityEngine.Terrain terrain = GetComponent<UnityEngine.Terrain>(); // get the terrain component from the GameObject
        TerrainData terrainData = terrain.terrainData; // retrievee the terrain's data

        for(int i = 0; i < objectCount; i++){
            //Pick a random prefab
            GameObject prefab = objects[Random.Range(0, objects.Length)];
            // get random position within terrain
            float x = Random.Range(0, terrainData.size.x);
            float z = Random.Range(0, terrainData.size.z);
            float y = terrain.SampleHeight(new Vector3(x,0,z)); // the height at (x,z)

            Vector3 position = new Vector3(x,y,z);

            //Spawn the tree
            Instantiate(prefab, position, Quaternion.Euler(0, Random.Range(0,360), 0)); // spawn them at different rotations around the y axis(horizontally) 
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
