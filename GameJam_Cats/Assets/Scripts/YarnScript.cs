using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YarnScript : MonoBehaviour
{
    LineRenderer line;

    // Start is called before the first frame update
    void Start()
    {
        line = GetComponent<LineRenderer>();
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag != "Cat") return;


    }
}
