using UnityEngine;
using UnityEngine.AI;

public class Zombie : MonoBehaviour
{
    [SerializeField]
    public int health = 100; // Salute dello zombie

    private Animator animator; // Riferimento all'animatore

    private NavMeshAgent navMeshAgent; // Riferimento al NavMeshAgent

    public ZombieHand zombieHand; // Riferimento alla mano dello zombie
    public int damage; // Danno inflitto dallo zombie

    [HideInInspector]
    public bool isDead = false; // Stato di morte dello zombie

    // Metodo chiamato all'inizio
    private void Awake()
    {
        // Inizializza i riferimenti
        animator = GetComponent<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        zombieHand.damage = damage;

        // Imposta lo stato di morte a falso
        animator.SetBool("isDead", false);
    }

    // Metodo per infliggere danno allo zombie
    public void Damage(int damage, bool isAnimated)
    {
        health -= damage; // Riduce la salute dello zombie

        if(health <= 0)
        {
            // Se la salute è zero o inferiore, lo zombie muore
            int random = Random.Range(0, 2); // 0 o 1
            animator.ResetTrigger("HIT");
            if(random == 0)
            {
                Debug.Log("DIE1");
                animator.SetTrigger("DIE1");
            } 
            else
            {
                Debug.Log("DIE2");
                animator.SetTrigger("DIE2");
            }
            animator.SetBool("isDead", true);
            isDead = true;

            // Disabilita il collider dello zombie
            GetComponent<Collider>().enabled = false;

            // Elimina lo zombie dopo 10 secondi
            Destroy(gameObject, 10f);

            // Riproduce il suono di morte dello zombie
            SoundManager._Instance.zombieChannel2.PlayOneShot(SoundManager._Instance.zombieDieClip);
        } 
        else
        {
            // Se lo zombie è ancora vivo e l'animazione è abilitata
            if(isAnimated)
            {
                animator.SetTrigger("HIT");
            }
            // Riproduce il suono di colpo allo zombie
            SoundManager._Instance.zombieChannel2.PlayOneShot(SoundManager._Instance.zombieHitClip);
        }
    }

    // Metodo per disegnare i gizmo nella scena
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 2.5f); // Distanza di attacco

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, 18f); // Distanza di rilevamento (inizio inseguimento)

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 21f); // Distanza di stop inseguimento
    }

    // Metodo per applicare un effetto di stordimento allo zombie
    public void TacticalEffect()
    {
        animator.SetTrigger("Agonizing");
    }

    // Metodo per incrementare la salute dello zombie --> in base alla difficoltà
    public void IncrementHealth(int increment)
    {
        health += increment;
    }

    // Metodo per incrementare il danno dello zombie --> in base alla difficoltà
    public void IncrementDamage(int increment)
    {
        damage += increment;
        zombieHand.damage = damage;
    }
}