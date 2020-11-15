using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionArea : MonoBehaviour
{
    bool m_isAction;

    private void OnTriggerEnter(Collider other)
    {
        if (m_isAction)
            return;

        if (other.tag == "Player" || other.tag == "Ally")
        {
            m_isAction = true;
        }
    }
}
