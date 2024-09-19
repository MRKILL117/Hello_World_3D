using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This struct will be used to store the player's input
public struct GameplayInput
{
    public Vector2 moveDirection;
    public Vector2 lookDirection;
    public bool jump;
    public bool sprint;
    public bool crouch;
}
public class PlayerInput : MonoBehaviour
{

    public GameplayInput input => _input;
    private GameplayInput _input;

    public void ResetInputs()
    {
        _input.moveDirection = default;
        _input.jump = false;
        _input.sprint = false;
        _input.crouch = false;
    }

    // Update is called once per frame
    void Update()
    {
        this.GetPlayerInputs();
    }

    private void GetPlayerInputs()
    {
        // Get the player's input
        _input.moveDirection = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        _input.lookDirection += new Vector2(-Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"));
        _input.jump = Input.GetKey(KeyCode.Space);
        _input.sprint = Input.GetKey(KeyCode.LeftShift);
        _input.crouch = Input.GetKey(KeyCode.LeftControl);
    }
}
