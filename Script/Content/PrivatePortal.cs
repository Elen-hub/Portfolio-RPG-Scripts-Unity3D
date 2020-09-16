using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrivatePortal : MonoBehaviour
{
    public int Handle;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Player")
            return;

        NetworkMng.Instance.RequestCharacterJoinPrivateMap(Handle);
    }
}
