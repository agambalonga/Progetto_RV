using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeDot : MonoBehaviour
{
    public static ChangeDot _Instance { get; set; }
    Weapon currentWeapon;
    Weapon previousWeapon;

    // Riferimento all'immagine del puntino (MiddleDot)
    private Image middleDotImage;

    // Immagini del puntino per le varie armi
    public Sprite defaultDot;
    public Sprite pistolDot;
    public Sprite rifleDot;
    public Sprite shotgunDot;

    // Scale per i vari puntini
    public Vector3 defaultScale = new Vector3(1, 1, 1);
    public Vector3 pistolScale = new Vector3(10f, 10f, 1);
    public Vector3 rifleScale = new Vector3(10f, 10f, 1);
    public Vector3 shotgunScale = new Vector3(15f, 15f, 1);
    public Color hitColor = Color.red; // Colore del puntino quando colpisce un bersaglio
    public Color originalColor = Color.white;

    // Scale per l'effetto di sparo
    public Vector3 shootScaleIncrease = new Vector3(50f, 50f, 1);  // Quanto aumentare lo scale quando si spara
    public float shootEffectDuration = 0.4f;  // Quanto dura l'effetto di allargamento
    public float hitZombieEffectDuration = 0.2f;

    //Variabile per controllo sparo
    private float resetDelay = 1f;
    private Coroutine resetCoroutine;
    private Vector3 maxCrosshairScale = new Vector3(20f, 20f, 2.0f); // Scala massima del puntatore


    void Awake()
    {
        // Assicura che ci sia solo una singola istanza
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

    void Start()
    {
        // Ottenere il componente Image del GameObject
        middleDotImage = GetComponent<Image>();
        currentWeapon = WeaponManager._Instance.GetActiveWeapon();
        if (currentWeapon != null)
        {
            ChangeCrosshair(currentWeapon);
        }
    }

    void Update()
    {
        currentWeapon = WeaponManager._Instance.GetActiveWeapon();
        // Chiama una funzione che cambia il puntino in base all'arma attualmente equipaggiata
        // Se l'arma � cambiata, aggiorna il puntatore
        if (currentWeapon != previousWeapon)
        {
            if (currentWeapon != null)
            {
                ChangeCrosshair(currentWeapon);
            }
            else
            {
                middleDotImage.sprite = defaultDot;
                transform.localScale = defaultScale;
            }
            previousWeapon = currentWeapon;
        }
    }

    // Funzione per cambiare il puntino centrale in base all'arma
    public void ChangeCrosshair(Weapon weapon)
    {
        switch (weapon.currentWeaponModel)
        {
            case Weapon.WeaponModel.Pistol:
                middleDotImage.sprite = pistolDot;
                transform.localScale = pistolScale;
                break;
            case Weapon.WeaponModel.M4:
                middleDotImage.sprite = rifleDot;
                transform.localScale = rifleScale;
                break;
            case Weapon.WeaponModel.Benelli:
                middleDotImage.sprite = shotgunDot;
                transform.localScale = shotgunScale;
                break;
            default:
                middleDotImage.sprite = defaultDot;
                transform.localScale = defaultScale;
                break;
        }
    }

    // Funzione per simulare lo sparo e l'effetto di allargamento del puntino
    public void OnShoot()
    {
        Debug.Log("Sparato con arma: " + (currentWeapon != null ? currentWeapon.currentWeaponModel.ToString() : "Nessuna arma"));
        // Aumenta la scala del puntino
        if (transform.localScale.x < maxCrosshairScale.x)
        {
            transform.localScale += shootScaleIncrease;
        }
        // Se c'� un reset in corso, lo riavvia (cos� non ripristiniamo subito mentre si sta ancora sparando)
        if (resetCoroutine != null)
        {
            StopCoroutine(resetCoroutine);
        }

        // Avvia la coroutine che attende un periodo di inattivit� prima di resettare il puntatore
        resetCoroutine = StartCoroutine(ResetCrosshairAfterDelay());
    }

    // Funzione per simulare lo sparo e l'effetto di allargamento del puntino
    public void OnShoot(Weapon weap)
    {
        if(!weap.isADSMode)
        {
            StartCoroutine(HitFeedback());
        }

    }

    // Coroutine per resettare il puntatore dopo un intervallo di inattivit�
    private IEnumerator ResetCrosshairAfterDelay()
    {
        // Attendi che passi il tempo di inattivit� definito
        yield return new WaitForSeconds(resetDelay);

        // Ripristina la scala al valore originale in base all'arma attuale
        Debug.Log("Sto resettando il puntatore dopo inattivit�");
        ChangeCrosshair(currentWeapon);
    }

    // Coroutine per gestire l'effetto di allargamento e ritorno alla dimensione originale
    private IEnumerator ShootEffect()
    {
        // Aumenta la scala del puntino
        transform.localScale += shootScaleIncrease;

        // Attendi per un breve intervallo
        yield return new WaitForSeconds(shootEffectDuration);

        // Ripristina la scala al valore originale in base all'arma attuale
        ChangeCrosshair(currentWeapon);
    }

    public IEnumerator HitFeedback()
    {
        // Cambia il colore del puntino per segnalare che il bersaglio � stato colpito
        middleDotImage.color = hitColor;
        Debug.Log("Zombie colpito");

        // Attendi per la durata del feedback visivo
        yield return new WaitForSeconds(hitZombieEffectDuration);

        // Ripristina la scala al valore originale in base all'arma attuale
        ChangeCrosshair(currentWeapon);
        // Ripristina il colore originale del puntino
        middleDotImage.color = originalColor;
    }
}
