using UnityEngine;

public class PauseUiManager : MonoBehaviour
{
    [SerializeField]
    private GameObject pausePanel;
    [SerializeField]
    private GameObject optionsPanel;

    private void Start()
    {
        if (pausePanel != null)
            pausePanel.SetActive(false);
    }

    public void MontrePauseMenu()
    {
        if (pausePanel != null)
        {
            pausePanel.SetActive(true);
            Time.timeScale = 0f;

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void ReprendreJeu()
    {
        pausePanel.SetActive(false);
        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void MontreOptionsMenu()
    {
        if (optionsPanel != null)
        {
            pausePanel.SetActive(false);
            optionsPanel.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void RetournePauseMenu()
    {
        if (optionsPanel != null && pausePanel != null)
        {
            pausePanel.SetActive(true);
            optionsPanel.SetActive(false);
        }
    }

    /// <summary>
    /// Quitte l'appli
    /// </summary>
    public void Quitter()
    {
        Application.Quit();

        // Si dans Éditeur cela est mieux !!!!
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}
