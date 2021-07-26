using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Parllax : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += .25f * Vector3.right * Time.deltaTime;
        
        if(transform.position.x > 331.5f)
        {
            transform.position = new Vector3(-644, 4.84f, 0);
        }
    }
}
