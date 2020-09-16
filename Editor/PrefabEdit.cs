using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Text;
using UnityEngine.AI;

public class PrefabEdit : EditorWindow
{
    bool m_attachToggle;
    bool m_hpBarToggle;
    bool m_nameTextToggle;
    bool m_chatboxToggle;
    GameObject m_instantiateObj;
    GameObject m_prefab;
    public GameObject Prefab { get { return m_prefab; } set { m_prefab = value; } }
    string m_directory;

    public Stat InputPrefab(ref int posY, Stat stat, float windowSize, GUIStyle guiStyle, string directory, string path)
    { 
        EditorGUI.LabelField(new Rect(0, posY, 100, 20), "Prefab: "); 
        m_prefab = (GameObject)EditorGUI.ObjectField(new Rect(100, posY, windowSize - 100, 20), m_prefab, typeof(GameObject), false);
        posY += 20;
        m_directory = directory;
        if (m_instantiateObj)
        { 
            GUI.Label(new Rect(0, posY, 100, 20), "Original Path :");
            GUI.Label(new Rect(100, posY, windowSize - 100, 20), "<color=red>" + PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(m_prefab) + "</color>", guiStyle);

            posY += 20;
            GUI.Label(new Rect(0, posY, 100, 20), "Save Path :");
            GUI.Label(new Rect(100, posY, windowSize - 100, 20), "<color=blue>" + directory + "/" + path + ".prefab" + "</color>", guiStyle);
            posY += 20;
        }
        if(!m_instantiateObj && m_prefab)
        {
            m_instantiateObj = Editor.Instantiate(m_prefab);
            StringBuilder builder = new StringBuilder(m_instantiateObj.name);
            m_instantiateObj.name = builder.ToString(0, m_instantiateObj.name.Length - 7);
            stat.Path = m_instantiateObj.name;
            ((SceneView)GetWindow(typeof(SceneView))).LookAt(m_instantiateObj.transform.position);
            NavMeshAgent agent = m_instantiateObj.GetComponent<NavMeshAgent>();
            if(agent == null)
            {
                agent = m_instantiateObj.AddComponent<NavMeshAgent>();
                agent.radius = 0.1f;
                agent.height = 1;
                agent.speed = 3.5f;
                agent.angularSpeed = 720;
                agent.acceleration = 8;
            }
            BoxCollider collider = m_instantiateObj.GetComponent<BoxCollider>();
            if (collider == null)
            {
                collider = m_instantiateObj.AddComponent<BoxCollider>();
                collider.isTrigger = true;
                collider.center = new Vector3(0, 0.75f, 0);
                collider.size = new Vector3(0.5f, 1.5f, 0.5f);
            }
        }
        if (!m_prefab && m_instantiateObj)
            Reset();

        if (m_prefab && m_instantiateObj)
            AttachPoint(ref posY, windowSize, guiStyle);

        return stat;
    }
    public NPCStat InputPrefab(ref int posY, NPCStat stat, float windowSize, GUIStyle guiStyle, string directory, string path)
    {
        EditorGUI.LabelField(new Rect(0, posY, 100, 20), "Prefab: ");
        m_prefab = (GameObject)EditorGUI.ObjectField(new Rect(100, posY, windowSize - 100, 20), m_prefab, typeof(GameObject), false);
        posY += 20;
        m_directory = directory;
        if (m_instantiateObj)
        {
            GUI.Label(new Rect(0, posY, 100, 20), "Original Path :");
            GUI.Label(new Rect(100, posY, windowSize - 100, 20), "<color=red>" + PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(m_prefab) + "</color>", guiStyle);

            posY += 20;
            GUI.Label(new Rect(0, posY, 100, 20), "Save Path :");
            GUI.Label(new Rect(100, posY, windowSize - 100, 20), "<color=blue>" + directory + "/" +  path + m_instantiateObj.name + ".prefab" + "</color>", guiStyle);
            posY += 20;
        }
        if (!m_instantiateObj && m_prefab)
        {
            m_instantiateObj = Editor.Instantiate(m_prefab);
            m_instantiateObj.GetComponent<BaseNPC>().Handle = stat.Handle;
            StringBuilder builder = new StringBuilder(m_instantiateObj.name);
            m_instantiateObj.name = builder.ToString(0, m_instantiateObj.name.Length - 7);
            stat.Path = m_instantiateObj.name;
            ((SceneView)GetWindow(typeof(SceneView))).LookAt(m_instantiateObj.transform.position);
        }
        if (!m_prefab && m_instantiateObj)
            Reset();

        if (m_prefab && m_instantiateObj)
            AttachPoint(ref posY, windowSize, guiStyle);

        return stat;
    }
    public void AttachPoint(ref int posY, float windowSize, GUIStyle guiStyle)
    {
        m_attachToggle = GUI.Toggle(new Rect(0, posY, windowSize, 20), m_attachToggle, "Use AttachPoint");
        posY += 20;

        if (!m_attachToggle)
            return;
        
        // Gizmos.DrawSphere(m_attachDic[EAttachPoint.UnderHead].position, 0.5f);


        Transform underHead = m_instantiateObj.transform.Find("Attach_" + EAttachPoint.UnderHead);
        if (!underHead)
        {
            underHead =  new GameObject("Attach_" + EAttachPoint.UnderHead).transform;
            underHead.SetParent(m_instantiateObj.transform);
            underHead.localPosition = Vector3.zero;
        }

        GUI.Label(new Rect(0, posY, windowSize, 20), "UnderHead          YPosition: " + underHead.localPosition.y);
        posY += 20;

        Transform hp = m_instantiateObj.transform.Find("Attach_" + EAttachPoint.HP);
        m_hpBarToggle = hp;
        m_hpBarToggle = GUI.Toggle(new Rect(0, posY, 100, 20), m_hpBarToggle, "HPBar");

        if (m_hpBarToggle)
        {
            if (!hp)
            {
                hp = new GameObject("Attach_" + EAttachPoint.HP).transform;
                hp.SetParent(m_instantiateObj.transform);
            }
            hp.localPosition = new Vector3(0, underHead.localPosition.y + 0.3f);
        }
        else
        {
            if (hp) DestroyImmediate(hp.gameObject);
        }
        posY += 20;

        Transform name = m_instantiateObj.transform.Find("Attach_" + EAttachPoint.Name);
        m_nameTextToggle = name;
        m_nameTextToggle = GUI.Toggle(new Rect(0, posY, 100, 20), m_nameTextToggle, "NameText");
        posY += 20;

        if (m_nameTextToggle)
        {
            if (!name)
            {
                name = new GameObject("Attach_" + EAttachPoint.Name).transform;
                name.SetParent(m_instantiateObj.transform);
            }
            name.localPosition = new Vector3(0, underHead.localPosition.y + 0.5f);
        }
        else
        {
            if (name) DestroyImmediate(name.gameObject);
        }
        posY += 20;

        Transform chatBox = m_instantiateObj.transform.Find("Attach_" + EAttachPoint.ChatBox);
        m_chatboxToggle = chatBox;
        m_chatboxToggle = GUI.Toggle(new Rect(0, posY, 100, 20), m_chatboxToggle, "ChatBox");

        if (m_chatboxToggle)
        {
            if (!chatBox)
            {
                chatBox = new GameObject("Attach_" + EAttachPoint.ChatBox).transform;
                chatBox.SetParent(m_instantiateObj.transform);
            }
            chatBox.localPosition = new Vector3(0, underHead.localPosition.y + 0.7f);
        }
        else
        {
            if (chatBox) DestroyImmediate(chatBox.gameObject);
        }
        posY += 20;


        Transform foot = m_instantiateObj.transform.Find("Attach_" + EAttachPoint.Foot);
        if (!foot)
        {
            foot = new GameObject("Attach_" + EAttachPoint.Foot).transform;
            foot.SetParent(m_instantiateObj.transform);
        }
        foot.localPosition = new Vector3(0, 0);

        Transform chest = m_instantiateObj.transform.Find("Attach_" + EAttachPoint.Chest);
        if (!chest)
        {
            chest = new GameObject("Attach_" + EAttachPoint.Chest).transform;
            chest.SetParent(m_instantiateObj.transform);
        }
        chest.localPosition = new Vector3(0, (underHead.localPosition.y + foot.localPosition.y)*0.5f);
    }
    public void Save(string path)
    {
        if (m_instantiateObj != null)
            PrefabUtility.SaveAsPrefabAssetAndConnect(m_instantiateObj, path + m_instantiateObj.name + ".prefab", InteractionMode.UserAction);
    }
    public void Reset()
    {
        m_attachToggle = false;
        m_hpBarToggle = false;
        m_chatboxToggle = false;
        m_nameTextToggle = false;

        m_prefab = null;
        if (m_instantiateObj)
            DestroyImmediate(m_instantiateObj);
    }
}
