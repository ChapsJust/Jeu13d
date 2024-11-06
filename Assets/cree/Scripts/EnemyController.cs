using UnityEngine;
using UnityEngine.AI;
public class EnemyController : MonoBehaviour
{
    // Code inspiré pour l'ennemi:  https://www.youtube.com/watch?v=UvDqnbjEEak
    private Transform player;
    private NavMeshAgent enemy;

    private void Awake()
    {
        enemy = GetComponent<NavMeshAgent>();

        GameObject playerObjet = GameObject.FindGameObjectWithTag("Player");
        if(playerObjet != null)
            player = playerObjet.transform;
    }

    private void Update()
    {
        if(player != null) 
            enemy.SetDestination(player.position);
    }
}
