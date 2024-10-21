using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Android;

public class WeaponManager : MonoBehaviour
{
    // Istanza singleton del WeaponManager
    public static WeaponManager _Instance { get; set; }

    // Lista degli slot delle armi
    public List<GameObject> weaponSlots = new List<GameObject>();

    // Slot dell'arma attiva
    public GameObject activeWeaponSlot;

    [Header("Ammo")]
    public int totalRifleAmmo = 0; // Munizioni totali per il fucile
    public int totalPistolAmmo = 0; // Munizioni totali per la pistola
    public int totalBenelliAmmo = 0; // Munizioni totali per il fucile Benelli

    [Header("Throwables General")]
    public float throwForce = 20f; // Forza di lancio
    public GameObject throwableSpawnPoint; // Punto di spawn degli oggetti lanciabili
    public float forceMultiplier = 0; // Moltiplicatore di forza
    public float maxForce = 2f; // Forza massima

    [Header("Lethals")]
    public int lethalsCount = 0; // Conteggio degli oggetti letali
    public Throwable.ThrowableType equippedLethalType; // Tipo di oggetto letale equipaggiato
    public GameObject grenadePrefab; // Prefab della granata
    private readonly int MAX_LETHALS = 2; // Numero massimo di oggetti letali

    [Header("Tacitcals")]
    public int tacticalsCount = 0; // Conteggio degli oggetti tattici
    public Throwable.ThrowableType equippedTacticalType; // Tipo di oggetto tattico equipaggiato
    public GameObject smokeGrenadePrefab; // Prefab della granata fumogena
    private readonly int MAX_TACTICALS = 1; // Numero massimo di oggetti tattici
    private readonly int LIMIT_LETHAL_SPAWN = 4; // Limite di spawn degli oggetti letali
    private readonly int LIMIT_TACITCAL_SPAWN = 2; // Limite di spawn degli oggetti tattici

    [Header("Limit_Ammo")]
    public int limitPistol = 50; // Limite munizioni pistola
    public int limitShotgun = 32; // Limite munizioni fucile a pompa
    public int limitRifle = 120; // Limite munizioni fucile

    [Header("Spawn_Weapon")]
    public GameObject m4; // Prefab del fucile M4
    public GameObject bennelli; // Prefab del fucile Benelli

    [Header("Spawn_Throwable")]
    public GameObject lethalSpawn; // Punto di spawn degli oggetti letali
    public GameObject tacticalSpawn; // Punto di spawn degli oggetti tattici

    private int numberOfGrenadesSpawned = 0; // Numero di granate spawnate
    private int numberOfTacticalSpawned = 0; // Numero di oggetti tattici spawnati

    private void Start()
    {
        // Imposta lo slot dell'arma attiva e i tipi di oggetti equipaggiati
        activeWeaponSlot = weaponSlots[0];
        equippedLethalType = Throwable.ThrowableType.None;
        equippedTacticalType = Throwable.ThrowableType.None;
    }

    public void Awake()
    {
        // Imposta l'istanza singleton
        if (_Instance == null)
        {
            _Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void Update() 
    {
        // Attiva lo slot dell'arma attiva e disattiva gli altri
        foreach(GameObject weaponSlot in weaponSlots)
        {
            if(weaponSlot == activeWeaponSlot)
            {
                weaponSlot.SetActive(true);
            } else
            {
                weaponSlot.SetActive(false);
            }
        }

        // Cambia arma con i tasti numerici
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            SwitchWeapon(0);
        } else if(Input.GetKeyDown(KeyCode.Alpha2))
        {
            SwitchWeapon(1);
        }

        // Incrementa la forza del lancio in base a quanto tempo il giocatore tiene premuto il tasto G o F
        if(Input.GetKey(KeyCode.G) || Input.GetKey(KeyCode.F))
        {
            forceMultiplier += Time.deltaTime;

            if(forceMultiplier > maxForce)
            {
                forceMultiplier = maxForce;
            }
        }

        // Lancia l'oggetto letale quando il tasto G viene rilasciato
        if(Input.GetKeyUp(KeyCode.G))
        {
            if(lethalsCount > 0)
            {
                ThrowLethal();
            }

            forceMultiplier = 0;
        }

        // Lancia l'oggetto tattico quando il tasto F viene rilasciato
        if(Input.GetKeyUp(KeyCode.F))
        {
            if(tacticalsCount > 0)
            {
                ThrowTactical();
            }

            forceMultiplier = 0;
        }
    }

    public void PickUpWeapon(GameObject pickedUpWeapon)
    {
        GameObject weaponSlotEmpty = GetWeaponSlotEmpty();

        if(weaponSlotEmpty == null) {
            DropCurrentWeapon(pickedUpWeapon);
        } else
        {
            PlaceWeaponInSlot(pickedUpWeapon, weaponSlotEmpty);
        }
    }

    #region || --- Weapon --- ||
    private void PlaceWeaponInSlot(GameObject pickedUpWeapon, GameObject weaponSlotEmpty)
    {
        // Imposta il genitore dell'arma raccolta come lo slot dell'arma vuoto
        pickedUpWeapon.transform.SetParent(weaponSlotEmpty.transform, false);

        // Ottiene il componente Weapon dall'arma raccolta
        Weapon weapon = pickedUpWeapon.GetComponent<Weapon>();

        // Imposta la posizione locale dell'arma raccolta in base alla posizione di spawn dell'arma
        pickedUpWeapon.transform.localPosition = new Vector3(weapon.spawnPosition.x, weapon.spawnPosition.y, weapon.spawnPosition.z);
        // Imposta la rotazione locale dell'arma raccolta in base alla rotazione di spawn dell'arma
        pickedUpWeapon.transform.localRotation = Quaternion.Euler(weapon.spawnRotation.x, weapon.spawnRotation.y, weapon.spawnRotation.z);

        // Se lo slot dell'arma vuoto è lo slot dell'arma attiva, imposta l'arma come attiva
        if(weaponSlotEmpty == activeWeaponSlot)
        {
            weapon.isActiveWeapon = true;
        } 
        else
        {
            weapon.isActiveWeapon = false;
        }

        // Abilita l'animatore dell'arma
        weapon.animator.enabled = true;

        // Accendi la luce dell'arma
        GameObject torchObject = pickedUpWeapon.transform.Find("Torch").gameObject;
        if(torchObject != null)
        {
            torchObject.SetActive(true);  // Attiva l'intero GameObject (e quindi la luce)
        }
    }

    private GameObject GetWeaponSlotEmpty()
    {
        // Itera attraverso tutti gli slot delle armi
        foreach(GameObject weaponSlot in weaponSlots)
        {
            // Controlla se lo slot dell'arma è vuoto (non ha figli)
            if(weaponSlot.transform.childCount == 0)
            {
                // Ritorna lo slot dell'arma vuoto
                return weaponSlot;
            }
        }

        // Se nessuno slot dell'arma è vuoto, ritorna null
        return null;
    }

    private void DropCurrentWeapon(GameObject pickedUpWeapon)
    {
        // Ottiene l'arma attualmente equipaggiata dallo slot dell'arma attiva
        GameObject weaponToDrop = activeWeaponSlot.transform.GetChild(0).gameObject;

        // Imposta il genitore dell'arma da lasciare come il genitore dell'arma raccolta
        weaponToDrop.transform.SetParent(pickedUpWeapon.transform.parent);

        // Imposta la posizione e la rotazione dell'arma da lasciare in base all'arma raccolta
        weaponToDrop.transform.localPosition = pickedUpWeapon.transform.localPosition;
        weaponToDrop.transform.localRotation = pickedUpWeapon.transform.localRotation;
        weaponToDrop.GetComponent<Weapon>().isActiveWeapon = false;
        weaponToDrop.GetComponent<Weapon>().animator.enabled = false;

        // Disattiva la luce dell'arma da lasciare
        GameObject torchObject = weaponToDrop.transform.Find("Torch").gameObject;
        if(torchObject != null)
        {
            torchObject.SetActive(false);  // Disattiva l'intero GameObject (e quindi la luce)
        }

        // Imposta il genitore dell'arma raccolta come lo slot dell'arma attiva
        pickedUpWeapon.transform.SetParent(activeWeaponSlot.transform, false);

        // Ottiene il componente Weapon dall'arma raccolta
        Weapon weapon = pickedUpWeapon.GetComponent<Weapon>();

        // Imposta la posizione e la rotazione dell'arma raccolta in base alla posizione e rotazione di spawn dell'arma
        pickedUpWeapon.transform.localPosition = new Vector3(weapon.spawnPosition.x, weapon.spawnPosition.y, weapon.spawnPosition.z);
        pickedUpWeapon.transform.localRotation = Quaternion.Euler(weapon.spawnRotation.x, weapon.spawnRotation.y, weapon.spawnRotation.z);

        // Attiva la luce dell'arma raccolta
        GameObject torchWeapon = pickedUpWeapon.transform.Find("Torch").gameObject;
        if(torchWeapon != null)
        {
            torchWeapon.SetActive(true);  // Attiva l'intero GameObject (e quindi la luce)
        }

        // Imposta l'arma raccolta come attiva e abilita l'animatore
        weapon.isActiveWeapon = true;
        weapon.animator.enabled = true;
    }

    public void SwitchWeapon(int weaponIndex)
    {
        // Disattiva lo slot dell'arma attualmente attiva
        activeWeaponSlot.SetActive(false);

        // Imposta il nuovo slot dell'arma attiva in base all'indice fornito
        activeWeaponSlot = weaponSlots[weaponIndex];

        // Attiva il nuovo slot dell'arma attiva
        activeWeaponSlot.SetActive(true);

        // Se il nuovo slot dell'arma attiva contiene un'arma, imposta l'arma come attiva
        if(activeWeaponSlot.transform.childCount > 0)
        {
            activeWeaponSlot.transform.GetChild(0).GetComponent<Weapon>().isActiveWeapon = true;
        }
    }

    public void DecreaseTotalAmmo(Weapon.WeaponModel currentWeaponModel, int bulletsToDecrease)
    {
        // Diminuisce il numero totale di munizioni in base al modello dell'arma
        switch(currentWeaponModel)
        {
            case Weapon.WeaponModel.Pistol:
                totalPistolAmmo -= bulletsToDecrease;
                break;
            case Weapon.WeaponModel.M4:
                totalRifleAmmo -= bulletsToDecrease;
                break;
            case Weapon.WeaponModel.Benelli:
                totalBenelliAmmo -= bulletsToDecrease;
                break;
        }
    }

    public int CheckAmmoLeft(Weapon.WeaponModel currentWeaponModel)
    {
        // Restituisce il numero di munizioni rimanenti in base al modello dell'arma
        switch(currentWeaponModel)
        {
            case Weapon.WeaponModel.Pistol:
                return totalPistolAmmo;
            case Weapon.WeaponModel.M4:
                return totalRifleAmmo;
            case Weapon.WeaponModel.Benelli:
                return totalBenelliAmmo;
            default:
                return 0;
        }
    }

#endregion

#region || --- Ammo --- ||
    public void PickUpAmmoBox(AmmoBox pickedUpAmmoBox)
    {
        // Aggiunge le munizioni raccolte in base al tipo di munizioni
        switch (pickedUpAmmoBox.ammoType)
        {
            case AmmoBox.AmmoType.RifleAmmo:
                totalRifleAmmo += pickedUpAmmoBox.ammoAmount;
                break;
            case AmmoBox.AmmoType.PistolAmmo:
                totalPistolAmmo += pickedUpAmmoBox.ammoAmount;
                break;
            case AmmoBox.AmmoType.BenelliAmmo:
                totalBenelliAmmo += pickedUpAmmoBox.ammoAmount;
                break;
        }
    }

    public void PickAmmoTotal()
    {
        // Imposta il numero totale di munizioni ai limiti massimi
        totalRifleAmmo = limitRifle;
        totalPistolAmmo = limitPistol;
        totalBenelliAmmo = limitShotgun;
    }

    #endregion

    #region || --- Throwable --- ||
    public void PickUpThrowable(Throwable hoveredThrowable)
    {
        // Raccoglie un oggetto lanciabile in base al tipo
        switch(hoveredThrowable.throwableType)
        {
            case Throwable.ThrowableType.Grenade:
                PickupThrowableAsLethal(Throwable.ThrowableType.Grenade);
                break;
            case Throwable.ThrowableType.Smoke_Grenade:
                PickupThrowableAsTactical(Throwable.ThrowableType.Smoke_Grenade);
                break;
        }
    }

    private void PickupThrowableAsTactical(Throwable.ThrowableType tactical)
    {
        // Raccoglie un oggetto tattico
        if(equippedTacticalType == tactical || equippedTacticalType == Throwable.ThrowableType.None)
        {
            equippedTacticalType = tactical;

            if(tacticalsCount < MAX_TACTICALS)
            {
                tacticalsCount += 1;
                Destroy(InteractionManager._Instance.hoveredThrowable.gameObject);
                HUDManager._Instance.UpdateThrowablesUI();
            }
            else 
            {
                Debug.Log("You can't carry more than 2 tacticals");
            }
        } 
        else
        {
            // Scambia gli oggetti tattici
        }
    }

    private void PickupThrowableAsLethal(Throwable.ThrowableType lethal)
    {
        // Raccoglie un oggetto letale
        if(equippedLethalType == lethal || equippedLethalType == Throwable.ThrowableType.None)
        {
            equippedLethalType = lethal;

            if(lethalsCount < MAX_LETHALS)
            {
                lethalsCount += 1;
                Destroy(InteractionManager._Instance.hoveredThrowable.gameObject);
                HUDManager._Instance.UpdateThrowablesUI();
            }
            else 
            {
                Debug.Log("You can't carry more than 2 lethals");
            }
        }
        else
        {
            // Scambia gli oggetti letali
        }
    }

    private void ThrowLethal()
    {
        // Lancia un oggetto letale
        GameObject lethalPrefab = GetThrowablePrefab(equippedLethalType);

        GameObject throwable = Instantiate(lethalPrefab, throwableSpawnPoint.transform.position, Camera.main.transform.rotation);
        Rigidbody rb = throwable.GetComponent<Rigidbody>();

        rb.AddForce(Camera.main.transform.forward * (throwForce * forceMultiplier), ForceMode.Impulse);

        throwable.GetComponent<Throwable>().hasBeenThrown = true;
        lethalsCount -= 1;

        if(lethalsCount == 0)
        {
            // Resetta a NONE in modo da poter raccogliere nuovamente gli esplosivi letali
            equippedLethalType = Throwable.ThrowableType.None;
        }

        HUDManager._Instance.UpdateThrowablesUI();
    }

    private void ThrowTactical()
    {
        // Lancia un oggetto tattico
        GameObject tacticalPrefab = GetThrowablePrefab(equippedTacticalType);

        GameObject throwable = Instantiate(tacticalPrefab, throwableSpawnPoint.transform.position, Camera.main.transform.rotation);
        Rigidbody rb = throwable.GetComponent<Rigidbody>();

        rb.AddForce(Camera.main.transform.forward * (throwForce * forceMultiplier), ForceMode.Impulse);

        throwable.GetComponent<Throwable>().hasBeenThrown = true;
        tacticalsCount -= 1;

        if(tacticalsCount == 0)
        {
            // Resetta a NONE in modo da poter raccogliere nuovamente gli esplosivi tattici
            equippedTacticalType = Throwable.ThrowableType.None;
        }

        HUDManager._Instance.UpdateThrowablesUI();
    }

    private GameObject GetThrowablePrefab(Throwable.ThrowableType throwableType)
    {
        // Restituisce il prefab dell'oggetto lanciabile in base al tipo
        switch(throwableType)
        {
            case Throwable.ThrowableType.Grenade:
                return grenadePrefab;
            case Throwable.ThrowableType.Smoke_Grenade:
                return smokeGrenadePrefab;
            default:
                return null;
        }
    }
    #endregion

    public Weapon GetActiveWeapon()
    {
        // Restituisce l'arma attiva se presente
        if (activeWeaponSlot.transform.childCount > 0)
        {
            return activeWeaponSlot.transform.GetChild(0).GetComponent<Weapon>();
        }
        return null; // Ritorna null se non c'è alcuna arma attiva
    }

    public void SpawnWeapon(Weapon.WeaponModel weaponModel)
    {
        // Attiva il prefab dell'arma in base al modello
        switch(weaponModel)
        {
            case Weapon.WeaponModel.M4:
                m4.SetActive(true);
                break;
            case Weapon.WeaponModel.Benelli:
                bennelli.SetActive(true);
                break;
        }
    }

    public void DisableWeapon()
    {
        // Disattiva tutte le armi negli slot
        foreach(GameObject weaponSlot in weaponSlots)
        {
            if(weaponSlot.transform.childCount > 0)
            {
                weaponSlot.transform.GetChild(0).gameObject.SetActive(false);
            }
        }
    }

    public void SpawnLethals(Throwable.ThrowableType throwableType)
    {
        // Spawna oggetti letali in base al tipo
        switch(throwableType)
        {
            case Throwable.ThrowableType.Grenade:
                if(numberOfGrenadesSpawned < LIMIT_LETHAL_SPAWN)
                {
                    for (int i = 0; i < LIMIT_LETHAL_SPAWN-numberOfGrenadesSpawned; i++)
                    {
                        // Calcola un offset per separare le granate
                        Vector3 offset = new Vector3(i * 0.1f, 0, 0);
                        Vector3 spawnPosition = lethalSpawn.transform.position + offset;

                        // Instanzia la granata con l'offset applicato
                        Instantiate(grenadePrefab, spawnPosition, Quaternion.identity);
                    }
                }
                break;
        }
    }

    public void SpawnTacticals(Throwable.ThrowableType throwableType)
    {
        // Spawna oggetti tattici in base al tipo
        switch(throwableType)
        {
            case Throwable.ThrowableType.Smoke_Grenade:
                if(numberOfTacticalSpawned < LIMIT_TACITCAL_SPAWN)
                {
                    for (int i = 0; i < LIMIT_TACITCAL_SPAWN-numberOfTacticalSpawned; i++)
                    {
                        // Calcola un offset per separare le granate
                        Vector3 offset = new Vector3(i * 0.1f, 0, 0);
                        Vector3 spawnPosition = tacticalSpawn.transform.position + offset;

                        // Instanzia la granata con l'offset applicato
                        Instantiate(smokeGrenadePrefab, spawnPosition, Quaternion.identity);
                    }
                }
                break;
        }
    }
}