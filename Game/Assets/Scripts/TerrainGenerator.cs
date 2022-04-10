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
    private int generateChunkLength = 10;
    [SerializeField]
    private int minDeformationWidth = 10;
    [SerializeField]
    private int minDeformationLength = 10;
    [SerializeField]
    private float deformationMaxHeight = 2;
    [SerializeField, Range(0f, 1f)]
    private float deformationPropability = 0.5f;
    [SerializeField]
    private int sideDeformationWidth = 5;
    [SerializeField]
    private float sideDeformationHeight = 50f;


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
        vertices = new Vector3[(xSize + 1 + 2 * sideDeformationWidth) * (2 * drawDistance + 1)];

        GenerateVertices(0);
        GenerateSideDeformation(0);
        UpdateLastLocation();
        CreateTriangles();
        UpdateMesh();
    }

    private void GenerateVertices(int fromRow)
    {
        var positionZ = (int)center.position.z;
        var widthHalf = xSize / 2 + sideDeformationWidth;
        var startIndex = fromRow * (xSize + 1 + 2 * sideDeformationWidth);
        for (int i = startIndex, z = positionZ - drawDistance + fromRow; z <= positionZ + drawDistance; z++)
        {
            for (int x = -widthHalf; x <= widthHalf; x++)
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
        triangles = new int[(xSize + sideDeformationWidth * 2) * 2 * drawDistance * 2 * 3];
        int vert = 0;
        int tris = 0;
        var totalWidth = xSize + 2 * sideDeformationWidth;
        for (int z = -drawDistance; z < drawDistance; z++)
        {
            for (int x = 0; x < totalWidth; x++)
            {
                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = vert + totalWidth + 1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + totalWidth + 1;
                triangles[tris + 5] = vert + totalWidth + 2;

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
        var shiftZ = (int)(center.position.z - lastZLocation);
        if (shiftZ < generateChunkLength) return;

        UpdateVertices(shiftZ);
        UpdateMesh();
        UpdateObstacles();
        RemoveFarObstacles();
    }

    private void UpdateVertices(int shiftZ)
    {
        ShiftVerticesBy(shiftZ);
        GenerateVertices(2 * drawDistance - shiftZ);
        GenerateSideDeformation(2 * drawDistance - shiftZ);
        if (Random.Range(0f, 1f) <= deformationPropability)
        {
            GenerateDeformation(2 * drawDistance - shiftZ);
        }
        UpdateLastLocation();
    }

    private void ShiftVerticesBy(int shift)
    {
        for (int i = 0, j = shift * (xSize + 1 + 2 * sideDeformationWidth); j < vertices.Length; j++, i++)
        {
            vertices[i] = vertices[j];
        }
    }

    private void GenerateSideDeformation(int fromRow)
    {
        var positionZ = (int)center.position.z;
        var xSizeHalf = xSize / 2;
        var startIndex = fromRow * (xSize + 1 + 2 * sideDeformationWidth);
        var deformationStartZ = positionZ - drawDistance + fromRow;
        var deformationEndZ = positionZ + drawDistance;

        for (int i = startIndex, z = deformationStartZ; z <= deformationEndZ; z++)
        {
            var deformationStep = Mathf.PI / (2 * sideDeformationWidth + 2);
            var currentDeformation = 0f;

            i += xSize + sideDeformationWidth;
            for (int x = xSizeHalf; x <= xSizeHalf + sideDeformationWidth; x++)
            {
                vertices[i] += new Vector3(0, Mathf.Sin(currentDeformation) * sideDeformationHeight, 0);
                i++;
                currentDeformation += deformationStep;
            }

            i -= xSize + 2 * sideDeformationWidth + 1;
            for (int x = -xSizeHalf - sideDeformationWidth; x <= -xSizeHalf; x++)
            {
                vertices[i] += new Vector3(0, Mathf.Sin(currentDeformation) * sideDeformationHeight, 0);
                i++;
                currentDeformation += deformationStep;
            }
            i += xSize + sideDeformationWidth;
        }
    }

    private void GenerateDeformation(int fromRow)
    {
        var positionZ = (int)center.position.z;
        var xSizeHalf = xSize / 2;
        var startIndex = fromRow * (xSize + 1 + 2 * sideDeformationWidth);
        var deformationStartX = (int)Random.Range(transform.position.x - xSizeHalf, transform.position.x + xSizeHalf - minDeformationWidth);
        var deformationEndX = (int)Random.Range(deformationStartX + minDeformationWidth, transform.position.x + xSizeHalf);
        var deformationStartZ = (int)Random.Range(positionZ - drawDistance + fromRow, positionZ + drawDistance - minDeformationLength);
        var deformationEndZ = (int)Random.Range(deformationStartZ + minDeformationLength, positionZ + drawDistance);
        var deformationHeight = Random.Range(-deformationMaxHeight, deformationMaxHeight);

        var indexPreIndent = deformationStartX - ((int)transform.position.x - xSizeHalf) + sideDeformationWidth;
        var indexPostIndent = ((int)transform.position.x + xSizeHalf) - deformationEndX + sideDeformationWidth;
        var deformationStepZ = Mathf.PI / (deformationEndZ - deformationStartZ);
        var currentDeformationZ = 0f;
        for (int i = startIndex + indexPreIndent, z = deformationStartZ; z <= deformationEndZ; z++)
        {
            var deformationStepX = Mathf.PI / (deformationEndX - deformationStartX);
            var currentDeformationX = 0f;
            for (int x = deformationStartX; x <= deformationEndX; x++)
            {
                vertices[i] += new Vector3(0, Mathf.Sin(currentDeformationX) * Mathf.Sin(currentDeformationZ) * deformationHeight, 0);
                i++;
                currentDeformationX += deformationStepX;
            }
            i += indexPostIndent + indexPreIndent;
            currentDeformationZ += deformationStepZ;
        }
    }

    private void UpdateObstacles()
    {
        if (center.position.z - transform.position.z > generateChunkLength * (segmentsObstacles.Count + 1))
        {
            PlaceObstacles();
            RemoveFarObstacles();
        }
    }

    private void PlaceObstacles()
    {
        var startZ = (int)center.position.z + drawDistance - generateChunkLength;
        var endZ = startZ + generateChunkLength;
        var halfWidth = xSize / 2;

        var height = (int)generateChunkLength;

        var obstaclesToPlace = segmentsObstacles.Count / 10 + 1;
        var placedObstacles = new List<GameObject>(obstaclesToPlace);
        for (int i = 0; i < obstaclesToPlace; i++)
        {
            var x = Random.Range(-halfWidth, halfWidth);
            var z = Random.Range(startZ, endZ);
            var y = GetVertexPositionAt(x, z).y;
            placedObstacles.Add(GameObject.Instantiate(obstacle, new Vector3(x, y, z), obstacle.transform.rotation));
        }
        segmentsObstacles.Add(placedObstacles);
    }

    private Vector3 GetVertexPositionAt(int x, int z)
    {
        var meshStart = transform.TransformPoint(mesh.bounds.min);
        var vertex = vertices[(z - (int)meshStart.z) * (xSize + 1 + 2 * sideDeformationWidth) + (x - (int)meshStart.x)];
        return transform.TransformPoint(vertex);
    }

    private void RemoveFarObstacles()
    {
        var removeCount = (int)(2 * drawDistance / generateChunkLength + 1);
        if (segmentsObstacles.Count > removeCount)
        {
            foreach (var obstacle in segmentsObstacles[segmentsObstacles.Count - removeCount - 1])
            {
                GameObject.Destroy(obstacle);
            }
        }
    }
}
