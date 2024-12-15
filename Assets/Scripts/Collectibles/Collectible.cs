using UnityEngine;

public class Collectible : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        PlayerState.instance.AddCollectibles(3);

        Destroy(gameObject);
    }
}
