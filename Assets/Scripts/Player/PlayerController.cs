using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public CharacterController controller;

    public float walkSpeed = 11f;
    public float sprintSpeed = 14f;
    public float jumpHeight = 1.25f;
    public float gravity = -20f;
    public float airControl = 0.5f;

    private Vector3 velocity;
    private bool isGrounded;
    private bool isSprinting;

    void Update()
    {
        isGrounded = CheckGround();

        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;

        isSprinting = Input.GetKey(KeyCode.LeftShift);
        float speed = isSprinting ? sprintSpeed : walkSpeed;
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move.normalized * speed * (isGrounded ? 1 : airControl) * Time.deltaTime);

        if (Input.GetButtonDown("Jump") && isGrounded)
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    bool CheckGround()
    {
        return Physics.Raycast(transform.position, Vector3.down, 1.1f, ~0, QueryTriggerInteraction.Ignore);
    }

    public bool IsSprinting()
    {
        return isSprinting;
    }
}
