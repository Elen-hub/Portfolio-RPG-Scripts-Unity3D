using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using System.Runtime.Serialization;

namespace System
{
    public struct HashInt 
    {
        int value;
        int KeyNum;
        const byte KeyLength = 8;
        readonly static int[] Keys = new int[]
        {
        4656164,
        -1516583,
        9756823,
        -1235793,
        8551457,
       -6438912,
       9143196,
       -6672547,
        };
        public HashInt(int value)
        {
            this.value = 0;
            KeyNum = new Random().Next(0, KeyLength);
            Encode = value;
        }
        int Encode {
            set {
                this.value = value ^ intDataHash(Keys[KeyNum]);
            }
        }
        int Decode {
            get  {
                if (value == 0)
                    return 0;

                return value ^ intDataHash(Keys[KeyNum]);
            }
        }
        public int intDataHash(int val)
        {
            val += val << 2;
            val ^=val >> 3;
            val += ~val << 5;
            val += val >> 4;
            val ^= val << 1;
            return val;
        }

        #region Operator
        //public static HashInt operator +(HashInt a, int b)
        //{
        //    return a.Decode + b;
        //}
        //public static HashInt operator -(HashInt a, int b)
        //{
        //    return a.Decode - b;
        //}
        //public static bool operator ==(HashInt a, int b)
        //{
        //    return a.Decode == b;
        //}
        //public static bool operator !=(HashInt a, int b)
        //{
        //    return a.Decode != b;
        //}
        //public static HashInt operator +(HashInt a, HashInt b)
        //{
        //    return a.Decode + b.Decode;
        //}
        //public static HashInt operator -(HashInt a, HashInt b)
        //{
        //    return a.Decode - b.Decode;
        //}
        //public static bool operator ==(HashInt a, HashInt b)
        //{
        //    return a.Decode == b.Decode;
        //}
        //public static bool operator !=(HashInt a, HashInt b)
        //{
        //    return a.Decode != b.Decode;
        //}
        //public static HashInt operator ++(HashInt a)
        //{
        //    return a.Decode + 1;
        //}
        //public static HashInt operator --(HashInt a)
        //{
        //    return a.Decode - 1;
        //}

        public static implicit operator HashInt(int val)
        {
            return new HashInt(val);
        }
        public static implicit operator int(HashInt val)
        {
            return val.Decode;
        }
        #endregion

        #region Serialization
        public override int GetHashCode()
        {
            return Decode.GetHashCode();
        }
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }
        public override string ToString()
        {
            return Decode.ToString();
        }
        #endregion
    }

    public struct HashFloat
    {
        byte[] value;
        int KeyNum;
        const byte KeyLength = 8;
        readonly static int[] Keys = new int[]
        {
            4656164, 
            -1516583, 
            9756823, 
            -1235793, 
            8551457, 
            -6438912, 
            9143196,  
            -6672547,
        };
        public HashFloat(float value)
        {
            this.value = new byte[4];
            KeyNum = new Random().Next(0, KeyLength);
            Encode = value;
        }
        public float Encode {
            set {
                this.value = FloatDataHash(BitConverter.GetBytes(value));
            }
        }
        public float Decode  {
            get {
                byte[] copy = new byte[4];
                if (value == null)
                    return 0;

                copy[0] = value[0];
                copy[1] = value[1];
                copy[2] = value[2];
                copy[3] = value[3];
                copy = FloatDataHash(copy);

                return BitConverter.ToSingle(copy, 0);
            }
        }
        byte[] FloatDataHash(byte[] val)
        {
            val[0] ^= (byte)(Keys[KeyNum] << 2);
            val[1] ^= (byte)(Keys[KeyNum] >> 3);
            val[2] ^= (byte)(~Keys[KeyNum] << 5);
            val[3] ^= (byte)(Keys[KeyNum] >> 4);
            return val;
        }

        #region Operator
        //public static HashFloat operator +(HashFloat a, int b)
        //{
        //    return a.Decode + b;
        //}
        //public static HashFloat operator -(HashFloat a, int b)
        //{
        //    return a.Decode - b;
        //}
        //public static bool operator ==(HashFloat a, int b)
        //{
        //    return a.Decode == b;
        //}
        //public static bool operator !=(HashFloat a, int b)
        //{
        //    return a.Decode != b;
        //}
        //public static HashFloat operator +(HashFloat a, HashFloat b)
        //{
        //    return a.Decode + b.Decode;
        //}
        //public static HashFloat operator -(HashFloat a, HashFloat b)
        //{
        //    return a.Decode - b.Decode;
        //}
        //public static bool operator ==(HashFloat a, HashFloat b)
        //{
        //    return a.Decode == b.Decode;
        //}
        //public static bool operator !=(HashFloat a, HashFloat b)
        //{
        //    return a.Decode != b.Decode;
        //}
        //public static HashFloat operator ++(HashFloat a)
        //{
        //    return a.Decode + 1;
        //}
        //public static HashFloat operator --(HashFloat a)
        //{
        //    return a.Decode - 1;
        //}

        public static implicit operator HashFloat(float val)
        {
            return new HashFloat(val);
        }
        public static implicit operator float(HashFloat val)
        {
            return val.Decode;
        }
        #endregion

        #region Serialization
        public override int GetHashCode()
        {
            return Decode.GetHashCode();
        }
        public override string ToString()
        {
            return Decode.ToString();
        }
        public string ToString(string format)
        {
            return Decode.ToString(format);
        }
        #endregion
    }
}