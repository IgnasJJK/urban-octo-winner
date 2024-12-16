using UnityEngine;

public class Inventory : MonoBehaviour
{
    int currentInventory = 0;

    [SerializeField] float ringRadius = 0.65f;
    [SerializeField] float rotationSpeed = 3.0f;

    int inventorySize = 10;

    [SerializeField] GameObject[] objects;
    int objectI;

    [SerializeField] GameObject itemPrefab;

    float rotationFraction = 0f;

    void Start()
    {
        objects = new GameObject[inventorySize];
    }

    // Update is called once per frame
    void Update()
    {
        float collectibles = (float)PlayerState.instance.collectibles;


        if (currentInventory != collectibles && collectibles > 0)
        {
            rotationFraction = (2f * Mathf.PI) / collectibles;
        }

        // FIXME: Out of bounds when inventory size exceeded.
        while (currentInventory < collectibles)
        {
            objects[objectI++] = Instantiate(itemPrefab, transform);
            ++currentInventory;
        }

        while (currentInventory > collectibles)
        {
            Destroy(objects[--objectI]);
            --currentInventory;
        }

        for (int i = 0; i < objectI; ++i)
        {
            Transform t = objects[i].transform;

            t.localPosition = new Vector3(
                Mathf.Cos(i*rotationFraction + Time.time * rotationSpeed) * ringRadius,
                0,
                Mathf.Sin(i*rotationFraction + Time.time * rotationSpeed) * ringRadius
            );
        }
    }
}
