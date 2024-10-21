using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationScript : MonoBehaviour
{
    public float walkSpeed = 2f;
    public float runSpeed = 5f;

    public Animator animator;
    private CharacterController characterController;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        Move();
    }

    void Move()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 direction = new Vector3(horizontal, 0, vertical).normalized;
        animator.SetFloat("X", vertical);

        if (direction.magnitude >= 0.1f)
        {
            // V�rifie si le joueur veut courir
            bool isRunning = Input.GetKey(KeyCode.LeftShift);
            

            // Calcule la vitesse de d�placement
            float speed = isRunning ? runSpeed : walkSpeed;
            characterController.Move(direction * speed * Time.deltaTime);

            // Tourne le personnage dans la direction du mouvement
            transform.forward = direction;
        }
        else
        {
            // Si le personnage ne bouge pas, on passe � l'�tat Idle
            animator.SetBool("IsWalking", false);
            animator.SetBool("IsRunning", false);
        }
    }
}
