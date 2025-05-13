using System.Collections;
using UnityEngine;

public class knuckles : MonoBehaviour
{

    [SerializeField] public Animator animator;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    float last_y;

    void Start()
    {
        last_y = transform.position.y;
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
            last_y = transform.position.y;
            yield return new WaitForSeconds(0.05f);
            if (last_y - transform.position.y >= 0.05 && !animator.GetBool("hit"))
            {
                animator.SetBool("jump2", true);

            }
            else
            {

                animator.SetBool("jump2", false);
            }

        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.name.Contains("target"))
        {
            animator.SetBool("running", false);
            animator.SetBool("jump1", false);
            animator.SetBool("jump2", false);
            animator.SetBool("hit", true);
            StartCoroutine(HitCooldown());
            Destroy(collision.gameObject);
            
        }
    }
    IEnumerator HitCooldown()
    {
        yield return new WaitForSeconds(0.5f);
        animator.SetBool("hit", false);
    }
}
