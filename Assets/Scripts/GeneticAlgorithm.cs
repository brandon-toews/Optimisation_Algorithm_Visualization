using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GeneticAlgorithm : MonoBehaviour
{
    [Header("Genetic Algorithm Settings")]
    [SerializeField] private int populationSize = 50;
    [SerializeField] private int generations = 100;
    [SerializeField] private float mutationRate = 0.1f;
    [SerializeField] private float mutationRange = 5f;
    [SerializeField] private float crossoverRate = 0.7f;

    [Header("Visualization")]
    [SerializeField] private GameObject individualPrefab;
    [SerializeField] private GameObject bestMarkerPrefab;
    [SerializeField] private bool showPopulation = true;
    [SerializeField] private float visualUpdateDelay = 0.2f;

    [Header("References")]
    [SerializeField] private TerrainGenerator terrain;

    private List<GameObject> populationObjects = new List<GameObject>();
    private List<GameObject> bestPositionMarkers = new List<GameObject>();
    private Vector3 bestPosition;
    private float bestFitness = float.MaxValue;
    private bool isSearching = false;

    public bool IsSearching => isSearching;

    private class Individual
    {
        public Vector2 position;
        public float fitness;

        public Individual(Vector2 pos, float fit)
        {
            position = pos;
            fitness = fit;
        }
    }

    private void Start()
    {
        if (terrain == null)
            terrain = FindFirstObjectByType<TerrainGenerator>();
    }

    public void StartGeneticSearch()
    {
        ClearBestPositionMarkers();
        if (!isSearching)
            StartCoroutine(GeneticSearchRoutine());
    }

    public void StepGeneticAlgorithm()
    {
        if (!isSearching)
            StartCoroutine(GeneticSearchStep());
    }

    public void ResetAlgorithm()
    {
        StopAllCoroutines();
        ClearVisualization();
        ClearBestPositionMarkers();
        isSearching = false;
    }

    public void SetVisualUpdateDelay(float delay)
    {
        visualUpdateDelay = delay;
    }

    private IEnumerator GeneticSearchRoutine()
    {
        isSearching = true;
        List<Individual> population = InitializePopulation();

        for (int gen = 0; gen < generations; gen++)
        {
            yield return GeneticSearchStepRoutine(population);
        }

        transform.position = bestPosition;
        isSearching = false;
    }

    private IEnumerator GeneticSearchStepRoutine(List<Individual> population)
    {
        EvaluatePopulation(population);

        Individual best = population.OrderBy(i => i.fitness).First();
        if (best.fitness < bestFitness)
        {
            bestFitness = best.fitness;
            bestPosition = new Vector3(best.position.x,
                terrain.GetHeightAtPoint(best.position.x, best.position.y) + 0.5f,
                best.position.y);
            MarkBestPosition(bestPosition);
        }

        if (showPopulation)
            VisualizePopulation(population);

        List<Individual> newPopulation = new List<Individual>();

        while (newPopulation.Count < populationSize)
        {
            Individual parent1 = SelectParent(population);
            Individual parent2 = SelectParent(population);

            if (Random.value < crossoverRate)
            {
                var (child1, child2) = Crossover(parent1, parent2);
                newPopulation.Add(child1);
                if (newPopulation.Count < populationSize)
                    newPopulation.Add(child2);
            }
            else
            {
                newPopulation.Add(parent1);
                if (newPopulation.Count < populationSize)
                    newPopulation.Add(parent2);
            }
        }

        for (int i = 0; i < newPopulation.Count; i++)
        {
            if (Random.value < mutationRate)
                Mutate(newPopulation[i]);
        }

        population.Clear();
        population.AddRange(newPopulation);

        yield return new WaitForSeconds(visualUpdateDelay);
    }

    private IEnumerator GeneticSearchStep()
    {
        isSearching = true;
        List<Individual> population = InitializePopulation();

        yield return GeneticSearchStepRoutine(population);

        transform.position = bestPosition;
        isSearching = false;
    }

    private List<Individual> InitializePopulation()
    {
        List<Individual> population = new List<Individual>();
        for (int i = 0; i < populationSize; i++)
        {
            Vector2 pos = new Vector2(
                Random.Range(0, terrain.Width),
                Random.Range(0, terrain.Length)
            );
            population.Add(new Individual(pos, float.MaxValue));
        }
        return population;
    }

    private void EvaluatePopulation(List<Individual> population)
    {
        foreach (var individual in population)
        {
            individual.fitness = terrain.GetHeightAtPoint(individual.position.x, individual.position.y);
        }
    }

    private Individual SelectParent(List<Individual> population)
    {
        int tournamentSize = 3;
        Individual best = null;
        float bestFitness = float.MaxValue;

        for (int i = 0; i < tournamentSize; i++)
        {
            Individual candidate = population[Random.Range(0, population.Count)];
            if (best == null || candidate.fitness < bestFitness)
            {
                best = candidate;
                bestFitness = candidate.fitness;
            }
        }

        return best;
    }

    private (Individual, Individual) Crossover(Individual parent1, Individual parent2)
    {
        float alpha = Random.value;
        Vector2 child1Pos = Vector2.Lerp(parent1.position, parent2.position, alpha);
        Vector2 child2Pos = Vector2.Lerp(parent1.position, parent2.position, 1 - alpha);

        return (
            new Individual(child1Pos, float.MaxValue),
            new Individual(child2Pos, float.MaxValue)
        );
    }

    private void Mutate(Individual individual)
    {
        individual.position += new Vector2(
            Random.Range(-mutationRange, mutationRange),
            Random.Range(-mutationRange, mutationRange)
        );

        individual.position.x = Mathf.Clamp(individual.position.x, 0, terrain.Width);
        individual.position.y = Mathf.Clamp(individual.position.y, 0, terrain.Length);
    }

    private void VisualizePopulation(List<Individual> population)
    {
        ClearVisualization();

        foreach (var individual in population)
        {
            GameObject obj = Instantiate(individualPrefab, transform);
            float height = terrain.GetHeightAtPoint(individual.position.x, individual.position.y);
            obj.transform.position = new Vector3(individual.position.x, height + 0.5f, individual.position.y);
            populationObjects.Add(obj);
        }
    }

    private void ClearVisualization()
    {
        foreach (var obj in populationObjects)
            Destroy(obj);
        populationObjects.Clear();
    }

    private void MarkBestPosition(Vector3 position)
    {
        if (bestMarkerPrefab != null)
        {
            GameObject marker = Instantiate(bestMarkerPrefab, position, Quaternion.identity);
            bestPositionMarkers.Add(marker);
        }
    }

    private void ClearBestPositionMarkers()
    {
        foreach (var marker in bestPositionMarkers)
            Destroy(marker);
        bestPositionMarkers.Clear();
    }
}
