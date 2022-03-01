using System.Collections;
using UnityEngine;

public class Movement : MonoBehaviour
{

    private bool sprinting, bogged, exit, sneak, groundedPlayer, idle, flight, flightRestriction = false;
    private float speed;
    private float baseSpeed = 10f;
    private float flightBoost = 3.0f;
    private float sprintBoost = 2.0f;
    private float jumpSpeed = 10.0F;
    private float gravity = 20.0F;
    private Vector3 moveDirection = Vector3.zero;
    private GameObject player;
    private float notGroundedTime;
    private Vector3 playerVelocity;
    private float minFallTime = 1f;
    private float maxFallTime = 5f;
    private float heightBeforeDrop;


    CharacterController controller;
    Animator anim;
    private Vector3 prevPos;
    private Vector3 currVel;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        player = GameObject.FindGameObjectWithTag("Player");
        anim = GetComponent<Animator>();
        StartCoroutine(CalcVelocity());
        //Set speed
            speed = baseSpeed;
    }
    IEnumerator CalcVelocity()
    {
        while (Application.isPlaying)
        {
            // Position at frame start
            prevPos = transform.position;
            // Wait till it the end of the frame
            yield return new WaitForEndOfFrame();
            // Calculate velocity: Velocity = DeltaPosition / DeltaTime
            currVel = (prevPos - transform.position) / Time.deltaTime;
        }
    }

    void Update()
    {
        // is the controller on the ground?
        if (controller.isGrounded)
        {
            flight = false;
            anim.SetBool("Flight", false);

            //Feed moveDirection with input.
            moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            moveDirection = transform.TransformDirection(moveDirection);
            //Multiply it by speed.
            moveDirection *= speed;
            //Jumping
            if (Input.GetButtonDown("Jump"))
            {
                //Start Flying
                if (!flight)
                {
                    moveDirection.y = jumpSpeed;
                    anim.SetTrigger("Jumping");
                    anim.SetBool("Sneaking", false);
                }
            }
            //Punching
            if (Input.GetButton("Fire1"))
                anim.SetBool("Boxing", true);
            else anim.SetBool("Boxing", false);
            //Kicking
            if (Input.GetButton("Fire2"))
                anim.SetBool("Kicking", true);
            else anim.SetBool("Kicking", false);

            //Sneaking
            if (Input.GetKeyDown(KeyCode.C))
            {
                if (!sneak)
                {
                    sneak = true;
                    anim.SetBool("Sneaking", true);
                    anim.SetBool("Sprinting", false);
                }
                else
                {
                    sneak = false;
                    anim.SetBool("Sneaking", false);
                }
            }
            //Gesturing
            if (Input.GetKeyDown(KeyCode.G))
            {
                anim.SetTrigger("Emote");
            }
        }
        if (!controller.isGrounded && !flightRestriction)
        {
            if (Input.GetButtonDown("Jump"))
            {
                flight = true;
                anim.SetBool("Flight", true);
            }
        }
        //Flying
        if (flight)
        {
            jumpSpeed = 30f;
            gravity = 0;
            //Feed moveDirection with input.
            moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            moveDirection = transform.TransformDirection(moveDirection);
            //Multiply it by speed.
            moveDirection *= speed;

            //Up and Down movement
            if (Input.GetButton("Jump"))
            {
                moveDirection.y = jumpSpeed;
            }
            else if (Input.GetKey(KeyCode.LeftAlt))
            {
                moveDirection.y = -jumpSpeed;
            }
            else
            {
                moveDirection.y = 0;
            }
        }
        //End Flying
        else
        {
            gravity = 20;
            jumpSpeed = 10;
        }

        //Applying gravity to the controller
        moveDirection.y -= gravity * Time.deltaTime;
        //Making the character move
        controller.Move(moveDirection * Time.deltaTime);

        //Idling
        if ((currVel.x == 0) || (currVel.z == 0))
        {
            idle = true;
            anim.SetBool("Idle", true);
        }
        else
        {
            anim.SetBool("Idle", false);
            idle = false;
        }

        //Animating Movement
        if (Input.GetKey(KeyCode.W))
        {
            anim.SetBool("Moving forward", true);
        }
        else anim.SetBool("Moving forward", false);
        if (Input.GetKey(KeyCode.A))
        {
            anim.SetBool("Moving left", true);
        }
        else anim.SetBool("Moving left", false);
        if (Input.GetKey(KeyCode.S))
        {
            anim.SetBool("Moving backward", true);
        }
        else anim.SetBool("Moving backward", false);
        if (Input.GetKey(KeyCode.D))
        {
            anim.SetBool("Moving right", true);
        }
        else anim.SetBool("Moving right", false);

        //Sprinting and flying faster
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            sprinting = true;
            if (flight)
            {
                speed = (baseSpeed * flightBoost);
            }
            else if (!flight)
            {
                speed = (baseSpeed * sprintBoost);
            }
            anim.SetBool("Sprinting", true);
            anim.SetBool("Sneaking", false);
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            speed = baseSpeed;
            anim.SetBool("Sprinting", false);
            sprinting = false;
        }
        UpdateGroundedState();
    }
    //Evironmental Effects
    private void OnTriggerEnter(Collider other)
    {
        //Slowing down when in water
        if (other.CompareTag("Water"))
        {
            bogged = true;
        }
        //Win condition
        if (other.CompareTag("Access"))
        {
            exit = true;
        }
    }
    //Prevent player from "Falling" after a small jump
    private void UpdateGroundedState()
    {

        // transitioning from grounded to not grounded
        if (groundedPlayer && controller.isGrounded == false)
        {
            notGroundedTime += Time.deltaTime;
        }
        // previously grounded and still grounded
        else if (!groundedPlayer && controller.isGrounded == false)
        {
            notGroundedTime += Time.deltaTime;
        }
        // transitioning from not grounded to grounded
        else if (!groundedPlayer && controller.isGrounded == true)
        {
            notGroundedTime = 0.0f;
        }
        //Detect if Player is actually Falling
        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0.0f;
        }
        ProcessFalling();
    }
    //Player Falling aand Landing
    private void ProcessFalling()
    {
        if ((!groundedPlayer && notGroundedTime >= minFallTime))
        {
            heightBeforeDrop = this.transform.position.y;
            anim.SetBool("Grounded", false);
            if (!groundedPlayer && notGroundedTime >= maxFallTime)
            {
                anim.SetBool("Splat", false);
                //hp - (notGroundedTime * fallDamage);
            }
            else
            {
                anim.SetBool("Splat", true);
            }
        }
        //Prevent player from "Falling" while Flying
        else if (!flight)
        {
            anim.SetBool("Grounded", true);
        }
        //else anim.SetBool("Flight", true);

        //allow the player to start flying after a jump without falling
        if (!groundedPlayer && notGroundedTime >= (minFallTime / 4))
        {
            flightRestriction = false;
        }
        else flightRestriction = true;
    }
}
