using UnityEngine;
using UnityEngine.UI;

public class Stamina : MonoBehaviour
{
    private float lerpSpeed = 0.05f;
    [SerializeField] public Slider staminaSlider;
    [SerializeField] public Slider easeSlider;
    public static float maxStamina = 1200;
    
    public static bool enabled = true;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        enabled = true;
        maxStamina = 1200;
        staminaSlider.value = maxStamina;
        easeSlider.value = maxStamina;
        gameObject.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {

        staminaSlider.maxValue = maxStamina;
        easeSlider.maxValue = maxStamina;

        if (enabled)
            staminaSlider.GetComponentInChildren<Image>().color = new Color(0, 175, 255, 255);
        else
            staminaSlider.GetComponentInChildren<Image>().color = Color.gray;

        // interpolation
        if (staminaSlider.value != tails_behaviour.stamina)
        {
            staminaSlider.value = tails_behaviour.stamina;
        }
        if (staminaSlider.value != easeSlider.value)
        {
            easeSlider.value = Mathf.Lerp(easeSlider.value, tails_behaviour.stamina, lerpSpeed);
        }
    }

}