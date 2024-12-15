using UnityEngine;

public class PlayerLocomotion : MonoBehaviour
{
    CharacterController controller;

    [SerializeField] float movementSpeed = 10f;
    [SerializeField] float gravity = 9.81f;

    [SerializeField] float pushStrength = 10.0f;
    [SerializeField] float jumpStrength = 5.0f;

    [SerializeField] float coyoteTime = 0.2f;
    [SerializeField] float maxAirJumpCount = 2;

    float verticalVelocity = 0;
    float inAirTime = 0;
    float inAirJumps = 0;

    bool coyoteJumped;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {

        float v = Input.GetAxis("Vertical");
        float h = Input.GetAxis("Horizontal");

        Vector3 movementDelta = (v * transform.forward + h * transform.right) * movementSpeed * Time.deltaTime;
        
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
        if (Input.GetButtonDown("Jump"))
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
