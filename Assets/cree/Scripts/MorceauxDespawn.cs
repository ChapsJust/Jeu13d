using System.Collections;
using UnityEngine;

public class MorceauxDespawn : MonoBehaviour
{
    private float intervalleDespawn = 0.7f;

    /// <summary>
    /// Fonciton public pour commencer le despawn
    /// </summary>
    public void StartDespawn()
    {
        StartCoroutine(DeSpawnMorceauDelai());
    }

    /// <summary>
    /// Coroutine qui fait disparaitre les morceaux un a un tranquillement et détruit le parent une fois tous détruit
    /// </summary>
    /// <returns></returns>
    private IEnumerator DeSpawnMorceauDelai()
    {
        yield return new WaitForSeconds(2f);

        while (transform.childCount > 0)
        {
            Transform morceau = transform.GetChild(0);

            while (morceau.childCount > 0)
            {
                Transform cellMorceau = morceau.GetChild(0);
                Destroy(cellMorceau.gameObject);
                yield return new WaitForSeconds(intervalleDespawn);
            }

            Destroy(morceau.gameObject);
            yield return new WaitForSeconds(intervalleDespawn);
        }

        Destroy(gameObject); 
    }
}
