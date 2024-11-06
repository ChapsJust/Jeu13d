using System.Collections;
using UnityEngine;

public class MorceauxDespawn : MonoBehaviour
{
    private float intervalleDespawn = 0.7f;

    public void StartDespawn()
    {
        StartCoroutine(DeSpawnMorceauDelai());
    }

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
