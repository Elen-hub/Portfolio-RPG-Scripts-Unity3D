using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchPortal : MonoBehaviour
{
    public int Number;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Player")
            return;

        UIMng.Instance.Open<SelectStage>(UIMng.UIName.SelectStage).Open(Number);
    }
}
