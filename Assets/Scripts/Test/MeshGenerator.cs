using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MeshGenerator
{
   public static MeshData GenerateTerrainMesh(float[,] heightMap, float heightMultiplier, AnimationCurve _heightCurve, int levelOfDetail){
    
      AnimationCurve heightCurve = new AnimationCurve(_heightCurve.keys); 
        
      int meshSimplificationIncrement = (levelOfDetail==0)?1:levelOfDetail*2;

      int borderedSize = heightMap.GetLength(0);
      int meshSize = borderedSize - 2*meshSimplificationIncrement;
      int meshSizeUnsimplified = borderedSize-2;

      
      float topLeftX = (meshSizeUnsimplified-1)/-2f;
      float topLeftZ = (meshSizeUnsimplified-1)/2f;
      
      int verticesPerLine = (meshSize-1)/meshSimplificationIncrement + 1;

      MeshData meshData = new MeshData(verticesPerLine);
      
      int[,] vertexIndicesMap = new int[borderedSize, borderedSize];
      int meshVertexIndex = 0;
      int borderVertexIndex = -1;
      
      for(int y = 0; y < borderedSize; y+=meshSimplificationIncrement){
        for(int x = 0; x < borderedSize; x+= meshSimplificationIncrement){
          bool isBorderVertex = y == 0 || y == borderedSize - 1 || x == 0 || x == borderedSize - 1;

          if(isBorderVertex){
            vertexIndicesMap[x,y] = borderVertexIndex;
            borderVertexIndex--;
          }
          else{
            vertexIndicesMap[x,y] = meshVertexIndex;
            meshVertexIndex++;
          }
        }
      }  


      for(int y = 0; y < borderedSize; y+=meshSimplificationIncrement){
        for(int x = 0; x < borderedSize; x+= meshSimplificationIncrement){
          int vertexIndex = vertexIndicesMap[x,y];
          Vector2 percent = new Vector2((x-meshSimplificationIncrement)/(float)meshSize, (y-meshSimplificationIncrement)/(float)meshSize);
          float height = heightCurve.Evaluate(heightMap[x,y])*heightMultiplier;
          Vector3 vertexPosition = new Vector3(topLeftX + percent.x*meshSizeUnsimplified,height,topLeftZ - percent.y*meshSizeUnsimplified);
          
          meshData.AddVertex(vertexPosition, percent, vertexIndex);

          if(x < borderedSize - 1 && y < borderedSize - 1){ // ignore the right and bottom edges
            int a = vertexIndicesMap[x,y];
            int b = vertexIndicesMap[x+meshSimplificationIncrement,y];
            int c = vertexIndicesMap[x,y+meshSimplificationIncrement];
            int d = vertexIndicesMap[x+meshSimplificationIncrement,y+meshSimplificationIncrement];

            meshData.AddTris(a,d,c);
            meshData.AddTris(d,a,b);
          }
          vertexIndex++;
        }
      }
      return meshData;
   } 
}

public class MeshData{
    Vector3[] vertices;
    int[] tris;
    Vector2[] uvs;
    
    Vector3[] borderVertices;
    int[] borderTris;
    
    int triIndex;
    int borderTriIndex;

    public MeshData(int verticesPerLine){
      vertices = new Vector3[verticesPerLine*verticesPerLine]; 
      uvs = new Vector2[verticesPerLine*verticesPerLine];
      tris = new int[(verticesPerLine-1)*(verticesPerLine-1)*6];
      
      int verticesLength = verticesPerLine*4+4;
      Debug.Log(verticesLength);
      if(verticesLength>int.MaxValue){
        Debug.LogError("Vertices length too large");
        return;
      }

      int trisLength = 6*verticesPerLine*4;
      Debug.Log(trisLength);

      if(trisLength>int.MaxValue){
        Debug.LogError("Tri lenth too large");
        return;
      }

      borderVertices = new Vector3[verticesLength];
      borderTris = new int[trisLength];
    }

    public void AddVertex(Vector3 vertexPosition, Vector2 uv, int vertexIndex){
      if(vertexIndex < 0){
        borderVertices[-vertexIndex-1] = vertexPosition;
      }
      else{
        vertices[vertexIndex] = vertexPosition;
        uvs[vertexIndex] = uv;
      }
    }

    public void AddTris(int x, int y, int z){
      if(x < 0 || y < 0 || z < 0){
        borderTris[borderTriIndex] = x;
        borderTris[borderTriIndex+1] = y;
        borderTris[borderTriIndex+2] = z;
        borderTriIndex+=3;
      }
      else{
        tris[triIndex] = x;
        tris[triIndex+1] = y;
        tris[triIndex+2] = z;
        triIndex+=3;
      }
    }
    
    Vector3[] CalculateNormals(){
      Vector3[] vertexNormals = new Vector3[vertices.Length];
      int triCount = tris.Length/3;


      for(int i = 0; i < triCount; i++){
        int normalTriIndex = i*3;
        int vertexIndexA = tris[normalTriIndex];
        int vertexIndexB = tris[normalTriIndex + 1];
        int vertexIndexC = tris[normalTriIndex + 2];

        Vector3 triangleNormal = SurfaceNormalFromIndicies(vertexIndexA, vertexIndexB, vertexIndexC);

        vertexNormals[vertexIndexA] += triangleNormal;
        vertexNormals[vertexIndexB] += triangleNormal;
        vertexNormals[vertexIndexC] += triangleNormal;

      }

      int borderTriCount = borderTris.Length/3;
      for(int i = 0; i < borderTriCount; i++){
        int normalTriIndex = i*3;
        int vertexIndexA = borderTris[normalTriIndex];
        int vertexIndexB = borderTris[normalTriIndex + 1];
        int vertexIndexC = borderTris[normalTriIndex + 2];

        Vector3 triangleNormal = SurfaceNormalFromIndicies(vertexIndexA, vertexIndexB, vertexIndexC);

        if(vertexIndexA>=0)
          vertexNormals[vertexIndexA] += triangleNormal;

        if(vertexIndexB>=0)
          vertexNormals[vertexIndexB] += triangleNormal;

        if(vertexIndexC>=0)
          vertexNormals[vertexIndexC] += triangleNormal;

      }
      for(int i = 0; i < vertexNormals.Length; i++){
        vertexNormals[i].Normalize();
      }
      return vertexNormals;

    }

    Vector3 SurfaceNormalFromIndicies(int indexA, int indexB, int indexC){
      Vector3 pointA = (indexA<0)?borderVertices[-indexA-1]:vertices[indexA];
      Vector3 pointB = (indexB<0)?borderVertices[-indexB-1]:vertices[indexB];
      Vector3 pointC = (indexC<0)?borderVertices[-indexC-1]:vertices[indexC];

      Vector3 sideAB = pointB - pointA;
      Vector3 sideAC = pointC - pointA;
      return Vector3.Cross(sideAB, sideAC).normalized;
    }

    public Mesh CreateMesh(){
      Mesh mesh = new Mesh();
      mesh.vertices = vertices;
      mesh.triangles = tris;
      mesh.uv = uvs;
      mesh.normals = CalculateNormals();
      return mesh;
    }
}
