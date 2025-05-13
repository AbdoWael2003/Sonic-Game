using UnityEngine;

public class chain_behaviour : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    float speed = 5f;
    bool increment = true;
    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0, speed, 0);

      

    }
}
