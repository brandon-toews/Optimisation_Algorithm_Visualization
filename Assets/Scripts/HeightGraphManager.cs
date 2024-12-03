using UnityEngine;
using UnityEngine.UI;

public class HeightGraphManager : MonoBehaviour
{
    [Header("Graph Settings")]
    [SerializeField] private int graphWidth = 500;
    [SerializeField] private int graphHeight = 300;
    [SerializeField] private Color gradientDescentColor = Color.red;
    [SerializeField] private Color geneticColor = Color.blue;
    
    [Header("References")]
    [SerializeField] private RectTransform graphContainer;
    [SerializeField] private GameObject linePointPrefab;
    
    private readonly int maxDataPoints = 100;
    private Vector2[] gradientDescentPoints;
    private Vector2[] geneticPoints;
    private GameObject[] gradientDescentDots;
    private GameObject[] geneticDots;
    
    private void Start()
    {
        InitializeArrays();
        CreateGraphBackground();
    }
    
    private void InitializeArrays()
    {
        gradientDescentPoints = new Vector2[maxDataPoints];
        geneticPoints = new Vector2[maxDataPoints];
        gradientDescentDots = new GameObject[maxDataPoints];
        geneticDots = new GameObject[maxDataPoints];
    }
    
    private void CreateGraphBackground()
    {
        // Create graph container if not assigned
        if (graphContainer == null)
        {
            GameObject containerObj = new GameObject("GraphContainer", typeof(RectTransform));
            containerObj.transform.SetParent(transform);
            graphContainer = containerObj.GetComponent<RectTransform>();
        }
        
        // Set size
        graphContainer.sizeDelta = new Vector2(graphWidth, graphHeight);
        
        // Create background
        GameObject backgroundObj = new GameObject("Background", typeof(Image));
        backgroundObj.transform.SetParent(graphContainer);
        Image background = backgroundObj.GetComponent<Image>();
        background.color = new Color(0.2f, 0.2f, 0.2f, 1f);
        RectTransform backgroundRect = backgroundObj.GetComponent<RectTransform>();
        backgroundRect.anchorMin = Vector2.zero;
        backgroundRect.anchorMax = Vector2.one;
        backgroundRect.sizeDelta = Vector2.zero;
        
        // Create grid lines
        CreateGridLines();
    }
    
    private void CreateGridLines()
    {
        int gridLines = 4;
        
        for (int i = 1; i < gridLines; i++)
        {
            // Horizontal lines
            CreateGridLine(true, i / (float)gridLines);
            // Vertical lines
            CreateGridLine(false, i / (float)gridLines);
        }
    }
    
    private void CreateGridLine(bool horizontal, float position)
    {
        GameObject lineObj = new GameObject("GridLine", typeof(Image));
        lineObj.transform.SetParent(graphContainer);
        
        Image line = lineObj.GetComponent<Image>();
        line.color = new Color(1, 1, 1, 0.2f);
        
        RectTransform rectTransform = lineObj.GetComponent<RectTransform>();
        if (horizontal)
        {
            rectTransform.anchorMin = new Vector2(0, position);
            rectTransform.anchorMax = new Vector2(1, position);
            rectTransform.sizeDelta = new Vector2(0, 2);
        }
        else
        {
            rectTransform.anchorMin = new Vector2(position, 0);
            rectTransform.anchorMax = new Vector2(position, 1);
            rectTransform.sizeDelta = new Vector2(2, 0);
        }
    }
    
    public void UpdateGraph(float gradientHeight, float geneticHeight, float maxHeight, float minHeight)
    {
        // Shift existing points left
        ShiftPoints();
        
        // Add new points
        float normalizedGradient = NormalizeHeight(gradientHeight, minHeight, maxHeight);
        float normalizedGenetic = NormalizeHeight(geneticHeight, minHeight, maxHeight);
        
        gradientDescentPoints[maxDataPoints - 1] = new Vector2(graphWidth, normalizedGradient * graphHeight);
        geneticPoints[maxDataPoints - 1] = new Vector2(graphWidth, normalizedGenetic * graphHeight);
        
        // Update visuals
        UpdateVisuals();
    }
    
    private float NormalizeHeight(float height, float min, float max)
    {
        return Mathf.Clamp01((height - min) / (max - min));
    }
    
    private void ShiftPoints()
    {
        float shiftAmount = graphWidth / (maxDataPoints - 1);
        
        for (int i = 0; i < maxDataPoints - 1; i++)
        {
            gradientDescentPoints[i] = gradientDescentPoints[i + 1];
            geneticPoints[i] = geneticPoints[i + 1];
            
            gradientDescentPoints[i].x -= shiftAmount;
            geneticPoints[i].x -= shiftAmount;
        }
    }
    
    private void UpdateVisuals()
    {
        // Update or create point visualizations
        for (int i = 0; i < maxDataPoints; i++)
        {
            UpdatePointVisual(ref gradientDescentDots[i], gradientDescentPoints[i], gradientDescentColor);
            UpdatePointVisual(ref geneticDots[i], geneticPoints[i], geneticColor);
        }
        
        // Draw lines between points
        DrawLines();
    }
    
    private void UpdatePointVisual(ref GameObject dot, Vector2 position, Color color)
    {
        if (dot == null)
        {
            dot = Instantiate(linePointPrefab, graphContainer);
            dot.GetComponent<Image>().color = color;
        }
        
        dot.GetComponent<RectTransform>().anchoredPosition = position;
    }
    
    private void DrawLines()
    {
        // Draw lines using LineRenderer or UI.Line components
        // Implementation depends on your preferred visualization method
    }
}
