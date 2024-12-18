using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Camera Settings")]
    public Transform target; // Center of the terrain
    [SerializeField] private float radius = 20f; // Distance from the target
    [SerializeField] private float height = 5f; // Height of the camera
    [SerializeField] private float rotationSpeed = 50f; // Speed of rotation

    private float angle = 0f; // Current angle around the target

    private void Update()
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
        float x = (target.position.x) + radius * Mathf.Cos(radian);
        float z = (target.position.z) + radius * Mathf.Sin(radian);

        transform.position = new Vector3(x, target.position.y + height, z);

        // Look at the target
        transform.LookAt(target);
    }
}