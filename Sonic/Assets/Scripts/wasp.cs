using System.Collections;
using System.Net.NetworkInformation;
using UnityEngine;

public class wasp : MonoBehaviour
{


    public GameObject ring;

    public GameObject sonic;
    public GameObject knuckles;
    public GameObject tails;


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
                
            PlayerPhysics.health -= (map_manager.level + 1) * 10;
            if(Score.score > 0)
                DropCoins();
            Score.score = 0;
            //Debug.Log($"Pushed {collision.gameObject.name} with force!");
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

        for(int i = 0; i < 8; i++)
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

        while(true)
        {


            for(int i = 0; i < 20; i++)
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

