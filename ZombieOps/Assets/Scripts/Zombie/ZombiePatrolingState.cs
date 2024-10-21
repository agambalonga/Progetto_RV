using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombiePatroilingState : StateMachineBehaviour
{
    float timer; // Timer per tenere traccia del tempo trascorso nello stato di pattugliamento
    public float patrolingTime = 10f; // Tempo di pattugliamento prima di passare allo stato di idle

    Transform player; // Riferimento al giocatore
    NavMeshAgent agent; // Riferimento al NavMeshAgent dello zombie

    public float detectionArea = 18f; // Raggio dell'area di rilevamento del giocatore
    public float patrolSpeed = 2f; // Velocità di pattugliamento dello zombie

    List<Transform> wayPointsList = new List<Transform>(); // Lista dei punti di pattugliamento

    // OnStateEnter viene chiamato quando una transizione inizia e la macchina a stati inizia a valutare questo stato
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Inizializzazione del player
        player = GameObject.FindGameObjectWithTag("Player").transform;
        agent = animator.GetComponent<NavMeshAgent>();
        
        // Imposta la velocità di pattugliamento dello zombie
        agent.speed = patrolSpeed;
        timer = 0;

        // Inizializzazione dei punti di pattugliamento
        GameObject wayPointCluster = GameObject.FindGameObjectWithTag("WayPoints");
        foreach(Transform wayPoint in wayPointCluster.transform)
        {
            wayPointsList.Add(wayPoint);
        }

        if(wayPointsList.Count == 0)
        {
            Debug.LogError("No waypoints found");
        }

        // Inizializzazione del primo punto di pattugliamento
        agent.SetDestination(wayPointsList[Random.Range(0, wayPointsList.Count)].position);
    }

    // OnStateUpdate viene chiamato ad ogni frame di aggiornamento tra OnStateEnter e OnStateExit
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Riproduce il suono di camminata dello zombie se non è già in riproduzione
        if(!SoundManager._Instance.zombieChannel.isPlaying)
        {
            SoundManager._Instance.zombieChannel.clip = SoundManager._Instance.zombieWalkClip;
            
            if (SoundManager._Instance.zombieChannel.enabled) // Riproduci solo se l'audio source è attivo
                SoundManager._Instance.zombieChannel.PlayDelayed(1f);
        }

        // Controlla se lo zombie ha raggiunto il punto di pattugliamento
        if(agent.remainingDistance <= agent.stoppingDistance)
        {
            // Imposta una nuova destinazione di pattugliamento
            agent.SetDestination(wayPointsList[Random.Range(0, wayPointsList.Count)].position);
        }

        // Transizione allo stato di idle
        timer += Time.deltaTime;
        if(timer > patrolingTime)
        {
            animator.SetBool("isPatroling", false);
        }

        // Transizione allo stato di inseguimento
        float distanceFromPlayer = Vector3.Distance(player.position, animator.transform.position);
        if(distanceFromPlayer < detectionArea)
        {
            animator.SetBool("isChasing", true);
        }
    }

    // OnStateExit viene chiamato quando una transizione finisce e la macchina a stati termina la valutazione di questo stato
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Ferma l'agente nella posizione corrente
        agent.SetDestination(agent.transform.position);

        // Ferma il suono di camminata dello zombie
        SoundManager._Instance.zombieChannel.Stop();
    }
}