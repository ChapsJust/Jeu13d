using Unity.VisualScripting;
using UnityEngine;

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
    private float delaiSon = 0.3f;
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
        if (other.CompareTag("Couteau"))
        {
            PrendreDegat();
        }
    }

    public void PrendreDegat()
    {
        vieMur -= degats;
        if (vieMur <= 0 && !estCasser)
            Break();
    }

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
    }
}

