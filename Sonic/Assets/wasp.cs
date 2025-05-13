using System.Collections;
using UnityEngine;

public class wasp : MonoBehaviour
{

    public bool isMovingRight;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    float last_x;
    void Start()
    {
        last_x = transform.position.x;
        StartCoroutine(clock());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator clock()
    {
        while (true)
        {
            last_x = transform.position.y;
            yield return new WaitForSeconds(0.1f);
            if (last_x - transform.position.x > 0)
                isMovingRight = false;
            else
                isMovingRight = true;
        }
    }


    void OnTriggerEnter(Collider other)
    {
        Rigidbody otherRb = other.GetComponent<Rigidbody>();

        Debug.Log($"local scale = {transform.parent.gameObject.transform.localScale.x}");
        

        if (otherRb != null)
        {
            // Calculate push direction (from me to them, or just opposite of my forward)
            int pushDir = isMovingRight ? -1 : 1;

            // Apply force to the other object
            otherRb.AddForce(new Vector3(pushDir * 16 * transform.parent.gameObject.transform.localScale.x, 0, 0) * 150f, ForceMode.Impulse);
                
            //Debug.Log($"Pushed {collision.gameObject.name} with force!");
        }
    }



}
