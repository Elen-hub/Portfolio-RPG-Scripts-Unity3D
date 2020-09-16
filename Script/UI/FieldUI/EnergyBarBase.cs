using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnergyBarBase : BaseFieldUI
{
    public Image Img;

    public override void Init()
    {
        Img = transform.GetComponentInChildren<Image>();
        Img.type = Image.Type.Filled;
        Img.fillMethod = Image.FillMethod.Horizontal;
    }
}
