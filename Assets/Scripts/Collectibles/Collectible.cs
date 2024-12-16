using UnityEngine;

public class Collectible : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        PlayerState.instance.collectibles += 3;

        Destroy(gameObject);
    }
}
