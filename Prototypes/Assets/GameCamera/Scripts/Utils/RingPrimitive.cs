// Version 1.1.12
// ©2013 Reindeer Games
// All rights reserved
// Redistribution of source code without permission not allowed

using System;
using System.Diagnostics;
using UnityEngine;

namespace RG_GameCamera.Utils
{
    /// <summary>
    /// class for creating Ring primitive
    /// </summary>
    public class RingPrimitive
    {
        /// <summary>
        /// create Ring game object
        /// </summary>
        /// <param name="radiusA">radius A of ring</param>
        /// <param name="radiusB">radius B of ring</param>
        /// <param name="thickness">thickness of the ring</param>
        /// <param name="segments">number of segments</param>
        /// <returns>Ring game object</returns>
        public static GameObject Create(float radiusA, float radiusB, float thickness, int segments, Color color)
        {
            var ring = new GameObject("DebugRing");

            var meshFilter = ring.AddComponent<MeshFilter>();
            var renderer = ring.AddComponent<MeshRenderer>();

            renderer.sharedMaterial = new Material(Shader.Find("VertexLit")) {color = color};

            if (meshFilter.sharedMesh == null)
            {
                meshFilter.sharedMesh = new Mesh();
            }

            GenerateGeometry(meshFilter.sharedMesh, radiusA, radiusB, thickness, segments);
            ring.transform.Rotate(Vector3.right, 180.0f);
            return ring;
        }

        public static void Generate(GameObject obj, float radiusA, float radiusB, float thickness, int segments)
        {
            GenerateGeometry(obj.GetComponent<MeshFilter>().sharedMesh, radiusA, radiusB, thickness, segments);
        }

        private static void GenerateGeometry(Mesh mesh, float radiusA, float radiusB, float thickness, int segments)
        {
            radiusA = Mathf.Clamp(radiusA, 0, 100);
            radiusB = Mathf.Clamp(radiusB, 0, 100);
            thickness = Mathf.Clamp(thickness, 0, 100);
            segments = Mathf.Clamp(segments, 3, 100);

            mesh.Clear();

            var verticesNum = segments * 2;
            var trianglesNum = segments * 2;

            if (verticesNum > 60000)
            {
                UnityEngine.Debug.LogError("Too much vertices!");
            }

            var vertices = new Vector3[verticesNum];
            var normals = new Vector3[verticesNum];
            var uvs = new Vector2[verticesNum];
            var triangles = new int[trianglesNum*3];

            // generate vertices
            var vertIndex = 0;
            for (int i = 0; i < segments; i++)
            {
                var angle = (float) i/segments * Mathf.PI * 2.0f;
                var v0 = new Vector3(Mathf.Sin(angle), 0.0f, Mathf.Cos(angle));

                var uvRatio = 0.5f * (radiusA / radiusB);

                var uvV = new Vector2(v0.x * 0.5f, v0.z * .5f);
                var uvVInner = new Vector2(v0.x * uvRatio, v0.z * uvRatio);
                var uvCenter = new Vector2(0.5f, 0.5f);

                vertices[vertIndex + 0] = new Vector3(v0.x*radiusA, 0.0f, v0.z*radiusB);
                normals[vertIndex + 0] = new Vector3(0, 1, 0);
                uvs[vertIndex + 0] = uvCenter + uvVInner;

                vertices[vertIndex + 1] = new Vector3(v0.x * (radiusA-thickness), 0.0f, v0.z * (radiusB-thickness));
                normals[vertIndex + 1] = new Vector3(0, 1, 0);
                uvs[vertIndex + 1] = uvCenter + uvV;

                vertIndex += 2;
            }

            // generate triangles
            var triVert = 0;
            var triIdx = 0;
            for (int i = 0; i < segments; i++)
            {
                triangles[triIdx + 0] = triVert + 0;
                triangles[triIdx + 1] = triVert + 1;
                triangles[triIdx + 2] = triVert + 3;

                triangles[triIdx + 3] = triVert + 2;
                triangles[triIdx + 4] = triVert + 0;
                triangles[triIdx + 5] = triVert + 3;

                if (i == segments - 1)
                {
                    triangles[triIdx + 2] = 1;
                    triangles[triIdx + 3] = 0;
                    triangles[triIdx + 5] = 1;
                }

                triVert += 2;
                triIdx += 6;
            }

            for (int i = 0; i < normals.Length; i++)
            {
                normals[i] = -normals[i];
            }

            for (int i = 0; i < triangles.Length; i += 3)
            {
                int temp = triangles[i + 0];
                triangles[i + 0] = triangles[i + 1];
                triangles[i + 1] = temp;
            }

            mesh.vertices = vertices;
            mesh.normals = normals;
            mesh.uv = uvs;
            mesh.triangles = triangles;

            mesh.RecalculateBounds();
            mesh.Optimize();
        }
    }
}
