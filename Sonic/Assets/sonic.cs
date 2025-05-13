using UnityEngine;
using UnityEngine.EventSystems;

public class sonic : MonoBehaviour
{


    public Rigidbody rb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log("Sonic?????");
        rb = GetComponent<Rigidbody>();

    }

    private void Update()
    {

      

    }
    private void FixedUpdate()
    {
        //rb.linearVelocity = new Vector3(rb.linearVelocity.x, rb.linearVelocity.y, rb.linearVelocity.z * 10);

        //if (Input.GetKeyDown(KeyCode.Z))
        //{
        //    rb.linearVelocity = new Vector3(rb.linearVelocity.x, rb.linearVelocity.y, rb.linearVelocity.z * 10);
        //}
    }

    // Update is called once per frame
    //void Update()
    //{
    //    Debug.Log("Alooooooooooooooo??");
    //    rb.linearVelocity = new Vector3(rb.linearVelocity.x, rb.linearVelocity.y, rb.linearVelocity.z * 10);

    //    if (Input.GetKeyDown(KeyCode.Z))
    //    {
    //        rb.linearVelocity = new Vector3(rb.linearVelocity.x,rb.linearVelocity.y, rb.linearVelocity.z * 10);
    //    }
    //}
}
