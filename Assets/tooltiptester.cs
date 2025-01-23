using UnityEngine;

// Add this temporary test script to any GameObject
public class TooltipTester : MonoBehaviour
{
    public ItemTooltip tooltip;
    public Item testItem;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            Debug.Log("Testing tooltip display");
            tooltip.ShowTooltip(testItem, Input.mousePosition);
        }
        if (Input.GetKeyDown(KeyCode.H))
        {
            Debug.Log("Testing tooltip hide");
            tooltip.HideTooltip();
        }
    }
}
