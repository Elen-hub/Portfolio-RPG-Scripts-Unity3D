using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseCamera : MonoBehaviour
{
    public bool Fade_Switch;
    public bool Infrared_Switch;
    public bool RainDrop_Switch;
    // public FastMobileBloom Bloom;
    public new Camera camera;
    public bool Enabled
    {
        get{ return gameObject.activeSelf; }
        set { gameObject.SetActive(value); }
    }
    public virtual void Init()
    {
        camera = GetComponentInChildren<Camera>();

        // Bloom = GetComponentInChildren<FastMobileBloom>();
    }
}
