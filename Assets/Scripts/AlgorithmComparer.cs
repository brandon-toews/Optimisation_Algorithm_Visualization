using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class AlgorithmComparer : MonoBehaviour
{
    [Header("Algorithm References")]
    [SerializeField] private GeneticAlgorithm geneticAlgorithm;
    [SerializeField] private GradientDescent gradientDescent;
    
    [Header("UI Elements")]
    [SerializeField] private Button startButton;
    [SerializeField] private Button resetButton;
    [SerializeField] private TextMeshProUGUI statusText;

    private bool isRunning = false;

    private void Start()
    {
        // Setup UI listeners
        if (startButton != null)
            startButton.onClick.AddListener(StartComparison);
        if (resetButton != null)
            resetButton.onClick.AddListener(ResetComparison);
            
        // Initial setup
        ResetComparison();
    }

    public void StartComparison()
    {
        if (!isRunning)
        {
            isRunning = true;
            UpdateUI();
            
            // Place GD at random position and start both algorithms
            gradientDescent.RandomlyPlace();
            gradientDescent.StartGradientDescent();
            geneticAlgorithm.StartGeneticSearch();
            
            // Start monitoring progress
            StartCoroutine(MonitorProgress());
        }
    }

    public void ResetComparison()
    {
        isRunning = false;
        
        // Reset both algorithms to starting positions
        geneticAlgorithm.transform.position = Vector3.zero;
        gradientDescent.transform.position = Vector3.zero;
        
        // Clear any existing GA population visualization
        geneticAlgorithm.SendMessage("ClearVisualization", SendMessageOptions.DontRequireReceiver);
        
        UpdateUI();
    }

    private IEnumerator MonitorProgress()
    {
        while (isRunning)
        {
            // Check if both algorithms have finished
            if (!geneticAlgorithm.IsSearching && !gradientDescent.IsDescending)
            {
                isRunning = false;
                UpdateUI();
                yield break;
            }

            // Update status text
            if (statusText != null)
            {
                string status = "Running:\n";
                status += $"Genetic Algorithm: {(geneticAlgorithm.IsSearching ? "Searching" : "Complete")}\n";
                status += $"Gradient Descent: {(gradientDescent.IsDescending ? "Descending" : "Complete")}";
                statusText.text = status;
            }

            yield return new WaitForSeconds(0.1f);
        }
    }

    private void UpdateUI()
    {
        if (startButton != null)
            startButton.interactable = !isRunning;
        if (resetButton != null)
            resetButton.interactable = !isRunning;
        if (statusText != null)
            statusText.text = isRunning ? "Running..." : "Ready";
    }

    private void OnValidate()
    {
        // Auto-find references if not set
        if (geneticAlgorithm == null)
            geneticAlgorithm = FindFirstObjectByType<GeneticAlgorithm>();
        if (gradientDescent == null)
            gradientDescent = FindFirstObjectByType<GradientDescent>();
    }
}