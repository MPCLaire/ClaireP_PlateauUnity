using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationStateController : MonoBehaviour
{
    public float speed = 2f;
    public float runSpeedMultiplier = 1.5f;
    public float jumpForce = 10f; // Force de saut

    private Animator animator;
    private Rigidbody rb;
    private Vector2 movement;
    private float rotationSpeed = 10f;
    private bool TouchesFloor;

    private bool isIdleSwitching = false;
    private int randomIdleIndex = -1;
    private bool isGrounded = true;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // Récupération de l'entrée de mouvement
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? speed * runSpeedMultiplier : speed;

        // Déplacement
        Vector3 move = new Vector3(movement.x, 0, movement.y).normalized;
        transform.Translate(move * currentSpeed * Time.deltaTime, Space.World);

        // Rotation vers la direction de déplacement
        RotateTowardsMovement(movement);

        // Gestion des animations
        HandleAnimations(movement, currentSpeed);

        // Gestion du saut
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Jump();
        }
    }

    void OnCollisionStay(Collision collision)
    {
        // Vérifie si le joueur est au sol
        if (collision.gameObject.CompareTag("Sol"))
        {
            isGrounded = true;
            animator.SetBool("IsJumping", false); // Arrête l'animation de saut quand on touche le sol
        }
    }

    void OnCollisionExit(Collision collision)
    {
        // Le joueur n'est plus au sol lorsqu'il quitte le sol
        if (collision.gameObject.CompareTag("Sol"))
        {
            isGrounded = false;
        }
    }


    void Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);
        animator.SetBool("IsJumping", true); // Déclenche l'animation de saut
        isGrounded = false; // Le joueur n'est plus au sol
    }


    void HandleAnimations(Vector2 movement, float currentSpeed)
    {
        if (movement.magnitude > 0)
        {
            animator.SetBool("IsRunning", currentSpeed > speed);
            animator.SetBool("IsWalking", currentSpeed <= speed);
        }
        else if (isGrounded)
        {
            animator.SetBool("IsWalking", false);
            animator.SetBool("IsRunning", false);
        }
    }

    IEnumerator SwitchIdleAnimation()
    {
        isIdleSwitching = true;

        while (movement.magnitude == 0 && Mathf.Approximately(rb.velocity.y, 0))
        {
            if (randomIdleIndex == 0)
                animator.SetTrigger("PlayIdle1");
            else
                animator.SetTrigger("PlayIdle2");

            yield return new WaitForSeconds(5f);

            randomIdleIndex = Random.Range(0, 2);
        }

        isIdleSwitching = false;
    }

    void RotateTowardsMovement(Vector2 movement)
    {
        if (movement.magnitude > 0)
        {
            Vector3 targetDirection = new Vector3(movement.x, 0, movement.y).normalized;
            Vector3 currentDirection = transform.forward;
            Vector3 newDirection = Vector3.RotateTowards(currentDirection, targetDirection, rotationSpeed * Time.deltaTime, 0.0f);
            transform.rotation = Quaternion.LookRotation(newDirection);
        }
    }
}
