using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// Questa classe gestisce la generazione degli zombie in onde successive
public class ZombieSpawnController : MonoBehaviour
{
    // Numero iniziale di zombie per ogni onda
    public int initialZombiesPerWave = 5;
    // Numero corrente di zombie per ogni onda
    public int currentZombiesPerWave;

    // Incremento del numero di zombie per ogni nuova onda
    public int incrementZombiesPerWave = 5;

    // Ritardo tra uno spawn e l'altro di uno zombie
    public float spawnDelay = 1f;

    // Numero corrente dell'onda
    public int currentWave = 0;
    // Ritardo tra la fine di un'onda e l'inizio della successiva
    public float waveDelay = 10f;

    // Indica se il gioco è in fase di pre-onda
    public bool isInPreWave;
    // Contatore per il cooldown tra le onde
    public float cooldownCounter = 0;

    // Lista degli zombie attualmente vivi
    public List<Zombie> currentZombiesAlive;

    // Lista dei punti di spawn
    public List<Transform> spawnPoints;

    // Lista dei prefab degli zombie da spawnare
    public List<GameObject> zombiePrefabsList;

    public float healthIncrementPerWave;
    public float damageIncrementPerWave;

    public float healthIncrementBoss;
    public float damageIncrementBoss;
    private int currentZombieBoss;

    // Metodo chiamato all'inizio del gioco
    private void Start()
    {   
        currentZombieBoss = 0;
        // Imposta il numero corrente di zombie per onda al valore iniziale
        currentZombiesPerWave = initialZombiesPerWave;

        // Imposta l'onda corrente a 0 nelle referenze globali
        GlobalReferencies._Instance.currentWave = 0;

        // Inizia la prima onda
        StartNextWave();
    }

    // Metodo chiamato ad ogni frame
    private void Update()
    {
        // Lista temporanea per gli zombie da rimuovere
        List<Zombie> zombiesToRemove = new List<Zombie>();
        foreach (var zombie in currentZombiesAlive)
        {
            // Se lo zombie è morto, aggiungilo alla lista da rimuovere
            if (zombie.isDead)
            {
                zombiesToRemove.Add(zombie);
            }
        }

        // Rimuove gli zombie morti dalla lista degli zombie vivi
        foreach (Zombie zombie in zombiesToRemove)
        {
            currentZombiesAlive.Remove(zombie);
        }

        // Pulisce la lista temporanea
        zombiesToRemove.Clear();

        // Se non ci sono zombie vivi e non siamo in fase di pre-onda, inizia la fase di pre-onda
        if (currentZombiesAlive.Count == 0 && !isInPreWave)
        {
            StartCoroutine(StartPreWave());
        }

        // Aggiorna il contatore del cooldown se siamo in fase di pre-onda
        if (isInPreWave)
        {
            cooldownCounter -= Time.deltaTime;
        }
        else
        {
            cooldownCounter = waveDelay;
        }

        // Aggiorna l'interfaccia utente con il contatore del cooldown
        HUDManager._Instance.preStartWaveCounterUI.text = cooldownCounter.ToString("F0");
    }

    // Coroutine per gestire la fase di pre-onda
    private IEnumerator StartPreWave()
    {
        // Imposta la fase di pre-onda a true
        isInPreWave = true;
        // Mostra l'interfaccia utente della fine dell'onda
        HUDManager._Instance.waveOverUI.gameObject.SetActive(true);

        if (currentWave + 1 == 3)
        {
            WeaponManager._Instance.SpawnWeapon(Weapon.WeaponModel.M4);
            HUDManager._Instance.weaponUnlockedUI.gameObject.SetActive(true);
        }
        else if (currentWave + 1 == 6)
        {
            WeaponManager._Instance.SpawnWeapon(Weapon.WeaponModel.Benelli);
            HUDManager._Instance.weaponUnlockedUI.gameObject.SetActive(true);
        }

        if(currentWave + 1 >= 5)
        {
            WeaponManager._Instance.SpawnLethals(Throwable.ThrowableType.Grenade);
            WeaponManager._Instance.SpawnTacticals(Throwable.ThrowableType.Smoke_Grenade);
        }

        // Aspetta per il tempo del cooldown
        yield return new WaitForSeconds(cooldownCounter);

        // Imposta la fase di pre-onda a false
        isInPreWave = false;
        // Nasconde l'interfaccia utente della fine dell'onda
        HUDManager._Instance.waveOverUI.gameObject.SetActive(false);
        if(HUDManager._Instance.weaponUnlockedUI.IsActive())
        {
            HUDManager._Instance.weaponUnlockedUI.gameObject.SetActive(false);
        }

        // Incrementa il numero di zombie per onda
        currentZombiesPerWave += incrementZombiesPerWave; // Da modificare a seconda della difficoltà
        // Inizia la prossima onda
        StartNextWave();
    }

    // Metodo per iniziare la prossima onda
    private void StartNextWave()
    {
        // Pulisce la lista degli zombie vivi
        currentZombiesAlive.Clear();

        // Incrementa il numero dell'onda corrente
        currentWave++;
        // Aggiorna l'onda corrente nelle referenze globali
        GlobalReferencies._Instance.currentWave = currentWave;

        // Aggiorna l'interfaccia utente con il numero dell'onda corrente
        HUDManager._Instance.currentWaveUI.text = "Wave: " + currentWave.ToString();

        // Inizia la coroutine per spawnare gli zombie dell'onda
        StartCoroutine(SpawnWave());
    }

    // Coroutine per spawnare gli zombie dell'onda
    private IEnumerator SpawnWave()
    {
        //BOSS LEVEL
        if(currentWave % 5 == 0)
        {
            currentZombieBoss++;
            for(int i=0; i<currentZombieBoss; i++)
            {
                var zombieBoss = Instantiate(zombiePrefabsList[1], spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Count)].position, Quaternion.identity);
                Zombie z = zombieBoss.GetComponent<Zombie>();

                if(currentWave > 5)
                {
                    z.IncrementHealth((int)Math.Ceiling(healthIncrementBoss * (currentWave - 1)));
                    z.IncrementDamage((int)Math.Ceiling(damageIncrementBoss * (currentWave - 1)));
                }
                
                currentZombiesAlive.Add(z);
            }

        }

        for (int i = 0; i < currentZombiesPerWave; i++)
        {

            // Seleziona un punto di spawn casuale dalla lista
            Transform spawnPoint = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Count)];
    
            // Calcola una posizione di spawn casuale intorno al punto di spawn
            Vector3 spawnOffset = new Vector3(UnityEngine.Random.Range(-1.2f, 1.2f), 0, UnityEngine.Random.Range(-3f, 3f));
            Vector3 spawnPosition = spawnPoint.position + spawnOffset;

            // Instanzia uno zombie alla posizione di spawn
            var zombie = Instantiate(zombiePrefabsList[0], spawnPosition, Quaternion.identity);

            if(currentWave > 1)
            {
                Zombie z = zombie.GetComponent<Zombie>();
                z.IncrementHealth((int)Math.Ceiling(healthIncrementPerWave * (currentWave - 1)));
                z.IncrementDamage((int)Math.Ceiling(damageIncrementPerWave * (currentWave - 1)));
            }
            

            // Aggiunge lo zombie alla lista degli zombie vivi
            currentZombiesAlive.Add(zombie.GetComponent<Zombie>());

            // Aspetta per il tempo del ritardo tra uno spawn e l'altro
            yield return new WaitForSeconds(spawnDelay);
        }
    }
}