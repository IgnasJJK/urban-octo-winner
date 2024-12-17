using UnityEngine;

public class Collectible : MonoBehaviour
{

    void OnTriggerEnter(Collider other)
    {
        // Ignore non-player collisions
        if (other.gameObject.layer != 10)
        {
            return;
        }

        PlayerState.instance.collectibles += 3;

        Destroy(gameObject);
    }
}
