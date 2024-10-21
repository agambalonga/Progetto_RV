using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestroy : MonoBehaviour
{
    public float timeToDestroy;


    void Start()
    {
       StartCoroutine(DestroySelf(timeToDestroy));
    }

    private IEnumerator DestroySelf(float timeToDestroy)
    {
        yield return new WaitForSeconds(timeToDestroy);

        Destroy(gameObject);
    }
}
