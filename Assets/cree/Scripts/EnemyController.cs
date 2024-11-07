using UnityEngine;
using UnityEngine.AI;
public class EnemyController : MonoBehaviour
{
    // Code inspiré pour l'ennemi:  https://www.youtube.com/watch?v=UvDqnbjEEak
    private Transform player;
    private NavMeshAgent enemy;
    private Animator animator;
    private AudioSource audioSource;

    private int maxVie = 100;
    private int currentVie;

    private int degatZomb = 10;

    private float attackCooldown = 2f;
    private float derniereAttackTemps;

    /// <summary>
    /// Awake
    /// </summary>
    private void Awake()
    {
        enemy = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        GameObject playerObjet = GameObject.FindGameObjectWithTag("Player");
        if(playerObjet != null)
            player = playerObjet.transform;

        currentVie = maxVie;

        derniereAttackTemps = -attackCooldown;
    }

    /// <summary>
    /// Update
    /// </summary>
    private void Update()
    {
        if (player != null)
        {
            float distance = Vector3.Distance(transform.position, player.position);

            if (distance <= 15f)
            {


                if (distance > 1.5f)
                {
                    enemy.speed = 3f;
                    enemy.SetDestination(player.position);
                }
                else
                {
                    enemy.speed = 1f;
                    enemy.ResetPath();
                }
            }
            bool isWalking = enemy.velocity.magnitude > 0.05f;
            animator.SetBool("IsWalking", isWalking);
        }
    }

    /// <summary>
    /// Fontion qui enlève de la vie a l'ennemie
    /// </summary>
    /// <param name="degat"></param>
    public void PrendreDegat(int degat)
    {
        currentVie -= degat;
        Debug.Log("VieEnemy : " + currentVie);
        if (currentVie < 0)
            Destroy(gameObject);
    }
    /// <summary>
    /// Trigger qui detecte le couteau et apres Prend des dégats
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Couteau"))
        {
            audioSource.Play();
            int randomDegat = Random.Range(10, 51);
            PrendreDegat(randomDegat);
        }
    }

    /// <summary>
    /// Collision Enter qui cause dégat au Joueur
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (Time.time - derniereAttackTemps >= attackCooldown)
            {
                PlayerController player = collision.gameObject.GetComponent<PlayerController>();
                if (player != null)
                {
                    Vector3 knockback = (collision.transform.position - transform.position).normalized;
                    Debug.Log("ZombieAttaqueJoueur");
                    player.PrendreDegat(degatZomb, knockback);
                }

                derniereAttackTemps = Time.time;
            }
        }
    }
}
