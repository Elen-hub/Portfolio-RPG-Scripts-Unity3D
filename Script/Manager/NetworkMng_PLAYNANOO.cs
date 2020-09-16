using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayNANOO;
using PlayNANOO.SimpleJSON;
using Nettention.Proud;

public partial class NetworkMng : TSingleton<NetworkMng>
{
    Plugin m_plugin;
    public List<PostInfo> PostList = new List<PostInfo>();
    public List<string> CurrSubscriptionList = new List<string>();
    public Dictionary<int, string> SubscriptionArr = new Dictionary<int, string>();
    public void RequestPostSearch()
    {
        m_plugin.AccessEvent((state, message, rawData, dictionary) =>
        {
            if (state.Equals(Configure.PN_API_STATE_SUCCESS))
            {
                if (dictionary.ContainsKey("postbox_count"))
                {
                    int postCount = (dictionary["postbox_count"] as JSONNumber).AsInt;
                }
                if (dictionary.ContainsKey("server_timestamp"))
                {
                    
                }
                if (dictionary.ContainsKey("postbox_subscription"))
                {
                    foreach (Dictionary<string, object> subscription in (ArrayList)dictionary["postbox_subscription"])
                    {
                        string product = (string)subscription["product"];
                        int ttl = int.Parse((string)subscription["ttl"]);
                        CurrSubscriptionList.Add(product);
                    }
                }
            }
            else
            {
                Debug.Log("실패");
            }
        });
    }
    public void RequestPostReiceve(string code)
    {
        m_plugin.PostboxItemUse(code, (state, message, rawData, dictionary) =>
        {
            if (state.Equals(Configure.PN_API_STATE_SUCCESS))
            {
                int itemHandle = (dictionary["item_code"] as JSONString).AsInt;
                int itemCount = (dictionary["item_count"] as JSONString).AsInt;

                m_alterProxy.RequestGetCoupon(HostID.HostID_Server, RmiContext.ReliableSend, itemHandle, itemCount);
                m_alterStub.ReplyGetCoupon = (Nettention.Proud.HostID remote, Nettention.Proud.RmiContext rmiContext, bool isSuccess) =>
                {
                    if (isSuccess)
                    {
                        if (itemHandle != 0)
                        {
                            Item_Base item = ItemMng.Instance.GetItemList[itemHandle].Clone();
                            IItemNumber number = item as IItemNumber;
                            if (number != null)
                                number.Number = itemCount;
                            ItemMng.Instance.AddItem(item);
                            UIMng.Instance.Open<SelectPopup>(UIMng.UIName.SelectPopup).RewordPopup.Enabled(0, new SQuestReword(itemHandle, itemCount));
                            RequestPostData();
                        }
                        else
                        {
                            PlayerMng.Instance.MainPlayer.Gold += itemCount;
                            UIMng.Instance.Open<SelectPopup>(UIMng.UIName.SelectPopup).RewordPopup.Enabled(itemCount, new SQuestReword(itemHandle, itemCount));
                        }
                    }
                    return true;
                };
            }
            else
            {
                Debug.Log("실패");
            }
        });
    }
    public void RequestPostData()
    {
        PostList.Clear();
        m_plugin.PostboxItem((state, message, rawData, dictionary) =>
        {
            if (state.Equals(Configure.PN_API_STATE_SUCCESS))
            {
                ArrayList items = (ArrayList)dictionary["item"];
                foreach (Dictionary<string, object> item in items)
                {
                    PostInfo post = new PostInfo();
                    post.ID = item["uid"] as JSONString;
                    post.Package = (item["item_code"] as JSONString).AsInt;
                    post.Number = (item["item_count"] as JSONNumber).AsInt;
                    post.Date = (item["expire_sec"] as JSONNumber).AsInt;
                    post.Subject = item["message"] as JSONString;
                    PostList.Add(post);
                }

                UIMng.Instance.Open<Post>(UIMng.UIName.Post).Open();
            }
            else
            {
                Debug.Log("실패");
            }
        });
    }
    public void RequestSubscriptionAccount(int number)
    {
        string code = SubscriptionArr[number];
        if (CurrSubscriptionList.Contains(code))
        {
            Debug.Log("이미 가입됨");
            return;
        }
        m_plugin.PostboxSubscriptionRegister(code, (state, message, rawData, dictionary) => {
            if (state.Equals(Configure.PN_API_STATE_SUCCESS))
            {
                CurrSubscriptionList.Add(code);
                Debug.Log("완료");
            }
            else
            {
                Debug.Log("실패");
            }
        });
    }
    public void RequestSubscriptionCancel(string code)
    {
        m_plugin.PostboxSubscriptionCancel(code, (state, message, rawData, dictionary) => {
            if (state.Equals(Configure.PN_API_STATE_SUCCESS))
            {
                Debug.Log("완료");
            }
            else
            {
                Debug.Log("실패");
            }
        });
    }
    public void RequestCouponUse(string code)
    {
        if(ItemMng.Instance.MaxInventorySize <= ItemMng.Instance.Inventory.Count)
        {
            SystemMessage.Instance.PushMessage(SystemMessage.MessageType.Sub, "인벤토리 공간이 부족합니다.");
            return;
        }
        m_plugin.Coupon(code, (state, message, rawData, dictionary) => {
            if (state.Equals(Configure.PN_API_STATE_SUCCESS))
            {
                int itemHandle = int.Parse((string)dictionary["item_code"]);
                int itemCount = int.Parse((string)dictionary["item_count"]);
                m_alterProxy.RequestGetCoupon(HostID.HostID_Server, RmiContext.ReliableSend, itemHandle, itemCount);
                m_alterStub.ReplyGetCoupon = (Nettention.Proud.HostID remote, Nettention.Proud.RmiContext rmiContext, bool isSuccess) =>
                {
                    if (isSuccess)
                    {
                        if (itemHandle != 0)
                        {
                            Item_Base item = ItemMng.Instance.GetItemList[itemHandle].Clone();
                            IItemNumber number = item as IItemNumber;
                            if (number != null)
                                number.Number = itemCount;
                            ItemMng.Instance.AddItem(item);
                            UIMng.Instance.Open<SelectPopup>(UIMng.UIName.SelectPopup).RewordPopup.Enabled(0, new SQuestReword(itemHandle, itemCount));
                        }
                        else
                        {
                            PlayerMng.Instance.MainPlayer.Gold += itemCount;
                            UIMng.Instance.Open<SelectPopup>(UIMng.UIName.SelectPopup).RewordPopup.Enabled(itemCount, new SQuestReword(itemHandle, itemCount));
                        }
                    }
                    return true;
                };
            }
            else
            {
                SystemMessage.Instance.PushMessage(SystemMessage.MessageType.Sub, "유효하지 않은 쿠폰입니다.");
            }
        });
    }
}
