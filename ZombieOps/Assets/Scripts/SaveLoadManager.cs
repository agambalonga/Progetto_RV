using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveLoadManager : MonoBehaviour
{
    // Istanza singleton del SaveLoadManager
    public static SaveLoadManager _Instance { get; set; }

    // Chiave per salvare il punteggio più alto
    public static string HIGH_SCORE_KEY = "HighScoreKey";

    // Metodo chiamato all'inizio
    public void Awake()
    {
        // Imposta l'istanza singleton
        if (_Instance == null)
        {
            _Instance = this;
            DontDestroyOnLoad(this.gameObject); // Non distruggere l'oggetto quando si carica una nuova scena
        }
        else
        {
            _Instance = this;
        }

        DontDestroyOnLoad(this); // Non distruggere l'oggetto quando si carica una nuova scena
    }

    // Metodo per salvare il punteggio più alto
    public void SaveHighScore(int highScore)
    {
        // Salva il punteggio solo se è maggiore del punteggio salvato
        if (highScore > LoadHighScore())
            PlayerPrefs.SetInt(HIGH_SCORE_KEY, highScore);
    }

    // Metodo per caricare il punteggio più alto
    public int LoadHighScore()
    {
        // Controlla se esiste un punteggio salvato
        if(PlayerPrefs.HasKey(HIGH_SCORE_KEY))
        {
            // Restituisce il punteggio salvato
            return PlayerPrefs.GetInt(HIGH_SCORE_KEY, 0);
        }

        // Restituisce 0 se non esiste un punteggio salvato
        return 0;
    }
}