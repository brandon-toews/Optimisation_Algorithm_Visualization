using System;
using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class PlantGrowthSimulation : MonoBehaviour
{
    [Header("Grid Settings")]
    [SerializeField] private GameObject plantPrefab;
    [SerializeField] public int gridSize = 30;
    [SerializeField] public float gridSpacing = 10.0f;

    [Header("Height Settings")]
    [SerializeField] private float minHeight = 1f;
    [SerializeField] private float maxHeight = 4f;
    [SerializeField] private int seed;

    [Header("Visual Settings")]
    [SerializeField] private Material highlightMaterial;
    [SerializeField] private Material axesPlaneMaterial;
    [SerializeField] private float transparency = 0.3f;
    
    [Header("References")]
    [SerializeField] private CameraController cameraController;
    [SerializeField] private GameObject centerPoint;
    [SerializeField] private Slider gridSlider;
    [SerializeField] private TextMeshProUGUI gridText;
    [SerializeField] private GradientDescent gradientDescent;

    public GameObject[,] plantGrid;
    public float[,] heightMap;
    public List<GameObject> axesNumbers = new List<GameObject>();
    public List<GameObject> markers = new List<GameObject>();
    
    private void Awake()
    {
        /*centerPoint.transform.position = new Vector3
        {
            x = gridSize*gridSpacing / 2,
            y = 0f,
            z = gridSize*gridSpacing / 2
        };*/
        gridSize = (int)gridSlider.value;
        gridText.text = gridSize.ToString();
    }

    private void Start()
    {
        GenerateMap();
    }
    
    
    private void Update()
    {
        /*foreach (TextMesh text in FindObjectsOfType<TextMesh>())
        {
            text.transform.rotation = Quaternion.LookRotation(text.transform.position - Camera.main.transform.position);
        }*/
        foreach (var number in axesNumbers)
        {
            number.transform.rotation = Quaternion.LookRotation(number.transform.position - Camera.main.transform.position);
        }
    }

    private void CleanupExistingMap()
    {
        gradientDescent.ClearGradientVisuals();
        
        // Destroy all existing plants
        if (plantGrid != null)
        {
            for (int x = 0; x < plantGrid.GetLength(0); x++)
            {
                for (int z = 0; z < plantGrid.GetLength(1); z++)
                {
                    if (plantGrid[x, z] != null)
                    {
                        Destroy(plantGrid[x, z]);
                    }
                }
            }
        }

        // Clear existing axes numbers and markers
        foreach (var number in axesNumbers)
        {
            if (number != null)
            {
                Destroy(number);
            }
        }
        axesNumbers.Clear();

        foreach (var marker in markers)
        {
            if (marker != null)
            {
                Destroy(marker);
            }
        }
        markers.Clear();
    }

    public void GenerateMap()
    {
        cameraController.target = centerPoint;
        // First, cleanup existing map
        CleanupExistingMap();
        gridSize = (int)gridSlider.value;
        GenerateHeightMap();
        GeneratePlantGrid();
        GenerateAxes();
        HighlightShortestPlant();
        centerPoint.transform.position = new Vector3
        {
            x = gridSize*gridSpacing / 2,
            y = 0f,
            z = gridSize*gridSpacing / 2
        };
    }


    private void GenerateHeightMap()
    {
        heightMap = new float[gridSize, gridSize];
        Random.InitState(seed);

        float scale = 0.2f; // Controls the frequency of the noise
        float amplitude = maxHeight - minHeight; // Controls the range of heights

        for (int x = 0; x < gridSize; x++)
        {
            for (int z = 0; z < gridSize; z++)
            {
                // Generate Perlin noise and map it to the desired height range
                float noiseValue = Mathf.PerlinNoise(x * scale, z * scale);
                heightMap[x, z] = minHeight + noiseValue * amplitude;
            }
        }
    }


    private void GeneratePlantGrid()
    {
        plantGrid = new GameObject[gridSize, gridSize];

        for (int x = 0; x < gridSize; x++)
        {
            for (int z = 0; z < gridSize; z++)
            {
                float height = heightMap[x, z];
                
                Vector3 position = new Vector3(x * gridSpacing, 0, z * gridSpacing);
                GameObject plant = Instantiate(plantPrefab, position, Quaternion.identity);
                
                plant.transform.localScale = new Vector3(1, height, 1);
                SetTransparency(plant, transparency);

                plantGrid[x, z] = plant;
            }
        }
    }
    
    private void GenerateAxes()
    {
        // Create X-axis
        for (int i = 0; i < gridSize; i++)
        {
            string axis = "X";
            Vector3 position = new Vector3(i * gridSpacing, 0, -gridSpacing);
            markers.Add(CreateAxisMarker(position, i.ToString(), Color.blue, axis));
            
            position = new Vector3(i * gridSpacing, 0, gridSize*gridSpacing);
            markers.Add(CreateAxisMarker(position, i.ToString(), Color.blue, axis));
            
        }

        // Create Z-axis
        for (int i = 0; i < gridSize; i++)
        {
            string axis = "Z";
            Vector3 position = new Vector3(-gridSpacing, 0, i * gridSpacing);
            markers.Add(CreateAxisMarker(position, i.ToString(), Color.yellow, axis));
            
            position = new Vector3(gridSize*gridSpacing, 0, i * gridSpacing);
            markers.Add(CreateAxisMarker(position, i.ToString(), Color.yellow, axis));
        }
    }

    private GameObject CreateAxisMarker(Vector3 position, string label, Color color, string axis)
    {
        GameObject marker = GameObject.CreatePrimitive(PrimitiveType.Plane);
        
        
        if (axis == "Z")
        {
            if (position.x < 0)
            {
                marker.transform.position = new Vector3(position.x + 5f, position.y, position.z);
            }
            else
            {
                marker.transform.position = new Vector3(position.x - 5f, position.y, position.z);
            }
            marker.transform.localScale = new Vector3(0.3f, 1, 0.1f); // Scale to span grid
        }
        else if (axis == "X")
        {
            if (position.z < 0)
            {
                marker.transform.position = new Vector3(position.x, position.y, position.z + 5f);
            }
            else
            {
                marker.transform.position = new Vector3(position.x, position.y, position.z - 5f);
            }
            marker.transform.localScale = new Vector3(0.1f, 1, 0.3f); // Scale to span grid
        }
        //marker.transform.localScale = new Vector3(0.1f, 1, gridSize / 10f); // Scale to span grid
        Renderer renderer = marker.GetComponent<Renderer>();
        Material material = axesPlaneMaterial;
        //material.color = new Color(color.r, color.g, color.b, 0.5f); // Semi-transparent
        renderer.material = material;

        // Add a label (optional)
        GameObject textObject = new GameObject(label);
        textObject.transform.position = position; // Offset for visibility
        //textObject.transform.position = new Vector3(position.x, position.y + 30f, position.z); // Offset for visibility
        TextMesh textMesh = textObject.AddComponent<TextMesh>();
        textMesh.text = label;
        textMesh.fontSize = 40;
        textMesh.color = color;
        textMesh.anchor = TextAnchor.MiddleCenter;
        axesNumbers.Add(textObject);

        return marker;
    }



    void HighlightShortestPlant()
    {
        int shortestX = 0; 
        int shortestZ = 0;
        float shortestHeight = float.MaxValue;

        for (int x = 0; x < gridSize; x++)
        {
            for (int z = 0; z < gridSize; z++)
            {
                float height = heightMap[x, z];
                if (height < shortestHeight)
                {
                    shortestHeight = height;
                    shortestX = x;
                    shortestZ = z;
                }
            }
        }
        
        

        Vector2 highestPlantCoor = new Vector2 (shortestX, shortestZ);
        shortestHeight -= 0.1f;
        plantGrid[(int)highestPlantCoor.x, (int)highestPlantCoor.y].transform.localScale = new Vector3(1, shortestHeight, 1);
        heightMap[shortestX, shortestZ] = shortestHeight;
        HighlightPlant(highestPlantCoor, highlightMaterial);
        
        // Add a label (optional)
        GameObject textObject = new GameObject("Shortest");
        textObject.transform.position = new Vector3(shortestX*gridSpacing, shortestHeight*15f, shortestZ*gridSpacing); // Offset for visibility
        //textObject.transform.position = new Vector3(position.x, position.y + 30f, position.z); // Offset for visibility
        TextMesh textMesh = textObject.AddComponent<TextMesh>();
        textMesh.text = shortestHeight.ToString();
        textMesh.fontSize = 40;
        textMesh.color = Color.cyan;
        textMesh.anchor = TextAnchor.MiddleCenter;
        axesNumbers.Add(textObject);
    }

    public void SetTransparency(GameObject obj, float alpha)
    {
        Renderer renderer = obj.GetComponent<Renderer>();
        if (renderer != null)
        {
            foreach (var mat in renderer.materials)
            {
                Color color = mat.color;
                color.a = alpha;
                mat.color = color;
            }
        }
    }

    public void HighlightPlant(Vector2 bestPosition, Material highlightMaterial)
    {
        int x = Mathf.RoundToInt(bestPosition.x);
        int z = Mathf.RoundToInt(bestPosition.y);

        GameObject bestPlant = plantGrid[x, z];
        Renderer bestRenderer = bestPlant.GetComponent<Renderer>();
        if (bestRenderer != null)
        {
            var materials = new List<Material>(bestRenderer.materials);
            if (materials.Count<=3)
            {
                materials.Add(highlightMaterial);
                bestRenderer.materials = materials.ToArray();
            }
        }
    }
    
    public void UnhighlightPlant(Vector2 bestPosition)
    {
        int x = Mathf.RoundToInt(bestPosition.x);
        int z = Mathf.RoundToInt(bestPosition.y);

        GameObject bestPlant = plantGrid[x, z];
        Renderer bestRenderer = bestPlant.GetComponent<Renderer>();
        if (bestRenderer != null)
        {
            var materials = new List<Material>(bestRenderer.materials);
            if (materials.Count>=3)
            {
                materials.RemoveAt(3);
                bestRenderer.materials = materials.ToArray();
            }
        }
        HighlightShortestPlant();
    }

    public void AdjustGridSizeSlider()
    {
        
        gridText.text = gridSlider.value.ToString();
    }
    
}