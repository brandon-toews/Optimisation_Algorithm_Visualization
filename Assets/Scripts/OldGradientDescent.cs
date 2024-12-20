using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OldGradientDescent : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float learningRate = 0.5f;
    [SerializeField] private float stepSize = 0.1f;
    [SerializeField] private float stopThreshold = 0.01f;
    [SerializeField] private int maxSteps = 1000;
    [SerializeField] private float moveDelay = 0.1f;

    [Header("Visualization")]
    [SerializeField] private GameObject gradientDescentPrefab;
    [SerializeField] private GameObject pathMarkerPrefab;
    [SerializeField] private Material lineMaterial; // Line material for drawing
    [SerializeField] private int markerInterval = 5;

    [Header("References")]
    [SerializeField] private TerrainGenerator terrain;

    private Vector3 currentPosition;
    private bool isDescending = false;
    private List<GameObject> pathMarkers = new List<GameObject>();
    private GameObject gradientDescentInstance;

    private int steps = 0;
    private float previousHeight = float.MaxValue;

    public bool IsDescending => isDescending;

    private void Start()
    {
        if (terrain == null)
            terrain = FindFirstObjectByType<TerrainGenerator>();
    }

    public void RandomlyPlace()
    {
        ClearPathMarkers();

        float x = Random.Range(0, terrain.Width);
        float z = Random.Range(0, terrain.Length);
        float y = terrain.GetHeightAtPoint(x, z);

        transform.position = new Vector3(x, y + 1f, z);
        currentPosition = transform.position;

        InstantiateGradientDescentPrefab();
        PlacePathMarker(currentPosition);
    }

    public void StartGradientDescent()
    {
        if (!isDescending)
        {
            isDescending = true;
            steps = 0;
            previousHeight = float.MaxValue;
            StartCoroutine(GradientDescentRoutine());
        }
        
        if (lineMaterial == null)
            return;

        lineMaterial.SetPass(0);
        GL.Begin(GL.LINES);
        GL.Color(Color.blue);
        GL.Vertex3(currentPosition.x, currentPosition.y, currentPosition.z);
        GL.Vertex3(currentPosition.x, currentPosition.y + 10f, currentPosition.z);
        GL.End();
    }

    public void StepGradientDescent()
    {
        if (!isDescending)
        {
            isDescending = true;
            GradientDescentStepRoutine();
            if (Mathf.Abs(previousHeight - currentPosition.y) < stopThreshold || steps >= maxSteps)
                isDescending = false;
        }
    }

    public void ResetAlgorithm()
    {
        StopAllCoroutines();
        ClearPathMarkers();
        Destroy(gradientDescentInstance);
        isDescending = false;
    }

    public void SetMoveDelay(float delay)
    {
        moveDelay = delay;
    }

    private IEnumerator GradientDescentRoutine()
    {
        while (steps < maxSteps)
        {
            GradientDescentStepRoutine();

            if (Mathf.Abs(previousHeight - currentPosition.y) < stopThreshold)
                break;

            yield return new WaitForSeconds(moveDelay);
        }

        PlacePathMarker(currentPosition);
        isDescending = false;
    }

    private void GradientDescentStepRoutine()
    {
        float centerHeight = terrain.GetHeightAtPoint(currentPosition.x, currentPosition.z);
        float gradX = (terrain.GetHeightAtPoint(currentPosition.x + stepSize, currentPosition.z) -
                      terrain.GetHeightAtPoint(currentPosition.x - stepSize, currentPosition.z)) / (2 * stepSize);
        float gradZ = (terrain.GetHeightAtPoint(currentPosition.x, currentPosition.z + stepSize) -
                      terrain.GetHeightAtPoint(currentPosition.x, currentPosition.z - stepSize)) / (2 * stepSize);

        Vector3 gradient = new Vector3(gradX, 0, gradZ);
        Vector3 newPosition = currentPosition - learningRate * gradient;

        newPosition.x = Mathf.Clamp(newPosition.x, 0, terrain.Width);
        newPosition.z = Mathf.Clamp(newPosition.z, 0, terrain.Length);

        float newHeight = terrain.GetHeightAtPoint(newPosition.x, newPosition.z);
        newPosition.y = newHeight + 0.5f;

        currentPosition = newPosition;
        transform.position = currentPosition;

        if (gradientDescentInstance != null)
            gradientDescentInstance.transform.position = currentPosition;

        if (steps % markerInterval == 0)
            PlacePathMarker(currentPosition);

        previousHeight = newHeight;
        steps++;
    }

    private void PlacePathMarker(Vector3 position)
    {
        if (pathMarkerPrefab != null)
        {
            GameObject marker = Instantiate(pathMarkerPrefab, position, Quaternion.identity);
            pathMarkers.Add(marker);
        }
    }

    private void ClearPathMarkers()
    {
        foreach (var marker in pathMarkers)
            Destroy(marker);

        pathMarkers.Clear();
    }

    private void InstantiateGradientDescentPrefab()
    {
        if (gradientDescentPrefab != null)
        {
            Destroy(gradientDescentInstance);
            gradientDescentInstance = Instantiate(gradientDescentPrefab, currentPosition, Quaternion.identity);
        }
    }

    private void OnPostRender()
    {
        if (lineMaterial == null)
        {
            Debug.LogWarning("Line material is not set!");
            return;
        }

        Debug.Log($"Drawing line at position {currentPosition}");
        lineMaterial.SetPass(0);
        GL.Begin(GL.LINES);
        GL.Color(Color.blue);
        GL.Vertex3(currentPosition.x, currentPosition.y, currentPosition.z);
        GL.Vertex3(currentPosition.x, currentPosition.y + 10f, currentPosition.z);
        GL.End();
    }

    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(currentPosition, currentPosition + Vector3.up * 10f);
    }

}
