using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Throwable : MonoBehaviour
{
    [SerializeField] float delay = 3f;
    [SerializeField] float explosionRadius = 20f;
    [SerializeField] float explosionForce = 1200f;

    [SerializeField] int explosionDamage;

    float countDown;

    bool hasExploded = false;
    public bool hasBeenThrown = false;

    public enum ThrowableType 
    {
         None,
         Grenade,
         Smoke_Grenade,
    };


    public ThrowableType throwableType;


    private void Start()
    {
        countDown = delay;
    }

    public void Update()
    {
        if(hasBeenThrown)
        {
            countDown -= Time.deltaTime;
            if(countDown <= 0 && !hasExploded)
            {
                Explode();
                Destroy(gameObject);
                hasExploded = true;
            }
        }
    }

    private void Explode()
    {
        GetThrowableEffect();
    }

    private void GetThrowableEffect()
    {
        switch(throwableType)
        {
            case ThrowableType.Grenade:
                GrenadeEffect();
                break;
            case ThrowableType.Smoke_Grenade:
                SmokeGrenadeEffect();
                break;
        }
    }

    private void SmokeGrenadeEffect()
    {
    
        // Effetto dell'esplosione
        GameObject smokeEffect = GlobalReferencies._Instance.smokeGrenadeEffect;
        Instantiate(smokeEffect, transform.position, transform.rotation);

        //Suono dell'esplosione
        SoundManager._Instance.throwablesChannel.PlayOneShot(SoundManager._Instance.grenadeClip);

        // Effetti fisiici dell'esplosione
        // Trova tutti i collider all'interno del raggio di esplosione
        // Applica una forza esplosiva a tutti i rigidbody trovati
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach(Collider collider in colliders)
        {

            Rigidbody rb = collider.GetComponent<Rigidbody>();
            if(rb != null)
            {
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
            }

            // Se il collider ha un componente Zombie, infliggi danno
            if(collider.gameObject.CompareTag("Zombie"))
            {
                Zombie zombie = collider.GetComponent<Zombie>();
                if(zombie != null && !zombie.isDead)
                {
                    zombie.TacticalEffect();
                }
            }
            
        }
    }

    private void GrenadeEffect()
    {
        // Effetto dell'esplosione
        GameObject explosionEffect = GlobalReferencies._Instance.grenadeExplosionEffect;
        Instantiate(explosionEffect, transform.position, transform.rotation);

        //Suono dell'esplosione
        SoundManager._Instance.throwablesChannel.PlayOneShot(SoundManager._Instance.grenadeClip);

        // Effetti fisiici dell'esplosione
        // Trova tutti i collider all'interno del raggio di esplosione
        // Applica una forza esplosiva a tutti i rigidbody trovati
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach(Collider collider in colliders)
        {
            Rigidbody rb = collider.GetComponent<Rigidbody>();
            if(rb != null)
            {
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
            }

            // Se il collider ha un componente Zombie, infliggi danno
            if(collider.gameObject.CompareTag("Zombie"))
            {
                Zombie zombie = collider.GetComponent<Zombie>();
                if(zombie != null && !zombie.isDead)
                {
                    zombie.Damage(explosionDamage, true);
                }
            }

            
        }
    }
}
