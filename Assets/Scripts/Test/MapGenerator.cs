using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;
using System.Collections;

public class MapGenerator : MonoBehaviour
{

  public enum DrawMode { NoiseMap, ColorMap, Mesh, FalloffMap }

  public DrawMode drawmode;

  public Noise.NormaliseMode normaliseMode;

  public const int mapChunkSize = 241;
  [Range(0, 6)]
  public int editorPreviewLOD; // the lower, the less simplified

  public float noiseScale;

  public int octaves;
  public float lacunarity;
  [Range(0, 1)]
  public float persistance;

  public int seed;
  public Vector2 offset;

  public bool useFalloffMap;

  public float meshHeightMultiplier;
  public AnimationCurve meshHeightCurve;

  public bool autoUpdate;

  public TerrainType[] regions;

  float[,] falloffMap;

  Queue<MapThreadInfo<MapData>> mapDataThreadInfoQueue = new Queue<MapThreadInfo<MapData>>();
 Queue<MapThreadInfo<MeshData>> meshDataThreadInfoQueue = new Queue<MapThreadInfo<MeshData>>();

  void Awake(){
    falloffMap = FalloffGenerator.GenerateFalloffMap(mapChunkSize);
  }
  public void DrawMapInEditor()
  {

    MapData mapData = GenerateMapData(Vector2.zero);
    MapDisplay display = FindObjectOfType<MapDisplay>();

    if (drawmode == DrawMode.NoiseMap)
      display.DrawTexture(
          TextureGenerator.TexturefromHeightMap(mapData.heightMap));

    else if (drawmode == DrawMode.ColorMap)
      display.DrawTexture(TextureGenerator.TextureFromColorMap(
          mapData.colorMap, mapChunkSize, mapChunkSize));

    else if (drawmode == DrawMode.Mesh)
      display.DrawMesh(MeshGenerator.GenerateTerrainMesh(
                           mapData.heightMap, meshHeightMultiplier,
                           meshHeightCurve, editorPreviewLOD),
                       TextureGenerator.TextureFromColorMap(
                           mapData.colorMap, mapChunkSize, mapChunkSize));
    else if(drawmode == DrawMode.FalloffMap)
      display.DrawTexture(TextureGenerator.TexturefromHeightMap(FalloffGenerator.GenerateFalloffMap(mapChunkSize)));
  }

  public void RequestMapData(Vector2 centre, Action<MapData> callback){
    ThreadStart threadStart = delegate {
        MapDataThread(centre, callback);
    };

    new Thread(threadStart).Start();
  }

  void MapDataThread(Vector2 centre, Action<MapData> callBack){
    MapData mapData = GenerateMapData(centre);
    lock (mapDataThreadInfoQueue){
    mapDataThreadInfoQueue.Enqueue(new MapThreadInfo<MapData>(callBack, mapData));
    }
  }
  public void RequestMeshData(MapData mapData, int lod, Action<MeshData> callback){
    ThreadStart threadStart = delegate{
      MeshDataThread(mapData, lod, callback);
    };

    new Thread (threadStart).Start();
    
  }
  void MeshDataThread(MapData mapData, int lod, Action<MeshData> callBack){
    MeshData meshData = MeshGenerator.GenerateTerrainMesh(mapData.heightMap, meshHeightMultiplier, meshHeightCurve, lod);
    lock (meshDataThreadInfoQueue){
    meshDataThreadInfoQueue.Enqueue(new MapThreadInfo<MeshData>(callBack, meshData));
    }
  }


  void Update(){
    if(mapDataThreadInfoQueue.Count > 0) {
      for(int i = 0; i < mapDataThreadInfoQueue.Count; i++){
        MapThreadInfo<MapData> threadInfo = mapDataThreadInfoQueue.Dequeue();
        threadInfo.callback(threadInfo.parameter);
      }
      
    }
    if(meshDataThreadInfoQueue.Count > 0){
      for(int i = 0; i< meshDataThreadInfoQueue.Count; i++){
        MapThreadInfo<MeshData> threadInfo = meshDataThreadInfoQueue.Dequeue();
        threadInfo.callback(threadInfo.parameter);
      }
    }
  }

  MapData GenerateMapData(Vector2 centre)
  {
    float[,] noiseMap =
        Noise.GenerateNoiseMap(mapChunkSize, mapChunkSize, seed, noiseScale,
                               octaves, persistance, lacunarity, centre + offset, normaliseMode);

    Color[] colorMap = new Color[mapChunkSize * mapChunkSize];
    for (int y = 0; y < mapChunkSize; y++)
    {
      for (int x = 0; x < mapChunkSize; x++)
      {  
        if(useFalloffMap){
            noiseMap[x,y] = Mathf.Clamp01(noiseMap[x,y] - falloffMap[x,y]);
        } 
        float currentHeight = noiseMap[x, y];
        for (int i = 0; i < regions.Length; i++)
        {
         if (currentHeight >= regions[i].height)
          {
            colorMap[y * mapChunkSize + x] = regions[i].color;
          }
          else{
            break;
          }
        }
      }
    }
    return new MapData(noiseMap, colorMap);
  }

  void OnValidate()
  {
    if (lacunarity < 1)
      lacunarity = 1;

    if (octaves < 0)
      octaves = 0;

    falloffMap = FalloffGenerator.GenerateFalloffMap(mapChunkSize);
  }

  struct MapThreadInfo<T>{
    public readonly Action<T> callback;
    public readonly T parameter;

    public MapThreadInfo(Action<T> callback, T parameter){
      this.callback = callback;
      this.parameter = parameter;
    }
    
  }
}

[System.Serializable]
public struct TerrainType
{
  public string name;
  public float height;
  public Color color;
}

public struct MapData
{
  public readonly float[,] heightMap;
  public readonly Color[] colorMap;

  public MapData(float[,] heightMap, Color[] colorMap)
  {
    this.heightMap = heightMap;
    this.colorMap = colorMap;
  }
}
