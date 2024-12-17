using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] float ringRadius = 0.65f;
    [SerializeField] float rotationSpeed = 3.0f;

    [SerializeField] int maxDisplaySize = 24;

    List<GameObject> objects = new List<GameObject>();

    [SerializeField] GameObject itemPrefab;

    float rotationFraction = 0f;

    void Update()
    {
        float collectibles = (float)PlayerState.instance.collectibles;

        if (objects.Count != collectibles && collectibles > 0)
        {
            rotationFraction = (2f * Mathf.PI) / Mathf.Min(collectibles, maxDisplaySize);
        }

        // Adjust number of objects.
        // TODO: Disabling excess objects (and re-enabling as needed) might be more optimal.
        {
            while (objects.Count < collectibles)
            {
                GameObject i = Instantiate(itemPrefab, transform);
                objects.Add(i);
            }

            while (objects.Count > collectibles)
            {
                int deleteId = objects.Count - 1;
                Destroy(objects[deleteId]);
                objects.RemoveAt(deleteId);
            }
        }

        // Animate each object.
        for (int i = 0; i < objects.Count; ++i)
        {
            objects[i].transform.localPosition = new Vector3(
                Mathf.Cos(i*rotationFraction + Time.time * rotationSpeed) * ringRadius,
                0,
                Mathf.Sin(i*rotationFraction + Time.time * rotationSpeed) * ringRadius
            );
        }
    }
}
