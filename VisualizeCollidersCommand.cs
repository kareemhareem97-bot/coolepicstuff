using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using AdminToys;
using MapGeneration;
using Mirror;
using PlayerRoles;
using RemoteAdmin;
using UnityEngine;

namespace CommandSystem.Commands.RemoteAdmin
{
	// Token: 0x02000A24 RID: 2596
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class VisualizeCollidersCommand : ICommand
	{
		// Token: 0x17000C9C RID: 3228
		// (get) Token: 0x06003AB2 RID: 15026 RVA: 0x000C1CD5 File Offset: 0x000BFED5
		public string Command { get; } = "visualizecolliders";

		// Token: 0x17000C9D RID: 3229
		// (get) Token: 0x06003AB3 RID: 15027 RVA: 0x000C1CDD File Offset: 0x000BFEDD
		public string[] Aliases { get; } = new string[]
		{
			"visualisecolliders",
			"viscols"
		};

		// Token: 0x17000C9E RID: 3230
		// (get) Token: 0x06003AB4 RID: 15028 RVA: 0x000C1CE5 File Offset: 0x000BFEE5
		public string Description { get; } = "Toggles box collider visualization in the current room.";

		// Token: 0x06003AB5 RID: 15029 RVA: 0x000C1CF0 File Offset: 0x000BFEF0
		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			RoomIdentifier roomIdentifier;
			PrimitiveObjectToy original;
			if (!VisualizeCollidersCommand.TryPrevalidate(sender, out response, out roomIdentifier, out original))
			{
				return false;
			}
			VisualizeCollidersCommand.ClearNullInstances();
			bool flag = false;
			foreach (VisualizeCollidersCommand.RoomInstancePair roomInstancePair in VisualizeCollidersCommand.ActiveInstances)
			{
				if (!(roomInstancePair.Room != roomIdentifier))
				{
					flag = true;
					NetworkServer.Destroy(roomInstancePair.Instance.gameObject);
				}
			}
			if (flag)
			{
				response = "Colliders visualization turned off.";
				return true;
			}
			BoxCollider[] componentsInChildren = roomIdentifier.GetComponentsInChildren<BoxCollider>();
			int num = 0;
			foreach (BoxCollider boxCollider in componentsInChildren)
			{
				string key = LayerMask.LayerToName(boxCollider.gameObject.layer);
				Color valueOrDefault = VisualizeCollidersCommand.LayerToColor.GetValueOrDefault(key, Color.white);
				PrimitiveObjectToy primitiveObjectToy = UnityEngine.Object.Instantiate<PrimitiveObjectToy>(original);
				primitiveObjectToy.NetworkPrimitiveType = PrimitiveType.Cube;
				primitiveObjectToy.NetworkPrimitiveFlags = PrimitiveFlags.Visible;
				primitiveObjectToy.NetworkMaterialColor = new Color(valueOrDefault.r, valueOrDefault.g, valueOrDefault.b, 0.7f);
				Transform transform = primitiveObjectToy.transform;
				Transform transform2 = boxCollider.transform;
				Vector3 a;
				Quaternion rotation;
				transform2.GetPositionAndRotation(out a, out rotation);
				Vector3 b = transform2.TransformVector(boxCollider.center);
				transform.SetPositionAndRotation(a + b, rotation);
				transform.localScale = Vector3.Scale(transform2.lossyScale, boxCollider.size);
				num++;
				VisualizeCollidersCommand.ActiveInstances.Add(new VisualizeCollidersCommand.RoomInstancePair(roomIdentifier, primitiveObjectToy));
				NetworkServer.Spawn(primitiveObjectToy.gameObject, null);
			}
			response = string.Format("Successfully visualized {0} box colliders.", num);
			return true;
		}

		// Token: 0x06003AB6 RID: 15030 RVA: 0x000C1E9C File Offset: 0x000C009C
		private static bool TryPrevalidate(ICommandSender sender, out string response, out RoomIdentifier rid, out PrimitiveObjectToy primitiveTemplate)
		{
			rid = null;
			primitiveTemplate = null;
			if (!sender.CheckPermission(PlayerPermissions.FacilityManagement, out response))
			{
				return false;
			}
			PlayerCommandSender playerCommandSender = sender as PlayerCommandSender;
			if (playerCommandSender == null)
			{
				response = "You must be in-game to use this command!";
				return false;
			}
			if (!playerCommandSender.ReferenceHub.IsAlive())
			{
				response = "You need to be alive to run this command!";
				return false;
			}
			if (!playerCommandSender.ReferenceHub.TryGetCurrentRoom(out rid))
			{
				response = "Current room not found.";
				return false;
			}
			using (Dictionary<uint, GameObject>.ValueCollection.Enumerator enumerator = NetworkClient.prefabs.Values.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.TryGetComponent<PrimitiveObjectToy>(out primitiveTemplate))
					{
						return true;
					}
				}
			}
			response = "Primitive template not found.";
			return false;
		}

		// Token: 0x06003AB7 RID: 15031 RVA: 0x000C1F58 File Offset: 0x000C0158
		private static void ClearNullInstances()
		{
			for (int i = VisualizeCollidersCommand.ActiveInstances.Count - 1; i >= 0; i--)
			{
				VisualizeCollidersCommand.RoomInstancePair roomInstancePair = VisualizeCollidersCommand.ActiveInstances[i];
				if (!(roomInstancePair.Instance != null) || !(roomInstancePair.Room != null))
				{
					VisualizeCollidersCommand.ActiveInstances.RemoveAt(i);
				}
			}
		}

		// Token: 0x06003AB9 RID: 15033 RVA: 0x000C1FEC File Offset: 0x000C01EC
		// Note: this type is marked as 'beforefieldinit'.
		static VisualizeCollidersCommand()
		{
			Dictionary<string, Color> dictionary = new Dictionary<string, Color>();
			dictionary["Default"] = Color.red;
			dictionary["InvisibleCollider"] = Color.green;
			dictionary["Glass"] = Color.blue;
			dictionary["Fence"] = Color.cyan;
			dictionary["TransparentFX"] = Color.clear;
			VisualizeCollidersCommand.LayerToColor = dictionary;
		}

		// Token: 0x04002B69 RID: 11113
		private static readonly List<VisualizeCollidersCommand.RoomInstancePair> ActiveInstances = new List<VisualizeCollidersCommand.RoomInstancePair>();

		// Token: 0x04002B6A RID: 11114
		private static readonly Dictionary<string, Color> LayerToColor;

		// Token: 0x02000A25 RID: 2597
		private class RoomInstancePair : IEquatable<VisualizeCollidersCommand.RoomInstancePair>
		{
			// Token: 0x06003ABA RID: 15034 RVA: 0x000C205D File Offset: 0x000C025D
			public RoomInstancePair(RoomIdentifier Room, PrimitiveObjectToy Instance)
			{
				this.Room = Room;
				this.Instance = Instance;
				base..ctor();
			}

			// Token: 0x17000C9F RID: 3231
			// (get) Token: 0x06003ABB RID: 15035 RVA: 0x000C2073 File Offset: 0x000C0273
			[Nullable(1)]
			[CompilerGenerated]
			protected virtual Type EqualityContract
			{
				[NullableContext(1)]
				[CompilerGenerated]
				get
				{
					return typeof(VisualizeCollidersCommand.RoomInstancePair);
				}
			}

			// Token: 0x17000CA0 RID: 3232
			// (get) Token: 0x06003ABC RID: 15036 RVA: 0x000C207F File Offset: 0x000C027F
			// (set) Token: 0x06003ABD RID: 15037 RVA: 0x000C2087 File Offset: 0x000C0287
			public RoomIdentifier Room { get; set; }

			// Token: 0x17000CA1 RID: 3233
			// (get) Token: 0x06003ABE RID: 15038 RVA: 0x000C2090 File Offset: 0x000C0290
			// (set) Token: 0x06003ABF RID: 15039 RVA: 0x000C2098 File Offset: 0x000C0298
			public PrimitiveObjectToy Instance { get; set; }

			// Token: 0x06003AC0 RID: 15040 RVA: 0x000C20A4 File Offset: 0x000C02A4
			[NullableContext(1)]
			[CompilerGenerated]
			public override string ToString()
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("RoomInstancePair");
				stringBuilder.Append(" { ");
				if (this.PrintMembers(stringBuilder))
				{
					stringBuilder.Append(' ');
				}
				stringBuilder.Append('}');
				return stringBuilder.ToString();
			}

			// Token: 0x06003AC1 RID: 15041 RVA: 0x000C20F0 File Offset: 0x000C02F0
			[NullableContext(1)]
			[CompilerGenerated]
			protected virtual bool PrintMembers(StringBuilder builder)
			{
				RuntimeHelpers.EnsureSufficientExecutionStack();
				builder.Append("Room = ");
				builder.Append(this.Room);
				builder.Append(", Instance = ");
				builder.Append(this.Instance);
				return true;
			}

			// Token: 0x06003AC2 RID: 15042 RVA: 0x000C212A File Offset: 0x000C032A
			[NullableContext(2)]
			[CompilerGenerated]
			public static bool operator !=(VisualizeCollidersCommand.RoomInstancePair left, VisualizeCollidersCommand.RoomInstancePair right)
			{
				return !(left == right);
			}

			// Token: 0x06003AC3 RID: 15043 RVA: 0x000C2136 File Offset: 0x000C0336
			[NullableContext(2)]
			[CompilerGenerated]
			public static bool operator ==(VisualizeCollidersCommand.RoomInstancePair left, VisualizeCollidersCommand.RoomInstancePair right)
			{
				return left == right || (left != null && left.Equals(right));
			}

			// Token: 0x06003AC4 RID: 15044 RVA: 0x000C214A File Offset: 0x000C034A
			[CompilerGenerated]
			public override int GetHashCode()
			{
				return (EqualityComparer<Type>.Default.GetHashCode(this.EqualityContract) * -1521134295 + EqualityComparer<RoomIdentifier>.Default.GetHashCode(this.<Room>k__BackingField)) * -1521134295 + EqualityComparer<PrimitiveObjectToy>.Default.GetHashCode(this.<Instance>k__BackingField);
			}

			// Token: 0x06003AC5 RID: 15045 RVA: 0x000C218A File Offset: 0x000C038A
			[NullableContext(2)]
			[CompilerGenerated]
			public override bool Equals(object obj)
			{
				return this.Equals(obj as VisualizeCollidersCommand.RoomInstancePair);
			}

			// Token: 0x06003AC6 RID: 15046 RVA: 0x000C2198 File Offset: 0x000C0398
			[NullableContext(2)]
			[CompilerGenerated]
			public virtual bool Equals(VisualizeCollidersCommand.RoomInstancePair other)
			{
				return this == other || (other != null && this.EqualityContract == other.EqualityContract && EqualityComparer<RoomIdentifier>.Default.Equals(this.<Room>k__BackingField, other.<Room>k__BackingField) && EqualityComparer<PrimitiveObjectToy>.Default.Equals(this.<Instance>k__BackingField, other.<Instance>k__BackingField));
			}

			// Token: 0x06003AC8 RID: 15048 RVA: 0x000C21F9 File Offset: 0x000C03F9
			[CompilerGenerated]
			protected RoomInstancePair([Nullable(1)] VisualizeCollidersCommand.RoomInstancePair original)
			{
				this.Room = original.<Room>k__BackingField;
				this.Instance = original.<Instance>k__BackingField;
			}

			// Token: 0x06003AC9 RID: 15049 RVA: 0x000C2219 File Offset: 0x000C0419
			[CompilerGenerated]
			public void Deconstruct(out RoomIdentifier Room, out PrimitiveObjectToy Instance)
			{
				Room = this.Room;
				Instance = this.Instance;
			}
		}
	}
}
