using UnityEngine;

public class CellEndTrigger : MonoBehaviour
{
    public FinUIManager finUIManager;

    /// <summary>
    /// trigger qui détecte quand le Joueur a fini 
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Trigger1");
        if (collision.gameObject.CompareTag("Player"))
        { 
        Debug.Log("Trigger2");
        finUIManager.MontreFinPanel();
        }
    }
}
