using UnityEngine;
using UnityEngine.InputSystem;

public class CameraControl : MonoBehaviour
{
    public GameObject focalPoint;

    InputAction mouseLookAction;

    [SerializeField] float startingDistanceToTarget = 5f;
    [SerializeField] Vector2 lookSpeed = new Vector2(1.0f, 1.0f);
    [SerializeField] float minVerticalLookAngle = -60f;
    [SerializeField] float maxVerticalLookAngle = 70f;
    [SerializeField] float smoothingSpeed = 0.1f;

    [SerializeField] LayerMask environmentLayerMask = (1 << 7);

    public bool invertVertical = true;
    public bool invertHorizontal = false;

    float distanceToTarget;
    // x -- rotation around x (pitch); y -- rotation around y (yaw)
    Vector2 angle;

    Vector3 targetPosition;

    void Start()
    {
        mouseLookAction = InputSystem.actions.FindAction("Look");

        distanceToTarget = startingDistanceToTarget;

        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        Vector2 mouseLook = mouseLookAction.ReadValue<Vector2>();
        // NOTE: Looking axes are correct. Moving the mouse horizontally (x), 
        // will rotate around the y axis. Analogous for vertical (y).
        angle.x += lookSpeed.x * mouseLook.y * (invertVertical ? -1 : 1);
        angle.x = Mathf.Clamp(angle.x, minVerticalLookAngle, maxVerticalLookAngle);
        angle.y += lookSpeed.y * mouseLook.x * (invertHorizontal ? -1 : 1);

        Quaternion pitchYawRotation = Quaternion.Euler(angle.x, angle.y, 0);

        Vector3 cameraOffset = new Vector3(0, 0, -distanceToTarget);

        targetPosition = focalPoint.transform.position;
        targetPosition += pitchYawRotation * cameraOffset;

        if(Physics.Raycast(focalPoint.transform.position, targetPosition - focalPoint.transform.position, out RaycastHit hitInfo, distanceToTarget, environmentLayerMask))
        {
            Vector3 offsetFromWall = ((focalPoint.transform.position - hitInfo.point) * 0.03f);
            targetPosition = hitInfo.point + offsetFromWall;
        }
        
        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothingSpeed);
        transform.rotation = Quaternion.LookRotation(focalPoint.transform.position - transform.position);
    }
}
