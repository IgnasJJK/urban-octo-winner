using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class PlayerLocomotion : MonoBehaviour
{

    CharacterController controller;
    [SerializeField] GameObject cameraObj;

    [Header("Movement")]
    [SerializeField] float movementSpeed = 10f;
    [SerializeField] float gravity = 9.81f;
    [Header("Interaction")]
    [SerializeField] float pushStrength = 10.0f;
    [Header("Jump")]
    [SerializeField] float jumpStrength = 5.0f;
    [SerializeField] float coyoteTime = 0.2f;
    [SerializeField] float maxAirJumpCount = 2;

    float verticalVelocity = 0;
    float inAirTime = 0;
    float inAirJumps = 0;

    bool coyoteJumped;

    InputAction movementAction;
    InputAction jumpAction;


    void Start()
    {
        controller = GetComponent<CharacterController>();

        movementAction = InputSystem.actions.FindAction("Move");
        jumpAction = InputSystem.actions.FindAction("Jump");

#if UNITY_EDITOR
        if (cameraObj == null)
        {
            Debug.LogWarning("Camera reference not set on '" + gameObject.name + "'.");
        }
#endif
    }

    void Update()
    {
#if UNITY_EDITOR
        Vector3 forward, right;
        if (cameraObj == null)
        {
            forward = transform.forward;
            right = transform.right;
        }
        else
        {
            forward = cameraObj.transform.forward;
            right = cameraObj.transform.right;
        }
#else
        Vector3 forward = cameraObj.transform.forward;
        Vector3 right = cameraObj.transform.right;
#endif
        forward.y = right.y = 0;
        forward.Normalize();
        right.Normalize();

        Vector2 movement = movementAction.ReadValue<Vector2>();
        Vector3 movementDelta = (movement.y * forward + movement.x * right) * movementSpeed * Time.deltaTime;

#region Vertical movement, jumps
        if (controller.isGrounded)
        {
            inAirTime = 0;
            inAirJumps = 0;
            verticalVelocity = 0;
            coyoteJumped = false;
        }
        else
        {
            inAirTime += Time.deltaTime;

            // NOTE: Acceleration is measure in m/s^2.
            verticalVelocity -= gravity * Time.deltaTime;
        }

        // Calculate vertical velocity from jumping
        if (jumpAction.WasPressedThisFrame())
        {
            if (controller.isGrounded)
            {
                verticalVelocity = jumpStrength;
            }
            else 
            {
                // Coyote Jump - extra time after starting to fall to register an on-ground jump.
                if (inAirTime < coyoteTime && !coyoteJumped)
                {
                    verticalVelocity = jumpStrength;
                    coyoteJumped = true;
                }
                else if (inAirJumps < maxAirJumpCount)
                {
                    verticalVelocity = jumpStrength;
                    ++inAirJumps;
                }
            }
        }
        movementDelta.y += verticalVelocity * Time.deltaTime;
#endregion

        controller.Move(movementDelta);
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        // NOTE: Mostly lifted from the docs at this point.

        Rigidbody body = hit.collider.attachedRigidbody;

        if (body == null || body.isKinematic) return;

        // To not push objects below
        //if (hit.moveDirection.y < -0.3) return;

        // Calculate push direction
        Vector3 pushDir = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);

        body.linearVelocity = pushDir * pushStrength / body.mass;
    }
}
