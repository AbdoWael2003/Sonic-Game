using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;
using UnityEngine.PlayerLoop;
using UnityEngine.UIElements;
public class tails_behaviour : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public Rigidbody body;
  

    [SerializeField] public Animator animator;
    public GameObject bossStartPoint;

    bool onGround = true;
    public static int stamina = 1200;

    void Start()
    {
        stamina = 1200;
        last_y = transform.position.y;
        StartCoroutine(clock());
    }


    // Update is called once per frame
    bool ability = true;
    float last_y;
    void FixedUpdate()
    {
     

        if (PlayerPhysics.current_player_index != 2)
            return;

        //Debug.Log($"stamina = {this.stamina}");

        //animator.SetTrigger("run");

        if(stamina == 0)
        {
            animator.SetBool("flying", false);
            ability = false;
            StartCoroutine(Recover());

        }
        // special power
        if (Input.GetKey(KeyCode.Z) && tails_behaviour.stamina >= 0 && ability)
        {
            animator.SetBool("running", false);
            animator.SetBool("flying", true);


            //Fly();
            body.AddForce(Vector3.up * 0.212f, ForceMode.Impulse);
            stamina -= 8;
        }
        else if (stamina <= Stamina.maxStamina)
        {
            stamina += 2;
            animator.SetBool("flying", false);
        }

    }

    


    IEnumerator clock()
    {
        while (true)
        {
            last_y = transform.position.y;
            yield return new WaitForSeconds(0.05f);
            if (last_y - transform.position.y >= 0.05)
            {
                animator.SetBool("jump2", true);

            }
            else
            {

                animator.SetBool("jump2", false);
            }

        }
    }

    IEnumerator Recover()
    {
        Debug.Log("Recover!");
        Stamina.enabled = false;
        //staminaSlider.GetComponentInChildren<Image>().tintColor = Color.red;
        yield return new WaitForSeconds(4f); // Wait for 3 seconds
        Stamina.enabled = true;
        // Now set your variable
        ability = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        animator.SetBool("jump2", false);
        string name = collision.gameObject.name;
        if (name.Contains("ramp") || name.Contains("straight"))
        {
            //Debug.Log("Ground Touched!");
            onGround = true;
        }

        if (collision.gameObject.transform.parent != null && collision.gameObject.transform.parent.CompareTag("bossPoint") && transform.position.z > map_manager.lastZBossPosition)
        {
            map_manager.lastZBossPosition = map_manager.originalPosition.z;

            PlayerPhysics.switchRequest = true;

            Rigidbody rb = GetComponent<Rigidbody>();

            //rb.linearVelocity = Vector3.zero;
            //rb.angularVelocity = Vector3.zero;
            rb.MovePosition(bossStartPoint.transform.position); // start position of the boss map
            map_manager.bossFight = true;
        }

    }

  
    //public float jumpForce = 7f;
    //public float hoverTime = 5f;
    //IEnumerator FlyRoutine()
    //{
    //    // Disable gravity
    //    body.useGravity = false;

    //    // Apply upward impulse
    //    body.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

    //    // Wait until upward movement stops or after a brief moment (optional)
    //    yield return new WaitForSeconds(1f);

    //    // Zero out velocity to simulate hovering
    //    body.linearVelocity = Vector3.zero;

    //    // Wait while dangling in air
    //    yield return new WaitForSeconds(hoverTime);
    //    // Re-enable gravity
    //    body.useGravity = true;
    //}

  

    //IEnumerator HighJump(float duration)
    //{
    //    body.useGravity = false;

    //    float timer = 0f;
    //    while (timer < duration)
    //    {
    //        body.AddForce(Vector3.up * 3f, ForceMode.Acceleration);
    //        timer += Time.fixedDeltaTime;
    //        yield return new WaitForFixedUpdate();
    //    }

    //    StartCoroutine(Wait(5));
    //}

    //IEnumerator Wait(float seconds)
    //{
    //    body.AddForce(-Vector3.up * 3f, ForceMode.Acceleration);
    //    yield return new WaitForSeconds(seconds);
    //    StartCoroutine(Gravity(3));
    //}

    //IEnumerator Gravity(float seconds)
    //{
    //    yield return new WaitForSeconds(seconds);
    //    body.useGravity = true;
    //}
}
