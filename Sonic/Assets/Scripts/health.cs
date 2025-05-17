using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class health : MonoBehaviour
{


    public Canvas gameCanvas;

    private float lerpSpeed = 0.05f;
    public Slider healthSlider;
    public Slider easeSlider;
    public static float maxHealth = 1000f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PlayerPhysics.health = maxHealth;
        easeSlider.maxValue = maxHealth;
        healthSlider.maxValue = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        // interpolation
        if(healthSlider.value != PlayerPhysics.health)
        {
            healthSlider.value = PlayerPhysics.health;
        }
        if (healthSlider.value != easeSlider.value)
        {
            easeSlider.value = Mathf.Lerp(easeSlider.value, PlayerPhysics.health, lerpSpeed);
        }


        if (PlayerPhysics.health <= 0)
        {
            StartCoroutine(EndGameplay());
        }
    }
    
    IEnumerator EndGameplay()
    {
        // Create a new GameObject for the text
        GameObject textObject = new GameObject("GameOverText");

        // Add TextMeshProUGUI component
        TextMeshProUGUI tmpText = textObject.AddComponent<TextMeshProUGUI>();

        tmpText.text = "GAME OVER";
        tmpText.fontSize = 100;
        tmpText.alignment = TextAlignmentOptions.Center;
        tmpText.color = Color.red;
        
        //if (fontAsset != null)
        //    tmpText.font = fontAsset;

        // Set it as child of the canvas
        textObject.transform.SetParent(gameCanvas.transform, false);

        // Stretch the RectTransform to full screen and center the text
        RectTransform rectTransform = tmpText.GetComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(1, 1);
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;

        // Wait 5 seconds
        yield return new WaitForSeconds(5f);

        // Destroy the text object
        Destroy(textObject);
        SceneManager.LoadScene(0);
    }
}


