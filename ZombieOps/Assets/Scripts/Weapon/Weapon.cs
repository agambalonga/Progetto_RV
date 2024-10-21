using System.Collections;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public bool isActiveWeapon;
    public int weaponDamage;
    private GameObject middleDot;

    // Variabili per il tiro
    [Header("Shooting")]
    public bool isShooting;
    public bool readyToShoot;
    bool allowReset = true;
    public float shootingDelay = 2f;
    // Variabili per il tiro a raffica
    public int bulletsPerBurst = 3;
    public int burstBulletsLeft;

    // Intensità della dispersione dei proiettili
    [Header("Spread")]
    private float spreadIntensity;
    public float noAdsSpreadIntensity;
    public float adsSpreadIntensity;

    // Variabili per il proiettile
    [Header("Bullet")]
    public GameObject bulletPrefab;
    public Transform bulletSpawn;
    public float bulletVelocity = 30f;
    public float bulletPrefabLifetime = 3f;
    // Variabili per l'effetto della fiamma del proiettile e l'animazione di rinculo
    public GameObject muzzleEffect;
    internal Animator animator;

    [Header("Reload")]
    public float reloadTime;
    public int bulletsLeft;
    public int bulletsPerMag;
    public bool isReloading;

    [Header("ADS")]
    public bool isADSMode;

    [Header("Weapon Spawn")]
    public Vector3 spawnPosition;
    public Vector3 spawnRotation;
    public Camera weapoCamera;

    private PlayerMovement player;

    // Modalità di tiro
    public enum ShootingMode { Single, Burst, Auto };
    public ShootingMode currentShootingMode;

    public enum WeaponModel
    {
        Pistol,
        M4,
        Benelli,
    }

    public WeaponModel currentWeaponModel;

    private void Awake()
    {
        // Inizializza le variabili
        readyToShoot = true;
        burstBulletsLeft = bulletsPerBurst;
        animator = GetComponent<Animator>();
        bulletsLeft = bulletsPerMag;

        spreadIntensity = noAdsSpreadIntensity;
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
        middleDot = GameObject.Find("MiddleDot");
    }

    // Update is called once per frame
    void Update()
    {
        if(isActiveWeapon)
        {
            // Imposta il layer dell'arma attiva per il rendering
            transform.gameObject.layer = LayerMask.NameToLayer("WeaponRender");
            UpdateLayerRecursively(transform, LayerMask.NameToLayer("WeaponRender"));

            // Gestisce l'entrata e l'uscita dalla modalità ADS (mirino)
            if(Input.GetMouseButtonDown(1))
            {
                EnterADSMode();
            } 

            if(Input.GetMouseButtonUp(1))
            {
                ExitADSMode();
            }

            // Riproduce il suono del caricatore vuoto se i proiettili sono finiti e si sta sparando
            if(bulletsLeft == 0 && isShooting)
            {
                SoundManager._Instance.emptyMagazine.Play();
            }

            // Controlla la modalità di tiro corrente
            if(currentShootingMode == ShootingMode.Auto && !isReloading)
            {
                // Tieni premuto il pulsante sinistro del mouse per sparare
                isShooting = Input.GetKey(KeyCode.Mouse0);
            }
            else if((currentShootingMode == ShootingMode.Single || currentShootingMode == ShootingMode.Burst) && !isReloading)
            {
                // Premi il pulsante sinistro del mouse per sparare (una volta)
                isShooting = Input.GetKeyDown(KeyCode.Mouse0);
            }

            // Gestisce la ricarica manuale
            if(Input.GetKeyDown(KeyCode.R) && bulletsLeft < bulletsPerMag && !isReloading && WeaponManager._Instance.CheckAmmoLeft(currentWeaponModel) > 0 && !isShooting)
            {
                Reload();
            }

            // Ricarica automatica
            if(readyToShoot && !isShooting && !isReloading && bulletsLeft <= 0 && WeaponManager._Instance.CheckAmmoLeft(currentWeaponModel) > 0)
            {
                Reload();
            }

            // Se pronto a sparare e il giocatore sta sparando
            if(readyToShoot && isShooting && bulletsLeft > 0)
            {
                // Reset del conteggio dei proiettili per la raffica
                burstBulletsLeft = bulletsPerBurst;
                Fire();
            }
        } 
        else
        {
            // Imposta il layer dell'arma inattiva come predefinito
            transform.gameObject.layer = LayerMask.NameToLayer("Default");
            UpdateLayerRecursively(transform, LayerMask.NameToLayer("Default"));
        }
    }

    private void EnterADSMode()
    {
        isADSMode = true;
        animator.SetTrigger("enterADS");
        HUDManager._Instance.middleDot.SetActive(false);
        spreadIntensity = adsSpreadIntensity;

        StartCoroutine(ChangeFOV(Camera.main, 60, 40, 0.1f));
        StartCoroutine(ChangeFOV(weapoCamera, 60, 40, 0.1f));

        player.isAds = true;
    }

    private void ExitADSMode()
    {
        isADSMode = false;
        animator.SetTrigger("exitADS");
        HUDManager._Instance.middleDot.SetActive(true);
        spreadIntensity = noAdsSpreadIntensity;

        StartCoroutine(ChangeFOV(Camera.main, 40, 60, 0.1f));
        StartCoroutine(ChangeFOV(weapoCamera, 40, 60, 0.1f));

        player.isAds = false;
    }

    private IEnumerator ChangeFOV(Camera camera, float startFOV, float endFOV, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            Camera.main.fieldOfView = Mathf.Lerp(startFOV, endFOV, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        Camera.main.fieldOfView = endFOV;
    }

    private void Fire() 
    {
        bulletsLeft--;

        // Effetto della fiamma del proiettile
        muzzleEffect.GetComponent<ParticleSystem>().Play();

        // Attiva l'animazione di rinculo
        if(!isADSMode)
        {
            if (middleDot != null)
            {
                middleDot.GetComponent<ChangeDot>().OnShoot();
            }
            animator.SetTrigger("RECOIL");
        } 
        else
        {
            animator.SetTrigger("RECOIL_ADS");
        }

        SoundManager._Instance.PlayShootingSount(currentWeaponModel);

        // Imposta lo stato di non pronto a sparare
        readyToShoot = false;
        // Calcola la direzione del tiro e la dispersione
        Vector3 shootingDirection = CalculateDirectionAndSpread().normalized;

        // Instanzia un proiettile
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position, Quaternion.identity);
        Bullet bull = bullet.GetComponent<Bullet>();

        bull.damage = weaponDamage;
        bullet.transform.forward = shootingDirection;

        // Spara il proiettile aggiungendo una forza
        bullet.GetComponent<Rigidbody>().AddForce(shootingDirection * bulletVelocity, ForceMode.Impulse); 

        // Distrugge il proiettile dopo un certo tempo
        StartCoroutine(DestroyBullet(bullet, bulletPrefabLifetime));

        // Se è permesso il reset, imposta un ritardo per il prossimo tiro
        if(allowReset)
        {
            Invoke("ResetShot", shootingDelay);
            allowReset = false;
        }

        // Modalità di tiro a raffica
        if(currentShootingMode == ShootingMode.Burst && burstBulletsLeft > 1) // > 1 perchè il proiettile appena sparato è già stato contato
        {
            // Dobbiamo sparare altri proiettili della raffica
            burstBulletsLeft--;
            Invoke("Fire", shootingDelay);
        }
    }

    private void Reload() 
    {
        isReloading = true;

        animator.SetTrigger("RELOAD");
        SoundManager._Instance.PlayReloadSound(currentWeaponModel);
        
        // Attendi per un certo tempo prima di ricaricare
        Invoke("ReloadFinished", reloadTime);
    }

    private void ReloadFinished()
    {
        int ammoLeft = WeaponManager._Instance.CheckAmmoLeft(currentWeaponModel);
        int bulletsToReload = bulletsPerMag - bulletsLeft;

        if(ammoLeft >= bulletsToReload)
        {
            bulletsLeft += bulletsToReload;
            WeaponManager._Instance.DecreaseTotalAmmo(currentWeaponModel, bulletsToReload);
        }
        else
        {
            bulletsLeft += ammoLeft;
            WeaponManager._Instance.DecreaseTotalAmmo(currentWeaponModel, ammoLeft);
        }

        isReloading = false;
    }

    private void ResetShot()
    {
        // Resetta lo stato di pronto a sparare
        readyToShoot = true;
        allowReset = true;
    }

    private Vector3 CalculateDirectionAndSpread()
    {
        // Crea un raggio dalla camera del giocatore
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        Vector3 targetPoint;
        // Se il raggio colpisce qualcosa, imposta il punto di destinazione
        if(Physics.Raycast(ray, out hit))
        {
            targetPoint = hit.point;
        }
        else
        {
            // Altrimenti, imposta un punto lontano
            targetPoint = ray.GetPoint(100);
        }

        // Calcola la direzione del tiro
        Vector3 direction = targetPoint - bulletSpawn.position;

        // Aggiunge dispersione alla direzione del tiro (per simulare la precisione delle armi)
        float z = UnityEngine.Random.Range(-spreadIntensity, spreadIntensity);
        float y = UnityEngine.Random.Range(-spreadIntensity, spreadIntensity);

        return direction + new Vector3(0, y, z);
    }

    private IEnumerator DestroyBullet(GameObject bullet, float delay)
    {
        // Attende per un certo tempo
        yield return new WaitForSeconds(delay);

        // Distrugge il proiettile
        Destroy(bullet);
    }

    void UpdateLayerRecursively(Transform parent, int layer)
    {
        parent.gameObject.layer = layer;

        if(parent.childCount == 0)
        {
            return;
        }

        foreach(Transform child in parent)
        {
            UpdateLayerRecursively(child, layer);
        }
    }
}