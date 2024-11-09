using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class MurBrisable : MonoBehaviour
{
    [Header("Options Mur Brisable")]
    [SerializeField]
    private float vieMur = 100f;
    [SerializeField]
    private float degats = 50f;
    [SerializeField]
    private GameObject murCassePrefab;

    private AudioSource audioSource;
    private float delaiSon = 0.2f;
    private bool estCasser = false;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    /// <summary>
    /// Si le mur est touché par un couteau, il prend des dégats
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("MurBrisableTrigger");
        if (other.CompareTag("Couteau"))
        {
            PrendreDegat();
        }
    }

    /// <summary>
    /// Fonction de perte de dégat du mur
    /// </summary>
    public void PrendreDegat()
    {
        vieMur -= degats;
        if (vieMur <= 0 && !estCasser)
            Break();
    }

    /// <summary>
    /// Fonction qui gère se qui se produit quand le mur brisable doit casser
    /// </summary>
    public void Break() 
    {
        estCasser = true;
        if (audioSource != null && audioSource.clip != null)
        {
            Debug.Log("AudioSource");
            audioSource.PlayOneShot(audioSource.clip);
        }
        GameObject murCasser = Instantiate(murCassePrefab, transform.position, transform.rotation);
        Rigidbody[] morceaux = murCasser.GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody morceau in morceaux)
        {
            Vector3 impulse = (morceau.transform.position - transform.position) * 15f;
            morceau.AddForce(impulse, ForceMode.Impulse);
        }

        Destroy(gameObject, delaiSon);

        MorceauxDespawn morceauxDespawn = murCasser.AddComponent<MorceauxDespawn>();
        morceauxDespawn.StartDespawn();
    }
}

