using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Autodestroy : MonoBehaviour
{
    [SerializeField] private float duration;

    private void Start() {
        GameObject.Destroy(gameObject, duration);
    }
}
