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
    Vector3 m_initPosition;
    float m_commentElapsedTime;
    float m_alterElapsedTime;
    public void Awake()
    {
        StatSystem = CharacterMng.Instance.GetNPCStat(Handle);
        m_initPosition = transform.position;
        Animator = GetComponent<Animator>();

        MoveSystem = gameObject.AddComponent<MoveSystem>();
        MoveSystem.Init();
        MoveSystem.MoveSpeed = StatSystem.MoveSpeed;
        AttachSystem = gameObject.AddComponent<AttachSystem>();
        AttachSystem.Init();
        StatSystem.Init();

        m_actionDic.Add(CharacterState.Idle, Idle);
        m_actionDic.Add(CharacterState.Move, Move);
        m_actionDic.Add(CharacterState.Chase, Chase);
        m_actionDic.Add(CharacterState.Death, Death);
        State = CharacterState.Idle;

        gameObject.layer = LayerMask.NameToLayer("NPC");
        UIMng.Instance.GetUI<FieldUI>(UIMng.UIName.FieldUI).SetNameText(this, StatSystem.Name);
        UIMng.Instance.GetUI<Game>(UIMng.UIName.Game).SubWindow.MapWindow.SetDynamicIcon(SubWindow_Map_DynamicIcon.EMapIconOption.NPC, this);
    }
    protected override void Idle()
    {
        m_alterElapsedTime += Time.deltaTime;
        if (m_alterElapsedTime > 1)
        {
            Animator.SetInteger("Alter", Random.Range(0, 101));
            m_alterElapsedTime = 0;
        }

        Animator.SetInteger("State", 0);
    }
    protected override void Move()
    {
        Animator.SetInteger("State", 1);

        if (MoveSystem.MoveToPosition(m_initPosition, 0))
            State = CharacterState.Idle;
    }
    protected override void Chase()
    {

    }
    protected override void Death()
    {
        Animator.Play("Death");
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
