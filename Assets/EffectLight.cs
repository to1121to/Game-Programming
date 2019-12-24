using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EffectLight : MonoBehaviour
{
    public float offset;
    public float probability;
    public float seconds;
    private float intensity;
    private int t;

    void Start(){
        intensity = GetComponent<Light>().intensity;
        t = 0;
    }

    void FixedUpdate()
    {
        if(t <= 0)
        {
            // turn on/off
            int n = Random.Range(0, 100);
            int p = (int)(probability * 100.0f);
            GetComponent<Light>().enabled = (n <= p);
            // light intensity
            GetComponent<Light>().intensity = intensity + Random.Range(-offset, offset);
            // next update delay
            t = (int)(seconds / Time.fixedDeltaTime);
        }
        else
        {
            t--;
        }        
    }
}
