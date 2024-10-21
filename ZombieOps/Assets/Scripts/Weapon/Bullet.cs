using UnityEngine;

public class Bullet : MonoBehaviour
{

    [HideInInspector]
    public int damage; // Danno del proiettile



    // Metodo chiamato quando il proiettile collide con un altro oggetto
    private void OnCollisionEnter(Collision collision)
    {

        // Controlla se il proiettile ha colpito un oggetto con il tag "Wall"
        if(collision.gameObject.CompareTag("Wall")) 
        {
            // Stampa un messaggio di debug con il nome dell'oggetto colpito
            //print("hit " + collision.gameObject.name);
            Weapon weapon = WeaponManager._Instance.GetActiveWeapon();

            if (weapon != null && weapon.name != ("Bennelli_M4"))
            {
                // Crea un effetto di impatto del proiettile
                CreateBulletImpactEffect(collision);
            }
            else
            {
                CreateShotgunImpactEffect(collision, 10, 0.1f);
            }

            // Se il proiettile collide con un oggetto, distruggilo
            Destroy(gameObject);
        } 
        else if (collision.gameObject.CompareTag("Zombie"))
        {
            Debug.Log("Hit Zombie");

            Weapon weapon = WeaponManager._Instance.GetActiveWeapon();

            ChangeDot._Instance.OnShoot(weapon);

            // Ottiene il componente Zombie dell'oggetto colpito
            Zombie zombie = collision.gameObject.GetComponent<Zombie>();

            // Se il componente Zombie è diverso da null
            if(zombie != null && !zombie.isDead)
            {
                // Chiamare il metodo Damage del componente Zombie
                zombie.Damage(damage, true);
                CreateBloodSprayEffect(collision);
            }

            // Se il proiettile collide con un oggetto, distruggilo
            Destroy(gameObject);
        }
        else if (collision.gameObject.CompareTag("ZombieHead"))
        {
            Debug.Log("Hit Zombie Head");

            // Ottiene il componente Zombie dell'oggetto colpito
            Zombie zombie = collision.gameObject.GetComponent<Zombie>();

            // Se il componente Zombie è diverso da null
            if(zombie != null && !zombie.isDead)
            {
                // Chiamare il metodo Damage del componente Zombie
                zombie.Damage(100, true);
                CreateBloodSprayEffect(collision);
            }

            // Se il proiettile collide con un oggetto, distruggilo
            Destroy(gameObject);
        }
    }

    private void CreateBloodSprayEffect(Collision objHitted)
    {
        // Ottiene il punto di contatto della collisione
        ContactPoint contactPoint = objHitted.contacts[0];

        // Istanzia l'effetto di impatto del proiettile nel punto di contatto
        GameObject bloodSprayPref = Instantiate(
            GlobalReferencies._Instance.bloodSprayEffect, // Prefab dell'effetto di impatto
            contactPoint.point, // Posizione del punto di contatto
            Quaternion.LookRotation(contactPoint.normal) // Rotazione basata sulla normale del punto di contatto
        );

        // Imposta l'oggetto creato come figlio dell'oggetto colpito
        bloodSprayPref.transform.SetParent(objHitted.gameObject.transform);
    }

    // Metodo per creare un effetto di impatto del proiettile
    private void CreateBulletImpactEffect(Collision objHitted)
    {
        // Ottiene il punto di contatto della collisione
        ContactPoint contactPoint = objHitted.contacts[0];

        // Istanzia l'effetto di impatto del proiettile nel punto di contatto
        GameObject hole = Instantiate(
            GlobalReferencies._Instance.bulletImpactEffectPrefab, // Prefab dell'effetto di impatto
            contactPoint.point, // Posizione del punto di contatto
            Quaternion.LookRotation(contactPoint.normal) // Rotazione basata sulla normale del punto di contatto
        );

        // Imposta l'oggetto creato come figlio dell'oggetto colpito
        hole.transform.SetParent(objHitted.gameObject.transform);
    }

    // Metodo per creare l'effetto di sparo del fucile a pompa dopo una collisione
    private void CreateShotgunImpactEffect(Collision objHitted, int pellets, float spreadRadius)
    {
        // Ottieni il punto di contatto della collisione iniziale
        ContactPoint contactPoint = objHitted.contacts[0];

        // Loop per simulare i pellet del fucile a pompa
        for (int i = 0; i < pellets; i++)
        {
            // Genera una leggera dispersione attorno al punto di contatto originale
            Vector3 spreadPoint = contactPoint.point;
            spreadPoint += new Vector3(
                UnityEngine.Random.Range(-spreadRadius, spreadRadius), // Dispersione sull'asse X
                UnityEngine.Random.Range(-spreadRadius, spreadRadius), // Dispersione sull'asse Y
                UnityEngine.Random.Range(-spreadRadius, spreadRadius)  // Dispersione sull'asse Z
            );

            // Istanzia l'effetto di impatto del proiettile nel punto di dispersione
            GameObject hole = Instantiate(
                GlobalReferencies._Instance.bulletImpactEffectPrefab, // Prefab dell'effetto di impatto
                spreadPoint, // Posizione con dispersione
                Quaternion.LookRotation(contactPoint.normal) // Rotazione basata sulla normale del punto di contatto
            );

            // Imposta l'oggetto creato come figlio dell'oggetto colpito
            hole.transform.SetParent(objHitted.gameObject.transform);
        }
    }


}