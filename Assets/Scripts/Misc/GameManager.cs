using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager: MonoBehaviour{
    void Start()
    {
        SceneManager.LoadScene("PlayerScene", LoadSceneMode.Additive);
        Invoke("MovePlayerToTerrain", 1f); // Wait a second to ensure scene loads

        
    }

    void MovePlayerToTerrain()
    {

        PlayerController player = FindObjectOfType<PlayerController>();
        Terrain terrain = FindObjectOfType<Terrain>(); // Get the terrain
        if(terrain == null){
            Debug.LogError("No Terrain found in the scene");
            return;
        }
        if (player != null)
        {   
            // get the terrain dimensions
            float terrainWidth = terrain.terrainData.size.x;
            float terrainDepth = terrain.terrainData.size.z;
            float terrainHeight = terrain.terrainData.size.y;
            
            //calculate the spawn positions
            float spawnX = terrainWidth / 2;
            float spawnZ = terrainDepth / 2;
            float spawnY = terrain.SampleHeight(new Vector3(spawnX, 0 ,spawnZ)) + 2f;


            // move the player to the spawn position
            player.transform.position = new Vector3(spawnX, spawnY, spawnZ);
            Debug.Log($"Player moved to: {player.transform.position}");
        }
    }
}
