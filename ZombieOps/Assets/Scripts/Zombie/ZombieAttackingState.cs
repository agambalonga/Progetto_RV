using UnityEngine;
using UnityEngine.AI;

public class ZombieAttackingState : StateMachineBehaviour
{
    Transform player; // Riferimento al giocatore
    NavMeshAgent agent; // Riferimento al NavMeshAgent dello zombie
    public float stopAttackDistance = 2.5f; // Distanza alla quale lo zombie smette di attaccare

    // OnStateEnter viene chiamato quando una transizione inizia e la macchina a stati inizia a valutare questo stato
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Trova il giocatore nella scena
        player = GameObject.FindGameObjectWithTag("Player").transform;
        // Ottiene il NavMeshAgent dallo zombie
        agent = animator.GetComponent<NavMeshAgent>();
    }

    // OnStateUpdate viene chiamato ad ogni frame di aggiornamento tra OnStateEnter e OnStateExit
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Riproduce il suono di attacco dello zombie se non è già in riproduzione
        if(!SoundManager._Instance.zombieChannel.isPlaying)
        {
            SoundManager._Instance.zombieChannel.PlayOneShot(SoundManager._Instance.zombieAttackClip);
        }

        // Fa guardare lo zombie verso il giocatore
        LookAtPlayer();

        // Controlla se l'agente deve smettere di attaccare
        float distanceFromPlayer = Vector3.Distance(player.position, animator.transform.position);

        if(distanceFromPlayer > stopAttackDistance)
        {
            // Se la distanza dal giocatore è maggiore della distanza di stop, smette di attaccare
            animator.SetBool("isAttacking", false);
        }
    }

    private void LookAtPlayer()
    {
        // Calcola la direzione verso il giocatore
        Vector3 direction = player.position - agent.transform.position;
        // Ruota lo zombie verso il giocatore
        agent.transform.rotation = Quaternion.LookRotation(direction);

        // Mantiene solo la rotazione sull'asse Y (orientamento orizzontale)
        var yRotation = agent.transform.eulerAngles.y;
        agent.transform.eulerAngles = new Vector3(0, yRotation, 0);
    }

    // OnStateExit viene chiamato quando una transizione finisce e la macchina a stati termina la valutazione di questo stato
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Ferma il suono di attacco dello zombie
        SoundManager._Instance.zombieChannel.Stop();
    }
}