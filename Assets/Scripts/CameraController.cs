using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraController : MonoBehaviour
{
    [Header("Camera Settings")]
    public GameObject target; // Center of the terrain
    [SerializeField] private float radius = 80f; // Distance from the target
    [SerializeField] private float height = 80f; // Height of the camera
    [SerializeField] private float rotationSpeed = 50f; // Speed of rotation

    [SerializeField] private TextMeshProUGUI treeStats;
    [SerializeField] private PlantGrowthSimulation plantGrowthSimulation;

    private float angle = 0f; // Current angle around the target

    public GameObject centerPoint;
    
    // Create event listener for when centerPoint variable changes
    private void Start()
    {
        target = centerPoint;
    }

    private void Update()
    {
        MoveCamera();
        HandleRaycast();
        
        // If user presses escape program ends
        if (Input.GetKey("escape")) UnityEngine.Application.Quit();
    }

    void HandleRaycast()
    {
        //Don't do raycast if mouse is overtop of UI element
        if (!EventSystem.current.IsPointerOverGameObject())
        {

            //When mouse left click do raycast
            if (Input.GetMouseButtonDown(0))
            {
                //Create ray from main cam to mouse position
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                //Whatever is hit will be stored in "hit' var
                RaycastHit hit;

                //If there something was hit then execute
                if (Physics.Raycast(ray, out hit))
                {
                    string hitObject = hit.collider.gameObject.tag;
                    if (hitObject == "Tree")
                    {
                        // Debug.Log($"Tag: {hitObject}");
                        target = hit.collider.gameObject;
                        int x = (int)(target.transform.position.x / plantGrowthSimulation.gridSpacing);
                        int z = (int)(target.transform.position.z / plantGrowthSimulation.gridSpacing);
                        treeStats.text =
                            $"Selected Tree \n" +
                            $"Water(X): {x}\n" +
                            $"Sunshine(Z): {z}\n" +
                            $"Height(Y): {plantGrowthSimulation.heightMap[x, z]}";
                    }
                    
                    
                }
                else
                {
                    target = centerPoint;
                    treeStats.text =
                        $"Selected Tree/n" +
                        $"Water(X): None /n" +
                        $"Sunshine(Z): None /n" +
                        $"Height(Y): None ";
                }
            }
        }
    }

    void MoveCamera()
    {
        // Rotate camera with left/right arrow keys
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            angle -= rotationSpeed * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            angle += rotationSpeed * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
        {
            height += rotationSpeed * Time.deltaTime;
            radius += rotationSpeed * Time.deltaTime;
            height = Mathf.Clamp(height, 2f, 120f);
            radius = Mathf.Clamp(radius, 2f, 120f);
        }
        else if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
        {
            height -= rotationSpeed * Time.deltaTime;
            radius -= rotationSpeed * Time.deltaTime;
            height = Mathf.Clamp(height, 2f, 120f);
            radius = Mathf.Clamp(radius, 2f, 120f);
        }

        // Calculate new position
        float radian = Mathf.Deg2Rad * angle;
        float x = (target.transform.position.x) + radius * Mathf.Cos(radian);
        float z = (target.transform.position.z) + radius * Mathf.Sin(radian);

        transform.position = new Vector3(x, target.transform.position.y + height, z);

        Vector3 lookAtPosition = new Vector3(target.transform.position.x, target.transform.localScale.y*10f,
            target.transform.position.z);
        // Look at the target
        transform.LookAt(lookAtPosition);
    }
}