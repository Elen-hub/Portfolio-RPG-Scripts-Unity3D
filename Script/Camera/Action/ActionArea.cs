using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionArea : MonoBehaviour
{
    bool m_isAction;
    HashSet<int> m_hashSet = new HashSet<int>();
    Hashtable m_hashTable = new Hashtable();
    Dictionary<int, int> m_dic = new Dictionary<int, int>();
    List<int> m_list = new List<int>();
    ArrayList m_array = new ArrayList();

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
