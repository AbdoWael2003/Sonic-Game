using UnityEngine;
using System;
using System.Collections;
using UnityEditor.Playables;
using Unity.VisualScripting;

public class PlayerPhysics : MonoBehaviour
{


    // Player references
    public GameObject sonic;
    public GameObject knuckles;
    public GameObject tails;

    // Rigidbodies references
    public Rigidbody rigid_body_sonic;
    public Rigidbody rigid_body_knuckles;
    public Rigidbody rigid_body_tails;
    

    public knuckles knuckles_script;
    public tails_behaviour tails_script;
 

    // Camera reference
    public Camera mainCamera;

    // Movement parameters
    public float moveSpeed = 5f;
    public float strafeSpeed = 5f; // Speed for left/right movement
    public float jumpForce = 10f; // Increased jump force
    public float switchDelay = 0.2f;

    // Character switching parameters
    private float switchTimer = 0f;
    private bool isMoving = false;
    public static int current_player_index = 0;

    // Ground check parameters
    public float groundCheckDistance = 0.3f;
    public LayerMask groundLayer; // Set this in the inspector to include ground objects

    // Original positions (for reference only)
    private float[][] original_positions = new float[][] {
        new float[] { -0.17f, 1.337f, -7.52f},
        new float[] { 2.36f, 0.89f, -7.52f},
        new float[] { -2.55f, 1.013f, -7.52f}
    };

    // Character names
    private string[] players = new string[] { "sonic", "knuckles", "tails" };

    // Camera follow parameters
    public float cameraFollowSpeed = 5f;
    public Vector3 cameraOffset = new Vector3(0, 3, -5);

    private float switchCooldown = 1.5f;
    private float lastSwitchTime = -Mathf.Infinity;

    private bool onGround = true;

    private bool sonicBoost = false;

    float last_y;

    private void Start()
    {

        last_y = GetCurrentPlayerObject().transform.position.y;
        StartCoroutine(clock());

        // Initialize camera reference if not set
        if (mainCamera == null)
            mainCamera = Camera.main;

        // Initialize rigidbodies if not set in inspector
        if (rigid_body_sonic == null && sonic != null)
            rigid_body_sonic = sonic.GetComponent<Rigidbody>();

        if (rigid_body_knuckles == null && knuckles != null)
            rigid_body_knuckles = knuckles.GetComponent<Rigidbody>();

        if (rigid_body_tails == null && tails != null)
            rigid_body_tails = tails.GetComponent<Rigidbody>();

        // Set up all rigidbodies
        SetupRigidbodies();

       
        // Initialize active character
        //ActivateCurrentCharacter();
    }
    IEnumerator clock()
    {
        while (true)
        {
            last_y = transform.position.y;
            yield return new WaitForSeconds(0.05f);
            if (last_y - transform.position.y >= 0.05)
            {
                GetComponentInChildren<Animator>().SetBool("jump2", true);

            }
            else
            {

                GetComponentInChildren<Animator>().SetBool("jump2", false);
            }
        }
    }

    private void SetupRigidbodies()
    {
        // Make sure rigidbodies have proper settings
        ConfigureRigidbody(rigid_body_sonic);
        ConfigureRigidbody(rigid_body_knuckles);
        ConfigureRigidbody(rigid_body_tails);
    }

    private void ConfigureRigidbody(Rigidbody rb)
    {
        if (rb != null)
        {
            rb.constraints = RigidbodyConstraints.FreezeRotation; // Prevent character from tipping over
            rb.interpolation = RigidbodyInterpolation.Interpolate; // Smoother movement
        }
    }

    private void Update()
    {

        //Debug.Log($"onGround = {onGround}");
        // Update switch cooldown
        //if (switchTimer > 0)
        //    switchTimer -= Time.deltaTime;

        // Jump control with spacebar

        //if (Input.GetKeyDown(KeyCode.X) && Time.time - lastSwitchTime >= switchCooldown)
        //{
        //    SwitchCharacter();
        //    lastSwitchTime = Time.time;
        //}

        if (Input.GetKeyDown(KeyCode.X) && canSwitch)
        {

            StartCoroutine(SwitchWithCooldown());
        }

        if (Input.GetKeyDown(KeyCode.Space) && GetCurrentPlayer().transform.position.y <= 11 && GetCurrentPlayer().transform.position.y >= 0)
        {
            Jump();

        }

        if (Input.GetKey(KeyCode.Z) && current_player_index == 0 && sonic.transform.position.y <= 6 && sonic.transform.position.y >= 1.2)
        {

            GetComponentInChildren<Animator>().SetBool("boosting", true);
            sonicBoost = true;
        }
        else
        {
            GetComponentInChildren<Animator>().SetBool("boosting", false);
            sonicBoost = false;
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
       

        string name = collision.gameObject.name;
        //Debug.Log(name);
        if (name.Contains("ramp") || name.Contains("straight") || name.Contains("target") || name.Contains("floor"))
        {
            //Debug.Log("Ground Touched!");
            onGround = true;
          
        }
    }
    private bool canSwitch = true;

    IEnumerator SwitchWithCooldown()
    {
        canSwitch = false;
        switchRequest = true;
        yield return new WaitForSeconds(switchCooldown);
        canSwitch = true;
    }


    bool switchRequest = false;


    private float movementTimer = 0f;
    private void FixedUpdate()
    {

        movementTimer += Time.fixedDeltaTime;
        if (movementTimer >= 0.001f)
        {
            tails_script.animator.SetBool("running", false);
            knuckles_script.animator.SetBool("running", false);
            GetComponentInChildren<Animator>().SetBool("running", false);
            movementTimer = 0;
        }

        GameObject currentPlayerObj = GetCurrentPlayerObject();
        Rigidbody currentRigidbody = GetCurrentPlayer();

        //Debug.Log(currentPlayerObj);
        //Debug.Log(currentRigidbody);

        // Forward and backward movement
        float verticalInput = 0;
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            verticalInput = 1;
        else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            verticalInput = -1;

        // Left and right movement
        float horizontalInput = 0;
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            horizontalInput = -1;
        else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            horizontalInput = 1;

        // Rotate based on movement direction (if moving)
        if (verticalInput != 0 || horizontalInput != 0)
        {

            if(current_player_index == 0)
            {
                if (!GetComponentInChildren<Animator>().GetBool("jump1") && !GetComponentInChildren<Animator>().GetBool("jump2") && !GetComponentInChildren<Animator>().GetBool("boosting"))
                {
                    GetComponentInChildren<Animator>().SetBool("running", true);
                }
            }
            if(current_player_index == 1)
            {
                if (!knuckles_script.animator.GetBool("jump1") && !knuckles_script.animator.GetBool("jump2") && !knuckles_script.animator.GetBool("hit"))
                {
                    knuckles_script.animator.SetBool("running", true);
                }
                movementTimer = 0f;
            }
            if (current_player_index == 2)
            {
                if (!tails_script.animator.GetBool("jump1") && !tails_script.animator.GetBool("jump2"))
                {
                    tails_script.animator.SetBool("running", true);
                }
                movementTimer = 0f;
            }
            

            float targetRotation = Mathf.Atan2(horizontalInput, verticalInput) * Mathf.Rad2Deg;
            //currentPlayerObj.transform.rotation = Quaternion.Euler(0, targetRotation, 0);
            sonic.transform.rotation = Quaternion.Euler(0, targetRotation, 0);
            knuckles.transform.rotation = Quaternion.Euler(0, targetRotation, 0);
            tails.transform.rotation = Quaternion.Euler(0, targetRotation, 0);

        }
       

        // Apply movement
        Vector3 moveDirection1 = new Vector3(horizontalInput * strafeSpeed, 0, verticalInput * moveSpeed);
        Vector3 moveDirection2 = new Vector3(0, 0, verticalInput * moveSpeed);
               
 
        //currentRigidbody.linearVelocity = new Vector3(moveDirection.x, currentRigidbody.linearVelocity.y, moveDirection.z);
        rigid_body_sonic.linearVelocity = new Vector3((current_player_index == 0 ? moveDirection1.x : moveDirection2.x) * (sonicBoost ? 3f : 1), rigid_body_sonic.linearVelocity.y, moveDirection1.z * (sonicBoost ? 3f : 1));
        rigid_body_knuckles.linearVelocity = new Vector3(current_player_index == 1 ? moveDirection1.x : moveDirection2.x, rigid_body_knuckles.linearVelocity.y, moveDirection1.z);
        rigid_body_tails.linearVelocity = new Vector3(current_player_index == 2 ? moveDirection1.x : moveDirection2.x, rigid_body_tails.linearVelocity.y, moveDirection1.z);


      

        // Character switching
        if (switchRequest)
        {
            switchRequest = false;
            SwitchCharacter();
        }

        // Update camera position to follow the current player
        UpdateCameraPosition(verticalInput, horizontalInput);

        //if (!(Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.S)) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow))
        //    tails_script.animator.SetBool("running", false);

    }


   

    void Jump()
    {

        if (current_player_index == 0)
            GetComponentInChildren<Animator>().SetBool("jump1", true);
        if(current_player_index == 1)
            knuckles_script.animator.SetBool("jump1", true);
        if (current_player_index == 2)
            tails_script.animator.SetBool("jump1", true);

        StartCoroutine(JumpAnimation());

        onGround = false;
        //Debug.Log("Jump function called!");
        Rigidbody currentRigidbody = GetCurrentPlayer();

        if (IsGrounded())
        {
            // Reset any downward velocity first
            Vector3 velocity = currentRigidbody.linearVelocity;
            velocity.y = 0;
            currentRigidbody.linearVelocity = velocity;

            // Apply jump force directly to velocity for more consistent jumping
            currentRigidbody.AddForce(Vector3.up * 8f, ForceMode.Impulse);

            //Debug.Log("Jump force applied! Force: " + 10f);
        }
        else
        {
            //Debug.Log("Cannot jump - not grounded!");
        }
    }

    IEnumerator JumpAnimation()
    {
        if(current_player_index == 0)
        {
            GetComponentInChildren<Animator>().SetBool("running", false);
            GetComponentInChildren<Animator>().SetBool("boosting", false);
            GetComponentInChildren<Animator>().SetBool("jump1", true);
            yield return new WaitForSeconds(0.6f);
            GetComponentInChildren<Animator>().SetBool("jump1", false);
        }
        if (current_player_index == 1)
        {
            knuckles_script.animator.SetBool("running", false);
            knuckles_script.animator.SetBool("hit", false);
            knuckles_script.animator.SetBool("jump1", true);
            yield return new WaitForSeconds(0.6f);
            knuckles_script.animator.SetBool("jump1", false);
            //tails_script.animator.SetBool("jump2", true);
            //yield return new WaitForSeconds(0.8f);
            //tails_script.animator.SetBool("jump2", false);
        }  
        if (current_player_index == 2)
        {
            tails_script.animator.SetBool("running", false);
            tails_script.animator.SetBool("flying", false);
            tails_script.animator.SetBool("jump1", true);
            yield return new WaitForSeconds(0.6f);
            tails_script.animator.SetBool("jump1", false);
            //tails_script.animator.SetBool("jump2", true);
            //yield return new WaitForSeconds(0.8f);
            //tails_script.animator.SetBool("jump2", false);
        }
    }




    bool IsGrounded()
    {

        Rigidbody currentRigidbody = GetCurrentPlayer();
        GameObject currentPlayerObj = GetCurrentPlayerObject();

        if (currentRigidbody == null || currentPlayerObj == null)
            return false;

        // Get the collider bounds
        Collider collider = currentPlayerObj.GetComponent<Collider>();
        if (collider == null)
        {
            Debug.LogWarning("No collider found on " + currentPlayerObj.name);
            return false;
        }

        // Check if any point of the collider is close to the ground
        Vector3 checkPosition = collider.bounds.center - new Vector3(0, collider.bounds.extents.y, 0);

        // Draw debug ray
        Debug.DrawRay(checkPosition, Vector3.down * groundCheckDistance, Color.red, 0.1f);

        // Use a slightly larger ground check distance than the threshold
        if (Physics.Raycast(checkPosition, Vector3.down, groundCheckDistance, groundLayer))
        {
            return true;
        }

        // Alternative ground check - check if y velocity is very small
        return Mathf.Abs(currentRigidbody.linearVelocity.y) < 0.1f;
    }
    void DisablePhysics()
    {
        // Disable collisions
        sonic.GetComponent<Collider>().enabled = false;
        knuckles.GetComponent<Collider>().enabled = false;
        tails.GetComponent<Collider>().enabled = false;

        // Make rigidbodies kinematic
        rigid_body_sonic.isKinematic = true;
        rigid_body_knuckles.isKinematic = true;
        rigid_body_tails.isKinematic = true;
    }

    void EnablePhysics()
    {
        // Re-enable collisions
        sonic.GetComponent<Collider>().enabled = true;
        knuckles.GetComponent<Collider>().enabled = true;
        tails.GetComponent<Collider>().enabled = true;

        // Make rigidbodies non-kinematic
        rigid_body_sonic.isKinematic = false;
        rigid_body_knuckles.isKinematic = false;
        rigid_body_tails.isKinematic = false;
    }
    void SwitchCharacter()
    {

        DisablePhysics();

        // Store current player's position and rotation

        Vector3 currentPosition = GetCurrentPlayerObject().transform.position; // sonic

        current_player_index += 1;
        current_player_index %= 3;

        Vector3 newPosition = GetCurrentPlayerObject().transform.position; // knuckles

        current_player_index -= 1;
        if (current_player_index == -1)
            current_player_index = 2;

        GetCurrentPlayerObject().transform.position = newPosition;

        current_player_index += 1;
        current_player_index %= 3;

        GetCurrentPlayerObject().transform.position = currentPosition;

        EnablePhysics();


        //Quaternion currentRotation = GetCurrentPlayerObject().transform.rotation;
        //Vector3 currentVelocity = GetCurrentPlayer().linearVelocity;



        // Deactivate all, increment index, activate new one
        //sonic.SetActive(false);
        //knuckles.SetActive(false);
        //tails.SetActive(false);

        //ActivateCurrentCharacter();
        //current_player_index = (current_player_index + 1) % players.Length;
        //GetCurrentPlayerObject().SetActive(true);

        // Now set transform since the object is active
        //var currentObj = GetCurrentPlayerObject();
        
        //currentObj.transform.position = currentPosition + new Vector3(current_player_index, current_player_index, current_player_index);
        //currentObj.transform.rotation = currentRotation;

        //Rigidbody rb = GetCurrentPlayer();
        //rb.linearVelocity = currentVelocity; // Preserve momentum

        Debug.Log("Switched to: " + players[current_player_index]);
        Debug.Log("old position = " + currentPosition.ToString());
        Debug.Log("new position = " + newPosition.ToString());

    
    }

    void ActivateCurrentCharacter()
    {
        // Deactivate all characters first
        if (sonic != null) sonic.SetActive(false);
        if (knuckles != null) knuckles.SetActive(false);
        if (tails != null) tails.SetActive(false);

        // Activate only the current character
        GetCurrentPlayerObject()?.SetActive(true);
    }

    void DeactivateCurrentCharacter()
    {
        GetCurrentPlayerObject().SetActive(false);
    }

    Rigidbody GetCurrentPlayer()
    {
        switch (current_player_index)
        {
            case 0:
                return rigid_body_sonic;
            case 1:
                return rigid_body_knuckles;
            case 2:
                return rigid_body_tails;
            default:
                return null;
        }
    }

    GameObject GetCurrentPlayerObject()
    {
        switch (current_player_index)
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
    GameObject GetCurrentPlayerObject(int index)
    {
        switch (current_player_index)
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

    void UpdateCameraPosition(float forwardInput, float horizontalInput)
    {
        if (mainCamera == null)
            return;

        GameObject currentPlayer = GetCurrentPlayerObject();

        // Base camera position behind the player
        Vector3 targetPosition = currentPlayer.transform.position + cameraOffset;

        // Adjust camera based on movement for a more dynamic feel
        if (forwardInput != 0 || horizontalInput != 0)
        {
            // Move camera slightly in direction of movement
            Vector3 movementOffset = new Vector3(
                horizontalInput * 0.5f,
                0,
                forwardInput * 0.5f
            );
            targetPosition += movementOffset;
        }

        //Smoothly move the camera to follow the player
        mainCamera.transform.position = Vector3.Lerp(
            mainCamera.transform.position,
            targetPosition,
            cameraFollowSpeed * Time.deltaTime
        );


        // Make camera look at the player
        mainCamera.transform.LookAt(currentPlayer.transform.position + new Vector3(0, 1, 0));

        //mainCamera.transform.LookAt(currentPlayer.transform.position);
    }

    // Add this to see debug info in the editor
    private void OnGUI()
    {
        if (Debug.isDebugBuild)
        {
            Rigidbody rb = GetCurrentPlayer();
            GUI.Label(new Rect(10, 10, 300, 20), "Character: " + players[current_player_index]);
            GUI.Label(new Rect(10, 30, 300, 20), "Grounded: " + IsGrounded());
            if (rb != null)
                GUI.Label(new Rect(10, 50, 300, 20), "Velocity: " + rb.linearVelocity.ToString("F2"));
        }
    }
}