﻿




// Generated by PIDL compiler.
// Do not modify this file, but modify the source .pidl file.

using System;
using System.Net;

namespace Net_Battle_C2S
{
	internal class Proxy:Nettention.Proud.RmiProxy
	{
public bool RequestEnermyInstantiate(Nettention.Proud.HostID remote,Nettention.Proud.RmiContext rmiContext, int handle, UnityEngine.Vector3 pos, float angle)
{
	Nettention.Proud.Message __msg=new Nettention.Proud.Message();
		__msg.SimplePacketMode = core.IsSimplePacketMode();
		Nettention.Proud.RmiID __msgid= Common.RequestEnermyInstantiate;
		__msg.Write(__msgid);
		Arena_Server.Marshaler.Write(__msg, handle);
		Arena_Server.Marshaler.Write(__msg, pos);
		Arena_Server.Marshaler.Write(__msg, angle);
		
	Nettention.Proud.HostID[] __list = new Nettention.Proud.HostID[1];
	__list[0] = remote;
		
	return RmiSend(__list,rmiContext,__msg,
		RmiName_RequestEnermyInstantiate, Common.RequestEnermyInstantiate);
}

public bool RequestEnermyInstantiate(Nettention.Proud.HostID[] remotes,Nettention.Proud.RmiContext rmiContext, int handle, UnityEngine.Vector3 pos, float angle)
{
	Nettention.Proud.Message __msg=new Nettention.Proud.Message();
__msg.SimplePacketMode = core.IsSimplePacketMode();
Nettention.Proud.RmiID __msgid= Common.RequestEnermyInstantiate;
__msg.Write(__msgid);
Arena_Server.Marshaler.Write(__msg, handle);
Arena_Server.Marshaler.Write(__msg, pos);
Arena_Server.Marshaler.Write(__msg, angle);
		
	return RmiSend(remotes,rmiContext,__msg,
		RmiName_RequestEnermyInstantiate, Common.RequestEnermyInstantiate);
}
public bool RequestEnermyKill(Nettention.Proud.HostID remote,Nettention.Proud.RmiContext rmiContext, int uniqueID)
{
	Nettention.Proud.Message __msg=new Nettention.Proud.Message();
		__msg.SimplePacketMode = core.IsSimplePacketMode();
		Nettention.Proud.RmiID __msgid= Common.RequestEnermyKill;
		__msg.Write(__msgid);
		Arena_Server.Marshaler.Write(__msg, uniqueID);
		
	Nettention.Proud.HostID[] __list = new Nettention.Proud.HostID[1];
	__list[0] = remote;
		
	return RmiSend(__list,rmiContext,__msg,
		RmiName_RequestEnermyKill, Common.RequestEnermyKill);
}

public bool RequestEnermyKill(Nettention.Proud.HostID[] remotes,Nettention.Proud.RmiContext rmiContext, int uniqueID)
{
	Nettention.Proud.Message __msg=new Nettention.Proud.Message();
__msg.SimplePacketMode = core.IsSimplePacketMode();
Nettention.Proud.RmiID __msgid= Common.RequestEnermyKill;
__msg.Write(__msgid);
Arena_Server.Marshaler.Write(__msg, uniqueID);
		
	return RmiSend(remotes,rmiContext,__msg,
		RmiName_RequestEnermyKill, Common.RequestEnermyKill);
}
public bool RequestGetReword(Nettention.Proud.HostID remote,Nettention.Proud.RmiContext rmiContext, int uniqueID)
{
	Nettention.Proud.Message __msg=new Nettention.Proud.Message();
		__msg.SimplePacketMode = core.IsSimplePacketMode();
		Nettention.Proud.RmiID __msgid= Common.RequestGetReword;
		__msg.Write(__msgid);
		Arena_Server.Marshaler.Write(__msg, uniqueID);
		
	Nettention.Proud.HostID[] __list = new Nettention.Proud.HostID[1];
	__list[0] = remote;
		
	return RmiSend(__list,rmiContext,__msg,
		RmiName_RequestGetReword, Common.RequestGetReword);
}

public bool RequestGetReword(Nettention.Proud.HostID[] remotes,Nettention.Proud.RmiContext rmiContext, int uniqueID)
{
	Nettention.Proud.Message __msg=new Nettention.Proud.Message();
__msg.SimplePacketMode = core.IsSimplePacketMode();
Nettention.Proud.RmiID __msgid= Common.RequestGetReword;
__msg.Write(__msgid);
Arena_Server.Marshaler.Write(__msg, uniqueID);
		
	return RmiSend(remotes,rmiContext,__msg,
		RmiName_RequestGetReword, Common.RequestGetReword);
}
#if USE_RMI_NAME_STRING
// RMI name declaration.
// It is the unique pointer that indicates RMI name such as RMI profiler.
public const string RmiName_RequestEnermyInstantiate="RequestEnermyInstantiate";
public const string RmiName_RequestEnermyKill="RequestEnermyKill";
public const string RmiName_RequestGetReword="RequestGetReword";
       
public const string RmiName_First = RmiName_RequestEnermyInstantiate;
#else
// RMI name declaration.
// It is the unique pointer that indicates RMI name such as RMI profiler.
public const string RmiName_RequestEnermyInstantiate="";
public const string RmiName_RequestEnermyKill="";
public const string RmiName_RequestGetReword="";
       
public const string RmiName_First = "";
#endif
		public override Nettention.Proud.RmiID[] GetRmiIDList() { return Common.RmiIDList; } 
	}
}

