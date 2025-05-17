using UnityEngine;
using UnityEngine.UI;

public class bossHealth : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private float lerpSpeed = 0.05f;

    public Slider healthSlider;
    public Slider easeSlider;
    public static float maxHealth = 1000f;


    void Start()
    {        
        Boss.health = maxHealth;
        easeSlider.maxValue = maxHealth;
        healthSlider.maxValue = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        if (healthSlider.value != Boss.health)
        {
            healthSlider.value = Boss.health;
        }
        if (healthSlider.value != easeSlider.value)
        {
            easeSlider.value = Mathf.Lerp(easeSlider.value, Boss.health, lerpSpeed);
        }
    }
}
