using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
public class TerrainGenerator : MonoBehaviour
{
    [SerializeField]
    private int xSize = 20;
    [SerializeField]
    private int drawDistance = 20;
    [SerializeField]
    private Transform location;


    private MeshCollider meshCollider;
    private Mesh mesh;
    private Vector3[] vertices;
    private int[] triangles;

    // Start is called before the first frame update
    void Start()
    {
        meshCollider = GetComponent<MeshCollider>();
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        CreateShape();
        UpdateMesh();
    }

    void CreateShape()
    {
        vertices = new Vector3[(xSize + 1) * (drawDistance + 1)];

        for (int i = 0, z = 0; z <= drawDistance; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                float y = Mathf.PerlinNoise(x * 0.3f, z * 0.3f);
                vertices[i] = new Vector3(x, y, z);
                i++;
            }
        }

        triangles = new int[xSize * drawDistance * 6];
        int vert = 0;
        int tris = 0;
        for (int z = 0; z < drawDistance; z++)
        {
            for (int x = 0; x < xSize; x++)
            {
                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = vert + xSize + 1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + xSize + 1;
                triangles[tris + 5] = vert + xSize + 2;

                vert++;
                tris += 6;
            }
            vert++;
        }
    }

    private void UpdateMesh()
    {
        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        meshCollider.sharedMesh = mesh;
    }

    // Update is called once per frame
    void Update()
    {

    }
}