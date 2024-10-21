using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Riferimento al componente CharacterController
    private CharacterController controller;

    // Velocità di movimento del giocatore
    public float baseSpeed = 12f;

    private float speed;

    // Gravità applicata al giocatore
    public float gravity = -9.81f * 2;

    // Altezza del salto del giocatore
    public float jumpHeight = 3f;

    // Riferimento al Transform utilizzato per controllare se il giocatore è a terra
    public Transform groundCheck;

    // Distanza dal centro del giocatore per considerarlo a terra
    public float groundDistance = 0.4f;

    // LayerMask per determinare quali oggetti sono considerati "terra"
    public LayerMask groundMask;

    // Vettore per memorizzare la velocità del giocatore
    Vector3 velocity;

    // Booleano per verificare se il giocatore è a terra
    bool isGrounded;

    // Booleano per verificare se il giocatore si sta muovendo
    bool isMoving;

    // Ultima posizione del giocatore
    private Vector3 lastPosition = new Vector3(0f, 0f, 0f);

    // Booleano per verificare se il giocatore sta in modalità ADS
    [HideInInspector]
    public bool isAds = false;

    // Start is called before the first frame update
    void Start()
    {
        // Ottenere il componente CharacterController
        controller = GetComponent<CharacterController>();
        speed = baseSpeed; 
    }

    // Update is called once per frame
    void Update()
    {
        // Controllo se il giocatore è a terra
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            // Se il giocatore è a terra e la velocità verticale è negativa (sta cadendo), resetta la velocità verticale
            velocity.y = -2f;
        }

        // Ottenere l'input del giocatore
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        // Calcolare il movimento del giocatore
        Vector3 move = transform.right * x + transform.forward * z;

        if(Input.GetKey(KeyCode.LeftShift) && !isAds)
        {
            // Se il giocatore tiene premuto il tasto Shift, raddoppia la velocità
            speed = baseSpeed + 3f;
        } else {
            // Altrimenti, la velocità è quella di base
            speed = baseSpeed;
        }

        if(isAds)
        {
            // Se il giocatore è in modalità ADS, dimezza la velocità
            speed = baseSpeed / 2;
        }

        // Muovere il giocatore
        controller.Move(move * speed * Time.deltaTime);

        // Salto
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            // Calcolare la velocità verticale necessaria per raggiungere l'altezza del salto
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // Applicare la gravità
        velocity.y += gravity * Time.deltaTime;

        // Muovere il giocatore in base alla velocità verticale
        controller.Move(velocity * Time.deltaTime);

        // Controllare se il giocatore si sta muovendo
        if (lastPosition != transform.position && isGrounded)
        {
            isMoving = true;
        }
        else
        {
            isMoving = false;
        }

        // Aggiornare l'ultima posizione del giocatore
        lastPosition = transform.position;
    }
}