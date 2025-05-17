using Unity.Mathematics.Geometry;
using Unity.VisualScripting;
using UnityEngine;

public class ring : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0f, 300f * Time.deltaTime, 0f);
    }

    //private void OnCollisionEnter(Collision collision)
    //{
    //    GetComponent<Collider>().enabled = false;

    //    //Debug.Log("Coinnnnnnnnnnnn!");
    //    Destroy(gameObject);
    //}

    private void OnTriggerEnter(Collider other)
    {
        GetComponent<Collider>().enabled = false;
        Score.score += 1;
        if(Score.score % (Data.difficulty * 10) == 0)
        {
            PlayerPhysics.health += health.maxHealth * 0.1f + map_manager.level * 2;
            PlayerPhysics.health = System.Math.Min(PlayerPhysics.health, health.maxHealth);
        }    
        //Debug.Log("Coinnnnnnnnnnnn!");
        Destroy(gameObject);
    }
}
