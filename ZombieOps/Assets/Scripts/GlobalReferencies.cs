using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalReferencies : MonoBehaviour
{
    public static GlobalReferencies _Instance;

    public GameObject bulletImpactEffectPrefab;

    public GameObject grenadeExplosionEffect;
    public GameObject smokeGrenadeEffect;

    public GameObject bloodSprayEffect;

    public int currentWave = 0;

    private void Awake()
    {
        if(_Instance == null)
        {
            _Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
}
