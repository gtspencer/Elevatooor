using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Building building;

    [SerializeField] private float zoomSpeed = 10f; 
    [SerializeField]
    private float zoomMax = 50f;
    [SerializeField]
    private float zoomMin = 7f;
    [SerializeField]
    private float panSpeed = 10f;
    [SerializeField]
    private float borderThickness = 10f;

    [SerializeField]
    private float yLowerBounds = 5;
    private float yUpperBounds => (building.Floors) * Building.ROOM_HEIGHT - (Building.ROOM_HEIGHT / 2);
    [SerializeField]
    private float xLowerBounds = 0;
    private float xUpperBounds => (building.Units) * Building.UNIT_LENGTH - (Building.UNIT_LENGTH / 2);

    void Update()
    {
        var speed = panSpeed;
        if (Input.GetKey(KeyCode.LeftShift))
            speed *= 2;
        
        // Handle keyboard input
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        Vector3 keyboardMovement = new Vector3(horizontalInput, verticalInput, 0f) * speed * Time.deltaTime;
        transform.Translate(keyboardMovement);

        // Handle mouse input
        Vector3 mousePosition = Input.mousePosition;
        Vector3 cameraMovement = Vector3.zero;

        if (mousePosition.x < borderThickness)
        {
            cameraMovement.x -= speed * Time.deltaTime;
        }
        else if (mousePosition.x >= Screen.width - borderThickness)
        {
            cameraMovement.x += speed * Time.deltaTime;
        }

        if (mousePosition.y < borderThickness)
        {
            cameraMovement.y -= speed * Time.deltaTime;
        }
        else if (mousePosition.y >= Screen.height - borderThickness)
        {
            cameraMovement.y += speed * Time.deltaTime;
        }

        transform.Translate(cameraMovement);
        
        // Handle scroll wheel input for zooming
        float scrollWheelInput = Input.GetAxis("Mouse ScrollWheel");
        Vector3 zoomMovement = new Vector3(0f, 0f, scrollWheelInput * zoomSpeed * Time.deltaTime);
        transform.Translate(zoomMovement);

        // Clamp camera position within defined bounds
        Vector3 clampedPosition = transform.position;
        clampedPosition.x = Mathf.Clamp(clampedPosition.x, zoomMin, zoomMax);
        clampedPosition.z = Mathf.Clamp(clampedPosition.z, xLowerBounds, xUpperBounds);
        clampedPosition.y = Mathf.Clamp(clampedPosition.y, yLowerBounds, yUpperBounds);
        transform.position = clampedPosition;
    }
}