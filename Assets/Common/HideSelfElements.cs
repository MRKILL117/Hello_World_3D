using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using UnityEngine.Rendering;

public class HideSelfElements : NetworkBehaviour
{
    void Start()
    {
        // elements to hide from the player's view

        if (!this.HasStateAuthority) {
            Debug.Log("HideSelfElements: No state authority");
            return;
        }
        GameObject eyes = this.transform.Find("Render").gameObject.transform.Find("Eyes").gameObject;
        // this line hides the rendering but it stills cast shadows, and unfortunately, the shadows are still in the player view
        // eyes.GetComponent<MeshRenderer>().shadowCastingMode = ShadowCastingMode.ShadowsOnly;
        eyes.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
    }
}
