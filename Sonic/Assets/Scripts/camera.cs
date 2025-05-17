using System.Runtime.Serialization.Json;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class camera : MonoBehaviour
{
    public Transform target;        // The character to follow
    public float smoothSpeed = 10.5f; // How smoothly the camera moves

    public GameObject sonic;
    public GameObject knuckles;
    public GameObject tails;

    public GameObject bot;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        target = sonic.transform;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        //Debug.Log("I'm here!!");
        if(PlayerPhysics.current_player_index == 0)
            target = sonic.transform;
        if(PlayerPhysics.current_player_index == 1)
            target = knuckles.transform;
        if(PlayerPhysics.current_player_index == 2)
            target = tails.transform;

        if (PlayerPhysics.movementDisabled)
            target = bot.transform;

        if (target == null)
            return;

        // Calculate desired camera position relative to character's rotation
        Vector3 offset = new Vector3(0, 3, -7);
        Vector3 desiredPosition = target.TransformPoint(offset);



        // Smoothly interpolate to desired position
        transform.position = Vector3.Lerp(transform.position, desiredPosition, 5000);

        // Make the camera look at the target
        transform.LookAt(target);
    }

    //void update()
    //{
    //    if (target == null)
    //        return;

    //    // Calculate desired camera position relative to character's rotation
    //    Vector3 desiredPosition = target.TransformPoint(offset);

    //    // Smoothly interpolate to desired position
    //    transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

    //    // Make the camera look at the target
    //    transform.LookAt(target);
    //}
}
