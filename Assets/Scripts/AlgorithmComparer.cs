/*using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class AlgorithmComparer : MonoBehaviour
{
    [Header("Algorithm References")]
    [SerializeField] private GeneticAlgorithm geneticAlgorithm;
    [SerializeField] private GradientDescent gradientDescent;
    
    [Header("UI Elements")]
    [SerializeField] private Button startGAButton;
    [SerializeField] private Button startGDButton;
    [SerializeField] private Button resetButton;
    [SerializeField] private Button stepGAButton;
    [SerializeField] private Button stepGDButton;
    [SerializeField] private Slider speedControlSlider;
    [SerializeField] private TextMeshProUGUI statusText;

    private bool isRunning = false;

    private void Start()
    {
        // Setup UI listeners
        if (startGAButton != null)
            startGAButton.onClick.AddListener(StartGeneticAlgorithmOnly);
        if (startGDButton != null)
            startGDButton.onClick.AddListener(StartGradientDescentOnly);
        if (resetButton != null)
            resetButton.onClick.AddListener(ResetComparison);
        if (stepGAButton != null)
            stepGAButton.onClick.AddListener(StepGeneticAlgorithm);
        if (stepGDButton != null)
            stepGDButton.onClick.AddListener(StepGradientDescent);
        if (speedControlSlider != null)
            speedControlSlider.onValueChanged.AddListener(UpdateSpeed);

        ResetComparison();
    }

    public void StartGeneticAlgorithmOnly()
    {
        if (!isRunning)
        {
            isRunning = true;
            UpdateUI();

            geneticAlgorithm.StartGeneticSearch();
            StartCoroutine(MonitorProgress());
        }
    }

    public void StartGradientDescentOnly()
    {
        if (!isRunning)
        {
            isRunning = true;
            UpdateUI();

            gradientDescent.RandomlyPlace();
            gradientDescent.StartGradientDescent();
            StartCoroutine(MonitorProgress());
        }
    }

    public void StepGeneticAlgorithm()
    {
        if (!isRunning)
            geneticAlgorithm.StepGeneticAlgorithm();
    }

    public void StepGradientDescent()
    {
        if (!isRunning)
            gradientDescent.StepGradientDescent();
    }

    public void ResetComparison()
    {
        isRunning = false;

        geneticAlgorithm.ResetAlgorithm();
        gradientDescent.ResetAlgorithm();

        UpdateUI();
    }

    private void UpdateSpeed(float value)
    {
        geneticAlgorithm.SetVisualUpdateDelay(value);
        gradientDescent.SetMoveDelay(value);
    }

    private IEnumerator MonitorProgress()
    {
        while (isRunning)
        {
            if (!geneticAlgorithm.IsSearching && !gradientDescent.IsDescending)
            {
                isRunning = false;
                UpdateUI();
                yield break;
            }

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
        bool enableStartButtons = !isRunning;
        if (startGAButton != null)
            startGAButton.interactable = enableStartButtons;
        if (startGDButton != null)
            startGDButton.interactable = enableStartButtons;
        if (resetButton != null)
            resetButton.interactable = enableStartButtons;
        if (stepGAButton != null)
            stepGAButton.interactable = enableStartButtons;
        if (stepGDButton != null)
            stepGDButton.interactable = enableStartButtons;

        if (statusText != null)
            statusText.text = isRunning ? "Running..." : "Ready";
    }
}*/