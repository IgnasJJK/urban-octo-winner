using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class PlayerLocomotion : MonoBehaviour
{
    CharacterController controller;
    [SerializeField] GameObject cameraObj;
    [SerializeField] GameObject capsuleObj; // Player object (for wobble)

    [Header("Movement")]
    [SerializeField] float movementSpeed = 10f;
    [SerializeField] float gravity = 9.81f; // -Y acceleration
    [Header("Interaction")]
    [SerializeField] float pushStrength = 10.0f; // Multiplier to force applied to rigidbodies.
    [Header("Jump")]
    [SerializeField] float jumpStrength = 5.0f;
    [SerializeField] float coyoteTime = 0.2f;
    [SerializeField] float maxAirJumpCount = 2; // Extra jumps in addition to the jump-from-ground.

    float verticalVelocity = 0;
    float inAirTime = 0;
    float inAirJumps = 0;
    bool coyoteJumped;

    Quaternion targetRotation;

    InputAction movementAction;
    InputAction jumpAction;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        movementAction = InputSystem.actions.FindAction("Move");
        jumpAction = InputSystem.actions.FindAction("Jump");

        targetRotation = transform.rotation;

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
        // OBSOLETE: Unnecessary option to allow movement without having a camera reference.

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

        DoSillyMovementAnimation(movement);

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

        capsuleObj.transform.localRotation = Quaternion.Lerp(capsuleObj.transform.rotation, targetRotation, 0.1f);

        controller.Move(movementDelta);
    }


    [Header("Silly Movement Animation")]
    [SerializeField] float sillyAngle = 5f;
    [SerializeField] float sillyWobble = 0.2f;
    [SerializeField] float sillyMagnitude = 0.2f;
    float movementAnimationTimer = 0f;

    bool isMoving = false;


    private void DoSillyMovementAnimation(Vector2 movement)
    {
        if (capsuleObj == null) return;

        Vector3 forward = cameraObj.transform.forward;
        forward.y = 0;


        if (movement == Vector2.zero || inAirTime > 0.1f)
        {
            // Apply once
            if (isMoving)
            {
                movementAnimationTimer = 0;
                targetRotation = Quaternion.LookRotation(forward);
                isMoving = false;
            }
            // Apply continuosly
            capsuleObj.transform.localPosition = Vector3.Lerp(capsuleObj.transform.localPosition, Vector3.zero, 0.1f);
            return;
        }

        isMoving = true;

        movementAnimationTimer += Time.deltaTime;

        float mA = Mathf.Sin(movementAnimationTimer * 10f);
        float mB = Mathf.Sin(movementAnimationTimer * 10f + Mathf.PI);

        // FIXME: Semi-working hack here. The first iteration of this made the player wobble only
        // on the X axis, which made it nearly invisible when facing in X or -X direction.
        // 
        // Now the wobble works when looking from all angles, but sometimes the side-to-side
        // movement and rotation go out of sync, which makes the wobble inconsistent.
        float forwardDir = Vector3.Dot(forward, Vector3.forward);
        forwardDir = forwardDir >= 0 ? 1 : -1;

        targetRotation = Quaternion.LookRotation(forward) * Quaternion.Euler(0, 0, forwardDir*mA*sillyAngle);

        capsuleObj.transform.localPosition = new Vector3(mB*sillyWobble, Mathf.Abs(mA*sillyMagnitude), 0);
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
