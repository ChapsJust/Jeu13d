using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Player Body")]
    [SerializeField]
    private Transform playerBody;

    [Header("Options")]
    [SerializeField]
    private float speed = 5f;
    [SerializeField]
    private float jumpForce = 7f;
    [SerializeField]
    private float sprintSpeed = 8f;
    [SerializeField]
    private float crouchSpeed = 3f;

    [Header("BunnyHopOptions")]
    [SerializeField]
    private float maxBunnyHopSpeed = 10f;
    [SerializeField]
    private float bunnyHopAccelaration = 8f;

    //Variables
    private Rigidbody rb;
    private Vector2 mouvementInput;
    private bool isJumping = false;
    private bool isGrounded = true;
    private bool isSprinting = false;
    private bool isCrouching = false;

    //Start
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (isJumping && isGrounded)
        {
            Jump();
        }
        else if (!isGrounded && isSprinting && isJumping)
        {
            isJumping = false;
        }
    }

    //S'occupe des mouvements
    private void FixedUpdate()
    {
        MovePlayer();
    }

    //Bouge le joueur en fonction du regard du joueur
    private void MovePlayer()
    {
        //Direction Joueur
        Vector3 forward = playerBody.forward;
        Vector3 right = playerBody.right;

        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        Vector3 direction = forward * mouvementInput.y + right * mouvementInput.x;

        //Permet de changer la vitesse du joueur en fonction de l'etat du joueur
        float currentSpeed = speed;
        if(isCrouching)
            currentSpeed = crouchSpeed;
        else if(isSprinting)
            currentSpeed = sprintSpeed;

        Vector3 mouvement = direction * currentSpeed;
        rb.linearVelocity = new Vector3(mouvement.x, rb.linearVelocity.y, mouvement.z);
    }

    //Permet d'ajouter une force en y avec Impulse
    private void Jump()
    {
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        isGrounded = false;
        isJumping = false;
    }

    //S'execute si une collision avec le sol IsGrouded = True
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    //OnMove Input Values
    public void OnMove(InputAction.CallbackContext context)
    {
        mouvementInput = context.action.ReadValue<Vector2>();
    }

    //OnJump Input Bool
    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.action.triggered)
        {
            isJumping = true;
        }
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        if (context.started) 
            isSprinting = true;
        else if (context.canceled)
            isSprinting = false;
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.action.triggered)
        {
            Debug.Log("Attack");
        }
    }

    /// <summary>
    /// Si le Joueur appuie sur la touche Crouch alors isCrouching = true et quand le joueur relache la touche isCrouching = false
    /// </summary>
    /// <param name="context"></param>
    public void OnCrouch(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            isCrouching = true;
        }
        else if (context.canceled)
        {
            isCrouching = false;
        }
    }
}
