using System.Collections;
using NUnit.Framework;
using UnityEngine;

public class knuckles : MonoBehaviour
{
    public GameObject throwable;
    public GameObject bossStartPoint;

    [SerializeField] public Animator animator;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    float last_y;

    //public GameObject[] northTargetPoints;
    //public GameObject[] eastTargetPoints;
    //public GameObject[] westTargetPoints;

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

    //float projectileSpeed = 0.001f;
    //public IEnumerator Shoot(Rigidbody rb, Vector3 direction)
    //{
    //    while (true)
    //    {
    //        rb.AddForce(direction * projectileSpeed);
    //        yield return new WaitForFixedUpdate();
    //    }
    //}

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"Collison tag is {collision.gameObject.tag}" +
         $"button is pressed? {Input.GetKey(KeyCode.Z)}");


        if (collision.gameObject.name.Contains("target"))
        {
            animator.SetBool("running", false);
            animator.SetBool("jump1", false);
            animator.SetBool("jump2", false);
            animator.SetBool("hit", true);
            StartCoroutine(HitCooldown());
            Destroy(collision.gameObject);
        }

        if (collision.gameObject.tag.Contains("Throwables") && Input.GetKey(KeyCode.Z))
        {

            animator.SetBool("hit", true);
            StartCoroutine(HitCooldown());
            GameObject attackObject = Instantiate(throwable, transform.position, throwable.transform.rotation);
            StartCoroutine(DestroyThrowable(attackObject));

            //GameObject targetPoint = null;
            //if(collision.gameObject.CompareTag("northThrowables"))
            //    targetPoint = northTargetPoints[UnityEngine.Random.Range(0, northTargetPoints.Length - 1)]; 
            //if(collision.gameObject.CompareTag("eastThrowables"))
            //    targetPoint = eastTargetPoints[UnityEngine.Random.Range(0, eastTargetPoints.Length - 1)]; 
            //if(collision.gameObject.CompareTag("westThrowables"))
            //    targetPoint = westTargetPoints[UnityEngine.Random.Range(0, westTargetPoints.Length - 1)];

            Vector3 target = transform.position + transform.forward * 15;
            Vector3 direction = (target - attackObject.transform.position).normalized;
            attackObject.GetComponent<Rigidbody>().AddForce(direction * 20, ForceMode.Impulse); 

            //StartCoroutine(Shoot(attackObject.GetComponent<Rigidbody>(), pushDirection));
        }

        if (collision.gameObject.transform.parent != null && collision.gameObject.transform.parent.CompareTag("bossPoint") && transform.position.z > map_manager.lastZBossPosition)
        {
            map_manager.lastZBossPosition = map_manager.originalPosition.z;

            Rigidbody rb = GetComponent<Rigidbody>();
            //rb.linearVelocity = Vector3.zero;
            //rb.angularVelocity = Vector3.zero;
            rb.MovePosition(bossStartPoint.transform.position); // start position of the boss map
            map_manager.bossFight = true;
        }


    }

    IEnumerator DestroyThrowable(GameObject throwable)
    {
        yield return new WaitForSeconds(5f);
        Destroy(throwable);
    }

    IEnumerator HitCooldown()
    {
        yield return new WaitForSeconds(0.5f);
        animator.SetBool("hit", false);
    }


}
