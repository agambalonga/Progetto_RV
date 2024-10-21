using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    // Istanza singleton del HUDManager
    public static HUDManager _Instance { get; set; }

    [Header("Munizioni")]
    public TextMeshProUGUI magazineAmmoUI; // UI per le munizioni nel caricatore
    public TextMeshProUGUI totalAmmoUI; // UI per le munizioni totali
    public Image ammoTypeUI; // UI per il tipo di munizioni

    [Header("Armi")]
    public Image activeWeaponUI; // UI per l'arma attiva
    public Image unactiveWeaponUI; // UI per l'arma inattiva

    [Header("Oggetti Lanciabili")]
    public Image lethalUI; // UI per l'oggetto letale
    public TextMeshProUGUI lethalAmountUI; // UI per la quantità di oggetti letali
    public Image tacticalUI; // UI per l'oggetto tattico
    public TextMeshProUGUI tacticalAmountUI; // UI per la quantità di oggetti tattici

    public Sprite emptySlotSprite; // Sprite per lo slot vuoto
    public Sprite greySlotSprite; // Sprite per lo slot grigio

    private Sprite pistolSpriteW; // Sprite per la pistola
    private Sprite m4SpriteW; // Sprite per il fucile M4
    private Sprite bennelliSpriteW; // Sprite per il fucile Bennelli

    private Sprite pistolSpriteA; // Sprite per le munizioni della pistola
    private Sprite m4SpriteA; // Sprite per le munizioni del fucile M4
    private Sprite bennelliSpriteA; // Sprite per le munizioni del fucile Bennelli
    private Sprite grenadeSprite; // Sprite per la granata
    private Sprite smokeGrenadeSprite; // Sprite per la granata fumogena

    [Header("Mirino")]
    public GameObject middleDot; // UI per il punto centrale del mirino

    [Header("Salute del Giocatore")]
    public TextMeshProUGUI playerHealthUI; // UI per la salute del giocatore

    [Header("Effetti Schermo")]
    public GameObject bloodyScreen; // UI per l'effetto schermo insanguinato
    public float bloodyScreenFadeDuration; // Durata dell'effetto schermo insanguinato
    
    public GameObject playerDeadScreen; // UI per lo schermo di morte del giocatore
    public float playerDeadScreenFadeDuration; // Durata dell'effetto schermo di morte del giocatore
    public GameObject gameOverUI; // UI per lo schermo di game over

    [Header("Ondata")]
    public TextMeshProUGUI waveOverUI; // UI per la fine dell'ondata
    public TextMeshProUGUI preStartWaveCounterUI; // UI per il contatore pre-inizio ondata
    public TextMeshProUGUI currentWaveUI; // UI per l'ondata corrente

    public TextMeshProUGUI weaponUnlockedUI; // UI per l'arma sbloccata

    private bool isBloodyScreenActive = false; // Stato dell'effetto schermo insanguinato

    private void Start()
    {
        // Carica le risorse una sola volta
        pistolSpriteW = Resources.Load<GameObject>("Pistol1911_Weapon").GetComponent<SpriteRenderer>().sprite;
        m4SpriteW = Resources.Load<GameObject>("M4_Weapon").GetComponent<SpriteRenderer>().sprite;
        bennelliSpriteW = Resources.Load<GameObject>("Bennelli_Weapon").GetComponent<SpriteRenderer>().sprite;

        pistolSpriteA = Resources.Load<GameObject>("Pistol1911_Ammo").GetComponent<SpriteRenderer>().sprite;
        m4SpriteA = Resources.Load<GameObject>("M4_Ammo").GetComponent<SpriteRenderer>().sprite;
        bennelliSpriteA = Resources.Load<GameObject>("Bennelli_Ammo").GetComponent<SpriteRenderer>().sprite;

        grenadeSprite = Resources.Load<GameObject>("Grenade").GetComponent<SpriteRenderer>().sprite;
        smokeGrenadeSprite = Resources.Load<GameObject>("Smoke_Grenade").GetComponent<SpriteRenderer>().sprite;
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
        // Ottieni l'arma attiva e inattiva
        Weapon activeWeapon = WeaponManager._Instance.activeWeaponSlot.GetComponentInChildren<Weapon>();
        Weapon unactiveWeapon = GetUnactiveWeaponSlot().GetComponentInChildren<Weapon>();

        if(activeWeapon)
        {
            // Aggiorna l'UI delle munizioni
            magazineAmmoUI.text = $"{activeWeapon.bulletsLeft / activeWeapon.bulletsPerBurst}";
            totalAmmoUI.text = $"{WeaponManager._Instance.CheckAmmoLeft(activeWeapon.currentWeaponModel)}";

            Weapon.WeaponModel weaponModel = activeWeapon.currentWeaponModel;
            ammoTypeUI.sprite = GetAmmoSprite(weaponModel);

            activeWeaponUI.sprite = GetWeaponSprite(weaponModel);

            if(unactiveWeapon)
            {
                unactiveWeaponUI.sprite = GetWeaponSprite(unactiveWeapon.currentWeaponModel);
            }
        } 
        else 
        {
            // Se non c'è un'arma attiva, svuota l'UI delle munizioni
            magazineAmmoUI.text = "";
            totalAmmoUI.text = "";

            ammoTypeUI.sprite = emptySlotSprite;
            activeWeaponUI.sprite = emptySlotSprite;
            unactiveWeaponUI.sprite = emptySlotSprite;
        }

        // Aggiorna l'UI degli oggetti lanciabili
        if(WeaponManager._Instance.lethalsCount <= 0)
        {
            lethalUI.sprite = greySlotSprite;
        }

        if(WeaponManager._Instance.tacticalsCount <= 0)
        {
            tacticalUI.sprite = greySlotSprite;
        }
    }

    // Ottiene lo sprite dell'arma in base al modello dell'arma
    private Sprite GetWeaponSprite(Weapon.WeaponModel weaponModel)
    {
        switch (weaponModel)
        {
            case Weapon.WeaponModel.Pistol:
                return pistolSpriteW;
            case Weapon.WeaponModel.M4:
                return m4SpriteW;
            case Weapon.WeaponModel.Benelli:
                return bennelliSpriteW;
            default:
                return null;
        }
    }

    // Ottiene lo sprite delle munizioni in base al modello dell'arma
    private Sprite GetAmmoSprite(Weapon.WeaponModel weaponModel)
    {
        switch (weaponModel)
        {
            case Weapon.WeaponModel.Pistol:
                return pistolSpriteA;
            case Weapon.WeaponModel.M4:
                return m4SpriteA;
            case Weapon.WeaponModel.Benelli:
                return bennelliSpriteA;
            default:
                return null;
        }
    }

    // Ottiene lo slot dell'arma inattiva
    private GameObject GetUnactiveWeaponSlot()
    {
        foreach(GameObject weaponSlot in WeaponManager._Instance.weaponSlots)
        {
            if(weaponSlot != WeaponManager._Instance.activeWeaponSlot)
            {
                return weaponSlot;
            }
        }

        return null;
    }

    // Aggiorna l'UI degli oggetti lanciabili
    internal void UpdateThrowablesUI()
    {
        lethalAmountUI.text = $"{WeaponManager._Instance.lethalsCount}";
        tacticalAmountUI.text = $"{WeaponManager._Instance.tacticalsCount}";

        switch(WeaponManager._Instance.equippedLethalType)
        {
            case Throwable.ThrowableType.Grenade:
                lethalUI.sprite = grenadeSprite;
                break;
        }

        switch(WeaponManager._Instance.equippedTacticalType)
        {
            case Throwable.ThrowableType.Smoke_Grenade:
                tacticalUI.sprite = smokeGrenadeSprite;
                break;
        }
    }

    // Aggiorna l'UI della salute del giocatore
    public void UpdatePlayerHealthUI(int health)
    {
        Debug.Log($"Aggiorno hud: Player health: {health}");
        playerHealthUI.text = $"{health}";
    }

    // Effetto schermo insanguinato
    public IEnumerator BloodyScreenEffect()
    {
        if (isBloodyScreenActive)
        {
            yield break; // Esci se l'effetto è già in esecuzione
        }

        isBloodyScreenActive = true;

        if(!bloodyScreen.activeInHierarchy)
        {
            bloodyScreen.SetActive(true);
        }

        var image = bloodyScreen.GetComponentInChildren<UnityEngine.UI.Image>();

        Color startColor = image.color;
        startColor.a = 1f;

        float elapsedTime = 0f;
 
        while (elapsedTime < bloodyScreenFadeDuration)
        {
            // Interpola il colore tra inizio e fine nel tempo
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / bloodyScreenFadeDuration);

            // Aggiorna il colore dell'immagine
            Color newColor = image.color;
            newColor.a = alpha;
            image.color = newColor;

            // Incrementa il timer
            elapsedTime += Time.deltaTime;
    
            yield return null;
        }

        if(bloodyScreen.activeInHierarchy)
        {
            bloodyScreen.SetActive(false);
        }

        isBloodyScreenActive = false;
    }

    // Mostra lo schermo di morte del giocatore
    public IEnumerator ShowPlayerDeadScreen()
    {
        playerDeadScreen.SetActive(true);

        float timer = 0f;
        Color startColor = playerDeadScreen.GetComponent<UnityEngine.UI.Image>().color;
        Color endColor = new Color(0f, 0f, 0f, 1f); // Nero con alpha 1.
 
        while (timer < playerDeadScreenFadeDuration)
        {
            // Interpola il colore tra inizio e fine nel tempo
            playerDeadScreen.GetComponent<UnityEngine.UI.Image>().color = Color.Lerp(startColor, endColor, timer / playerDeadScreenFadeDuration);
            timer += Time.deltaTime;
            yield return null;
        }
 
        // Assicurati che l'immagine sia completamente nera alla fine
        playerDeadScreen.GetComponent<UnityEngine.UI.Image>().color = endColor;
    }

    // Mostra l'UI di game over
    public IEnumerator ShowGameOverUI()
    {
        // Mostra e sblocca il cursore del mouse
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        WeaponManager._Instance.DisableWeapon();

        yield return new WaitForSeconds(5f);
        gameOverUI.SetActive(true);

        int waveSurvived = GlobalReferencies._Instance.currentWave;

        SaveLoadManager._Instance.SaveHighScore(waveSurvived-1);

        StartCoroutine(ReturnToMainMenu());
    }

    // Ritorna al menu principale
    private IEnumerator ReturnToMainMenu()
    {
        yield return new WaitForSeconds(5f);

        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
}