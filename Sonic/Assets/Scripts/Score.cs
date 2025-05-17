using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public static int score = 0;
    [SerializeField] public Text widget;


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        widget.text = $"SCORE: {score}";
    }
}
