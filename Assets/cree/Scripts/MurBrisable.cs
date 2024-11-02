using Unity.VisualScripting;
using UnityEngine;

public class MurBrisable : MonoBehaviour
{
    [Header("Options Mur Brisable")]
    [SerializeField]
    private float vieMur = 50f;
    [SerializeField]
    private float degats = 25f;
    [SerializeField]
    private GameObject murCassePrefab;

    /// <summary>
    /// Si le mur est touché par un couteau, il prend des dégats
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Couteau"))
        {
            PrendreDegat();
            Debug.Log("OnTriggerEnter");
        }
    }

    public void PrendreDegat()
    {
        vieMur -= degats;
        if (vieMur <= 0)
        {
            Break();
        }
    }

    public void Break() 
    { 
        Instantiate(murCassePrefab, transform.position, transform.rotation);
        Destroy(gameObject);
    }
}

