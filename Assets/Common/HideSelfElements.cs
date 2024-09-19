using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class HideSelfElements : NetworkBehaviour
{
    void Start()
    {
        // elements to hide from the player's view

        if (!this.HasStateAuthority) {
            Debug.Log("HideSelfElements: No state authority");
            return;
        }
        GameObject eyes = this.transform.Find("Eyes").gameObject;
        if (eyes) eyes.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
    }
}
