using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nettention.Proud;
using Vector3 = UnityEngine.Vector3;

public enum EStatType
{
    STR,
    DEX,
    CON,
    INT,
    WIS
}
namespace Arena_Server
{
    public class Marshaler : Nettention.Proud.Marshaler
    {
        public static void Write(Message msg, ECoinShopType b)
        {
            msg.Write((int)b);
        }

        public static void Read(Message msg, out ECoinShopType b)
        {
            int protocol;
            msg.Read(out protocol);
            b = (ECoinShopType)protocol;
        }
        public static void Write(Message msg, EPartyState b)
        {
            msg.Write((int)b);
        }

        public static void Read(Message msg, out EPartyState b)
        {
            int protocol;
            msg.Read(out protocol);
            b = (EPartyState)protocol;
        }
        public static void Write(Message msg , EItemType b)
        {
            msg.Write((int)b);
        }

        public static void Read(Message msg, out EItemType b)
        {
            int protocol;
            msg.Read(out protocol);
            b = (EItemType)protocol;
        }
        public static void Write(Message msg, EAllyType b)
        {
            msg.Write((int)b);
        }

        public static void Read(Message msg, out EAllyType b)
        {
            int protocol;
            msg.Read(out protocol);
            b = (EAllyType)protocol;
        }

        public static void Write(Message msg, UnityEngine.Vector3 b)
        {
            msg.Write(b.x);
            msg.Write(b.y);
            msg.Write(b.z);
        }

        public static void Read(Message msg, out UnityEngine.Vector3 b)
        {
            b = new UnityEngine.Vector3();
            msg.Read(out b.x);
            msg.Read(out b.y);
            msg.Read(out b.z);
        }

        public static void Write(Message msg, EStatType type)
        {
            msg.Write((int)type);
        }

        public static void Read(Message msg, out EStatType type)
        {
            int b;
            msg.Read(out b);
            type = (EStatType)b;
        }
        public static void Write(Message msg, EAttackType type)
        {
            msg.Write((int)type);
        }

        public static void Read(Message msg, out EAttackType type)
        {
            int b;
            msg.Read(out b);
            type = (EAttackType)b;
        }
    }
}
