using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.UI;

public class PauseUiManager : MonoBehaviour
{
    [SerializeField]
    private GameObject pausePanel;
    [SerializeField]
    private GameObject optionsPanel;
    [SerializeField]
    private AudioMixer audioMixer;
    [SerializeField]
    private Slider volumeSlider;

    /// <summary>
    /// Start
    /// </summary>
    private void Start()
    {
        if (pausePanel != null)
            pausePanel.SetActive(false);
        if(volumeSlider != null)
        {
            float volume = volumeSlider.value;
            audioMixer.GetFloat("Master", out  volume);
            volumeSlider.value = ConvertirLineaireVersExponentiel(volume);
            volumeSlider.onValueChanged.AddListener(ChangerVolumeMaitre);
        }
    }

    //Code emprunté auteur: Alexandre Ouellet
    /// <summary>
    /// Change le volume du groupe maître.
    /// </summary>
    /// <param name="volume">Le pourcentage du volume maximal à affecter.</param>
    public void ChangerVolumeMaitre(float volume)
    {
        float volumeDB = ConvertirLineaireVersExponentiel(volume);

        audioMixer.GetFloat("Master", out float volumeActuel);
        if (!Mathf.Approximately(volumeDB, volumeActuel))
        {
            audioMixer.SetFloat("Master", volumeDB);
        }
    }

    //Code emprunté auteur: Alexandre Ouellet
    /// <summary>
    /// Convertit le pourcentage linéaire du bouton vers le bon niveau de décibels afin que l'effet du contrôle
    /// corresponde au volume entendu.
    /// </summary>
    /// <param name="volumeLineaire">Le pourcentage de puissance sonore à affecter.</param>
    /// <returns>Le nombre de décibels qui correspond à cet effet.</returns>
    private float ConvertirLineaireVersExponentiel(float volumeLineaire)
    {
        float minDb = -80 / 10.0f - 12.0f;
        float maxDb = 20 / 10.0f - 12.0f;
        float etenduDb = maxDb - minDb;
        float echelleExponentielle = Mathf.Lerp(1.0f, Mathf.Pow(2.0f, etenduDb), volumeLineaire);
        float pourcentageLog = Mathf.Log(echelleExponentielle, 2.0f) / etenduDb;
        float volumeDB = Mathf.Lerp(-80, 20, pourcentageLog);

        return volumeDB;

    }

    /// <summary>
    /// Montre le menu pause
    /// </summary>
    public void MontrePauseMenu()
    {
        if (pausePanel != null)
        {
            pausePanel.SetActive(true);
            optionsPanel.SetActive(false);
            Time.timeScale = 0f;

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    /// <summary>
    /// Reprend le jeu en cours
    /// </summary>
    public void ReprendreJeu()
    {
        pausePanel.SetActive(false);
        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    /// <summary>
    /// Montre le panel options
    /// </summary>
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

    /// <summary>
    /// Fonciton pour retourner au panel principal
    /// </summary>
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
