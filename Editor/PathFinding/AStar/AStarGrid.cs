using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class AStarGrid
{
    Vector3 m_worldSize;
    Vector3 m_gridSize;
    float m_minHeight;
    float m_minWalkableRange;
    float m_minUnwalkableRange;
    AStarNode[,,] m_nodes;

    public void CheckWorldSize()
    {
       
    }
    public void CreateGrid()
    {
        m_nodes = new AStarNode[Mathf.RoundToInt(m_worldSize.x / m_gridSize.x), Mathf.RoundToInt(m_worldSize.y / m_gridSize.y), Mathf.RoundToInt(m_worldSize.z / m_gridSize.z)];
        for (float i = -m_worldSize.x; i <= m_worldSize.x; ++m_gridSize.x)
        {
            for (float j = -m_worldSize.z; j <= m_worldSize.z; ++m_gridSize.z)
            {
                Vector3 RaycastPos = new Vector3(i, j, m_worldSize.y);
                Vector3 RaycastDir = Vector3.down;
                RaycastHit[] Hits = Physics.RaycastAll(RaycastPos, RaycastDir, m_worldSize.y * 2);
                for (int k = 0; k < Hits.Length; ++k)
                {
                    GameObject obj = Hits[k].transform.gameObject;
                    StaticEditorFlags StaticFlag = GameObjectUtility.GetStaticEditorFlags(obj);
                    if ((StaticFlag & StaticEditorFlags.NavigationStatic) != 0 && obj.tag == "Road")
                    {
                        
                    }
                }
            }
        }
    }
}
