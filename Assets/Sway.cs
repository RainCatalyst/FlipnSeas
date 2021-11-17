using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sway : MonoBehaviour
{
    
    public float amount = 1;


    private Vector3 pos;
    private Vector3 rot;

    private float t;

    private float offset;
    
    // Start is called before the first frame update
    void Start()
    {
        pos = transform.localPosition;
        rot = transform.eulerAngles;

        offset = transform.position.x+ transform.position.z;
    }

    // Update is called once per frame
    void Update()
    {
        t += Time.deltaTime;

        transform.localPosition = pos + Vector3.down * 0.05f *amount * Mathf.Sin(t - offset);
    }
}
