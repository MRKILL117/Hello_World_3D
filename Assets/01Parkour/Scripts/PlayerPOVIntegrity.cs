using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPOVIntegrity : MonoBehaviour
{
    public GameObject cameraPosition;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if(!cameraPosition) return;
        this.transform.position = cameraPosition.transform.position;
    }
}
