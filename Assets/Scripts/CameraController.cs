using System;
using UnityEngine;
using UnityEngine.Animations;

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
    private float xUpperBounds => (building.TotalUnits) * Building.UNIT_LENGTH - (Building.UNIT_LENGTH / 2);

    private Transform selectedElevator = null;
    private PositionConstraint positionConstraint;

    private void Start()
    {
        positionConstraint = this.GetComponentInChildren<PositionConstraint>();
    }

    void Update()
    {
        CheckClick();
        
        var speed = panSpeed;
        if (Input.GetKey(KeyCode.LeftShift))
            speed *= 2;
        
        // Handle keyboard input
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        Vector3 keyboardMovement = new Vector3(horizontalInput, verticalInput, 0f) * speed * Time.unscaledDeltaTime;
        transform.Translate(keyboardMovement);

        // Handle mouse input
        Vector3 mousePosition = Input.mousePosition;
        Vector3 cameraMovement = Vector3.zero;

        if (mousePosition.x < borderThickness)
        {
            cameraMovement.x -= speed * Time.unscaledDeltaTime;
        }
        else if (mousePosition.x >= Screen.width - borderThickness)
        {
            cameraMovement.x += speed * Time.unscaledDeltaTime;
        }

        if (mousePosition.y < borderThickness)
        {
            cameraMovement.y -= speed * Time.unscaledDeltaTime;
        }
        else if (mousePosition.y >= Screen.height - borderThickness)
        {
            cameraMovement.y += speed * Time.unscaledDeltaTime;
        }

        transform.Translate(cameraMovement);
        
        // Handle scroll wheel input for zooming
        float scrollWheelInput = Input.GetAxis("Mouse ScrollWheel");
        Vector3 zoomMovement = new Vector3(0f, 0f, scrollWheelInput * zoomSpeed * Time.unscaledDeltaTime);
        transform.Translate(zoomMovement);

        // Clamp camera position within defined bounds
        Vector3 clampedPosition = transform.position;
        clampedPosition.x = Mathf.Clamp(clampedPosition.x, zoomMin, zoomMax);
        clampedPosition.z = Mathf.Clamp(clampedPosition.z, xLowerBounds, xUpperBounds);
        clampedPosition.y = Mathf.Clamp(clampedPosition.y, yLowerBounds, yUpperBounds);
        transform.position = clampedPosition;
    }

    private void CheckClick()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // Check if the ray hits any GameObject
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Elevator"))
                {
                    selectedElevator = hit.collider.gameObject.transform;

                    for (int i = 0; i < positionConstraint.sourceCount; i++)
                    {
                        positionConstraint.RemoveSource(i);
                    }

                    positionConstraint.constraintActive = true;
                    positionConstraint.AddSource(new ConstraintSource()
                    {
                        sourceTransform = selectedElevator,
                        weight = 1
                    });
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            RemoveElevatorConstraint();
        }
    }

    private void RemoveElevatorConstraint()
    {
        for (int i = 0; i < positionConstraint.sourceCount; i++)
        {
            positionConstraint.RemoveSource(i);
        }

        positionConstraint.constraintActive = false;
        selectedElevator = null;
    }
}