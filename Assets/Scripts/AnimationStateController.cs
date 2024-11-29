using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationStateController : MonoBehaviour
{
    public float speed = 2f; 
    public float runSpeedMultiplier = 1.5f;
    private Animator animator; 
    private Rigidbody2D rb;
    private Vector2 movement;
    private float rotationSpeed = 10f; 

    private bool isIdleSwitching = false; 
    private int randomIdleIndex = -1; 

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
  
        movement.x = Input.GetAxisRaw("Horizontal"); 
        movement.y = Input.GetAxisRaw("Vertical"); 

        float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? speed * runSpeedMultiplier : speed;

        // Déplacement
        Vector3 move = new Vector3(movement.x, 0, movement.y).normalized;
        transform.Translate(move * currentSpeed * Time.deltaTime, Space.World);

        // Tourne le joueur dans la bonne direction quand il se déplace
        RotateTowardsMovement(movement);

        // Change les animations en fonction de la vitesse
        HandleAnimations(movement, currentSpeed);
    }

    void HandleAnimations(Vector2 movement, float currentSpeed)
    {
        if (movement.magnitude > 0)
        {

            isIdleSwitching = false; // Stop l'alternance des animations Idle
            animator.ResetTrigger("PlayIdle1");
            animator.ResetTrigger("PlayIdle2");

            if (currentSpeed > speed) // Si le joueur court
            {
                animator.SetBool("IsRunning", true);
                animator.SetBool("IsWalking", false);
            }
            else // Si le joueur marche
            {
                animator.SetBool("IsRunning", false);
                animator.SetBool("IsWalking", true);
            }
        }
        else
        {
            // Si le joueur est immobile
            animator.SetBool("IsWalking", false);
            animator.SetBool("IsRunning", false);

            if (!isIdleSwitching)
            {
                // Définir la variable randomIdleIndex à 0 ou 1 de manière aléatoire pour pouvoir changer d'idle aléatoirement
                randomIdleIndex = Random.Range(0, 2);
                StartCoroutine(SwitchIdleAnimation());
            }
        }
    }

    IEnumerator SwitchIdleAnimation()
    {
        isIdleSwitching = true;

        // Alterne entre Idle1 et Idle2
        while (movement.magnitude == 0)
        {
            if (randomIdleIndex == 0)
                animator.SetTrigger("PlayIdle1");
            else
                animator.SetTrigger("PlayIdle2");

            yield return new WaitForSeconds(5f);

            // Redéfinir la variable randomIdleIndex pour la prochaine animation
            randomIdleIndex = Random.Range(0, 2);
        }

        isIdleSwitching = false;
    }

    void RotateTowardsMovement(Vector2 movement)
    {
        if (movement.magnitude > 0)
        {
            // Calcul de la direction vers laquelle le joueur veut se tourner
            Vector3 targetDirection = new Vector3(movement.x, 0, movement.y).normalized;
            Vector3 currentDirection = transform.forward;
            Vector3 newDirection = Vector3.RotateTowards(currentDirection, targetDirection, rotationSpeed * Time.deltaTime, 0.0f);
            transform.rotation = Quaternion.LookRotation(newDirection);
        }
    }
}
