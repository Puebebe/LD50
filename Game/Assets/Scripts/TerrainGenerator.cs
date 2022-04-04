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
    private Transform center;


    private MeshCollider meshCollider;
    private Mesh mesh;
    private Vector3[] vertices;
    private int[] triangles;
    private float lastZLocation;

    void Start()
    {
        meshCollider = GetComponent<MeshCollider>();
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        CreateShiftedVertices();
        CreateTriangles();
        UpdateMesh();
    }

    void CreateShiftedVertices(float shift = 0)
    {
        vertices = new Vector3[(xSize + 1) * (2 * drawDistance + 1)];

        var xSizeHalf = xSize /2;
        for (int i = 0, z = -drawDistance; z <= drawDistance; z++)
        {
            for (int x = -xSizeHalf; x <= xSizeHalf; x++)
            {
                float shiftedZ = z + shift;
                float y = Mathf.PerlinNoise(x * 0.3f, shiftedZ * 0.3f);
                vertices[i] = new Vector3(x, y, shiftedZ);
                i++;
            }
        }

        lastZLocation = center.position.z;
    }

    void CreateTriangles()
    {
        triangles = new int[xSize * 2 * drawDistance * 6];
        int vert = 0;
        int tris = 0;
        for (int z = -drawDistance; z < drawDistance; z++)
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

    void Update()
    {
        UpdateVertices();
        UpdateMesh();
    }

    void UpdateVertices()
    {
        var shiftZ = (int)(center.position.z - lastZLocation);
        if (shiftZ == 0) return;

        var totalShiftZ = (int)(center.position.z - transform.position.z);
        CreateShiftedVertices(totalShiftZ);
    }
}
