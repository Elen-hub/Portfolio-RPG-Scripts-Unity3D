using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_Object : MonoBehaviour
{
    public GameObject GameObject;
    public Item_Base Item;
    Dictionary<EItemType, GameObject> m_itemModel = new Dictionary<EItemType, GameObject>();
    ParticleSystem m_glowParticle;
    ParticleSystem m_powerGlowPaticle;
    SphereCollider m_collider;

    ParticleSystem.MinMaxGradient m_particleColor = new ParticleSystem.MinMaxGradient();
    ParticleSystem.MainModule m_module;
    ParticleSystem.MainModule m_module2;
    ParticleSystem.MainModule m_module3;

    Vector3 m_targetPosition;
    float m_removeElasedTime;
    bool m_isRoot;
    int m_direction =1;
    public void Init()
    {
        m_collider = GetComponent<SphereCollider>();
        if (m_collider == null)
            m_collider = gameObject.AddComponent<SphereCollider>();

        m_collider.isTrigger = true;
        m_collider.enabled = false;
        m_collider.radius = GameSystem.ItemColliderRange;
        GameObject = gameObject;

        m_itemModel.Add(EItemType.Other, transform.Find(EItemType.Other.ToString()).gameObject);
        m_itemModel.Add(EItemType.Weapon, transform.Find(EItemType.Other.ToString()).gameObject);
        m_itemModel.Add(EItemType.Armor, transform.Find(EItemType.Other.ToString()).gameObject);
        m_itemModel.Add(EItemType.Gloves, transform.Find(EItemType.Other.ToString()).gameObject);
        m_itemModel.Add(EItemType.Shoes, transform.Find(EItemType.Other.ToString()).gameObject);
        m_itemModel.Add(EItemType.Ring, transform.Find(EItemType.Other.ToString()).gameObject);
        m_itemModel.Add(EItemType.Scroll, transform.Find(EItemType.Other.ToString()).gameObject);
        m_itemModel.Add(EItemType.Necklace, transform.Find(EItemType.Other.ToString()).gameObject);
        m_itemModel.Add(EItemType.Potion, transform.Find(EItemType.Other.ToString()).gameObject);

        m_glowParticle = transform.Find("GlowEffect").GetComponent< ParticleSystem>();
        m_powerGlowPaticle = transform.Find("PowerGlowEffect").GetComponent<ParticleSystem>();
        m_module = m_glowParticle.main;
        m_module2 = m_powerGlowPaticle.main;
        m_module3 = m_powerGlowPaticle.transform.GetChild(0).GetComponent<ParticleSystem>().main;
    }
    public void Enabled(Item_Base item, Vector3 position)
    {
        m_removeElasedTime = 0;
        m_isRoot = false;
        m_collider.enabled = true;
        m_targetPosition = position;
        transform.position = position;
        transform.localScale = Vector3.one;
        Item = item;

            switch (Item.Rarity)
            {
                case EItemRarity.Normal:
                m_particleColor.color = GameSystem.ColorNormal;
                break;
                case EItemRarity.Magic:
                    m_particleColor.color = GameSystem.ColorMagic;
                    break;
                case EItemRarity.Unique:
                    m_particleColor.color = GameSystem.ColorUnique;
                    break;
                case EItemRarity.Relic:
                    m_particleColor.color = GameSystem.ColorRelic;
                    break;
                case EItemRarity.Legend:
                    m_particleColor.color = GameSystem.ColorLegend;
                    break;
                case EItemRarity.Infinity:
                    m_particleColor.color = GameSystem.ColorInfinity;
                    break;
            }
        m_module.startColor = m_particleColor;
        m_module2.startColor = m_particleColor;
        m_module3.startColor = m_particleColor;
        m_glowParticle.Play();
        m_powerGlowPaticle.Play();
        m_itemModel[Item.Type].SetActive(true);
        GameObject.SetActive(true);
        StartCoroutine(DropAction(position));
    }
    public void Disabled()
    {
        m_glowParticle.Stop();
        m_powerGlowPaticle.Stop();
        m_itemModel[Item.Type].SetActive(false);
        Item = null;
        GameObject.SetActive(false);
    }
    private void Update()
    {
        m_removeElasedTime += Time.deltaTime;

        if (m_removeElasedTime > 60)
        {
            Disabled();
            return;
        }

        if (m_isRoot)
        {
            if (transform.position.y > m_targetPosition.y + 0.3f)
                m_direction = -1;
            if (transform.position.y < m_targetPosition.y + 0.1f)
                m_direction = 1;

            transform.position += Vector3.up * m_direction * Time.deltaTime * 0.1f;
            transform.Rotate(Vector3.up*0.2f);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Player")
            return;

        if (ItemMng.Instance.CheckInventoryCount(Item))
        {
            m_collider.enabled = false;
            NetworkMng.Instance.RequestGetReword(Item.UniqueID);
        }
    }
    public void Get(BaseCharacter character)
    {
        m_collider.enabled = false;
        StartCoroutine(AddAction(character.transform));
    }
    IEnumerator DropAction(Vector3 pos)
    {
        m_isRoot = false;
        int angle = Random.Range(0, 360);
        float height = Random.Range(1f, 2f);
        float distance = Random.Range(0.05f, 1f);
        transform.eulerAngles = new Vector3(0, angle, 0);
        Vector3 prevPos = transform.position;
        Vector3 targetPos = transform.forward * distance + transform.position;
        Vector3 currPos;
        WaitForSeconds wait = new WaitForSeconds(0.016f);

        for (int i =0; i<50; ++i)
        {
            currPos = Vector3.Lerp(prevPos, targetPos, i / 49f);
            currPos.y += height * Mathf.Sin(i / 49f * Mathf.PI);
            transform.position = currPos;
            transform.localScale = Vector3.one * ((i+1) * 0.02f);
            yield return wait;
        }

        m_collider.enabled = true;
        m_isRoot = true;

        yield return null;
    }
    IEnumerator AddAction(Transform target)
    {
        m_isRoot = false;
        Vector3 Pos = transform.position;
        Vector3 TargetPos;
        WaitForSeconds wait = new WaitForSeconds(0.016f);
        for (int i = 0; i < 50; ++i)
        {
            TargetPos = target.position;
            TargetPos.y += 0.5f;
            transform.position = Vector3.Lerp(Pos, TargetPos, i / 49f);
            transform.localScale = Vector3.one * (1 - i * 0.02f);
            yield return wait;
        }
        Disabled();
        yield return null;
    }
}
