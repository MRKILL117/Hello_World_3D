using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public float sensX = 2.0f;
    public float sensY = 2.0f;
    public Transform playerOrientation;

    private float xRotation = 0.0f;
    private float yRotation = 0.0f;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * Time.deltaTime * sensX;
        float mouseY = Input.GetAxis("Mouse Y") * Time.deltaTime * sensY;

        this.yRotation += mouseX;
        this.xRotation -= mouseY;

        this.xRotation = Mathf.Clamp(this.xRotation, -90f, 90f);

        this.transform.localRotation = Quaternion.Euler(this.xRotation, this.yRotation, 0.0f);
        this.playerOrientation.rotation = Quaternion.Euler(0.0f, this.yRotation, 0.0f);
    }
}
