using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitPortrait : MonoBehaviour
{
    [SerializeField] Camera unitPortraitCamera;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public Texture GetCameraTexture()
    {
        return unitPortraitCamera.activeTexture;
    }
}
