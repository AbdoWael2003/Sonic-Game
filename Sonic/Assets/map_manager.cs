using UnityEngine;
using System;
using Unity.VisualScripting;
using UnityEditorInternal;
using System.Collections;
using UnityEditor.Playables;
using System.Collections.Generic;

public class map_manager : MonoBehaviour
{


    public GameObject sonic;
    public GameObject knuckles;
    public GameObject tails;

    public GameObject ring;
    public GameObject straightBlock;

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

    public GameObject[] tiles;
    public GameObject[] villains;
    public float[] lengths;
    public float zSpawn = 0;
    public float tileLength = 1;

    private int cnt = 1;

    private GameObject block1;
    private GameObject block2;
    private GameObject block3;

    public int level = 0;
    System.Random rand;


    private class Limits{
        public float startBoundary { get; set; }
        public float endBoundary { get; set; }
        public float leftBoundary { get; set; }
        public float rightBoundary { get; set; }
        public float upperBoundary { get; set; }
        public float lowerBoundary { get; set; }
    }

    private List<KeyValuePair<KeyValuePair<GameObject, Limits>,Vector3>> eggmen = new List<KeyValuePair<KeyValuePair<GameObject, Limits>, Vector3>>(); 



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        rand = new System.Random();
        StartCoroutine(SwitchDirection());

        lengths = new float[tiles.Length];
        lengths[0] = 25;
        lengths[1] = 36;
        lengths[2] = 25;

        block1 = SpawnTile(0);
        block2 = SpawnTile(1);
        block3 = SpawnTile(2);

    }
    
    // Update is called once per frame
    void Update()
    {
        float current_z = GetCurrentPlayerObject().transform.position.z;
        //Debug.Log($"current_z = {current_z}");
        if (current_z >= cnt * 30)
        {
            level += 1;
            for (int i = 0; i < 3; i++)
            {
                int index = UnityEngine.Random.Range(0, 3);
                SpawnTile(index);
            }
            cnt++;
        }


        for(int i = 0; i < eggmen.Count; i++)
        {

            float x = eggmen[i].Value.x;
            float y = eggmen[i].Value.y;
            float z = eggmen[i].Value.z;



            Vector3 velocity = new Vector3(x, y, z);



            Vector3 pos = eggmen[i].Key.Key.transform.position;

            Vector3 newPosition = pos + velocity;

       



       

            // X boundaries
            if (pos.x <= eggmen[i].Key.Value.leftBoundary || pos.x >= eggmen[i].Key.Value.rightBoundary)
            {
                velocity.x *= -1;
            }

            // Y boundaries
            if (pos.y <= eggmen[i].Key.Value.lowerBoundary || pos.y >= eggmen[i].Key.Value.upperBoundary)
            {
                velocity.y *= -1;
            }

            // Z boundaries
            if (pos.z <= eggmen[i].Key.Value.startBoundary || pos.z >= eggmen[i].Key.Value.endBoundary)
            {
                velocity.z *= -1;
            }

            eggmen[i] = KeyValuePair.Create(KeyValuePair.Create(eggmen[i].Key.Key, eggmen[i].Key.Value), velocity);
            eggmen[i].Key.Key.transform.position += velocity;
            eggmen[i].Key.Key.transform.Rotate(0, velocity.x * 5, 0);


        }
    }


    enum Formations
    {
        HorizontalGrid = 0,
        VerticalGrid = 1,
        concave = 2
    }


    
    public GameObject SpawnWasp(float blockX, float blockY, float blockZ, float blockWidth, float blockLength, string blockName)
    {

        bool movingRight = (rand.Next(0,2) == 0);

      
        
        float x = movingRight ? blockX : blockX + blockWidth;


        float y = Math.Min(11, blockY + 0.5f + (blockName == "r(Clone)" ? 7f : 0f) + rand.Next(0,6));
        float z = UnityEngine.Random.Range(blockZ, blockZ + blockLength);


        Vector3 position = new Vector3(x, y, z);

        GameObject villain = Instantiate(villains[0], position, villains[0].transform.rotation);
        float speed = 0.12f + (level * 0.1f);
        Flip(villain);
        StartCoroutine(MoveWasp(villain, blockX, blockX + blockWidth + 5f, speed));

        return villain;
    }
    public GameObject SpawnEggmobile(float blockX, float blockY, float blockZ, float blockWidth, float blockLength, string blockName)
    {
        float x = UnityEngine.Random.Range(blockX, blockX + blockWidth); // Any x-position
        float y = Math.Min(11, blockY + 0.5f + (blockName == "r(Clone)" ? 7f : 0f) + rand.Next(0, 6));
        float z = UnityEngine.Random.Range(blockZ, blockZ + blockLength);

        Vector3 position = new Vector3(x, y, z);
        GameObject villain = Instantiate(villains[1], position, villains[1].transform.rotation);

        Vector3 velocity = new Vector3(
           UnityEngine.Random.Range(-0.1f, 0.1f),
           UnityEngine.Random.Range(-0.1f, 0.1f),
           UnityEngine.Random.Range(-0.1f, 0.1f)
        );

        Limits limits = new Limits();

        limits.startBoundary = blockZ + 5f;
        limits.endBoundary = blockZ + blockLength;
        limits.leftBoundary = blockX + 5f;
        limits.rightBoundary = blockX + blockWidth + 5f;
        limits.lowerBoundary = blockY + 5f;
        limits.upperBoundary = 15f;


        eggmen.Add(KeyValuePair.Create(KeyValuePair.Create(villain, limits), velocity));

        float speed = 0.12f + (level * 0.1f);

        //StartCoroutine(MoveEggmobile(villain,blockZ, blockZ + blockLength + 5f, blockX, blockX + blockWidth + 5f, 15f, 1.5f, speed));

        return null;
    }

    void Flip(GameObject wasp)
    {
        Vector3 scale = wasp.transform.localScale;
        scale.x *= -1;
        wasp.transform.localScale = scale;
    }

    IEnumerator MoveWasp(GameObject villain, float leftBoundary, float rightBoundary, float speed)
    {

        bool isMovingRight = true;
        while (true)
        {
          
            // Move wasp
            //Debug.Log($"{wasp.name} dir = {isMovingRight}");
            Vector3 direction = isMovingRight ? Vector3.right : Vector3.left;
            villain.transform.Translate(direction * speed);

            // Check boundaries and flip direction
            if (villain.transform.position.x >= rightBoundary)
            {
                isMovingRight = false;
                Flip(villain);
            }
            if (villain.transform.position.x <= leftBoundary)
            {
                isMovingRight = true;
                Flip(villain);
            }

            yield return null; // wait for next frame
        }
    }
    

    Vector3 seed = new Vector3();

    IEnumerator SwitchDirection()
    {
        while (true)
        {
            yield return new WaitForSeconds(2f);

            seed = new Vector3(
                UnityEngine.Random.Range(-0.1f, 0.2f),
                UnityEngine.Random.Range(-0.1f, 0.2f),
                UnityEngine.Random.Range(-0.1f, 0.2f)
            );

            for(int i = 0; i < eggmen.Count; i++)
            {
                Vector3 velocity = new Vector3(
                    UnityEngine.Random.Range(-0.1f, 0.1f),
                    UnityEngine.Random.Range(-0.1f, 0.1f),
                    UnityEngine.Random.Range(-0.1f, 0.1f)
               );


                eggmen[i] = KeyValuePair.Create(KeyValuePair.Create(eggmen[i].Key.Key, eggmen[i].Key.Value), velocity);
            }
        }
    }

   

    public void SpawnVillains(float blockX, float blockY, float blockZ, float blockWidth, float blockLength, string blockName)
    {

        for (int i = 0; i < 3 + level; i++)
        { 
            GameObject villain = SpawnWasp(blockX, blockY, blockZ, blockWidth, blockLength, blockName);
        }

        for(int i = 0; i < (level / 3); i++)
        {
            GameObject villain = SpawnEggmobile(blockX, blockY, blockZ, blockWidth, blockLength, blockName);

        }

    }

    public GameObject SpawnTile(int tileIndex)
    {
        GameObject obj = Instantiate(tiles[tileIndex],transform.forward * zSpawn, transform.rotation);
        if (obj.name == "w(Clone)")
            obj.transform.localScale = new Vector3(1, 4, 1);

        obj.transform.localScale = obj.transform.localScale + new Vector3(rand.Next(0, 4), 0, 0);


        bool leftTrack = rand.Next(0, 100) <= 80 ? true : false;
        bool midTrack = rand.Next(0, 100) <= 80 ? true : false;
        bool rightTrack = rand.Next(0, 100) <= 80 ? true : false;

        if (leftTrack == false && rightTrack == false && midTrack == false)
            midTrack = true;



        GameObject target = obj;

        if(obj.name == "w(Clone)")
            target = obj.transform.Find("s")?.gameObject;

        float blockWidth = target.GetComponentInChildren<Renderer>().bounds.size.x;
        float blockLength = target.GetComponentInChildren<Renderer>().bounds.size.z;
        float blockX = target.transform.position.x;
        float blockY = target.transform.position.y;
        float blockZ = target.transform.position.z;


        SpawnVillains(blockX, blockY, blockZ, blockWidth, blockLength, obj.name);

        float coinWidth = ring.GetComponentInChildren<Renderer>().bounds.size.x;
        
        float xPos = target.transform.position.x + ((blockWidth) / 2f) - coinWidth / 2;
        //Debug.Log($"xPos = {xPos}");

        Formations form = (Formations)rand.Next(0, 3);

        //form = Formations.concave;


        bool isRamp = (obj.name == "r(Clone)");
        bool isWall = (obj.name == "w(Clone)");

       


        int rows = rand.Next(1, 7);
        int columns = rand.Next(1, 5);


        while(form == Formations.concave && rows % 2 == 0) rows = rand.Next(1, 7);
        

        int[] gaussian = new int[rows];
        for(int i = 0, j = 0; i < rows; i++)
        {
            gaussian[i] = j;
            if (i >= rows / 2)
                j--;
            else
                j++;
            
        }


        for (int i = 0; i < rows; i++)
        {

            if(midTrack)
                if(form == Formations.HorizontalGrid)
                    Instantiate(ring, new Vector3(xPos + (isWall ?  5f : 0) - 2.2f, 1.43f * (isRamp ?  8.5f : 1), obj.transform.position.z + (isRamp ?  5f : 0) + 10 + 2.4f * i) , ring.transform.rotation);
                else if(form == Formations.VerticalGrid)
                {
                    for(int j = 0; j < columns; j++)
                    {
                        Instantiate(ring, new Vector3(xPos + (isWall ?  5f : 0) - 2.2f, 1.43f * (isRamp ?  8.5f : 1) + i, obj.transform.position.z + (isRamp ?  5f : 0) + 10 + 2.4f * j), ring.transform.rotation);
                    }
                }
                else if(form == Formations.concave)
                    Instantiate(ring, new Vector3(xPos + (isWall ?  5f : 0) - 2.2f, 1.43f * (isRamp ?  8.5f : 1) + gaussian[i], obj.transform.position.z + (isRamp ?  5f : 0) + 10 + 2.4f * i), ring.transform.rotation);
            if (leftTrack)
                if (form == Formations.HorizontalGrid)
                    Instantiate(ring, new Vector3(xPos + (isWall ?  5f : 0), 1.43f * (isRamp ?  8.5f : 1), obj.transform.position.z + (isRamp ?  5f : 0) + 10 + 2.4f * i), ring.transform.rotation);
                else if (form == Formations.VerticalGrid)
                {
                    for (int j = 0; j < columns; j++)
                    {
                        Instantiate(ring, new Vector3(xPos + (isWall ?  5f : 0), 1.43f * (isRamp ?  8.5f : 1) + i, obj.transform.position.z + (isRamp ?  5f : 0) + 10 + 2.4f * j), ring.transform.rotation);
                    }
                }
                else if (form == Formations.concave)
                    Instantiate(ring, new Vector3(xPos + (isWall ?  5f : 0), 1.43f * (isRamp ?  8.5f : 1) + gaussian[i], obj.transform.position.z + (isRamp ?  5f : 0) + 10 + 2.4f * i), ring.transform.rotation);

            if (rightTrack)
                if (form == Formations.HorizontalGrid)
                    Instantiate(ring, new Vector3(xPos + (isWall ?  5f : 0) + 2.2f, 1.43f * (isRamp ?  8.5f : 1), obj.transform.position.z + (isRamp ?  5f : 0) + 10 + 2.4f * i), ring.transform.rotation);
                else if (form == Formations.VerticalGrid)
                {
                    for (int j = 0; j < columns; j++)
                    {
                        Instantiate(ring, new Vector3(xPos + (isWall ?  5f : 0) + 2.2f, 1.43f * (isRamp ?  8.5f : 1) + i, obj.transform.position.z + (isRamp ?  5f : 0) + 10 + 2.4f * j), ring.transform.rotation);
                    }
                }
                else if (form == Formations.concave)
                    Instantiate(ring, new Vector3(xPos + (isWall ?  5f : 0) + 2.2f, 1.43f * (isRamp ?  8.5f : 1) + gaussian[i] , obj.transform.position.z + (isRamp ?  5f : 0) + 10 + 2.4f * i), ring.transform.rotation);


        }

        //Debug.Log(obj.name);
        //Debug.Log(obj.transform.position.z + (isRamp ?  5f : 0));
        zSpawn += lengths[tileIndex];


        return obj;
    }
}
