using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class TerrainGenerator : MonoBehaviour
{
    [Header("Terrain Settings")]
    [SerializeField] private int width = 100;
    [SerializeField] private int length = 100;
    [SerializeField] private float heightScale = 20f;
    
    [Header("Noise Settings")]
    [SerializeField] private float noiseScale = 50f;
    [SerializeField] private int octaves = 4;
    [SerializeField, Range(0f, 1f)] private float persistence = 0.5f;
    [SerializeField] private float lacunarity = 2f;
    [SerializeField] private int seed;
    [SerializeField] private Vector2 offset;

    private MeshFilter meshFilter;
    private float[,] heightMap;
    
    // Properties to access terrain dimensions
    public int Width => width;
    public int Length => length;

    private void Awake()
    {
        // Initialize on Awake to ensure heightMap exists before other scripts need it
        meshFilter = GetComponent<MeshFilter>();
        heightMap = GenerateHeightMap();
        GenerateTerrain();
    }

    public void GenerateTerrain()
    {
        // Create mesh data
        Mesh mesh = new Mesh();
        Vector3[] vertices = new Vector3[(width + 1) * (length + 1)];
        int[] triangles = new int[width * length * 6];
        Vector2[] uvs = new Vector2[vertices.Length];

        // Generate vertices
        for (int i = 0, z = 0; z <= length; z++)
        {
            for (int x = 0; x <= width; x++)
            {
                float height = heightMap[x, z];
                vertices[i] = new Vector3(x, height * heightScale, z);
                uvs[i] = new Vector2((float)x / width, (float)z / length);
                i++;
            }
        }

        // Generate triangles
        int vert = 0;
        int tris = 0;
        for (int z = 0; z < length; z++)
        {
            for (int x = 0; x < width; x++)
            {
                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = vert + width + 1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + width + 1;
                triangles[tris + 5] = vert + width + 2;

                vert++;
                tris += 6;
            }
            vert++;
        }

        // Apply mesh data
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();
        
        meshFilter.sharedMesh = mesh;
    }

    private float[,] GenerateHeightMap()
    {
        float[,] heightMap = new float[width + 1, length + 1];
        
        System.Random prng = new System.Random(seed);
        Vector2[] octaveOffsets = new Vector2[octaves];
        
        for (int i = 0; i < octaves; i++)
        {
            float offsetX = prng.Next(-100000, 100000) + offset.x;
            float offsetY = prng.Next(-100000, 100000) + offset.y;
            octaveOffsets[i] = new Vector2(offsetX, offsetY);
        }

        for (int y = 0; y <= length; y++)
        {
            for (int x = 0; x <= width; x++)
            {
                float amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;

                // Generate multiple layers of noise
                for (int i = 0; i < octaves; i++)
                {
                    float sampleX = (x - width/2f) / noiseScale * frequency + octaveOffsets[i].x;
                    float sampleY = (y - length/2f) / noiseScale * frequency + octaveOffsets[i].y;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                    noiseHeight += perlinValue * amplitude;

                    amplitude *= persistence;
                    frequency *= lacunarity;
                }

                heightMap[x, y] = noiseHeight;
            }
        }

        return heightMap;
    }
    
    public float GetHeightAtPoint(float x, float z)
    {
        // Ensure heightMap exists
        if (heightMap == null)
        {
            Debug.LogWarning("HeightMap was null, regenerating...");
            heightMap = GenerateHeightMap();
        }
        
        // Convert world position to heightmap coordinates
        int heightMapX = Mathf.RoundToInt(x);
        int heightMapZ = Mathf.RoundToInt(z);
        
        // Clamp to bounds
        heightMapX = Mathf.Clamp(heightMapX, 0, width);
        heightMapZ = Mathf.Clamp(heightMapZ, 0, length);
        
        return heightMap[heightMapX, heightMapZ] * heightScale;
    }
}