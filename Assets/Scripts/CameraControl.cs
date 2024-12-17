using UnityEngine;
using UnityEngine.InputSystem;

public class CameraControl : MonoBehaviour
{
    public GameObject focalPoint; // The object/point to orbit camera around
    [SerializeField] float distanceToTarget = 5f;
    [SerializeField] Vector2 lookSpeed = new Vector2(1.0f, 1.0f);
    [SerializeField] float minVerticalLookAngle = -60f;
    [SerializeField] float maxVerticalLookAngle = 70f;
    [SerializeField] float smoothingSpeed = 0.1f;
    [SerializeField] LayerMask environmentLayerMask = (1 << 7);

    public bool invertVertical = true;
    public bool invertHorizontal = false;

    InputAction mouseLookAction;

    Vector2 viewAngle; // x -- rotation around x (pitch); y -- rotation around y (yaw)
    Vector3 targetPosition;

    void Start()
    {
        mouseLookAction = InputSystem.actions.FindAction("Look");

        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        Vector2 mouseLook = mouseLookAction.ReadValue<Vector2>();

        // NOTE: Looking axes are correct. Moving the mouse horizontally (x), 
        // will rotate around the y axis. Analogous for vertical (y).
        viewAngle.x += lookSpeed.x * mouseLook.y * (invertVertical ? -1 : 1);
        viewAngle.x = Mathf.Clamp(viewAngle.x, minVerticalLookAngle, maxVerticalLookAngle);
        viewAngle.y += lookSpeed.y * mouseLook.x * (invertHorizontal ? -1 : 1);

        Quaternion pitchYawRotation = Quaternion.Euler(viewAngle.x, viewAngle.y, 0);
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
