using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class ChunkManager : MonoBehaviour{
    public GameObject terrainPrefab; // should be assigned in the inspector
    public int renderDistance = 3; // number of chunks to load
    public Dictionary<Vector2, GameObject> activeChunks = new Dictionary<Vector2, GameObject>();

    private Transform player;
    private MyTerrainGenerator terrainGen; // Cached reference

    void Start(){
        SceneManager.sceneLoaded += OnSceneLoaded; // start listening
        terrainGen = terrainPrefab.GetComponent<MyTerrainGenerator>();
        if (terrainGen == null)
        {
            Debug.LogError("MyTerrainGenerator script is missing on terrainPrefab!");
        }

    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode){
        if(scene.name == "PlayerScene"){
            AssignPlayer();
            SceneManager.sceneLoaded -= OnSceneLoaded; // remove the listener after the assignment
        }
    }

    void AssignPlayer(){
        PlayerController playercontroller = FindObjectOfType<PlayerController>();
        
        if(playercontroller != null){
            player = playercontroller.transform;
            Debug.Log("Player assigned");
        }
    }

    void Update(){
        if(player == null) return; // keep skipping frames until the player is assigned

        LoadNearbyChunks();
    }
    
    void LoadNearbyChunks(){
        if(terrainGen == null) return;
        var playerPos = player.position;
        
        Vector2 playerChunk = new Vector2(
            Mathf.Floor(playerPos.x/terrainGen.width),
            Mathf.Floor(playerPos.z/terrainGen.depth)
        );

        for(int x = -renderDistance; x <= renderDistance; x++){
            for(int z = -renderDistance; z <= renderDistance; z++){
                Vector2 chunkCoord = new Vector2(playerChunk.x+x, playerChunk.y+z);
                
                if(!activeChunks.ContainsKey(chunkCoord)){
                    SpawnChunk(chunkCoord);
                }      
            }
        }
    }

    void SpawnChunk(Vector2 chunkCoord){
        if(terrainGen==null) return;
        Vector3 position = new Vector3(chunkCoord.x*terrainGen.width, 0, chunkCoord.y*terrainGen.depth);
        GameObject newChunk = Instantiate(terrainPrefab, position, Quaternion.identity);
        StartCoroutine(DelayedInitialize(newChunk, chunkCoord));

        MyTerrainGenerator newTerrain = newChunk.GetComponent<MyTerrainGenerator>();
        if(newTerrain !=null){
            newTerrain.Initialize(chunkCoord);
            activeChunks[chunkCoord] = newChunk;
        }
        else{
            Debug.LogError("Spawned chunk is missing MyTerrainGenerator");
        }
    }
    private IEnumerator<object> DelayedInitialize(GameObject chunk, Vector2 chunkCoord) {
        yield return null; // Wait a frame
        chunk.GetComponent<MyTerrainGenerator>()?.Initialize(chunkCoord);
    }

}