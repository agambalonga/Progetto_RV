using UnityEngine;
using static Weapon;

public class SoundManager : MonoBehaviour
{
    // Istanza singleton del SoundManager
    public static SoundManager _Instance;

    [Header("Audio Sources")]
    // Le varie sorgenti audio utilizzate per riprodurre diversi suoni
    public AudioSource shootingAudioSource;  // Audio source per i suoni di sparo
    public AudioSource reloadM911;           // Audio source per il suono di ricarica della pistola M911
    public AudioSource reloadM4;             // Audio source per il suono di ricarica del fucile M4
    public AudioSource reloadBennelli;       // Audio source per il suono di ricarica del fucile Bennelli
    public AudioSource emptyMagazine;        // Audio source per il suono del caricatore vuoto
    public AudioSource throwablesChannel;    // Audio source per le armi da lancio, come le granate
    public AudioSource zombieChannel;        // Audio source per i suoni degli zombie
    public AudioSource zombieChannel2;       // Seconda sorgente per gli zombie (evita sovrapposizioni)
    public AudioSource playerChannel;        // Audio source per i suoni del giocatore (danni, morte, ecc.)

    [Header("Shooting Clips")]
    // Audio clip che contengono i suoni di sparo per le varie armi
    public AudioClip shootingM911Clip;       // Suono di sparo della pistola M911
    public AudioClip shootingM4Clip;         // Suono di sparo del fucile M4
    public AudioClip shootingBennelliClip;   // Suono di sparo del fucile Bennelli
    public AudioClip grenadeClip;            // Suono di esplosione della granata

    [Header("Zombie Clips")]
    // Audio clip per i suoni degli zombie
    public AudioClip zombieWalkClip;         // Suono degli zombie che camminano
    public AudioClip zombieRunClip;          // Suono degli zombie che corrono
    public AudioClip zombieAttackClip;       // Suono degli zombie che attaccano
    public AudioClip zombieDieClip;          // Suono degli zombie che muoiono
    public AudioClip zombieHitClip;          // Suono quando gli zombie vengono colpiti

    [Header("Player Clips")]
    public AudioClip playerHurtClip;         // Suono quando il giocatore viene ferito
    public AudioClip playerDieClip;          // Suono quando il giocatore muore

    // Array che conterrà tutti gli audio source collegati a questo oggetto
    private AudioSource[] audioSources;
    
    // Metodo chiamato alla creazione dell'oggetto, gestisce l'istanza singleton
    private void Awake()
    {
        if(_Instance == null)
        {
            _Instance = this;  // Se l'istanza non esiste, la assegna a questa
            DontDestroyOnLoad(this.gameObject);  // Previene la distruzione del SoundManager al cambio di scena
        }
        else
        {
            Destroy(this.gameObject);  // Se esiste già un'istanza, distrugge la nuova per mantenere il singleton
        }
    }

    // Metodo chiamato all'inizio, recupera tutti gli AudioSource associati a questo oggetto
    public void Start()
    {
        audioSources = GetComponents<AudioSource>();  // Ottiene tutti i componenti AudioSource di questo GameObject
    }

    // Metodo per riprodurre il suono di sparo in base al modello di arma
    public void PlayShootingSount(WeaponModel weaponModel)
    {
        switch (weaponModel)
        {
            case WeaponModel.Pistol:
                shootingAudioSource.PlayOneShot(shootingM911Clip);  // Riproduce il suono della pistola
                break;
            case WeaponModel.M4:
                shootingAudioSource.PlayOneShot(shootingM4Clip);    // Riproduce il suono del fucile M4
                break;
            case WeaponModel.Benelli:
                shootingAudioSource.PlayOneShot(shootingBennelliClip);  // Riproduce il suono del Bennelli
                break;
        }
    }

    // Metodo per riprodurre il suono di ricarica in base al modello di arma
    public void PlayReloadSound(WeaponModel weaponModel)
    {
        switch (weaponModel)
        {
            case WeaponModel.Pistol:
                reloadM911.Play();  // Riproduce il suono di ricarica della pistola M911
                break;
            case WeaponModel.M4:
                reloadM4.Play();    // Riproduce il suono di ricarica del fucile M4
                break;
            case WeaponModel.Benelli:
                reloadBennelli.Play();  // Riproduce il suono di ricarica del fucile Bennelli
                break;
        }
    }

    // Metodo per mettere in pausa tutti gli AudioSource
    public void MuteAllAudio()
    {
        foreach (var audioSource in audioSources)
        {
            audioSource.enabled = false;  // Disabilita ogni sorgente audio
        }
    }

    // Metodo per riattivare tutti gli AudioSource
    public void UnmuteAllAudio()
    {
        foreach (var audioSource in audioSources)
        {
            audioSource.enabled = true;   // Riattiva ogni sorgente audio
        }
    }
}
