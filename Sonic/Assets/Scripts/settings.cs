using UnityEngine;
using UnityEngine.UI;

public class settings : MonoBehaviour
{

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GetComponentInChildren<Slider>().value = Data.difficulty;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetDifficulty()
    {
        Data.difficulty = (int)GetComponentInChildren<Slider>().value;
    }

}
