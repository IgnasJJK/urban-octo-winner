using UnityEngine;

public class CollectibleAnimation : MonoBehaviour
{
    public Vector3 rotation;
    public Vector3 fluctuationMagnitude;
    public Vector3 fluctuationOffset;

    float timer;

    Vector3 pivot;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        pivot = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        transform.rotation = Quaternion.Euler(
            rotation.x * timer,
            rotation.y * timer,
            rotation.z * timer
        );

        Vector3 newPosition = new Vector3(
            fluctuationMagnitude.x * Mathf.Sin(timer + fluctuationOffset.x),
            fluctuationMagnitude.y * Mathf.Sin(timer + fluctuationOffset.y),
            fluctuationMagnitude.z * Mathf.Sin(timer + fluctuationOffset.z)
        );
        
        transform.position = pivot + newPosition;
    }
}
