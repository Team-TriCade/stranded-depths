using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(MeshFilter))] // just ensure that the mesh filter exists

public class MeshGeneratorOld : MonoBehaviour
{
    Mesh mesh;
    
    Vector3[] vertices;
    int[] triangles;


    public int xSize = 100;
    public int zSize = 100;


    public float[] scales = {10f,20f,40f,80f,160f};
    public float[] amplitudes = {1f,0.5f,0.25f,0.125f,0.0625f};
    public float multiplier = 10f; // the perlin noise multiplier


    float minTerrainHeight;
    float maxTerrainHeight;
    

    Renderer rendererNew;
    Material terrainMaterial;
    // Start is called before the first frame update
    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        rendererNew = GetComponent<Renderer>();
        terrainMaterial = rendererNew.material;
        CreateShape();
        UpdateMesh();
    } 
    

    void CreateShape(){
      int verticesCount = (xSize+1)*(zSize+1);
      vertices = new Vector3[verticesCount];
      for(int i = 0, z = 0; z <= zSize; z++){
        for(int x = 0; x <= xSize; x++){
          float y = CalculateNoise(x,z);
          vertices[i] = new Vector3(x,y,z);

          if(y>maxTerrainHeight) maxTerrainHeight = y;
          if(y<minTerrainHeight) minTerrainHeight = y;
          i++;
        }
      }
      
      triangles = new int[xSize*zSize*6];
      int vertex = 0;
      int tri = 0;
      for(int z = 0; z< zSize; z++){
        for(int x = 0; x < xSize; x++){
          triangles[0+tri] = vertex + 0;
          triangles[1+tri] = vertex+ xSize + 1;
          triangles[2+tri] = vertex+ 1;
          triangles[3+tri] = vertex+ 1;
          triangles[4+tri] = vertex+ xSize+1;
          triangles[5+tri] = vertex+ xSize+2;
          vertex++;
          tri += 6;
        }
        vertex++;
      }
      
      //colors = new Color[vertices.Length];
      //for(int i = 0, z = 0; z <= zSize; z++){
      //  for(int x = 0; x <= xSize; x++){
      //    float height = Mathf.InverseLerp(minTerrainHeight, maxTerrainHeight, vertices[i].y); // normalise the height
      //    colors[i] = gradient.Evaluate(height);
      //    i++;
      //  }
     // }

    }

    void UpdateMesh(){
      mesh.Clear(); // clear the previous data, if any
      mesh.vertices = vertices;
      mesh.triangles = triangles;

      mesh.RecalculateNormals();
    }
    
    float CalculateNoise(int x, int z){
      float xCoord = (float)x/xSize;
      float zCoord = (float)z/zSize;
      float totalNoise = 0;

      for(int i = 0; i<scales.Length; i++){
        float perlinNoise = Mathf.PerlinNoise(xCoord*scales[i], zCoord*scales[i])*2-1; // also normalise it between -1..1 
        totalNoise += perlinNoise*amplitudes[i]; // apply multiple levels of noise
      }

      return  totalNoise*multiplier; 
    }

  void passValues(){
    terrainMaterial.SetFloat("_MinHeight", minTerrainHeight);
    terrainMaterial.SetFloat("_MaxHeight", maxTerrainHeight);
  }
}
