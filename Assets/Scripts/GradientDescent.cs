using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GradientDescent : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float learningRate = 0.5f;
    [SerializeField] private float stepSize = 0.1f;
    [SerializeField] private float stopThreshold = 0.01f;
    [SerializeField] private float maxSteps = 1000;
    [SerializeField] private float moveDelay = 0.1f; // Increased delay between moves
    
    [Header("Visualization")]
    [SerializeField] private GameObject gradientDescentPrefab;
    [SerializeField] private GameObject pathMarkerPrefab; // Prefab for showing the path
    [SerializeField] private int markerInterval = 5; // Place a marker every N steps
    
    [Header("References")]
    [SerializeField] private TerrainGenerator terrain;
    
    private Vector3 currentPosition;
    private bool isDescending = false;
    private List<GameObject> pathMarkers = new List<GameObject>();
    
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
        
        // Place initial marker
        PlacePathMarker(currentPosition);
    }

    public void StartGradientDescent()
    {
        if (!isDescending)
            StartCoroutine(GradientDescentRoutine());
    }

    private IEnumerator GradientDescentRoutine()
    {
        isDescending = true;
        int steps = 0;
        float previousHeight = float.MaxValue;
        
        while (steps < maxSteps)
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
            
            if (Mathf.Abs(previousHeight - newHeight) < stopThreshold)
                break;
            
            currentPosition = newPosition;
            transform.position = currentPosition;
            
            // Place marker at intervals
            if (steps % markerInterval == 0)
            {
                PlacePathMarker(currentPosition);
            }
            
            previousHeight = newHeight;
            steps++;
            
            yield return new WaitForSeconds(moveDelay);
        }
        
        // Place final marker
        PlacePathMarker(currentPosition);
        
        isDescending = false;
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
        {
            Destroy(marker);
        }
        pathMarkers.Clear();
    }
}