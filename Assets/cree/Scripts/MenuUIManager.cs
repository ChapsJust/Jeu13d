using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuUIManager : MonoBehaviour
{
    [SerializeField]
    private GameObject mainPanel;
    [SerializeField]
    private GameObject optionsPanel;

    /// <summary>
    /// Joue la scene principal du jeu
    /// </summary>
    public void PartieStart()
    {
        SceneManager.LoadScene("MainScene");
    }

    /// <summary>
    /// D�sactive le menuPanel pour optionsPanel
    /// </summary>
    public void OuvrirOptions()
    {
        mainPanel.SetActive(false);
        optionsPanel.SetActive(true);
    }

    /// <summary>
    /// D�sactive le optionsPanel pour menuPanel
    /// </summary>
    public void FermerOptions()
    {
        mainPanel.SetActive(true);
        optionsPanel.SetActive(false);
    }

    /// <summary>
    /// Quitte l'appli
    /// </summary>
    public void Quitter()
    {
        Application.Quit();

        // Si dans �diteur cela est mieux !!!!
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}
