using System;
using UnityEngine;

public class GeneratePlantGrid : MonoBehaviour
{
    [SerializeField] private GameObject plantPrefab;
    [SerializeField] private GameObject xLabelPrefab;
    [SerializeField] private GameObject zLabelPrefab;
    [SerializeField] private int gridSize = 11;
    [SerializeField] private float gridSpacing = 1.0f;
    [SerializeField] private float waterOpt = 5.0f;
    [SerializeField] private float sunlightOpt = 5.0f;
    [SerializeField] private float sigmaW = 2.0f;
    [SerializeField] private float sigmaS = 2.0f;

    private GameObject[,] plantGrid;
    private GameObject bestPlant;

    private void Start()
    {
        GenerateGrid();
        GenerateAxes();
    }

    void GenerateGrid()
    {
        plantGrid = new GameObject[gridSize, gridSize];

        for (int x = 0; x < gridSize; x++)
        {
            for (int z = 0; z < gridSize; z++)
            {
                float water = x;
                float sunlight = z;
                float height = CalculateHeight(water, sunlight, waterOpt, sunlightOpt, sigmaW, sigmaS);

                // Instantiate and scale the plant sprite
                Vector3 position = new Vector3(x * gridSpacing, 0, z * gridSpacing);
                GameObject plant = Instantiate(plantPrefab, position, Quaternion.identity);
                plant.transform.localScale = new Vector3(1, height * 10, 1); // Scale height
                plantGrid[x, z] = plant;
            }
        }
    }

    void GenerateAxes()
    {
        for (int i = 0; i < gridSize; i++)
        {
            // X-axis labels
            Vector3 xPosition = new Vector3(i * gridSpacing, 0, -1);
            Instantiate(xLabelPrefab, xPosition, Quaternion.identity).GetComponent<TextMesh>().text = i.ToString();

            // Z-axis labels
            Vector3 zPosition = new Vector3(-1, 0, i * gridSpacing);
            Instantiate(zLabelPrefab, zPosition, Quaternion.identity).GetComponent<TextMesh>().text = i.ToString();
        }
    }

    
    float CalculateHeight(float water, float sunlight, float waterOpt, float sunlightOpt, float sigmaW, float sigmaS)
    {
        float waterTerm = Mathf.Exp(-Mathf.Pow(water - waterOpt, 2) / (2 * sigmaW * sigmaW));
        float sunlightTerm = Mathf.Exp(-Mathf.Pow(sunlight - sunlightOpt, 2) / (2 * sigmaS * sigmaS));
        return waterTerm * sunlightTerm;
    }
    
    

    void HighlightBestSolution(Vector2 bestPosition)
    {
        if (bestPlant != null)
            bestPlant.GetComponent<Renderer>().material.color = Color.white;

        int x = Mathf.RoundToInt(bestPosition.x);
        int z = Mathf.RoundToInt(bestPosition.y);
        bestPlant = plantGrid[x, z];
        bestPlant.GetComponent<Renderer>().material.color = Color.green;
    }



}
