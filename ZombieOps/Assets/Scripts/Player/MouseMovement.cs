using UnityEngine;

public class MouseMovement : MonoBehaviour
{
    // Sensibilità del mouse
    public float mouseSense;

    // Valori di rotazione
    float xRotation = 0f;
    float yRotation = 0f;

    // Limiti di rotazione
    public float topClamp = -90f;
    public float bottomClamp = 90f;

    // Start is called before the first frame update
    void Start()
    {
        // Nascondere il cursore e bloccarlo al centro dello schermo
        Cursor.lockState = CursorLockMode.Locked;
        mouseSense = PlayerPrefs.GetFloat("MouseSensitivity", 500f);
    }

    // Update is called once per frame
    void Update()
    {
        // Calcolare il movimento del mouse sugli assi x e y
        float mouseX = Input.GetAxis("Mouse X") * mouseSense * 0.1f;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSense * 0.1f;

        // Ruotare il giocatore sull'asse x 
        xRotation -= mouseY;

        // Limitare la rotazione in modo che il giocatore non possa guardare dietro di sé
        xRotation = Mathf.Clamp(xRotation, topClamp, bottomClamp);

        // Ruotare il giocatore sull'asse y
        yRotation += mouseX;

        // Applicare la rotazione al giocatore
        transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);
    }
}