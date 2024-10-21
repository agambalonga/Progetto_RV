using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionManager : MonoBehaviour
{
    // Istanza singleton del InteractionManager
    public static InteractionManager _Instance { get; set; }

    // Riferimenti agli oggetti attualmente evidenziati
    public Weapon hoveredWeapon = null;
    public AmmoBox hoveredAmmoBox = null;
    public Throwable hoveredThrowable = null;

    // Metodo chiamato all'inizio
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

    // Metodo chiamato ad ogni frame
    private void Update() 
    {
        // Raycast dal centro della camera
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit)) 
        {
            GameObject hitObject = hit.transform.gameObject;

            // Se l'oggetto colpito è un'arma
            if(hitObject.GetComponent<Weapon>() != null && hitObject.GetComponent<Weapon>().isActiveWeapon == false)
            {
                hoveredWeapon = hitObject.gameObject.GetComponent<Weapon>();
                hoveredWeapon.GetComponent<Outline>().enabled = true;
                
                // Se il tasto E viene premuto
                if(Input.GetKeyDown(KeyCode.E))
                {
                    hoveredWeapon.GetComponent<Outline>().enabled = false;
                    WeaponManager._Instance.PickUpWeapon(hitObject.gameObject);
                }
            } 
            else
            {
                // Disabilita l'evidenziazione se non è più un'arma
                if(hoveredWeapon != null)
                {
                    hoveredWeapon.GetComponent<Outline>().enabled = false;
                    hoveredWeapon = null;
                }
            }

            // Se l'oggetto è una cassetta di munizioni
            if(hitObject.GetComponent<AmmoBox>() != null)
            {
                hoveredAmmoBox = hitObject.gameObject.GetComponent<AmmoBox>();
                hoveredAmmoBox.GetComponent<Outline>().enabled = true;
                
                // Se il tasto E viene premuto
                if(Input.GetKeyDown(KeyCode.E))
                {
                    //hoveredAmmoBox.GetComponent<Outline>().enabled = false;
                    WeaponManager._Instance.PickAmmoTotal();
                    //Destroy(hoveredAmmoBox.gameObject);
                }
            } 
            else
            {
                // Disabilita l'evidenziazione se non è più una cassetta di munizioni
                if(hoveredAmmoBox != null)
                {
                    hoveredAmmoBox.GetComponent<Outline>().enabled = false;
                    hoveredAmmoBox = null;
                }
            }

            // Se l'oggetto è una granata
            if(hitObject.GetComponent<Throwable>() != null)
            {
                hoveredThrowable = hitObject.gameObject.GetComponent<Throwable>();
                hoveredThrowable.GetComponent<Outline>().enabled = true;
                
                // Se il tasto E viene premuto
                if(Input.GetKeyDown(KeyCode.E))
                {
                    hoveredThrowable.GetComponent<Outline>().enabled = false;
                    WeaponManager._Instance.PickUpThrowable(hoveredThrowable);
                }
            } 
            else
            {
                // Disabilita l'evidenziazione se non è più una granata
                if(hoveredThrowable != null)
                {
                    hoveredThrowable.GetComponent<Outline>().enabled = false;
                    hoveredThrowable = null;
                }
            }
        }
    }
}