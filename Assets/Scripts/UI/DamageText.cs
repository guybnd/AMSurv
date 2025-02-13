using UnityEngine;
using TMPro;
using System;

public class DamageText : MonoBehaviour
{
    public float moveSpeed = 200f;
    public float fadeSpeed = 1.5f;
    public float destroyDelay = 1f;
    private TextMeshProUGUI textMesh;
    public TMP_Text damageText;
    private Color textColor;
    private bool isInitialized = false;

  
    // public void Start()
    // {
    //     textMesh = GetComponent<TextMeshProUGUI>();
    //     if (textMesh == null)
    //     {
    //         Debug.LogError("TextMeshProUGUI component not found on this GameObject.");
    //     }

    // }
  // Initialize the DamageText    
    public void Initialize(float damageAmount, Color color, Vector3 location)
    {
        textMesh = GetComponent<TextMeshProUGUI>();
        if (textMesh == null)
        {
            Debug.LogError("TextMeshProUGUI component not found on this GameObject.");
            Destroy(gameObject);
            return;
        }
        int damageAmountInt = Mathf.RoundToInt(damageAmount);
        textMesh.text = damageAmountInt.ToString();
        transform.position = location;
        textColor = color;
        textMesh.color = textColor;
        isInitialized = true;
        Destroy(gameObject, destroyDelay);
    }

    void Update()
    {
        // if (textMesh == null)
        // {
        //     Debug.LogWarning("TextMesh is null in DamageText Update");
        //     return;
        // }
        if (Input.GetKeyDown(KeyCode.U))
        {
            Initialize(100, Color.red, new Vector3(0, 0, 0));
        }

        if (isInitialized) {
        // Move the text upwards in screen space
        RectTransform rectTransform = transform as RectTransform;
        Vector2 position = rectTransform.anchoredPosition;
        position += Vector2.up * moveSpeed * Time.deltaTime;
        rectTransform.anchoredPosition = position;

        // Fade out the text
        textColor.a -= fadeSpeed * Time.deltaTime;
        textMesh.color = textColor;
        }

        // Destroy the object when fully transparent
        // if (textColor.a <= 0)
        // {
        //     Destroy(gameObject);
        // }
    }
} 