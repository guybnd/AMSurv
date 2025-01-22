using UnityEngine;

public class ContainerManager : MonoBehaviour
{
    public bool AssignIDsOnStart = true;

    private void Start()
    {
        if (AssignIDsOnStart)
        {
            AssignIDs();
        }
    }

    public void AssignIDs()
    {
        // Loop through all child containers and assign IDs.
        for (int i = 0; i < transform.childCount; i++)
        {
            var container = transform.GetChild(i).GetComponent<Container>();

            if (container != null)
            {
                container.ID = i; // Assign a unique ID based on index.
            }
        }
    }
}
