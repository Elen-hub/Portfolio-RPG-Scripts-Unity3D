using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class Enermy_Cobra : Enermy_Boss_CloseAttack
{
    bool m_isFirstAction;
    
    public override void Init(int uniqueID, EAllyType allyType, NormalAttack attack, Stat stat)
    {
        base.Init(uniqueID, allyType, attack, stat);
        m_collider.radius = 15;
        AttackSystem.SuperArmor = true;
    }
    IEnumerator AppearanceAction()
    {
        yield return null;

        PlayerCamera playerCamera = CameraMng.Instance.GetCamera<PlayerCamera>(CameraMng.CameraStyle.Player);
        BaseCamera worldCamera = CameraMng.Instance.GetCamera(CameraMng.CameraStyle.World);
        Vector3 prevPos = new Vector3(6.68f, 13.06f, 7.26f);
        Vector3 nextPos = new Vector3(6.68f, 9.51f, 1.11f);
        Vector3 prevAngle = new Vector3(30, -180, 0);
        Vector3 nextAngle = new Vector3(0, -180, 0);
        worldCamera.transform.position = prevPos;
        worldCamera.transform.eulerAngles = prevAngle;
        CameraMng.Instance.SetCamera(CameraMng.CameraStyle.World);
        float a = 0;
        while(a<1)
        {
            yield return null;
            a += Time.deltaTime;
            worldCamera.transform.position = Vector3.Lerp(prevPos, nextPos, a);
        }
        a = 0;
        prevPos = worldCamera.transform.position;
        nextPos = new Vector3(15.49f, 10.73f, -2.34f);
        while (a<1)
        {
            yield return null;
            a += Time.deltaTime;
            worldCamera.transform.position = Vector3.Lerp(prevPos, nextPos, a);
            worldCamera.transform.eulerAngles = Vector3.Lerp(prevAngle, nextAngle, a);
        }
        InitPosition = new Vector3(-1.5f, 6.5f, -11.3f);
        State = CharacterState.Move;

        prevPos = worldCamera.transform.position;
        nextPos = new Vector3(10.39f, 13.76f, -4.66f);
        prevAngle = worldCamera.transform.eulerAngles;
        nextAngle = new Vector3(34.176f, -111.843f, 0);
        a = 0;
        while (a < 1)
        {
            yield return null;
            a += Time.deltaTime*0.7f;
            worldCamera.transform.position = Vector3.Lerp(prevPos, nextPos, a);
            worldCamera.transform.eulerAngles = Vector3.Lerp(prevAngle, nextAngle, a);
        }

        yield return new WaitForSeconds(3f);
        CameraMng.Instance.SetCamera(CameraMng.CameraStyle.Player);
    }
    private void OnTriggerStay(Collider other)
    {
        if (!m_isFirstAction)
        {
            if (other.tag == "Player" || other.tag == "Ally")
            {
                if (other.transform.position.y < 6)
                    return;

                m_isFirstAction = true;
                m_collider.radius = GameSystem.BossMonsterColliderRange;
                StartCoroutine(AppearanceAction());
            }
        }
    }
    protected override void OnTriggerEnter(Collider other)
    {
        if (!m_isFirstAction)
            return;

        base.OnTriggerEnter(other);
    }
}
