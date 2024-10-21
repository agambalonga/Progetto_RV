using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieIdleState : StateMachineBehaviour
{
    float timer; // Timer per tenere traccia del tempo trascorso nello stato di inattività
    public float idleTime = 0f; // Tempo di inattività prima di passare allo stato di pattugliamento

    Transform player; // Riferimento al giocatore

    public float detectionAreaRadius = 18f; // Raggio dell'area di rilevamento del giocatore

    // OnStateEnter viene chiamato quando una transizione inizia e la macchina a stati inizia a valutare questo stato
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Inizializza il timer
        timer = 0;
        // Trova il giocatore nella scena
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // OnStateUpdate viene chiamato ad ogni frame di aggiornamento tra OnStateEnter e OnStateExit
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Incrementa il timer
        timer += Time.deltaTime;
        
        // Transizione allo stato di pattugliamento se il tempo di inattività è trascorso
        if(timer > idleTime)
        {
            animator.SetBool("isPatroling", true);
        }

        // Calcola la distanza dal giocatore
        float distanceFromPlayer = Vector3.Distance(player.position, animator.transform.position);
        // Transizione allo stato di inseguimento se il giocatore è entro il raggio di rilevamento
        if(distanceFromPlayer < detectionAreaRadius)
        {
            animator.SetBool("isChasing", true);
        }
    }
}