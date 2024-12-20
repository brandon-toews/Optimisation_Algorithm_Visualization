/*
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class AlgorithmVisualizer : MonoBehaviour
{
    [Header("Algorithm References")]
    [SerializeField] private GradientDescent gradientDescent;
    [SerializeField] private GeneticAlgorithm genetic;
    [SerializeField] private TerrainGenerator terrain;
    
    [Header("Trail Settings")]
    [SerializeField] private bool showTrails = true;
    [SerializeField] private Material trailMaterial;
    [SerializeField] private Color gradientDescentColor = Color.red;
    [SerializeField] private Color geneticColor = Color.blue;
    [SerializeField] private float trailDuration = 5f;
    
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI statsText;
    [SerializeField] private Image progressBar;
    [SerializeField] private LineRenderer heightGraph;
    [SerializeField] private int graphPoints = 100;
    
    private TrailRenderer gradientTrail;
    private TrailRenderer geneticTrail;
    private float startTime;
    private Vector3[] gradientPath;
    private Vector3[] geneticPath;
    private int pathIndex;
    
    private void Start()
    {
        SetupTrails();
        SetupHeightGraph();
    }
    
    public void StartComparison()
    {
        ResetVisualization();
        StartCoroutine(ComparisonRoutine());
    }
    
    private void SetupTrails()
    {
        if (!showTrails) return;
        
        // Setup gradient descent trail
        gradientTrail = gradientDescent.gameObject.AddComponent<TrailRenderer>();
        SetupTrail(gradientTrail, gradientDescentColor);
        
        // Setup genetic trail for best solution
        geneticTrail = genetic.gameObject.AddComponent<TrailRenderer>();
        SetupTrail(geneticTrail, geneticColor);
    }
    
    private void SetupTrail(TrailRenderer trail, Color color)
    {
        trail.material = trailMaterial;
        trail.startColor = color;
        trail.endColor = new Color(color.r, color.g, color.b, 0);
        trail.time = trailDuration;
        trail.minVertexDistance = 0.1f;
        trail.widthMultiplier = 0.5f;
    }
    
    private void SetupHeightGraph()
    {
        heightGraph.positionCount = graphPoints;
        heightGraph.startWidth = 0.1f;
        heightGraph.endWidth = 0.1f;
    }
    
    private void ResetVisualization()
    {
        if (showTrails)
        {
            gradientTrail.Clear();
            geneticTrail.Clear();
        }
        
        pathIndex = 0;
        startTime = Time.time;
        progressBar.fillAmount = 0;
        
        // Initialize path arrays
        gradientPath = new Vector3[1000]; // Adjust size as needed
        geneticPath = new Vector3[1000];
    }
    
    private IEnumerator ComparisonRoutine()
    {
        bool isRunning = true;
        float maxHeight = float.MinValue;
        float minHeight = float.MaxValue;
        
        while (isRunning)
        {
            // Record positions
            Vector3 gradientPos = gradientDescent.transform.position;
            Vector3 geneticPos = genetic.transform.position;
            
            gradientPath[pathIndex] = gradientPos;
            geneticPath[pathIndex] = geneticPos;
            pathIndex++;
            
            // Update height range
            maxHeight = Mathf.Max(maxHeight, gradientPos.y, geneticPos.y);
            minHeight = Mathf.Min(minHeight, gradientPos.y, geneticPos.y);
            
            // Update stats
            UpdateStats(gradientPos, geneticPos);
            
            // Update height graph
            UpdateHeightGraph(minHeight, maxHeight);
            
            // Update progress
            float gradientProgress = gradientDescent.IsDescending ? 0 : 1;
            float geneticProgress = genetic.IsSearching ? 0 : 1;
            progressBar.fillAmount = (gradientProgress + geneticProgress) / 2;
            
            // Check if both algorithms are done
            isRunning = gradientDescent.IsDescending || genetic.IsSearching;
            
            yield return new WaitForSeconds(0.02f);
        }
        
        // Final update
        UpdateStats(gradientDescent.transform.position, genetic.transform.position, true);
    }
    
    private void UpdateStats(Vector3 gradientPos, Vector3 geneticPos, bool final = false)
    {
        float timeElapsed = Time.time - startTime;
        float gradientHeight = gradientPos.y;
        float geneticHeight = geneticPos.y;
        float heightDifference = Mathf.Abs(gradientHeight - geneticHeight);
        
        string status = final ? "Complete" : "Running";
        
        statsText.text = $"Algorithm Comparison - {status}\n\n" +
            $"Time: {timeElapsed:F1}s\n\n" +
            $"Gradient Descent Height: {gradientHeight:F2}\n" +
            $"Genetic Algorithm Height: {geneticHeight:F2}\n" +
            $"Height Difference: {heightDifference:F2}\n\n" +
            $"Gradient Position: ({gradientPos.x:F1}, {gradientPos.z:F1})\n" +
            $"Genetic Position: ({geneticPos.x:F1}, {geneticPos.z:F1})";
    }
    
    private void UpdateHeightGraph(float minHeight, float maxHeight)
    {
        if (pathIndex < 2) return;
        
        // Calculate positions for height graph
        Vector3[] positions = new Vector3[graphPoints];
        float step = (float)(pathIndex - 1) / (graphPoints - 1);
        
        for (int i = 0; i < graphPoints; i++)
        {
            float index = i * step;
            int lowIndex = Mathf.FloorToInt(index);
            int highIndex = Mathf.CeilToInt(index);
            float t = index - lowIndex;
            
            // Interpolate between recorded positions
            float gradientHeight = Mathf.Lerp(
                gradientPath[lowIndex].y,
                gradientPath[highIndex].y,
                t);
            
            float geneticHeight = Mathf.Lerp(
                geneticPath[lowIndex].y,
                geneticPath[highIndex].y,
                t);
            
            // Normalize heights to graph space
            float normalizedGradient = (gradientHeight - minHeight) / (maxHeight - minHeight);
            float normalizedGenetic = (geneticHeight - minHeight) / (maxHeight - minHeight);
            
            // Set graph positions
            positions[i] = new Vector3(i / (float)(graphPoints - 1), normalizedGradient, 0);
        }
        
        heightGraph.SetPositions(positions);
    }
}
*/
