using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Logo : BaseUI
{
    public override void Open()
    {
        base.Open();
        StartCoroutine(IEPlaySound());
    }
    IEnumerator IEPlaySound()
    {
        yield return null;
        yield return new WaitForSeconds(0.5f);
        PlaySound();
    }
}
