using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class pointLightTest : MonoBehaviour
{
    public Light2D light2D;
    public float intensity;
    // Start is called before the first frame update
    void Start()
    {
        //light2D = GetComponent<Light2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if(light2D.intensity != intensity)
            light2D.intensity = intensity;
    }
}
