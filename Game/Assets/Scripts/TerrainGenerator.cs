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
    private float scale = 1;
    [SerializeField]
    private Transform center;
    [SerializeField]
    private GameObject obstacle;
    [SerializeField]
    private float segmentLength = 10;


    private MeshCollider meshCollider;
    private Mesh mesh;
    private Vector3[] vertices;
    private int[] triangles;
    private float lastZLocation;
    private List<List<GameObject>> segmentsObstacles = new List<List<GameObject>>();

    void Start()
    {
        meshCollider = GetComponent<MeshCollider>();
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        vertices = new Vector3[(xSize + 1) * (2 * drawDistance + 1)];

        GenerateVertices(0);
        UpdateLastLocation();
        CreateTriangles();
        UpdateMesh();
    }

    private void GenerateVertices(int fromRow)
    {
        var positionZ = (int)center.position.z;
        var xSizeHalf = xSize / 2;
        var startZ = fromRow * (xSize + 1);
        for (int i = startZ, z = positionZ - drawDistance + fromRow; z <= positionZ + drawDistance; z++)
        {
            for (int x = -xSizeHalf; x <= xSizeHalf; x++)
            {
                float y = Mathf.PerlinNoise(x * scale, z * scale);
                vertices[i] = new Vector3(x, y, z);
                i++;
            }
        }
    }

    private void UpdateLastLocation()
    {
        lastZLocation = center.position.z;
    }

    private void CreateTriangles()
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
        UpdateObstacles();
        RemoveFarObstacles();
    }

    private void UpdateVertices()
    {
        var shiftZ = (int)(center.position.z - lastZLocation);
        if (shiftZ < 1f) return;

        ShiftVerticesBy(shiftZ);
        GenerateVertices(2 * drawDistance - shiftZ);
        UpdateLastLocation();
    }

    private void ShiftVerticesBy(int shift)
    {
        for (int i = 0, j = shift * (xSize + 1); j < vertices.Length; j++, i++)
        {
            vertices[i] = vertices[j];
        }
    }

    private void UpdateObstacles()
    {
        if (center.position.z - transform.position.z > segmentLength * segmentsObstacles.Count)
        {
            PlaceObstacles();
            RemoveFarObstacles();
        }
    }

    private void PlaceObstacles()
    {
        var startZ = segmentLength * segmentsObstacles.Count + drawDistance;
        var endZ = startZ + segmentLength;
        var halfWidth = xSize / 2;

        var height = (int)segmentLength;

        var obstaclesToPlace = segmentsObstacles.Count / 10 + 1;
        var placedObstacles = new List<GameObject>(obstaclesToPlace);
        for (int i = 0; i < obstaclesToPlace; i++)
        {
            placedObstacles.Add(GameObject.Instantiate(obstacle, new Vector3(UnityEngine.Random.Range(-halfWidth, halfWidth), transform.position.y, UnityEngine.Random.Range(startZ, endZ)), Quaternion.identity));
        }
        segmentsObstacles.Add(placedObstacles);
    }

    private void RemoveFarObstacles()
    {
        var removeCount = (int)(2 * drawDistance / segmentLength + 1);
        if (segmentsObstacles.Count > removeCount)
        {
            foreach (var obstacle in segmentsObstacles[segmentsObstacles.Count - removeCount - 1])
            {
                GameObject.Destroy(obstacle);
            }
        }
    }
}
