using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class English : MonoBehaviour
{
    public Vector3 english;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Rigidbody2D>().velocity = english;
    }
}
