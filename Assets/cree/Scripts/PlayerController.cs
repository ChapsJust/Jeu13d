using System.Collections;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    //Reference du Player Body et de la Camera
    [Header("Player Body")]
    [SerializeField]
    private Transform playerBodyRef;
    [SerializeField]
    private Transform cameraRef;

    //Player Body Principales Options
    [Header("Options")]
    [SerializeField]
    private float speed = 5f;
    [SerializeField]
    private float jumpForce = 7f;
    [SerializeField]
    private float sprintSpeed = 8f;
    [SerializeField]
    private float crouchSpeed = 3f;
    [SerializeField]
    private float crouchOffSet = 0.5f;
    [SerializeField]
    private float crouchTransition = 5f;

    [Header("Player Vie")]
    [SerializeField]
    private int maxVie = 100;
    private int currentVie;

    [Header("Couteau Options")]
    [SerializeField]
    private GameObject couteauPrefab;
    private BoxCollider couteauCollider;

    //Variables
    private Animator animator;
    private Rigidbody rb;
    private Vector2 mouvementInput;
    private Vector3 cameraOriginalPosition;
    private bool isJumping = false;
    private bool isGrounded = true;
    private bool isSprinting = false;
    private bool isCrouching = false;
    private bool isAttacking = false;

    /// <summary>
    /// Fonction Start
    /// </summary>
    private void Awake()
    {
        animator = couteauPrefab.GetComponent<Animator>();
        couteauCollider = couteauPrefab.GetComponent<BoxCollider>();
        couteauCollider.enabled = false;
    }

    /// <summary>
    /// Fonction Start
    /// </summary>
    private void Start()
    {
        //RigidBody
        rb = GetComponent<Rigidbody>();
        //Cherche la position de la camera
        cameraOriginalPosition = cameraRef.localPosition;
        
        currentVie = maxVie;
    }

    /// <summary>
    /// Update pour les Inputs
    /// </summary>
    private void Update()
    {
        //Regarde si le Joueur peut sauter ou pas 
        if (isJumping && isGrounded)
        {
            Jump();
        }
        else if (!isGrounded && isSprinting && isJumping)
        {
            isJumping = false;
        }

        if(isAttacking)
            Attack();

        //Ajuste la camera si accrroupi ou pas
        AjusterCameraCrouch();
    }

    /// <summary>
    /// S'occupe des mouvements
    /// </summary>
    private void FixedUpdate()
    {
        //Fonction pour bouger le joueur
        MovePlayer();
    }

    /// <summary>
    /// Bouge le joueur en fonction du regard du joueur
    /// </summary>
    private void MovePlayer()
    {
        //Direction Joueur
        Vector3 forward = playerBodyRef.forward;
        Vector3 right = playerBodyRef.right;

        //Remet à zéro le Y de la camera
        forward.y = 0f;
        right.y = 0f;
        //Normalise les vecteurs
        forward.Normalize();
        right.Normalize();

        //Direction ou le joueur doit aller
        Vector3 direction = forward * mouvementInput.y + right * mouvementInput.x;

        //Permet de changer la vitesse du joueur en fonction de l'etat du joueur
        float currentSpeed = speed;
        if(isCrouching)
            currentSpeed = crouchSpeed;
        else if(isSprinting)
            currentSpeed = sprintSpeed;

        //Applique la vitesse au joueur en fonction de la direction et de la vitesse
        Vector3 mouvement = direction * currentSpeed;
        rb.linearVelocity = new Vector3(mouvement.x, rb.linearVelocity.y, mouvement.z);
    }

    /// <summary>
    /// Je me suis insipiré de cette vidéo pour faire mon arme : https://www.youtube.com/watch?v=oAhgEbznVss
    /// </summary>
    public void Attack()
    {
        animator.SetTrigger("Attack");
        StartCoroutine(GestionAttack());
        isAttacking = false;
    }

    /// <summary>
    /// Desactive/Active la collison de l'arme
    /// </summary>
    /// <returns></returns>
    private IEnumerator GestionAttack()
    {
        yield return new WaitForSeconds(0.1f);
        couteauCollider.enabled = true;

        yield return new WaitForSeconds(0.5f);
        couteauCollider.enabled = false;
    }

    public void PrendreDegat(int degat, Vector3 knockback)
    {
        currentVie -= degat;
        Debug.Log(currentVie);
        if (currentVie < 0)
            Debug.Log("Le Joueur est ciao");
    }

    /// <summary>
    /// Permet d'ajouter une force en y avec Impulse
    /// </summary>
    private void Jump()
    {
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        isGrounded = false;
        isJumping = false;
    }

    /// <summary>
    /// S'execute si une collision avec le sol IsGrouded = True
    /// </summary>
    /// <param name="collision">collider</param>
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    /// <summary>
    /// OnMove Input Values
    /// </summary>
    /// <param name="context"></param>
    public void OnMove(InputAction.CallbackContext context)
    {
        mouvementInput = context.action.ReadValue<Vector2>();
    }

    /// <summary>
    /// OnJump Input Bool
    /// </summary>
    /// <param name="context"></param>
    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.action.triggered)
        {
            isJumping = true;
        }
    }

    /// <summary>
    /// OnSprint Input Bool
    /// </summary>
    /// <param name="context"></param>
    public void OnSprint(InputAction.CallbackContext context)
    {
        if (context.started) 
            isSprinting = true;
        else if (context.canceled)
            isSprinting = false;
    }

    /// <summary>
    /// OnAttack Input Bool
    /// </summary>
    /// <param name="context"></param>
    public void OnAttack(InputAction.CallbackContext context)
    {
        if(context.started)
            isAttacking = true;
        else if (context.canceled)
            isAttacking = false;
    }

    /// <summary>
    /// Si le Joueur appuie sur la touche Crouch alors isCrouching = true et quand le joueur relache la touche isCrouching = false
    /// </summary>
    /// <param name="context"></param>
    public void OnCrouch(InputAction.CallbackContext context)
    {
        if (context.started)
            isCrouching = true;
        else if (context.canceled)
            isCrouching = false;
    }

    /// <summary>
    /// Ajuster la camera en fonction de si le joueur est accroupi ou pas
    /// </summary>
    private void AjusterCameraCrouch()
    {
        Vector3 targetPosition;

        //Si le joueur est accroupi alors la camera est plus basse sinon elle est à sa position de base 
        if (isCrouching)
            targetPosition = cameraOriginalPosition - new Vector3(0, crouchOffSet, 0);
        else
            targetPosition = cameraOriginalPosition;

        //Lerp pour la transition de la camera
        cameraRef.localPosition = Vector3.Lerp(cameraRef.localPosition, targetPosition, Time.deltaTime * crouchTransition);
    }
}
