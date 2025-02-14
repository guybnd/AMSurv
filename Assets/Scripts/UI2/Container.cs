using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum ContainerType
{
    Inventory,
    Equipment,
    Stash,
    Loot
}

public class Container : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private ItemTooltip tooltip;
    [SerializeField] private CharacterStats characterStats;
    // Reference to the Weapon component that will receive weapon stat updates.
    [SerializeField] private Weapon equippedWeapon;

    public ContainerType GetContainerType() => _containerType;

    private RectTransform _rectTransform;
    private RectTransform _currentItemTransform;
    private Item _currentItem;
    private Color _originalColor;

    public int ID { get; set; }

    [SerializeField] public ContainerType _containerType = ContainerType.Inventory;
    [SerializeField] public ItemType _acceptedItemType = ItemType.BagItem;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();

        var image = GetComponent<Image>();
        if (image != null)
        {
            _originalColor = image.color;
        }

        if (tooltip == null)
        {
            tooltip = FindFirstObjectByType<ItemTooltip>();
        }

        // If no weapon has been assigned in the Inspector, try to find one in children.
        if (equippedWeapon == null)
        {
            equippedWeapon = GetComponentInChildren<Weapon>();
        }
    }

    public bool CanAcceptItem(Item item)
    {
        if (item == null) return false;

        switch (_containerType)
        {
            case ContainerType.Inventory:
            case ContainerType.Stash:
            case ContainerType.Loot:
                return true; // These containers accept any item

            case ContainerType.Equipment:
                return item.Type == _acceptedItemType;

            default:
                return false;
        }
    }

    public bool PutInside(RectTransform rect, Container previousContainer)
    {
        var draggable = rect.GetComponent<Draggable>();
        if (draggable == null || draggable.ItemData == null)
        {
            Debug.Log("Container: Invalid item being placed");
            return false;
        }

        if (!CanAcceptItem(draggable.ItemData))
        {
            Debug.Log("Container: Cannot accept item of type " + draggable.ItemData.Type);
            return false;
        }

        // Check if the slot already has an item
        if (_currentItemTransform != null)
        {
            var currentDraggable = _currentItemTransform.GetComponent<Draggable>();

            // Check if items can stack
            if (currentDraggable != null &&
                draggable.ItemData.IsStackable &&
                currentDraggable.ItemData.IsStackable &&
                draggable.ItemData.ID == currentDraggable.ItemData.ID) // Ensure items are identical
            {
                // Merge stacks
                int availableSpace = currentDraggable.ItemData.MaxStack - currentDraggable.CurrentStack;
                if (availableSpace > 0)
                {
                    int transferAmount = Mathf.Min(draggable.CurrentStack, availableSpace);
                    currentDraggable.CurrentStack += transferAmount;
                    draggable.CurrentStack -= transferAmount;

                    currentDraggable.UpdateStackText();
                    draggable.UpdateStackText();

                    // Remove the item if the entire stack has been transferred
                    if (draggable.CurrentStack <= 0)
                    {
                        Destroy(draggable.gameObject);
                    }

                    return true;
                }
                else
                {
                    Debug.Log("Container: No available space to stack items");
                    return false;
                }
            }
            else
            {
                Debug.Log("Container: Cannot stack items. Attempting to swap instead.");
                SwapItems(rect, previousContainer);
                return true;
            }
        }

        // Slot is empty, place the item
        _currentItemTransform = rect;
        _currentItem = draggable.ItemData;
        rect.position = _rectTransform.position;

        if (_containerType == ContainerType.Equipment)
        {
            EquipItem(_currentItem); // Override weapon stats if applicable.
        }

        if (previousContainer != null)
        {
            previousContainer.RemoveItem();
        }

        return true;
    }

    public void RemoveItem()
    {
        if (_currentItem != null && _containerType == ContainerType.Equipment)
        {
            UnequipItem(_currentItem);
        }

        _currentItemTransform = null;
        _currentItem = null;
    }

    private void SwapItems(RectTransform newItem, Container previousContainer)
    {
        var oldItemTransform = _currentItemTransform;
        var oldItemData = _currentItem;

        var newDraggable = newItem.GetComponent<Draggable>();

        // Unequip the currently held item if swapping from equipment
        if (_containerType == ContainerType.Equipment)
        {
            UnequipItem(oldItemData);
        }

        // Equip the new item
        _currentItemTransform = newItem;
        _currentItem = newDraggable.ItemData;
        newItem.position = _rectTransform.position;

        if (_containerType == ContainerType.Equipment)
        {
            EquipItem(_currentItem);
        }

        // Move the old item to the previous container
        if (previousContainer != null)
        {
            previousContainer._currentItemTransform = oldItemTransform;
            previousContainer._currentItem = oldItemData;
            oldItemTransform.position = previousContainer._rectTransform.position;

            var oldDraggable = oldItemTransform.GetComponent<Draggable>();
            if (oldDraggable != null)
            {
                oldDraggable.CurrentContainer = previousContainer;
            }
        }

        var newDraggableComponent = newItem.GetComponent<Draggable>();
        if (newDraggableComponent != null)
        {
            newDraggableComponent.CurrentContainer = this;
        }
    }

    /// <summary>
    /// Equips an item.
    /// If it's a weapon, the Weapon component's stats are completely overridden
    /// with the incoming weapon's stats. Any stat not provided by the item will be set to 0.
    /// </summary>
    public void EquipItem(Item item)
    {
        if (item == null || characterStats == null) return;

        if (item.Type == ItemType.Weapon)
        {
            if (equippedWeapon != null)
            {
                // Since Item.WeaponType is already a WeaponType, assign it directly.
                equippedWeapon.weaponType = item.WeaponType;

                // Build a dictionary of weapon stats from the item's StatModifiers.
                // (We assume that item.StatModifiers is a collection where each modifier has:
                //   - StatName: a string (e.g., "BaseAttackSpeed")
                //   - Value: a float representing the stat's value)
                Dictionary<WeaponStat, float> weaponStats = new Dictionary<WeaponStat, float>();
                foreach (var modifier in item.StatModifiers)
                {
                    WeaponStat stat;
                    if (!Enum.TryParse<WeaponStat>(modifier.StatName, true, out stat))
                    {
                        Debug.LogWarning("Could not convert " + modifier.StatName + " to WeaponStat.");
                        continue;
                    }
                    // For overriding, simply set the stat value.
                    weaponStats[stat] = modifier.Value;
                }
                // Override the weapon's stats completely.
                equippedWeapon.SetWeaponStats(weaponStats);
            }
        }
        else
        {
            // For non-weapon items, update the character stats as before.
            foreach (var modifier in item.StatModifiers)
            {
                characterStats.GetStat(modifier.StatName).AddModifier(modifier.StatName, modifier.Value, modifier.IsMultiplicative);
            }
        }
        // Instead of directly updating UI, notify that stats have changed.
        characterStats.NotifyStatsChanged();
    }

    /// <summary>
    /// Unequips an item.
    /// If it's a weapon, the Weapon component resets its stats to the base values.
    /// </summary>
    public void UnequipItem(Item item)
    {
        if (item == null || characterStats == null) return;

        if (item.Type == ItemType.Weapon)
        {
            if (equippedWeapon != null)
            {
                // Reset the weapon's stats to its base values.
                equippedWeapon.ResetWeaponStats();
            }
        }
        else
        {
            foreach (var modifier in item.StatModifiers)
            {
                characterStats.GetStat(modifier.StatName).RemoveModifier(modifier.StatName, modifier.Value, modifier.IsMultiplicative);
            }
        }
        // Notify stats change via CharacterStats.
        characterStats.NotifyStatsChanged();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Highlight(true);
        ShowTooltip();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Highlight(false);
        HideTooltip();
    }

    public void ShowTooltip()
    {
        if (_currentItem != null && tooltip != null)
        {
            tooltip.ShowTooltip(_currentItem, Input.mousePosition);
        }
    }

    public void HideTooltip()
    {
        if (tooltip != null)
        {
            tooltip.HideTooltip();
        }
    }

    public void Highlight(bool enable)
    {
        var image = GetComponent<Image>();
        if (image != null)
        {
            image.color = enable ? Color.yellow : _originalColor;
        }
    }

    public RectTransform GetCurrentItemTransform()
    {
        return _currentItemTransform;
    }

    public void SetCurrentItem(RectTransform itemTransform, Item itemData)
    {
        _currentItemTransform = itemTransform;
        _currentItem = itemData;
    }
}
