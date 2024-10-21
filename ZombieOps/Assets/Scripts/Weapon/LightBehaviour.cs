using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightBehaviour : MonoBehaviour
{

    private Light flashlight;
    public float maxIntensity = 100f;
    public float minIntensity = 0.5f;
    public float maxDistance = 5f; // Distanza massima per la riduzione dell'intensità

    void Start()
    {
        flashlight = GetComponent<Light>();
    }

    void Update()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, transform.forward, out hit, maxDistance))
        {
            // Calcola un valore di intensità che si riduce man mano che ti avvicini
            float distance = hit.distance;
            flashlight.intensity = Mathf.Lerp(minIntensity, maxIntensity, distance / maxDistance);
        }
        else
        {
            // Se non c'� nulla di vicino, usa l'intensità massima
            flashlight.intensity = maxIntensity;
        }
    }

}
