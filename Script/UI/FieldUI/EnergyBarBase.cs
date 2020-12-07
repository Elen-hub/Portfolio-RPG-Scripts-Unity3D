using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnergyBarBase : MonoBehaviour
{
    public Image Img;

    public virtual void Init()
    {
        Img = transform.GetComponentInChildren<Image>();
        Img.type = Image.Type.Filled;
        Img.fillMethod = Image.FillMethod.Horizontal;
    }
}
