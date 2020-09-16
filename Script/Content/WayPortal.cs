using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WayPortal : MonoBehaviour
{
    public int Number;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Player")
            return;

        Map map = MapMng.Instance.MapDic[MapMng.Instance.CurrMap.PortalList[Number].Handle];
        if(map.MinLevel > PlayerMng.Instance.MainPlayer.Level)
        {
            SystemMessage.Instance.PushMessage(SystemMessage.MessageType.Sub, "레벨이 부족합니다.");
            return;
        }

        NetworkMng.Instance.RequestCharacterJoinPublicPortal(Number);
    }
}
