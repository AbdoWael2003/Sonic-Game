using UnityEditor;
using UnityEngine;

public class Boss : MonoBehaviour
{

    public GameObject[] wayPoints;
    public GameObject[] wayPointsBattle;

    bool firstHit = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Boss.health = bossHealth.maxHealth;
    }

    // Update is called once per frame
    public float moveSpeed = 0.004f;
    public float turnSpeed = 360f;
    public float arrivalThreshold = 0.1f;
    public static float health = 1000;

    private int currentWaypointIndex = 0;

    //private void OnCollisionEnter(Collision collision)
    //{
    //    Debug.Log($"Hiiiiiiiiiiiiiiiiiiit!!!!!!!!!!! by {collision.gameObject.name} it's tag is {collision.gameObject.tag}");

    //    if(collision.gameObject.CompareTag("throwables"))
    //    {
    //        if (!firstHit)
    //        {
    //            currentWaypointIndex = 0;
    //            firstHit = true;
    //        }
    //        health -= 200 / (map_manager.level + 1);
    //    }
    //}
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("throwables"))
        {
            //GetComponent<Animator>().SetBool("base", false);
            //if (!firstHit)
            //{

            //    GetComponent<Animator>().SetTrigger("angery");
            //    //currentWaypointIndex = 0;
            //    //firstHit = true;
            //}
            //else
            //    GetComponent<Animator>().SetTrigger("hit");
            health -= 200 / (map_manager.level + 1) ;
        }
    }

    void FixedUpdate()
    {

        Animator anim = GetComponent<Animator>();

        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        if (!stateInfo.IsName("default") && stateInfo.normalizedTime >= 1.0f)
            anim.SetBool("base", true);


        if (stateInfo.IsName("default"))
        {
            // Get the target position

            GameObject[] targetPoints =  firstHit ? wayPointsBattle : wayPoints;
        
            Transform targetWaypoint = targetPoints[currentWaypointIndex].transform;
            Vector3 targetPosition = targetWaypoint.position;

            // Move towards target
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.fixedDeltaTime * 15);

            // Rotate toward target
            Vector3 direction = (targetPosition - transform.position).normalized;
            if (direction.magnitude > 0.01f)
            {
                Quaternion lookRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, turnSpeed * Time.fixedDeltaTime);
            }

            // Check if reached the waypoint
            if (Vector3.Distance(transform.position, targetPosition) < arrivalThreshold)
            {
                currentWaypointIndex++;
                currentWaypointIndex %= targetPoints.Length;
            }

        }

    }

    
}
