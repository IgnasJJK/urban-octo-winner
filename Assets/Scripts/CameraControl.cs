using UnityEngine;
using UnityEngine.InputSystem;

public class CameraControl : MonoBehaviour
{
    public GameObject target;

    InputAction mouseLookAction;

    [SerializeField] float startingDistanceToTarget = 5f;
    [SerializeField] Vector2 lookSpeed = new Vector2(1.0f, 1.0f);

    bool invertVertical = true;
    bool invertHorizontal = false;

    [SerializeField] float distanceToTarget;

    // x -- rotation around x (pitch); y -- rotation around y (yaw)
    Vector2 angle;

    void Start()
    {
        mouseLookAction = InputSystem.actions.FindAction("Look");

        distanceToTarget = startingDistanceToTarget;
    }

    void Update()
    {
        Vector2 mouseLook = mouseLookAction.ReadValue<Vector2>();
        // NOTE: Looking axes are correct. Moving the mouse horizontally (x), 
        // will rotate around the y axis. Analogous for vertical (y).
        angle.x += lookSpeed.x * mouseLook.y * (invertVertical ? -1 : 1);
        // FIXME: Clamping to -89, 89 allows the camera to overshoot when looking from above.
        angle.x = Mathf.Clamp(angle.x, -70f, 70f);
        angle.y += lookSpeed.y * mouseLook.x * (invertHorizontal ? -1 : 1);

        Quaternion pitchYawRotation = Quaternion.Euler(angle.x, angle.y, 0);

        Vector3 cameraOffset = new Vector3(0, 0, -distanceToTarget);

        Vector3 pos = target.transform.position;
        pos += pitchYawRotation * cameraOffset;

        transform.position = pos;
        transform.rotation = Quaternion.LookRotation(target.transform.position - transform.position);
    }
}
