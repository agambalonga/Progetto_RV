using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Variabile che memorizza la salute del giocatore (inizialmente a 100)
    private float HP = 100;

    // Flag che indica se il giocatore è stato attaccato
    private bool isAttacked = false;

    // Flag che indica se il giocatore è morto
    private bool isDead = false;

    // Tempo trascorso dall'ultimo attacco subito
    private float timeSinceLastAttack = 0f;

    // Ritardo in secondi prima che inizi la rigenerazione della salute
    private float regenDelay = 5f;

    // Tasso di rigenerazione della salute (vita rigenerata per secondo)
    private float regenRate = 10f;

    // Metodo chiamato una volta per frame
    private void Update()
    {
        // Se il giocatore è morto, aggiorna l'HUD con 0 salute e ritorna
        if(isDead)
        {
            HUDManager._Instance.UpdatePlayerHealthUI(0);
            return;
        }

        // Se non è stato attaccato, aumenta il tempo trascorso dall'ultimo attacco
        if(!isAttacked)
        {
             timeSinceLastAttack += Time.deltaTime;
        }

        // Se il tempo trascorso supera il ritardo di rigenerazione e la salute è inferiore a 100, rigenera la salute
        if (timeSinceLastAttack >= regenDelay && HP < 100)
        {
            HP += regenRate * Time.deltaTime;
            HP = Mathf.Min(HP, 100); // La salute non può superare il valore massimo di 100
        }

        // Aggiorna l'HUD con la salute attuale del giocatore
        HUDManager._Instance.UpdatePlayerHealthUI((int)HP);

        // Resetta il flag, così la prossima iterazione saprà se è stato attaccato o meno
        isAttacked = false;
    }

    // Metodo per infliggere danno al giocatore
    public void Damage(int damage)
    {
        // Se il giocatore non è morto
        if(!isDead)
        {
            isAttacked = true;  // Il giocatore è stato attaccato
            HP -= damage;  // Sottrae il danno dalla salute
            timeSinceLastAttack = 0f;  // Resetta il timer per la rigenerazione

            // Se la salute è minore o uguale a 0, il giocatore muore
            if(HP <= 0)
            {
                Debug.Log("Player is dead");
                isDead = true;
                PlayerDead();  // Chiama il metodo per gestire la morte del giocatore
            }
            else
            {
                // Riproduce il suono del danno subito
                SoundManager._Instance.playerChannel.PlayOneShot(SoundManager._Instance.playerHurtClip);
                // Mostra un effetto visivo di schermo sanguinante
                StartCoroutine(HUDManager._Instance.BloodyScreenEffect());
            }
        }
    }

    // Metodo chiamato quando il giocatore muore
    private void PlayerDead()
    {
        // Disabilita il movimento del giocatore e la rotazione del mouse
        GetComponent<PlayerMovement>().enabled = false;
        GetComponent<MouseMovement>().enabled = false;

        // Disabilita il menu di pausa
        FindObjectOfType<PauseMenu>().enabled = false;

        // Attiva l'animator del player
        GetComponentInChildren<Animator>().enabled = true;

        // Riproduce il suono della morte del giocatore
        SoundManager._Instance.playerChannel.PlayOneShot(SoundManager._Instance.playerDieClip);

        // Mostra la schermata di morte in fade out
        StartCoroutine(HUDManager._Instance.ShowPlayerDeadScreen());

        // Mostra l'interfaccia di game over
        StartCoroutine(HUDManager._Instance.ShowGameOverUI());
    }

    // Metodo chiamato quando il giocatore entra in collisione con un trigger
    private void OnTriggerEnter(Collider other)
    {
        // Controlla se il trigger è il "ZombieHand"
        if (other.CompareTag("ZombieHand"))
        {
            // Ottiene il componente Zombie del genitore di ZombieHand
            Zombie zombie = other.GetComponentInParent<Zombie>();

            // Se lo zombie esiste e non è morto, infligge danno al giocatore
            if (zombie != null && !zombie.isDead)
            {
                // Infligge il danno associato da "ZombieHand"
                Damage(other.gameObject.GetComponent<ZombieHand>().damage);
            }
        }
    }
}
