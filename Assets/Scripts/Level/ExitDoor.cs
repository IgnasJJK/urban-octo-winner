using System.Collections.Generic;
using UnityEngine;

public class ExitDoor : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != 10)
        {
            return;
        }

        if (PlayerState.instance.collectibles == 0)
        {
            return;
        }

        LevelManager.instance.GenerateRoom(transform.position, transform.rotation);

        --PlayerState.instance.collectibles;
        Destroy(this.gameObject);
    }
}
