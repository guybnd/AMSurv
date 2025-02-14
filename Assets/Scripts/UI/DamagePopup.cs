using UnityEngine;

public class DamagePopup : MonoBehaviour
{
    public GameObject damageTextPrefab;
    public Color playerDamageColor = Color.red;
    public Color enemyDamageColor = Color.yellow;
    [Tooltip("The Canvas where damage numbers will appear")]
    public Canvas targetCanvas;

    private void Start()
    {
        // Find the DamageReceiver component in this GameObject
        DamageReceiver damageReceiver = GetComponent<DamageReceiver>();
        if (damageReceiver == null)
        {
            Debug.LogError($"DamageReceiver component not found on {gameObject.name}");
            enabled = false;
            return;
        }

        // Verify we have a Canvas reference
        if (targetCanvas == null)
        {
            // Debug.LogError($"No Canvas assigned to DamagePopup on {gameObject.name}. Please assign a Canvas in the inspector!");
            enabled = false;
            return;
        }

        // Subscribe to the OnDamageTaken event
        damageReceiver.OnDamageTaken += HandleDamageTaken;
    }

    private void HandleDamageTaken(object sender, float damageAmount)
    {
        if (targetCanvas == null) return;

        // Convert world position to screen position
        Vector2 screenPosition = Camera.main.WorldToScreenPoint(transform.position);

        // Instantiate as a child of the canvas
        GameObject damageTextGo = Instantiate(damageTextPrefab, screenPosition, Quaternion.identity, targetCanvas.transform);
        
        DamageText damageText = damageTextGo.GetComponent<DamageText>();
        if (damageText == null)
        {
            Debug.LogError("DamageText component not found on the DamageText prefab.");
            Destroy(damageTextGo);
            return;
        }

        // Set the position using RectTransform
        RectTransform rectTransform = damageTextGo.GetComponent<RectTransform>();
        rectTransform.position = screenPosition;

        // Determine the color based on the GameObject's tag
        Color textColor = (gameObject.CompareTag("Player")) ? playerDamageColor : enemyDamageColor;

        // Initialize the DamageText
        damageText.Initialize(damageAmount, textColor, transform.position);
    }

    private void OnDestroy()
    {
        // Find the DamageReceiver to unsubscribe from the event
        DamageReceiver damageReceiver = GetComponent<DamageReceiver>();
        if (damageReceiver != null)
        {
            damageReceiver.OnDamageTaken -= HandleDamageTaken;
        }
    }
}