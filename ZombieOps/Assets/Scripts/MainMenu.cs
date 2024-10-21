using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public TMP_Text highScoreUI; // UI per visualizzare il punteggio più alto
    string GAME_EASY = "GameEasy"; // Nome della scena per il gioco facile
    string GAME_MEDIUM = "GameMedium"; // Nome della scena per il gioco medio
    string GAME_HARD = "GameHard"; // Nome della scena per il gioco difficile

    public AudioClip backgroundMusic; // Clip audio per la musica di sottofondo
    public AudioSource mainChannel; // Canale audio principale

    public Toggle easyToggle; // Toggle per la difficoltà facile
    public Toggle mediumToggle; // Toggle per la difficoltà media
    public Toggle hardToggle; // Toggle per la difficoltà difficile

    public Toggle volumeToggleON; // Toggle per il volume attivo
    public Toggle volumeToggleOFF; // Toggle per il volume disattivato

    public TextMeshProUGUI mouseSensitivityText; // UI per visualizzare la sensibilità del mouse

    private enum Difficulty
    {
        EASY,
        MEDIUM,
        HARD
    }

    private Difficulty currentDifficulty = Difficulty.EASY; // Difficoltà corrente

    // Start is called before the first frame update
    void Start()
    {
        // Riproduce la musica di sottofondo
        mainChannel.PlayOneShot(backgroundMusic);

        // Imposta l'UI del punteggio più alto
        highScoreUI.text = "High Score: " + SaveLoadManager._Instance.LoadHighScore();

        // Imposta i toggle della difficoltà
        easyToggle.isOn = true;
        mediumToggle.isOn = false;
        hardToggle.isOn = false;

        // Imposta i toggle del volume
        volumeToggleON.isOn = true;
        volumeToggleOFF.isOn = false;

        // Imposta la sensibilità del mouse
        PlayerPrefs.SetFloat("MouseSensitivity", 50);
        mouseSensitivityText.text = PlayerPrefs.GetFloat("MouseSensitivity").ToString();
    }

    public void StartGame()
    {
        // Ferma la musica di sottofondo
        mainChannel.Stop();
        // Carica la scena del gioco in base alla difficoltà corrente
        switch(currentDifficulty)
        {
            case Difficulty.EASY:
                UnityEngine.SceneManagement.SceneManager.LoadScene(GAME_EASY);
                break;
            case Difficulty.MEDIUM:
                UnityEngine.SceneManagement.SceneManager.LoadScene(GAME_MEDIUM);
                break;
            case Difficulty.HARD:
                UnityEngine.SceneManagement.SceneManager.LoadScene(GAME_HARD);
                break;
        }
    }

    public void QuitGame()
    {
        // Esce dal gioco
        Application.Quit();
    }

    public void OnToggleDifficultyChange()
    {
        // Cambia la difficoltà corrente in base al toggle selezionato
        if(easyToggle.isOn)
        {
            currentDifficulty = Difficulty.EASY;
        } 
        else if(mediumToggle.isOn)
        {
            currentDifficulty = Difficulty.MEDIUM;
        } 
        else if(hardToggle.isOn)
        {
            currentDifficulty = Difficulty.HARD;
        }

        Debug.Log("Current difficulty: " + currentDifficulty);
    }

    public void OnToggleVolumeChange()
    {
        // Cambia il volume in base al toggle selezionato
        if(volumeToggleON.isOn)
        {
            AudioListener.volume = 1f;
        } 
        else if(volumeToggleOFF.isOn)
        {
            AudioListener.volume = 0f;
        }
    }

    public void OnMouseSensitivityChange(GameObject slider)
    {
        Debug.Log("X Sensitivity: " + slider.GetComponent<Slider>().value);

        // Salva il valore della sensibilità del mouse
        PlayerPrefs.SetFloat("MouseSensitivity", slider.GetComponent<Slider>().value);
        mouseSensitivityText.text = PlayerPrefs.GetFloat("MouseSensitivity").ToString();
    }
}