﻿ 





// Generated by PIDL compiler.
// Do not modify this file, but modify the source .pidl file.

using System;
namespace Net_Contact_C2C
{
	internal class Common
	{
		// Message ID that replies to each RMI method. 
			public const Nettention.Proud.RmiID NotifyPlayerJoin = (Nettention.Proud.RmiID)5000+1;
			public const Nettention.Proud.RmiID NotifyPlayerInfo = (Nettention.Proud.RmiID)5000+2;
			public const Nettention.Proud.RmiID NotifyPlayerExit = (Nettention.Proud.RmiID)5000+3;
		// List that has RMI ID.
		public static Nettention.Proud.RmiID[] RmiIDList = new Nettention.Proud.RmiID[] {
			NotifyPlayerJoin,
			NotifyPlayerInfo,
			NotifyPlayerExit,
		};
	}
}
namespace Net_Behavior_C2C
{
	internal class Common
	{
		// Message ID that replies to each RMI method. 
			public const Nettention.Proud.RmiID NotifyCharacterIdle = (Nettention.Proud.RmiID)5100+1;
			public const Nettention.Proud.RmiID NotifyCharacterMove = (Nettention.Proud.RmiID)5100+2;
			public const Nettention.Proud.RmiID NotifyCharacterMoveTarget = (Nettention.Proud.RmiID)5100+3;
			public const Nettention.Proud.RmiID NotifyCharacterMovePosition = (Nettention.Proud.RmiID)5100+4;
			public const Nettention.Proud.RmiID NotifyCharacterAttack = (Nettention.Proud.RmiID)5100+5;
			public const Nettention.Proud.RmiID NotifyCharacterSkill = (Nettention.Proud.RmiID)5100+6;
			public const Nettention.Proud.RmiID NotifyCharacterSkillEnd = (Nettention.Proud.RmiID)5100+7;
			public const Nettention.Proud.RmiID NotifyItemEquip = (Nettention.Proud.RmiID)5100+8;
			public const Nettention.Proud.RmiID NotifyItemUnequip = (Nettention.Proud.RmiID)5100+9;
		// List that has RMI ID.
		public static Nettention.Proud.RmiID[] RmiIDList = new Nettention.Proud.RmiID[] {
			NotifyCharacterIdle,
			NotifyCharacterMove,
			NotifyCharacterMoveTarget,
			NotifyCharacterMovePosition,
			NotifyCharacterAttack,
			NotifyCharacterSkill,
			NotifyCharacterSkillEnd,
			NotifyItemEquip,
			NotifyItemUnequip,
		};
	}
}
namespace Net_Status_C2C
{
	internal class Common
	{
		// Message ID that replies to each RMI method. 
		// List that has RMI ID.
		public static Nettention.Proud.RmiID[] RmiIDList = new Nettention.Proud.RmiID[] {
		};
	}
}
namespace Net_Community_C2C
{
	internal class Common
	{
		// Message ID that replies to each RMI method. 
			public const Nettention.Proud.RmiID NotifyRequestJoinPrivateMap = (Nettention.Proud.RmiID)5300+1;
			public const Nettention.Proud.RmiID NotifyReplyJoinPrivateMap = (Nettention.Proud.RmiID)5300+2;
			public const Nettention.Proud.RmiID NotifyRequestJoinPrivatePortal = (Nettention.Proud.RmiID)5300+3;
			public const Nettention.Proud.RmiID NotifyReplyJoinPrivatePortal = (Nettention.Proud.RmiID)5300+4;
			public const Nettention.Proud.RmiID NotifyJoinPrivatePortalCancle = (Nettention.Proud.RmiID)5300+5;
			public const Nettention.Proud.RmiID NotifyRequestPartyList = (Nettention.Proud.RmiID)5300+6;
			public const Nettention.Proud.RmiID NotifyReplyPartyList = (Nettention.Proud.RmiID)5300+7;
			public const Nettention.Proud.RmiID NotifySendChat = (Nettention.Proud.RmiID)5300+8;
		// List that has RMI ID.
		public static Nettention.Proud.RmiID[] RmiIDList = new Nettention.Proud.RmiID[] {
			NotifyRequestJoinPrivateMap,
			NotifyReplyJoinPrivateMap,
			NotifyRequestJoinPrivatePortal,
			NotifyReplyJoinPrivatePortal,
			NotifyJoinPrivatePortalCancle,
			NotifyRequestPartyList,
			NotifyReplyPartyList,
			NotifySendChat,
		};
	}
}

				 
