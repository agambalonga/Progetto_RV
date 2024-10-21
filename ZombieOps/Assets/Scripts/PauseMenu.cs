using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    // Riferimento all'UI del menu di pausa
    public GameObject pauseMenuUI;
    // Riferimento al giocatore
    public GameObject player;
    // Riferimento all'arma attiva
    private Weapon weapon;
    // Stato del gioco (in pausa o no)
    private bool isPaused = false;

    // Metodo chiamato ad ogni frame
    void Update()
    {
        // Controlla se il tasto Esc è stato premuto
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                Resume(); // Riprende il gioco
            }
            else
            {
                Pause(); // Mette in pausa
            }
        }
    }

    // Metodo per riprendere il gioco
    public void Resume()
    {
        pauseMenuUI.SetActive(false); // Nasconde il menu di pausa
        Time.timeScale = 1f; // Riprende il tempo di gioco
        isPaused = false; // Indica che il gioco non è più in pausa
        EnableGameInput(); // Riabilita l'input di gioco
        Cursor.lockState = CursorLockMode.Locked; // Blocca il cursore
        Cursor.visible = false; // Nasconde il cursore
    }

    // Metodo per mettere in pausa il gioco
    void Pause()
    {
        pauseMenuUI.SetActive(true); // Mostra il menu di pausa
        Time.timeScale = 0f; // Ferma il tempo di gioco
        isPaused = true; // Indica che il gioco è in pausa
        DisableGameInput(); // Disabilita l'input di gioco
        Cursor.lockState = CursorLockMode.None; // Sblocca il cursore
        Cursor.visible = true; // Mostra il cursore
    }

    // Metodo per uscire dal gioco e tornare al menu principale
    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        if (Time.timeScale == 0f)
            Time.timeScale = 1f; // Riprende il tempo di gioco se è in pausa

        SceneManager.LoadScene("MainMenu"); // Carica la scena del menu principale
    }

    // Metodo per disabilitare l'input di gioco
    private void DisableGameInput()
    {   
        weapon = WeaponManager._Instance.GetActiveWeapon();
        if (weapon != null)
        {
            weapon.enabled = false; // Disabilita l'arma attiva
        }

        // Disattiva l'audio di gioco
        SoundManager._Instance.MuteAllAudio();

        // Disabilita il CharacterController
        var characterController = player.GetComponent<CharacterController>();
        if (characterController != null)
        {
            characterController.enabled = false; // Disabilita il CharacterController
        }

        // Disabilita lo script di movimento del mouse
        var mouseScript = player.GetComponent<MouseMovement>();
        if (mouseScript != null)
        {
            mouseScript.enabled = false; // Disabilita lo script di movimento del mouse
        }

        // Disabilita lo script di movimento del player
        var playerScript = player.GetComponent<PlayerMovement>(); 
        if (playerScript != null)
        {
            playerScript.enabled = false; // Disabilita lo script di movimento del player
        }

        // Disabilita lo script del player
        var playerGameScript = player.GetComponent<Player>(); 
        if (playerGameScript != null)
        {
            playerGameScript.enabled = false; // Disabilita lo script del player
        }
    }

    // Metodo per riabilitare l'input di gioco
    private void EnableGameInput()
    {
        weapon = WeaponManager._Instance.GetActiveWeapon();
        if (weapon != null)
        {
            weapon.enabled = true; // Riabilita l'arma attiva
        }
        SoundManager._Instance.UnmuteAllAudio(); // Riattiva l'audio di gioco

        // Riabilita il CharacterController
        var characterController = player.GetComponent<CharacterController>();
        if (characterController != null)
        {
            characterController.enabled = true; // Riabilita il CharacterController
        }

        // Riabilita lo script di movimento del mouse
        var mouseScript = player.GetComponent<MouseMovement>();
        if (mouseScript != null)
        {
            mouseScript.enabled = true; // Riabilita lo script di movimento del mouse
        }

        // Riabilita lo script di movimento del player
        var playerScript = player.GetComponent<PlayerMovement>();
        if (playerScript != null)
        {
            playerScript.enabled = true; // Riabilita lo script di movimento del player
        }

        // Riabilita lo script del player
        var playerGameScript = player.GetComponent<Player>(); 
        if (playerGameScript != null)
        {
            playerGameScript.enabled = true; // Riabilita lo script del player
        }
    }
}