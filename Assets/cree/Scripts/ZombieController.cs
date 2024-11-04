using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class ZombieController : MonoBehaviour
{
    [Header("Enemy Settings")]
    [SerializeField]
    private float detectionRange = 15f;
    [SerializeField]
    private float fieldOfView = 90f; 
    [SerializeField]
    private float moveSpeed = 3f; 
    [SerializeField]
    private float rotationSpeed = 2f; 
    [SerializeField]
    private float attackIntervale = 1f;
    [SerializeField]
    private int health = 3;

    private Transform player; 
    private bool playerDetecter = false; 
    private float attackCooldown = 0f;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Couteau"))
        {
            PrendDomage(1);
        }
    }

    /// <summary>
    /// Update pour le déplacement et l'attaque de l'ennemi
    /// </summary>
    private void Update()
    {
        DetecterJoueur();

        if (playerDetecter)
        {
            RotateVersJoueur();
            if (EstFaceAuJoueur())
            {
                DeplacerVersJoueur();
            }
        }
    }

    public void PrendDomage(int degat)
    {
        health -= degat;
        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }

    /// <summary>
    /// Essai un attaque si le cooldown est fini
    /// </summary>
    private void EssaiAttaque() 
    {
        if (attackCooldown <= 0f)
        {
            AttaquePlayer();
            attackCooldown = attackIntervale;
        }
        else
            attackCooldown -= Time.deltaTime;
    }

    /// <summary>
    /// Attaque le joueur
    /// </summary>
    private void AttaquePlayer()
    {
        // Attack logic here
    }

    /// <summary>
    /// Detecte le joueur dans le champ de vision de l'ennemi
    /// </summary>
    private void DetecterJoueur()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRange)
        {
            Vector3 directionToPlayer = (player.position - transform.position).normalized;
            float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

            if (angleToPlayer < fieldOfView / 2f)
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position, directionToPlayer, out hit, detectionRange))
                {
                    if (hit.transform.CompareTag("Player"))
                    {
                        playerDetecter = true;
                        return;
                    }
                }
            }
        }

        playerDetecter = false;
    }


    /// <summary>
    /// Fait tourner l'ennemi vers le joueur
    /// </summary>
    private void RotateVersJoueur()
    {
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(directionToPlayer.x, 0, directionToPlayer.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
    }

    /// <summary>
    /// Regarde si l'ennemi est face au joueur
    /// </summary>
    /// <returns>Retoune l'angle</returns>
    private bool EstFaceAuJoueur()
    {
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);
        return angleToPlayer < 30f; 
    }

    /// <summary>
    /// Deplace l'ennemi vers le joueur
    /// </summary>
    private void DeplacerVersJoueur()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0;
        Vector3 newPosition = transform.position + direction * moveSpeed * Time.deltaTime;
        transform.position = newPosition;
    }


    // Code pas fait par moi ne pas prendre en compte seulemnt pour débogage :)
    //private void OnDrawGizmosSelected()
    //{
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawWireSphere(transform.position, detectionRange);

    //    // Draw field of view lines
    //    Gizmos.color = Color.blue;
    //    Vector3 fovLine1 = Quaternion.AngleAxis(fieldOfView / 2, transform.up) * transform.forward * detectionRange;
    //    Vector3 fovLine2 = Quaternion.AngleAxis(-fieldOfView / 2, transform.up) * transform.forward * detectionRange;
    //    Gizmos.DrawRay(transform.position, fovLine1);
    //    Gizmos.DrawRay(transform.position, fovLine2);
    //}
}
