using UnityEngine;

public class Rotate : MonoBehaviour
{
    public Vector3 rotationSpeed;

    void Update()
    {
        transform.rotation = Quaternion.Euler(
            rotationSpeed.x * Time.time,
            rotationSpeed.y * Time.time,
            rotationSpeed.z * Time.time 
        );
    }
}
