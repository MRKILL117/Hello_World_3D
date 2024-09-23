using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using Fusion.Addons.SimpleKCC;

public class PlayerNetworked : NetworkBehaviour
{
    [Header("References")]
    public SimpleKCC simpleKCC;
    public PlayerInput playerInput;
    public LayerMask groundLayer;
    private GameObject cameraHolder;
    private GameObject playerCameraPos;

    [Header("Player Movement")]
    public float viewSensitivity = 1.0f;
    public float walkSpeed = 5f;
    public float runSpeed = 10f;
    public float jumpForce = 5f;
    public float upGravity = 20f;
    public float downGravity = 40f;

    [Header("Player Accelerations")]
    public float groundAcceleration = 55f;
    public float groundDesacceleration = 25f;
    public float airAcceleration = 25f;
    public float airDesacceleration = 1.3f;

    [Networked]
    private NetworkBool _isJumping { get; set; }

    private Vector3 _moveVelocity;

    private PlayerState playerState;
    enum PlayerState
    {
        walking,
        running,
        inAir,
        crouching,
    }

    void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (!HasStateAuthority) return;
    }

    public override void FixedUpdateNetwork()
    {
        this.HandlePlayerState();
        this.ProcessInput();

        if (this._isJumping)
        {
            this._isJumping = false;
        }

        this.playerInput.ResetInputs();
    }

    void LateUpdate()
    {
        // If the player has not authority, exit the function
        if (!HasStateAuthority) return;

        if (!this.cameraHolder) this.cameraHolder = GameObject.Find("CameraHolder");
        if (!this.playerCameraPos) this.playerCameraPos = this.transform.Find("CameraPos").gameObject;
        this.cameraHolder.transform.position = this.playerCameraPos.transform.position;
    }

    private void HandlePlayerState()
    {
        bool isGrounded = this.IsGrounded();
        if (isGrounded && playerInput.input.sprint) this.playerState = PlayerState.running;
        else if (isGrounded && playerInput.input.crouch) this.playerState = PlayerState.crouching;
        else if (!isGrounded) this.playerState = PlayerState.inAir;
        else this.playerState = PlayerState.walking;
    }

    private void ProcessInput()
    {
        float jumpImpulse = 0f;
        float speed = this.playerState == PlayerState.running ? this.runSpeed : this.walkSpeed;
        float acceleration = this.playerState == PlayerState.inAir ? this.airAcceleration : this.groundAcceleration;
        float gravity = simpleKCC.RealVelocity.y > 0f ? this.upGravity : this.downGravity;
        if(this.playerState != PlayerState.inAir && playerInput.input.jump)
        {
            jumpImpulse = this.jumpForce;
            this._isJumping = true;
        }
        // When setting up gravity character starts to float indefinitely
        // this.simpleKCC.SetGravity(gravity);

        // Get the player's input and clamp the xRotation
        float xRotation = Mathf.Clamp(this.playerInput.input.lookDirection.x * this.viewSensitivity, -90f, 90f);
        float yRotation = this.playerInput.input.lookDirection.y * this.viewSensitivity;

        // this code is for rotate the camera object based on the mouse input
        this.cameraHolder.transform.rotation = Quaternion.Euler(xRotation, yRotation, 0.0f);
        // this code is for rotate the player object based on the mouse input
        // We opnly rotate the player object in the y axis the x axis is controlled by the camera
        this.simpleKCC.SetLookRotation(new Vector3(0.0F, yRotation, 0.0f));

        var moveDirection = this.simpleKCC.TransformRotation * new Vector3(playerInput.input.moveDirection.x, 0, playerInput.input.moveDirection.y) * speed;
        if (moveDirection == Vector3.zero)
        {
            acceleration = this.playerState == PlayerState.inAir ? this.airDesacceleration : this.groundDesacceleration;
        }
        else
        {
            // this code is for rotate the player object to the direction of the movement
            // var currentRotation = this.simpleKCC.TransformRotation;
            // var targetRotation = Quaternion.LookRotation(moveDirection);
            // var nextRotation = Quaternion.Lerp(currentRotation, targetRotation, this.rotationSpeed * Runner.DeltaTime);
            // this.simpleKCC.SetLookRotation(nextRotation.eulerAngles);
            // this code is for rotate the player object to the direction of the movement

            acceleration = this.playerState == PlayerState.inAir ? this.airAcceleration : this.groundAcceleration;
        }

        this._moveVelocity = Vector3.Lerp(this._moveVelocity, moveDirection, acceleration * Runner.DeltaTime);

        if (this.simpleKCC.ProjectOnGround(this._moveVelocity, out Vector3 moveVelocity))
        {
            this._moveVelocity = moveVelocity;
        }

        this.simpleKCC.Move(this._moveVelocity, jumpImpulse);
    }

    private bool IsGrounded()
    {
        float playerHeight = this.transform.localScale.y;
        return Physics.Raycast(this.transform.position, Vector3.down, playerHeight + 0.2f, this.groundLayer);
    }
}
