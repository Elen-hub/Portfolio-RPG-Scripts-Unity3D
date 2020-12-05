using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class NPCStat
{
    public int Handle;
    public string Name;
    public string Path;
    public float MoveSpeed;

    public List<string> Comment;
    public List<string> Scripts;
    public ENpcOption Option;
    public List<int> ShopHandle;
    public List<Quest> QuestList;
    public int TrainHandle;
    public List<int> ProduceList;

    public void Init()
    {

    }
}
[System.Flags]
public enum ENpcOption
{
    Shop = 1,
    Aution = 2,
    Quest = 4,
    Train = 8,
    Produce = 16,
}
public class BaseNPC : BaseCharacter
{
    public int Handle;
    public new NPCStat StatSystem;
    BaseCharacter m_target;
    public Vector3 InitPosition;
    float m_commentElapsedTime;
    float m_alterElapsedTime;
    void Awake()
    {
        StatSystem = CharacterMng.Instance.GetNPCStat(Handle);
        InitPosition = transform.position;
        Animator = GetComponent<Animator>();

        MoveSystem = gameObject.AddComponent<MoveSystem>();
        MoveSystem.Init();
        MoveSystem.MoveSpeed = StatSystem.MoveSpeed;
        AttachSystem = gameObject.AddComponent<AttachSystem>();
        AttachSystem.Init();
        StatSystem.Init();

        m_stateDic.Add(CharacterState.Idle, new State_Idle_NPC(this));
        m_stateDic.Add(CharacterState.Move, new State_Move_NPC(this));

        gameObject.layer = LayerMask.NameToLayer("NPC");
        UIMng.Instance.GetUI<FieldUI>(UIMng.UIName.FieldUI).SetNameText(this, StatSystem.Name);
        UIMng.Instance.GetUI<Game>(UIMng.UIName.Game).SubWindow.MapWindow.SetDynamicIcon(SubWindow_Map_DynamicIcon.EMapIconOption.NPC, this);
    }

    protected override void Update()
    {
        m_commentElapsedTime += Time.deltaTime;

        // 코멘트 발생 트리거
        if (m_commentElapsedTime > 3)
        {
            if (Random.Range(0, 200) == 50)
            {
                UIMng.Instance.GetUI<FieldUI>(UIMng.UIName.FieldUI).SetChatBox(this, StatSystem.Comment[Random.Range(0, StatSystem.Comment.Count)]);
                m_commentElapsedTime = 0;
            }
        }
    }
}
