using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Headbomb : MonoBehaviour
{
    public float bobbingSpeed = 0.4f;  // Velocità dell'oscillazione
    public float bobbingAmount = 0.04f; // Ampiezza dell'oscillazione
    public float midpoint = 2.0f;       // Punto centrale della telecamera
    
    private float timer = 0.0f;
    private CharacterController controller;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        float waveslice = 0.0f;
        float horizontal = Input.GetAxis("Horizontal"); // Movimento orizzontale
        float vertical = Input.GetAxis("Vertical");     // Movimento avanti-indietro
        Weapon weapon = WeaponManager._Instance.GetActiveWeapon();
        if (weapon != null)
        {
            bobbingSpeed = 0.3f;
            bobbingAmount = 0.002f;

        }
        else
        {
            bobbingSpeed = 0.4f;
            bobbingAmount = 0.04f;
        }

        Vector3 position = transform.localPosition;

        // Controlla se il giocatore sta camminando
        if (Mathf.Abs(horizontal) == 0 && Mathf.Abs(vertical) == 0)
        {
            timer = 0.0f;  // Nessun movimento, quindi resetta l'oscillazione
        }
        else
        {
            waveslice = Mathf.Sin(timer);  // Calcola la sinusoide per l'oscillazione
            timer += bobbingSpeed;

            if (timer > Mathf.PI * 2)
            {
                timer = timer - (Mathf.PI * 2);
            }
        }
        

        // Se il giocatore si muove, applica l'oscillazione
        if (waveslice != 0)
        {
            float translateChange = waveslice * bobbingAmount;
            float totalAxes = Mathf.Abs(horizontal) + Mathf.Abs(vertical);

            totalAxes = Mathf.Clamp(totalAxes, 0.0f, 1.0f);
            translateChange = totalAxes * translateChange;
            position.y = midpoint + translateChange;  // Modifica la posizione Y per l'effetto bobbing
        }
        else
        {
            position.y = midpoint;  // Ripristina la posizione iniziale quando non si cammina
        }

        transform.localPosition = position;
    }
}
