using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noise 
{
   public enum NormaliseMode{
    Local,
    Global
   };
   public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, int seed,  float scale, int octaves, float persistance, float lacunarity, Vector2 offset, NormaliseMode normaliseMode){
      float[,] noiseMap = new float[mapWidth,mapHeight];
      
      System.Random prng = new System.Random(seed); //pseudo random number generator
      Vector2[] octaveOffsets = new Vector2[octaves];

      float maxPossibleHeight = 0;
      float amplitude = 1;
      float frequency = 1;

      for(int i = 0; i<octaves;i++){
        float offsetX  = prng.Next(-100000,100000) + offset.x;
        float offsetY = prng.Next(-100000,100000) - offset.y;
        octaveOffsets[i] = new Vector2(offsetX, offsetY);
        maxPossibleHeight += amplitude;
        amplitude *= persistance;
      }

      if(scale <= 0) 
        scale = 0.0001f; // clamp it down to something not 0
    
      float maxLocalNoiseHeight = float.MinValue;
      float minLocalNoiseHeight = float.MaxValue;

      float halfWidth = mapWidth/2f;
      float halfHeight = mapHeight/2f;

      for(int y = 0; y < mapHeight; y++){
        for(int x = 0; x < mapWidth; x++){
          amplitude = 1;
          frequency = 1;
          float noiseHeight = 0;
          
          //loop through all the octaves
          for(int i = 0; i<octaves; i++){
          float sampleX = (x - halfWidth + octaveOffsets[i].x) / scale * frequency;
          float sampleY = (y - halfHeight + octaveOffsets[i].y) / scale * frequency;
          
      
          float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1; // -1..1 
          noiseHeight += perlinValue * amplitude;

          amplitude *= persistance; // decreases each octave as persistance = 0..1
          frequency *= lacunarity; // increases each octave as lacunarity > 1
          }
          
          if(noiseHeight>maxLocalNoiseHeight)
            maxLocalNoiseHeight = noiseHeight;

          if(noiseHeight<minLocalNoiseHeight)
            minLocalNoiseHeight = noiseHeight;


          noiseMap[x,y] = noiseHeight;
        }
      }  
       //normalise 
      for(int y = 0; y < mapHeight; y++){
        for(int x = 0; x < mapWidth; x++){
          if(normaliseMode == NormaliseMode.Local)
            noiseMap[x,y] = Mathf.InverseLerp(minLocalNoiseHeight, maxLocalNoiseHeight, noiseMap[x,y]);
          else{
            float normalisedHeight = (noiseMap[x,y] + 1)/(2f*maxPossibleHeight/1.85f); // just an estimate that works *wink*
            noiseMap[x,y] = Mathf.Clamp(normalisedHeight, 0 , int.MaxValue);
          }
        }
      }
      
      return noiseMap;
  } 
}
