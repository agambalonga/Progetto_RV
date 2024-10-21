using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieChasingState : StateMachineBehaviour
{
    NavMeshAgent agent; // Riferimento al NavMeshAgent dello zombie
    Transform player; // Riferimento al giocatore

    public float chasingSpeed = 6f; // Velocità di inseguimento dello zombie

    public float stopChasingDistance = 21; // Distanza alla quale lo zombie smette di inseguire
    public float attackDistance = 2.5f; // Distanza alla quale lo zombie inizia ad attaccare

    // OnStateEnter viene chiamato quando una transizione inizia e la macchina a stati inizia a valutare questo stato
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Trova il giocatore nella scena
        player = GameObject.FindGameObjectWithTag("Player").transform;
        // Ottiene il NavMeshAgent dallo zombie
        agent = animator.GetComponent<NavMeshAgent>();

        // Imposta la velocità di inseguimento dello zombie
        agent.speed = chasingSpeed;
    }

    // OnStateUpdate viene chiamato ad ogni frame di aggiornamento tra OnStateEnter e OnStateExit
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Riproduce il suono di corsa dello zombie se non è già in riproduzione
        if(!SoundManager._Instance.zombieChannel.isPlaying)
        {
            if(SoundManager._Instance.zombieChannel.enabled)
                SoundManager._Instance.zombieChannel.PlayOneShot(SoundManager._Instance.zombieRunClip);
        }

        // Imposta la destinazione dell'agente come la posizione del giocatore
        agent.SetDestination(player.position);

        // Fa guardare lo zombie verso il giocatore
        animator.transform.LookAt(player);

        // Calcola la distanza dal giocatore
        float distanceFromPlayer = Vector3.Distance(player.position, animator.transform.position);

        // Controlla se l'agente deve fermare l'inseguimento
        if (distanceFromPlayer > stopChasingDistance)
        {
            animator.SetBool("isChasing", false);
        }

        // Controlla se l'agente deve attaccare il giocatore
        if (distanceFromPlayer < attackDistance)
        {
            animator.SetBool("isAttacking", true);
        }
    }

    // OnStateExit viene chiamato quando una transizione finisce e la macchina a stati termina la valutazione di questo stato
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Ferma l'agente nella posizione corrente
        agent.SetDestination(animator.transform.position);

        // Ferma il suono di corsa dello zombie
        SoundManager._Instance.zombieChannel.Stop();
    }
}