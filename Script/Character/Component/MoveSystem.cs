using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class MoveSystem : MonoBehaviour
{
    BaseCharacter m_character;
    Rigidbody m_rigidbody;
    NavMeshAgent m_navMesh;
    public Transform Target;
    public Vector3 TargetPos;
    public float ChaseDistance;
    Vector3 m_axis = Vector3.zero;
    float m_moveSpeed;

    UnityAction m_chaseAfterAction;
    public bool EnabledNavMeshAgent
    {
        set { m_navMesh.enabled = value; }
    }
    public bool Stop
    {
        set { m_navMesh.isStopped = value; m_navMesh.velocity = Vector3.zero; }
    }
    public Vector3 GetCurrAxis { get { return m_axis; } }
    public Vector3 SetPosition
    {
        set {
            m_navMesh.enabled = false;
            transform.position = value;
            m_navMesh.enabled = true;
        }
    }
    public float MoveSpeed {
       get { return m_moveSpeed; }
       set { m_moveSpeed = value; }
    }
    public float SetAxis {
        set { m_axis.y = value; }
    }
    public float GetAxis {
        get { return m_axis.y; }
    }
    public void SetMoveToTarget(Transform target, float distance, UnityAction action = null)
    {
        Target = target;
        ChaseDistance = distance;
        m_chaseAfterAction = action;
        m_character.State = BaseCharacter.CharacterState.Chase;
    }
    public void SetMoveToPosition(Vector3 target, float distance)
    {
        TargetPos = target;
        ChaseDistance = distance;
        m_character.State = BaseCharacter.CharacterState.Chase;
    }
    public void Init()
    {
        m_character = GetComponent<BaseCharacter>();
        m_rigidbody = GetComponent<Rigidbody>();
        m_navMesh = GetComponent<NavMeshAgent>();
    }
    public void RotateAxis()
    {
        transform.eulerAngles = m_axis;
    }
    public void MoveAxis()
    {
        m_navMesh.isStopped = true;
        transform.position += Time.deltaTime * transform.forward * m_moveSpeed;
    }
    public void NextFrameChase()
    {
         m_navMesh.isStopped = false;
         m_navMesh.speed = m_moveSpeed;
         m_navMesh.SetDestination(Target.position);

         if (m_navMesh.remainingDistance <= ChaseDistance)
         {
             if (!m_navMesh.hasPath || Mathf.Abs(m_navMesh.velocity.sqrMagnitude) < float.Epsilon)
                 return;

             m_chaseAfterAction?.Invoke();
             m_chaseAfterAction = null;
             m_navMesh.isStopped = true;
             m_navMesh.velocity = Vector3.zero;
             m_character.State = BaseCharacter.CharacterState.Idle;
         }
    }
    public bool MoveToPosition(Vector3 pos, float distance)
    {
        m_navMesh.isStopped = false;
        m_navMesh.speed = m_moveSpeed;
        m_navMesh.SetDestination(pos);

        if (m_navMesh.remainingDistance <= distance)
        {
            if (!m_navMesh.hasPath || Mathf.Abs(m_navMesh.velocity.sqrMagnitude) < float.Epsilon)
                return true;

            m_navMesh.isStopped = true;
            m_navMesh.velocity = Vector3.zero;
            m_character.State = BaseCharacter.CharacterState.Idle;
            return true;
        }
        return false;
    }
    public void MoveToPosition()
    {
        m_navMesh.isStopped = false;
        m_navMesh.speed = m_moveSpeed;
        m_navMesh.SetDestination(TargetPos);

        if (m_navMesh.remainingDistance <= ChaseDistance)
        {
            if (!m_navMesh.hasPath || Mathf.Abs(m_navMesh.velocity.sqrMagnitude) < float.Epsilon)
                return;

            m_navMesh.isStopped = true;
            m_navMesh.velocity = Vector3.zero;
            m_character.State = BaseCharacter.CharacterState.Idle;
            return;
        }
    }
}
