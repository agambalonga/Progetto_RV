using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoBox : MonoBehaviour
{
   public int ammoAmount = 200; // Quantità di munizioni nella scatola
   public AmmoType ammoType; // Tipo di munizioni
   
   public enum AmmoType
   {
        RifleAmmo,
        PistolAmmo,
        BenelliAmmo,
        Ammo_box
   }
}
