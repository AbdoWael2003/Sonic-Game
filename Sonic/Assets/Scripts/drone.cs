using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class drone : MonoBehaviour
{

    public GameObject sonic;
    public GameObject knuckles;
    public GameObject tails;

    public GameObject deathPoint;

    GameObject GetCurrentPlayerObject()
    {
        switch (PlayerPhysics.current_player_index)
        {
            case 0:
                return sonic;
            case 1:
                return knuckles;
            case 2:
                return tails;
            default:
                return sonic;
        }
    }

    private NavMeshAgent agent;

    private Vector3 originPosition;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        originPosition = transform.position;
    }

    private void Update()
    {
        agent.speed = (map_manager.level + 2);
        agent.speed = System.Math.Max(agent.speed, 7);
    }
    // Update is called once per frame
    private bool die = false;
    void FixedUpdate()
    {
        // to get the ratio
        //Debug.Log(Vector3.Distance(deathPoint.transform.position, agent.transform.position));

        GameObject currentPlayer = GetCurrentPlayerObject();
        Rigidbody rb = currentPlayer.GetComponent<Rigidbody>();
        



        //Debug.Log($"xdiff = {System.Math.Abs(agent.transform.position.x - currentPlayer.transform.position.x)}");
        //Debug.Log($"zdiff = {System.Math.Abs(agent.transform.position.z - currentPlayer.transform.position.z)}");
        //Debug.Log($"player x = {currentPlayer.transform.position.x}");
        //Debug.Log($"agent x = {agent.transform.position.x}");
        //Debug.Log($"player z = {currentPlayer.transform.position.z}");
        //Debug.Log($"agent z = {agent.transform.position.z}");
     

        if (PlayerPhysics.movementDisabled && Vector3.Distance(deathPoint.transform.position, agent.transform.position) < 2)
        {
            die = true;

            PlayerPhysics.movementDisabled = false;
            //Debug.Log("ENDDDDDDDDDDD!");
            // Move using MovePosition to respect physics
            map_manager.bossFight = false;
            currentPlayer.SetActive(true);
            rb.MovePosition(transform.position + 5f * Vector3.up);
            rb.useGravity = true;
            currentPlayer.GetComponentInChildren<Animator>().SetBool("jump2", true);

            // Then set its downward velocity
            //Coroutine routine = StartCoroutine(Drop(currentPlayer));
            //if (currentPlayer.transform.position.y < -100)
            //    StopCoroutine(routine);
            return;
        }
        if (
            !die &&
            (
                 PlayerPhysics.movementDisabled ||
                (System.Math.Abs(agent.transform.position.x - currentPlayer.transform.position.x) < 1.4f && System.Math.Abs(agent.transform.position.z - currentPlayer.transform.position.z) < 1.4f)
            )
        )
        {
            PlayerPhysics.movementDisabled = true;
            // to Disable the Ai chasing Behaviour Down there!
            PlayerPhysics.isChased = false;

            rb.MovePosition(new Vector3(agent.transform.position.x, currentPlayer.transform.position.y, agent.transform.position.z));
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            //Coroutine routine = StartCoroutine(Take(currentPlayer));
            if (transform.position.y + 3f > currentPlayer.transform.position.y)
            {
                rb.useGravity = false;
                rb.MovePosition(rb.position + Vector3.up * 0.05f);
            }
            else
            {
                currentPlayer.SetActive(false);
                agent.SetDestination(deathPoint.transform.position);
            }
            return;
        }

        // Ai Behaviour
        if (PlayerPhysics.isChased)
        {
            agent.SetDestination(currentPlayer.transform.position);
        }
        else
        {
            agent.SetDestination(originPosition);
        }
    }

    //IEnumerator Take(GameObject obj)
    //{
    //    while (true)
    //    {
    //        yield return new WaitForFixedUpdate();
    //    }
    //}

    //IEnumerator Drop(GameObject obj)
    //{

    //    while(true)
    //    {
    //        obj.transform.position += Vector3.down * Time.fixedDeltaTime * 0.2f;
    //        yield return new WaitForFixedUpdate();
    //    }
    //}
}
