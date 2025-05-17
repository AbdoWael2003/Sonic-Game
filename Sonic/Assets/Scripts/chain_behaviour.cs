using System.Collections;
using UnityEngine;

public class chain_behaviour : MonoBehaviour
{

    public GameObject sonic;
    public GameObject knuckles;
    public GameObject tails;
    public GameObject ring;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    float speed = 5f;
    bool increment = true;
    // Update is called once per frame
    void Update()
    {
        if(tag == "chain")
            transform.Rotate(0, speed, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the other object has a Rigidbody
        Rigidbody rb = other.GetComponent<Rigidbody>();
        if (rb != null)
        {
            // Calculate direction from this object to the other
            Vector3 direction = other.transform.position - transform.position;

            // Normalize and invert to push in the opposite direction
            Vector3 pushDirection = direction.normalized;

            Vector3 appliedForce = 12 * pushDirection * (map_manager.level + 1);

            appliedForce = new Vector3(appliedForce.x, System.Math.Max(-5, appliedForce.y), appliedForce.z); // To prevent the player from being swallowed by the ground

            // Apply force
            rb.AddForce(appliedForce, ForceMode.Impulse);

            // Decrease health and lose all the coins
            PlayerPhysics.health -= (map_manager.level + 1) * 10;
            if (Score.score > 0)
                DropCoins();
            Score.score = 0;
        }
    }


    public Vector3 GetPosition()
    {
        Vector3 mainPosition;

        if (PlayerPhysics.current_player_index == 0)
        {
            mainPosition = sonic.transform.position;
        }
        else if (PlayerPhysics.current_player_index == 1)
        {
            mainPosition = knuckles.transform.position;
        }
        else
        {
            mainPosition = tails.transform.position;
        }

        return mainPosition;
    }

    void DropCoins()
    {



        GameObject[] dropedRings = new GameObject[16];

        for (int i = 0; i < 8; i++)
        {
            dropedRings[i] = Instantiate(ring, GetPosition(), ring.transform.rotation);
            dropedRings[i].GetComponent<SphereCollider>().enabled = false;
            StartCoroutine(DropCoinAnimation(dropedRings[i]));
        }

    }

    IEnumerator DropCoinAnimation(GameObject ring)
    {

        Vector3 popVector = new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(0, 1.5f), UnityEngine.Random.Range(-1f, 1f));
        bool blinkFlag = true;

        while (true)
        {


            for (int i = 0; i < 20; i++)
            {
                yield return new WaitForSeconds(0.05f);
                ring.transform.position += popVector / 3;

            }

            for (int i = 0; i < 50; i++)
            {

                yield return new WaitForSeconds(0.05f);
                ring.transform.position += new Vector3(popVector.x / 3, -1 * popVector.y / 3, popVector.z / 3);
                if (blinkFlag)
                    ring.SetActive(false);
                else
                    ring.SetActive(true);

                blinkFlag = !blinkFlag;
            }
            Destroy(ring);
        }

    }
}

