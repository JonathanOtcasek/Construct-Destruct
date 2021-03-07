using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LimitKeeper : MonoBehaviour
{
    public static LimitKeeper limitKeeper;

    public bool limit = true;

    // Start is called before the first frame update
    void Start()
    {
        if(limitKeeper == null)
        {
            limitKeeper = this;
        } else
        {
            Destroy(this);
        }

        DontDestroyOnLoad(gameObject);
    }
}
