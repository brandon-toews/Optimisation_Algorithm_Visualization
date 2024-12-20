using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GradientDescent : MonoBehaviour
{
    [Header("Algorithm Settings")]
    [SerializeField] private float learningRate = 0.25f;
    [SerializeField] private float stepSize = 1.0f;
    [SerializeField] private int maxSteps = 100;
    
    [Header("Arrow Visualization")]
    [SerializeField] private GameObject waterArrowPrefab; // Prefab for arrows

    [SerializeField] private GameObject sunArrowPrefab;
    private GameObject xArrow;
    private GameObject zArrow;
    private GameObject xGradientText;
    private GameObject zGradientText;



    [Header("References")]
    [SerializeField] private PlantGrowthSimulation plantGrowthSimulation;
    //[SerializeField] private Text currentHeightText;
    [SerializeField] private TextMeshProUGUI gradientText;
    [SerializeField] private Material highlightMaterial;
    [SerializeField] private Slider learningRateSlider;
    [SerializeField] private TextMeshProUGUI learningRateText;
    [SerializeField] private Material planeMaterial;
    [SerializeField] private CameraController cameraController;
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;

    private Vector2 currentPosition;
    private int steps = 0;
    private bool isRunning = false;
    
    private GameObject plane;

    private void Awake()
    {
        learningRate = learningRateSlider.value;
    }

    private void ClearPlane()
    {
        if (meshFilter != null)
        {
            meshFilter.mesh = null;
        }
    }

    public void StartAlgorithm()
    {
        ClearPlane();
        // Choose a random starting plant
        int randomX = Random.Range(0, plantGrowthSimulation.gridSize-1);
        int randomZ = Random.Range(0, plantGrowthSimulation.gridSize-1);
        currentPosition = new Vector2(randomX, randomZ);
        
        cameraController.target = plantGrowthSimulation.plantGrid[randomX, randomZ];

        // Highlight the starting plant
        plantGrowthSimulation.HighlightPlant(currentPosition, highlightMaterial);
        CreatePlane();
        UpdateHeightAndGradientDisplay();
        steps = 0;
        isRunning = true;
    }

    public void StepAlgorithm()
    {
        if (!isRunning || steps >= maxSteps)
            return;

        Vector2 gradient = CalculateGradient(currentPosition);
        
        // Compute a tentative float position using gradient descent formula
        float nextX = currentPosition.x - learningRate * gradient.x;
        float nextZ = currentPosition.y - learningRate * gradient.y;

        // Snap the float position to the nearest grid point
        int snappedX = Mathf.Clamp(Mathf.RoundToInt(nextX), 0, plantGrowthSimulation.gridSize - 1);
        int snappedZ = Mathf.Clamp(Mathf.RoundToInt(nextZ), 0, plantGrowthSimulation.gridSize - 1);
        
        cameraController.target = plantGrowthSimulation.plantGrid[snappedX, snappedZ];

        // Move to the snapped position
        Vector2 nextPosition = new Vector2(snappedX, snappedZ);
        
        plantGrowthSimulation.SetTransparency(plantGrowthSimulation.plantGrid[(int)currentPosition.x, (int)currentPosition.y], 0.3f);
        currentPosition = nextPosition;
        plantGrowthSimulation.HighlightPlant(currentPosition, highlightMaterial);
        plantGrowthSimulation.SetTransparency(plantGrowthSimulation.plantGrid[(int)currentPosition.x, (int)currentPosition.y], 1);
        UpdateHeightAndGradientDisplay();
        steps++;

        if (steps >= maxSteps)
        {
            isRunning = false;
            Debug.Log("Gradient Descent completed.");
        }
    }

    private Vector2 CalculateGradient(Vector2 position)
    {
        int x = Mathf.RoundToInt(position.x);
        int z = Mathf.RoundToInt(position.y);

        float gradX = 0, gradZ = 0;

        // Gradient in X direction (central difference)
        if (x + 1 < plantGrowthSimulation.gridSize && x - 1 >= 0)
        {
            gradX = (plantGrowthSimulation.heightMap[x + 1, z] - plantGrowthSimulation.heightMap[x - 1, z]) / (2 * stepSize);
        }
        // Handle edge cases using forward or backward difference
        else if (x + 1 < plantGrowthSimulation.gridSize)
        {
            gradX = (plantGrowthSimulation.heightMap[x + 1, z] - plantGrowthSimulation.heightMap[x, z]) / stepSize;
        }
        else if (x - 1 >= 0)
        {
            gradX = (plantGrowthSimulation.heightMap[x, z] - plantGrowthSimulation.heightMap[x - 1, z]) / stepSize;
        }

        // Gradient in Z direction (central difference)
        if (z + 1 < plantGrowthSimulation.gridSize && z - 1 >= 0)
        {
            gradZ = (plantGrowthSimulation.heightMap[x, z + 1] - plantGrowthSimulation.heightMap[x, z - 1]) / (2 * stepSize);
        }
        // Handle edge cases using forward or backward difference
        else if (z + 1 < plantGrowthSimulation.gridSize)
        {
            gradZ = (plantGrowthSimulation.heightMap[x, z + 1] - plantGrowthSimulation.heightMap[x, z]) / stepSize;
        }
        else if (z - 1 >= 0)
        {
            gradZ = (plantGrowthSimulation.heightMap[x, z] - plantGrowthSimulation.heightMap[x, z - 1]) / stepSize;
        }

        Vector2 gradient = new Vector2(gradX * 10f, gradZ * 10f);

        // Update the visualization
        Vector3 worldPosition = new Vector3(
            x * plantGrowthSimulation.gridSpacing, 
            0, 
            z * plantGrowthSimulation.gridSpacing
        );
        
        UpdateVisualizationPlane(currentPosition);
        UpdateArrows(currentPosition, gradient);

        return gradient;
    }


    private void UpdateHeightAndGradientDisplay()
    {
        int x = Mathf.RoundToInt(currentPosition.x);
        int z = Mathf.RoundToInt(currentPosition.y);
        float height = plantGrowthSimulation.heightMap[x, z];
        Vector2 gradient = CalculateGradient(currentPosition);
        
        GameObject textObject = new GameObject("Current Position");
        textObject.transform.position = new Vector3(x*plantGrowthSimulation.gridSpacing, height*17f, z*plantGrowthSimulation.gridSpacing); // Offset for visibility
        TextMesh textMesh = textObject.AddComponent<TextMesh>();
        textMesh.text = height.ToString();
        textMesh.fontSize = 40;
        textMesh.color = Color.cyan;
        textMesh.anchor = TextAnchor.MiddleCenter;
        plantGrowthSimulation.axesNumbers.Add(textObject);
        
        gradientText.text = $"Gradients: \nWater(X): {gradient.x:F2} \nSunshine(Z): {gradient.y:F2}";
    }
    
    private void CreatePlane()
    {
        int x = (int)currentPosition.x;
        int z = (int)currentPosition.y;
        float height = plantGrowthSimulation.heightMap[x, z];
        plane = new GameObject("SlopePlane");
        meshFilter = plane.AddComponent<MeshFilter>();
        meshRenderer = plane.AddComponent<MeshRenderer>();
        meshRenderer.material = planeMaterial;
    }
    
    public void UpdateVisualizationPlane(Vector2 gridPosition)
    {
        int x = Mathf.RoundToInt(gridPosition.x);
        int z = Mathf.RoundToInt(gridPosition.y);
        float spacing = plantGrowthSimulation.gridSpacing;

        // Get the four corners of our plane
        Vector3[] corners = new Vector3[4];
        
        // Calculate actual world positions including height (excluding center point)
        //corners[0] = new Vector3((x+1) * spacing, plantGrowthSimulation.heightMap[Mathf.Min(x+1, plantGrowthSimulation.gridSize-1), z]*15f, z * spacing); // Right
        corners[0] = new Vector3((x+1) * spacing, plantGrowthSimulation.heightMap[Mathf.Min(x+1, plantGrowthSimulation.gridSize-1), Mathf.Min(z+1, plantGrowthSimulation.gridSize-1)]*15f, (z+1) * spacing); // Forward-Right
        //corners[2] = new Vector3(x * spacing, plantGrowthSimulation.heightMap[x, Mathf.Min(z+1, plantGrowthSimulation.gridSize-1)]*15f, (z+1) * spacing); // Forward
        corners[1] = new Vector3((x-1) * spacing, plantGrowthSimulation.heightMap[Mathf.Max(x-1, 0), Mathf.Min(z+1, plantGrowthSimulation.gridSize-1)]*15f, (z+1) * spacing); // Forward-Left
        //corners[4] = new Vector3((x-1) * spacing, plantGrowthSimulation.heightMap[Mathf.Max(x-1, 0), z]*15f, z * spacing); // Left
        corners[2] = new Vector3((x-1) * spacing, plantGrowthSimulation.heightMap[Mathf.Max(x-1, 0), Mathf.Max(z-1, 0)]*15f, (z-1) * spacing); // Backward-Left
        //corners[6] = new Vector3(x * spacing, plantGrowthSimulation.heightMap[x, Mathf.Max(z-1, 0)]*15f, (z-1) * spacing); // Backward
        corners[3] = new Vector3((x+1) * spacing, plantGrowthSimulation.heightMap[Mathf.Min(x+1, plantGrowthSimulation.gridSize-1), Mathf.Max(z-1, 0)]*15f, (z-1) * spacing); // Backward-Right

        // Create mesh from these points
        Mesh mesh = new Mesh();
        mesh.vertices = corners;
        // Define triangles to connect the outer vertices
        mesh.triangles = new int[] { 0, 3, 2, 2, 1, 0 };// { 0, 1, 2, 2, 3, 0 };
        mesh.RecalculateNormals();
        
        meshFilter.mesh = mesh;
        transform.position = corners[0];
    }

    public void ClearGradientVisuals()
    {
        ClearPlane();
        // Remove previous arrows
        if (xArrow != null) Destroy(xArrow);
        if (zArrow != null) Destroy(zArrow);
        if (xGradientText != null) Destroy(xGradientText);
        if (zGradientText != null) Destroy(zGradientText);
    }
    
    private void UpdateArrows(Vector2 position, Vector2 gradient)
    {
        int x = Mathf.RoundToInt(position.x);
        int z = Mathf.RoundToInt(position.y);
        float height = plantGrowthSimulation.heightMap[x, z]*15f;
        float spacing = plantGrowthSimulation.gridSpacing;

        // Remove previous arrows
        if (xArrow != null) Destroy(xArrow);
        if (zArrow != null) Destroy(zArrow);
        if (xGradientText != null) Destroy(xGradientText);
        if (zGradientText != null) Destroy(zGradientText);

        // Create X-axis arrow
        xArrow = Instantiate(waterArrowPrefab);
        float xHeight = ((plantGrowthSimulation.heightMap[Mathf.Max(x-1, 0), Mathf.Max(z-1, 0)] * 14f) +
                         (plantGrowthSimulation.heightMap[Mathf.Min(x+1, plantGrowthSimulation.gridSize-1), Mathf.Max(z-1, 0)] * 14f))/2;
        xArrow.transform.position = new Vector3(x * spacing, xHeight, Mathf.Max(z-1, 0) * spacing);
        xArrow.transform.localScale = new Vector3(0.5f, 0.5f, 1f);
        Vector3 xArrowLookPos = plantGrowthSimulation.plantGrid[Mathf.Min(x+1, plantGrowthSimulation.gridSize-1), Mathf.Max(z-1, 0)].transform.position;
        xArrowLookPos.y = plantGrowthSimulation.heightMap[Mathf.Min(x+1, plantGrowthSimulation.gridSize-1), Mathf.Max(z-1, 0)]*14f;
        if (xArrowLookPos.x == xArrow.transform.position.x)
        {
            xArrow.transform.position = new Vector3((x - 1) * spacing, plantGrowthSimulation.heightMap[(x - 1), (int)(xArrow.transform.position.z/spacing)] * 14f, xArrow.transform.position.z);
        }
        xArrow.transform.rotation = Quaternion.LookRotation(xArrowLookPos - xArrow.transform.position);
        
        // Gradient text for X-axis
        xGradientText = new GameObject("X Gradient Text");
        xGradientText.transform.rotation = xArrow.transform.rotation;
        xGradientText.transform.Rotate(Vector3.up, -90);
        xGradientText.transform.position = xArrow.transform.position + Vector3.up * -3f; // Position above arrow
        TextMesh xTextMesh = xGradientText.AddComponent<TextMesh>();
        xTextMesh.text = $"X: {gradient.x:F2}";
        xTextMesh.fontSize = 30;
        xTextMesh.color = Color.blue;
        xTextMesh.anchor = TextAnchor.MiddleCenter;

        // Create Z-axis arrow
        zArrow = Instantiate(sunArrowPrefab);
        float zHeight = ((plantGrowthSimulation.heightMap[Mathf.Max(x-1, 0), Mathf.Max(z-1, 0)] * 14f) +
                         (plantGrowthSimulation.heightMap[Mathf.Max(x-1, 0), Mathf.Min(z+1, plantGrowthSimulation.gridSize-1)] * 14f))/2;
        zArrow.transform.position = new Vector3((Mathf.Max(x-1, 0)) * spacing, zHeight, z * spacing);
        zArrow.transform.localScale = new Vector3(0.5f, 0.5f, 1f);
        Vector3 zArrowLookPos = plantGrowthSimulation.plantGrid[Mathf.Max(x-1, 0), Mathf.Min(z+1, plantGrowthSimulation.gridSize-1)].transform.position;
        zArrowLookPos.y = plantGrowthSimulation.heightMap[Mathf.Max(x-1, 0), Mathf.Min(z+1, plantGrowthSimulation.gridSize-1)]*14f;
        if (zArrowLookPos.z == zArrow.transform.position.z)
        {
            zArrow.transform.position = new Vector3(zArrow.transform.position.x, plantGrowthSimulation.heightMap[(int)
                (zArrow.transform.position.x/spacing), (z - 1)] * 14f, (z - 1) * spacing);
        }
        zArrow.transform.rotation = Quaternion.LookRotation(zArrowLookPos - zArrow.transform.position);
        
        // Gradient text for Z-axis
        zGradientText = new GameObject("Z Gradient Text");
        zGradientText.transform.rotation = zArrow.transform.rotation;
        zGradientText.transform.Rotate(Vector3.up, 90);
        zGradientText.transform.position = zArrow.transform.position + Vector3.up * -3f; // Position above arrow
        TextMesh zTextMesh = zGradientText.AddComponent<TextMesh>();
        zTextMesh.text = $"Z: {gradient.y:F2}";
        zTextMesh.fontSize = 30;
        zTextMesh.color = Color.yellow;
        zTextMesh.anchor = TextAnchor.MiddleCenter;
    }


    public void AdjustLearningRate()
    {
        learningRate = (float)Math.Round(learningRateSlider.value, 2);
        learningRateText.text = $"{learningRate}";
    }
}
