using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody rb;

    [Header("Movement")]
    public float acceleration = 50f;
    public float speed = 60f;
    public Transform playerOreintation;
    private float horizontalInput;
    private float verticalInput;
    private Vector3 moveDirection;
    private bool isSprinting;
    private bool isCrouching;

    [Header("Jump")]
    private bool isJumping;
    public int extraJumps = 2;
    public float jumpForce = 60.0f;
    public float airMultiplier = 0.4f;
    public float jumpCooldown = 0.25f;
    private bool canJump;
    private int currentExtraJumps;

    [Header("Ground")]
    public float groundDrag = 1.5f;
    public LayerMask groundLayer;
    private bool isGrounded;

    [Header("keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode crouchKey = KeyCode.LeftControl;

    private playerStates playerState;
    enum playerStates
    {
        walking,
        running,
        inAir,
        crouching,
    }

    void Start()
    {
        this.rb = this.GetComponent<Rigidbody>();
        this.rb.mass = 10.0f;
        rb.freezeRotation = true;
        this.playerState = playerStates.walking;
        this.currentExtraJumps = 0;
    }

    // Update is called once per frame
    void Update()
    {
        this.GetPlayerInputs();
        this.SpeedControl();
        if (this.isGrounded) this.rb.drag = this.groundDrag;
        else this.rb.drag = 0.0f;
    }

    void FixedUpdate()
    {
        this.MovePlayer();
    }

    private void HandlePlayerState()
    {
        if (this.isGrounded && this.isSprinting) this.playerState = playerStates.running;
        else if (this.isGrounded && this.isCrouching) this.playerState = playerStates.crouching;
        else if (!this.isGrounded) this.playerState = playerStates.inAir;
        else this.playerState = playerStates.walking;

        switch (this.playerState)
        {
            case playerStates.walking:
            case playerStates.inAir:
                this.speed = 6.0f;
                break;
            case playerStates.running:
                this.speed = 10.0f;
                break;
            case playerStates.crouching:
                this.speed = 3.0f;
                break;
        }
    }

    private void GetPlayerInputs()
    {
        this.horizontalInput = Input.GetAxis("Horizontal");
        this.verticalInput = Input.GetAxis("Vertical");
        this.isSprinting = Input.GetKey(this.sprintKey);
        this.isCrouching = Input.GetKey(this.crouchKey);
        this.isJumping = Input.GetKeyDown(this.jumpKey);
        this.isGrounded = this.IsGrounded();

        this.HandlePlayerState();
        // Handle jump
        this.canJump = (this.playerState != playerStates.inAir || this.currentExtraJumps < this.extraJumps) && this.canJump;
        if (this.isJumping && this.canJump)
        {
            Debug.Log("Jumping");
            this.canJump = false;
            if(this.playerState == playerStates.inAir) this.currentExtraJumps++;
            else this.currentExtraJumps = 0;
            this.Jump(this.jumpForce);
            Invoke(nameof(ResetJump), this.jumpCooldown);
        }
        else {
            this.canJump = true;
        }
    }

    private void MovePlayer()
    {
        this.moveDirection = this.playerOreintation.forward * this.verticalInput + this.playerOreintation.right * this.horizontalInput;
        this.moveDirection.y = 0.0f;

        switch (this.playerState)
        {
            case playerStates.walking:
            case playerStates.running:
            case playerStates.crouching:
                this.rb.AddForce(this.moveDirection.normalized * this.speed * this.acceleration, ForceMode.Force);
                break;
            case playerStates.inAir:
                this.rb.AddForce(this.moveDirection.normalized * this.speed * this.acceleration * this.airMultiplier, ForceMode.Force);
                break;
        }
    }

    private bool IsGrounded()
    {
        float playerHeight = this.transform.localScale.y;
        return Physics.Raycast(this.transform.position, Vector3.down, playerHeight + 0.2f, this.groundLayer);
    }

    private void SpeedControl()
    {
        Vector3 flatVelocity = new Vector3(this.rb.velocity.x, 0.0f, this.rb.velocity.z);

        if (flatVelocity.magnitude > this.speed)
        {
            flatVelocity = flatVelocity.normalized * this.speed;
            this.rb.velocity = new Vector3(flatVelocity.x, this.rb.velocity.y, flatVelocity.z);
        }
    }

    private void Jump(float jumpForce = 6.0f)
    {
        // Reset velocity in y axis
        this.rb.velocity = new Vector3(this.rb.velocity.x, 0.0f, this.rb.velocity.z);

        // Add jump force
        this.rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        this.canJump = true;
    }
}
