using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureGenerator 
{
  public static Texture2D TextureFromColorMap(Color[] colorMap, int width, int height){
     Texture2D texture = new Texture2D(width, height);
     texture.filterMode = FilterMode.Point;
     texture.wrapMode = TextureWrapMode.Clamp;
     texture.SetPixels(colorMap);
     texture.Apply();
     return texture;
  }

  public static Texture2D TexturefromHeightMap(float[,] noiseMap){
    
    int width = noiseMap.GetLength(0);
    int height = noiseMap.GetLength(1);


    Color[,] colorMap = new Color[width,height];

    for(int y = 0; y<height;y++){
      for(int x=0;x<width;x++){
        colorMap[x,y] = Color.Lerp(Color.black, Color.white, noiseMap[x,y]);
      }
    }
     
    // flatten the color map 

    Color[] colors = new Color[width*height];

    for(int i=0,y = 0; y<height;y++){
      for(int x=0;x<width;x++){
        colors[i] = colorMap[x,y];
        i++;
      }
    }
    return TextureFromColorMap(colors, width, height);
  }

}
