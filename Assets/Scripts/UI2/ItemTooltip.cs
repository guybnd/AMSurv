using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text;

public class ItemTooltip : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private GameObject tooltipPanel;
    [SerializeField] private Image itemIconImage;
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private TextMeshProUGUI itemTypeText;
    [SerializeField] private TextMeshProUGUI statsText;

    [Header("Visual Settings")]
    [SerializeField] private Color nameTextColor = Color.yellow;
    [SerializeField] private Color typeTextColor = Color.gray;
    [SerializeField] private Color statsTextColor = Color.white;

    private RectTransform _rectTransform;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        HideTooltip();
    }

    public void ShowTooltip(Item item, Vector2 position)
    {
        if (item == null) return;

        // Set item image
        if (itemIconImage != null)
        {
            itemIconImage.sprite = item.ItemImage;
            itemIconImage.gameObject.SetActive(item.ItemImage != null);
        }

        // Set item name
        itemNameText.text = item.ItemName;
        itemNameText.color = nameTextColor;

        // Set item type
        itemTypeText.text = item.Type.ToString();
        itemTypeText.color = typeTextColor;

        // Generate stats text
        StringBuilder statsBuilder = new StringBuilder();

        // Add stack information if item is stackable
        if (item.IsStackable)
        {
            statsBuilder.AppendLine($"Stack: {item.CurrentStack}/{item.MaxStack}");
        }

        // Add stat modifiers
        foreach (var statMod in item.StatModifiers)
        {
            string modifierSymbol = statMod.IsMultiplicative ? "%" : "";
            statsBuilder.AppendLine($"{statMod.StatName}: +{statMod.Value}{modifierSymbol}");
        }

        statsText.text = statsBuilder.ToString();
        statsText.color = statsTextColor;

        // Show and position the tooltip
        tooltipPanel.SetActive(true);
       // UpdatePosition(position);
    }

    private void UpdatePosition(Vector2 position)
    {
        Vector2 screenPoint = position + new Vector2(20, 20);

        // Ensure tooltip stays within screen bounds
        Vector2 size = _rectTransform.sizeDelta;
        if (screenPoint.x + size.x > Screen.width)
            screenPoint.x = Screen.width - size.x;
        if (screenPoint.y + size.y > Screen.height)
            screenPoint.y = Screen.height - size.y;

        _rectTransform.position = screenPoint;
    }

    public void HideTooltip()
    {
        tooltipPanel.SetActive(false);
    }
}