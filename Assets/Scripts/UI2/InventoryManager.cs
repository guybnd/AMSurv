using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private GameObject inventoryWindow; // Assign the inventory UI GameObject in the Inspector
    private bool isInventoryOpen = false;

    private void Update()
    {
        // Toggle inventory with "I" or close it with "Esc"
        if (Input.GetKeyDown(KeyCode.I))
        {
            ToggleInventory();
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && isInventoryOpen ) //!IsPauseMenuOpen && !IsSettingsMenuOpen) when implemented

        {
            CloseInventory();
        }
    }

    private void ToggleInventory()
    {
        if (isInventoryOpen)
        {
            CloseInventory();
        }
        else
        {
            OpenInventory();
        }
    }

    private void OpenInventory()
    {
        inventoryWindow.SetActive(true); // Show the inventory UI
        Time.timeScale = 0f; // Pause the game
        isInventoryOpen = true;
    }

    private void CloseInventory()
    {
        inventoryWindow.SetActive(false); // Hide the inventory UI
        Time.timeScale = 1f; // Resume the game
        isInventoryOpen = false;
    }
}
