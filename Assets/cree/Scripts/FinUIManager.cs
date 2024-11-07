using UnityEngine;
using UnityEngine.SceneManagement;

public class FinUIManager : MonoBehaviour
{
    [SerializeField]
    private GameObject finPanel;

    /// <summary>
    /// Désactive le Ui dès le début
    /// </summary>
    private void Start()
    {
        if (finPanel != null)
           finPanel.SetActive(false);
    }


    /// <summary>
    /// Fonction qui montre le panel de fin et ajuste le jeu en conséquence
    /// </summary>
    public void MontreFinPanel()
    {
        Debug.Log("PanelShow");
        if (finPanel != null)
        {
            finPanel.SetActive(true);
            Time.timeScale = 0f;

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    /// <summary>
    /// Recommence le dungeon en recommencant la scene
    /// </summary>
    public void RestartDungeon()
    {
        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    /// <summary>
    /// Quitte le jeu
    /// </summary>
    public void QuitterJeu()
    {
        Time.timeScale = 1f;
        Application.Quit();

        // Si dans Éditeur cela est mieux et plus rapide !!!!
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}

