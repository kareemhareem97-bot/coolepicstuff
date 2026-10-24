using System;
using System.Collections.Generic;
using InventorySystem.Disarming;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Handlers;
using Mirror;
using PlayerRoles;
using PlayerRoles.FirstPersonControl;
using UnityEngine;
using Utils.NonAllocLINQ;

// Token: 0x02000118 RID: 280
public static class Escape
{
	// Token: 0x14000014 RID: 20
	// (add) Token: 0x0600055C RID: 1372 RVA: 0x00013CFC File Offset: 0x00011EFC
	// (remove) Token: 0x0600055D RID: 1373 RVA: 0x00013D30 File Offset: 0x00011F30
	public static event Action<ReferenceHub> OnServerPlayerEscape;

	// Token: 0x0600055E RID: 1374 RVA: 0x00013D64 File Offset: 0x00011F64
	public static bool CanEscape(ReferenceHub hub, out IFpcRole role, out Bounds zone)
	{
		zone = default(Bounds);
		role = null;
		IFpcRole fpcRole = hub.roleManager.CurrentRole as IFpcRole;
		if (fpcRole == null)
		{
			return false;
		}
		role = fpcRole;
		Vector3 position = role.FpcModule.Position;
		foreach (Bounds bounds in Escape.EscapeZones)
		{
			if (bounds.Contains(position))
			{
				zone = bounds;
				return true;
			}
		}
		return false;
	}

	// Token: 0x0600055F RID: 1375 RVA: 0x00013DFC File Offset: 0x00011FFC
	[RuntimeInitializeOnLoadMethod]
	private static void Init()
	{
		StaticUnityMethods.OnUpdate += delegate()
		{
			if (!NetworkServer.active)
			{
				return;
			}
			ReferenceHub.AllHubs.ForEach(delegate(ReferenceHub x)
			{
				Escape.ServerHandlePlayer(x);
			});
		};
	}

	// Token: 0x06000560 RID: 1376 RVA: 0x00013E24 File Offset: 0x00012024
	private static void ServerHandlePlayer(ReferenceHub hub)
	{
		IFpcRole fpcRole;
		Bounds escapeZone;
		if (!Escape.CanEscape(hub, out fpcRole, out escapeZone))
		{
			return;
		}
		RoleTypeId newRole = RoleTypeId.None;
		HumanRole humanRole = fpcRole as HumanRole;
		Escape.EscapeScenarioType escapeScenarioType = (humanRole != null) ? Escape.ServerGetScenario(hub, humanRole) : Escape.EscapeScenarioType.None;
		switch (escapeScenarioType)
		{
		case Escape.EscapeScenarioType.ClassD:
		case Escape.EscapeScenarioType.CuffedScientist:
			newRole = RoleTypeId.ChaosConscript;
			break;
		case Escape.EscapeScenarioType.CuffedClassD:
			newRole = RoleTypeId.NtfPrivate;
			break;
		case Escape.EscapeScenarioType.Scientist:
			newRole = RoleTypeId.NtfSpecialist;
			break;
		}
		RoleTypeId roleTypeId = hub.roleManager.CurrentRole.RoleTypeId;
		PlayerEscapingEventArgs playerEscapingEventArgs = new PlayerEscapingEventArgs(hub, roleTypeId, newRole, escapeScenarioType, escapeZone);
		PlayerEvents.OnEscaping(playerEscapingEventArgs);
		if (!playerEscapingEventArgs.IsAllowed)
		{
			return;
		}
		newRole = playerEscapingEventArgs.NewRole;
		escapeScenarioType = playerEscapingEventArgs.EscapeScenario;
		if (escapeScenarioType == Escape.EscapeScenarioType.None)
		{
			return;
		}
		hub.connectionToClient.Send<Escape.EscapeMessage>(new Escape.EscapeMessage
		{
			ScenarioId = (byte)escapeScenarioType,
			EscapeTime = (ushort)Mathf.CeilToInt(hub.roleManager.CurrentRole.ActiveTime)
		}, 0);
		Escape.OnServerPlayerEscape(hub);
		hub.roleManager.ServerSetRole(newRole, RoleChangeReason.Escaped, RoleSpawnFlags.All, null);
		PlayerEvents.OnEscaped(new PlayerEscapedEventArgs(hub, roleTypeId, newRole, escapeScenarioType, escapeZone));
	}

	// Token: 0x06000561 RID: 1377 RVA: 0x00013F30 File Offset: 0x00012130
	private static Escape.EscapeScenarioType ServerGetScenario(ReferenceHub hub, HumanRole role)
	{
		if (role.ActiveTime < 10f)
		{
			return Escape.EscapeScenarioType.None;
		}
		bool flag = hub.inventory.IsDisarmed();
		if (flag && !CharacterClassManager.CuffedChangeTeam)
		{
			return Escape.EscapeScenarioType.None;
		}
		RoleTypeId roleTypeId = role.RoleTypeId;
		if (roleTypeId != RoleTypeId.ClassD)
		{
			if (roleTypeId != RoleTypeId.Scientist)
			{
				return Escape.EscapeScenarioType.None;
			}
			if (!flag)
			{
				return Escape.EscapeScenarioType.Scientist;
			}
			return Escape.EscapeScenarioType.CuffedScientist;
		}
		else
		{
			if (!flag)
			{
				return Escape.EscapeScenarioType.ClassD;
			}
			return Escape.EscapeScenarioType.CuffedClassD;
		}
	}

	// Token: 0x06000562 RID: 1378 RVA: 0x00002E9A File Offset: 0x0000109A
	private static void ClientReceiveMessage(Escape.EscapeMessage msg)
	{
	}

	// Token: 0x06000563 RID: 1379 RVA: 0x00013F84 File Offset: 0x00012184
	// Note: this type is marked as 'beforefieldinit'.
	static Escape()
	{
		Dictionary<Escape.EscapeScenarioType, Escape.EscapeScenarioText> dictionary = new Dictionary<Escape.EscapeScenarioType, Escape.EscapeScenarioText>();
		dictionary[Escape.EscapeScenarioType.ClassD] = new Escape.EscapeScenarioText(30, "You escaped as a Class D and joined the Chaos Insurgency.");
		dictionary[Escape.EscapeScenarioType.CuffedClassD] = new Escape.EscapeScenarioText(36, "You were recaptured by the Nine-Tailed Fox.\nWith one less threat in the facility, they were able to reinforce.");
		dictionary[Escape.EscapeScenarioType.Scientist] = new Escape.EscapeScenarioText(29, "You escaped as a Scientist and joined the MTF units.");
		dictionary[Escape.EscapeScenarioType.CuffedScientist] = new Escape.EscapeScenarioText(37, "You were taken prisoner as a scientist by the Chaos Insurgency.\nThey were able to gain an advantage from the information you gave them.");
		Escape.Scenarios = dictionary;
	}

	// Token: 0x0400047D RID: 1149
	public static readonly Bounds DefaultEscapeZone = new Bounds(new Vector3(124f, 289f, 31f), Vector3.one * 25f);

	// Token: 0x0400047E RID: 1150
	public static readonly List<Bounds> EscapeZones = new List<Bounds>
	{
		Escape.DefaultEscapeZone
	};

	// Token: 0x0400047F RID: 1151
	private static readonly Dictionary<Escape.EscapeScenarioType, Escape.EscapeScenarioText> Scenarios;

	// Token: 0x04000480 RID: 1152
	private const float MinAliveTime = 10f;

	// Token: 0x04000481 RID: 1153
	private const string TranslationKey = "Facility";

	// Token: 0x02000119 RID: 281
	public enum EscapeScenarioType
	{
		// Token: 0x04000483 RID: 1155
		None,
		// Token: 0x04000484 RID: 1156
		ClassD,
		// Token: 0x04000485 RID: 1157
		CuffedClassD,
		// Token: 0x04000486 RID: 1158
		Scientist,
		// Token: 0x04000487 RID: 1159
		CuffedScientist,
		// Token: 0x04000488 RID: 1160
		Custom
	}

	// Token: 0x0200011A RID: 282
	private readonly struct EscapeScenarioText
	{
		// Token: 0x170000D7 RID: 215
		// (get) Token: 0x06000564 RID: 1380 RVA: 0x00014029 File Offset: 0x00012229
		public string Text
		{
			get
			{
				return TranslationReader.Get("Facility", this._id, this._def);
			}
		}

		// Token: 0x06000565 RID: 1381 RVA: 0x00014041 File Offset: 0x00012241
		public EscapeScenarioText(int translationKey, string defaultText)
		{
			this._id = translationKey;
			this._def = defaultText;
		}

		// Token: 0x04000489 RID: 1161
		private readonly int _id;

		// Token: 0x0400048A RID: 1162
		private readonly string _def;
	}

	// Token: 0x0200011B RID: 283
	public struct EscapeMessage : NetworkMessage
	{
		// Token: 0x0400048B RID: 1163
		public byte ScenarioId;

		// Token: 0x0400048C RID: 1164
		public ushort EscapeTime;
	}
}
