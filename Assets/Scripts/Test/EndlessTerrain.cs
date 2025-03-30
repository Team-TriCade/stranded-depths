using System.Collections.Generic;
using UnityEngine;

public class EndlessTerrain : MonoBehaviour
{
   public const float maxViewDistance = 450;
   public Transform viewer;
   public Material mapMaterial;

   public static Vector2 viewerPosition;
   static MapGenerator mapGenerator;
   int chunkSize;
   int chunksVisibleInViewDistance;

   Dictionary<Vector2, TerrainChunk> terrainChunkDict = new Dictionary<Vector2, TerrainChunk>();
   List<TerrainChunk> terrainChunksVisibleLastUpdate = new List<TerrainChunk>();


   void Start(){
      mapGenerator = FindObjectOfType<MapGenerator>();
      chunkSize = MapGenerator.mapChunkSize-1;
      chunksVisibleInViewDistance = Mathf.RoundToInt(maxViewDistance/chunkSize);
   }
   
   void Update(){
    viewerPosition = new Vector2(viewer.position.x, viewer.position.z);
    UpdateVisibleChunk();
   }
   
   void UpdateVisibleChunk(){

      for(int i = 0; i < terrainChunksVisibleLastUpdate.Count; i++){
        terrainChunksVisibleLastUpdate[i].SetVisible(false);
      }
      terrainChunksVisibleLastUpdate.Clear();
      int currentChunkCoordX = Mathf.RoundToInt(viewerPosition.x/chunkSize);
      int currentChunkCoordY = Mathf.RoundToInt(viewerPosition.y/chunkSize);

      for(int yOffset = -chunksVisibleInViewDistance; yOffset <= chunksVisibleInViewDistance; yOffset++){
        for(int xOffset = -chunksVisibleInViewDistance; xOffset <= chunksVisibleInViewDistance; xOffset++){

          Vector2 viewedChunkCoord = new Vector2(currentChunkCoordX+xOffset, currentChunkCoordY+yOffset);

          if(terrainChunkDict.ContainsKey(viewedChunkCoord)){
            terrainChunkDict[viewedChunkCoord].UpdateChunk();
            if(terrainChunkDict[viewedChunkCoord].IsVisible()){
              terrainChunksVisibleLastUpdate.Add(terrainChunkDict[viewedChunkCoord]);
            }
          }
          else{
            terrainChunkDict.Add(viewedChunkCoord, new TerrainChunk(viewedChunkCoord, chunkSize, transform, mapMaterial));
          }
        }
      }
   }

   public class TerrainChunk{
      GameObject meshObject;
      Vector2 position;
   Bounds bounds;

   MapData mapData;

   MeshRenderer meshRenderer;
   MeshFilter meshFilter;

      public TerrainChunk(Vector2 coord, int size, Transform parent, Material material){
        position = coord * size;
        bounds = new Bounds(position, Vector2.one*size);
        Vector3 positionV3 = new Vector3(position.x, 0, position.y);
        meshObject = new GameObject("Terrain Chunk"); 
        meshRenderer = meshObject.AddComponent<MeshRenderer>();
        meshFilter = meshObject.AddComponent<MeshFilter>();

        meshRenderer.material = material;
        
        Debug.Log("Assigned Material: " + material.name);
        meshObject.transform.position =  positionV3;
        meshObject.transform.parent = parent;
        SetVisible(false); // make sure it isnt visible from the start

        mapGenerator.RequestMapData(OnMapDataReceived);
      }

      void OnMapDataReceived(MapData mapData){
        mapGenerator.RequestMeshData(mapData, OnMeshDataReceived); 
      }
      
      void OnMeshDataReceived(MeshData meshData){
        meshFilter.mesh = meshData.CreateMesh();

        Debug.Log("Mesh Assigned: " + meshFilter.mesh);

      }

      public void UpdateChunk(){ // to determine if the chunk should be visible or not
        float viewerDistanceFromNearestEdge = Mathf.Sqrt(bounds.SqrDistance(viewerPosition));
        bool isVisible = viewerDistanceFromNearestEdge <= maxViewDistance;
        SetVisible(isVisible);
      }

      public void SetVisible(bool isVisible){
        meshObject.SetActive(isVisible);

      }

      public bool IsVisible(){
        return meshObject.activeSelf;
      }
   }
}
