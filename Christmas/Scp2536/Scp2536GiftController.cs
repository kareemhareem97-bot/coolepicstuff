using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AudioPooling;
using Christmas.Scp2536.Gifts;
using Hints;
using Interactables;
using Interactables.Verification;
using Mirror;
using Mirror.RemoteCalls;
using PlayerRoles;
using UnityEngine;

namespace Christmas.Scp2536
{
	// Token: 0x020010D0 RID: 4304
	public class Scp2536GiftController : NetworkBehaviour, IServerInteractable, IInteractable
	{
		// Token: 0x170013DF RID: 5087
		// (get) Token: 0x06005FDD RID: 24541 RVA: 0x001408F8 File Offset: 0x0013EAF8
		public static List<Scp2536GiftBase> Gifts { get; } = (from gift in new List<Scp2536GiftBase>
		{
			new Naughty(),
			new Emergency(),
			new Throwables(),
			new KeycardUpgrade(),
			new LastStand(),
			new MedicalItems(),
			new StarterCard(),
			new TierOneWeapons(),
			new TierTwoWeapons(),
			new TierThreeWeapons(),
			new TierFourWeapons(),
			new NonStandardWeapons(),
			new TapeGift(),
			new CandyBagGift(),
			new BuddyInABox(),
			new FlyingCorpse(),
			new HealthBoost(),
			new HumeShieldRefresh(),
			new NoPresent(),
			new SpeedBoost(),
			new TemporaryFocus(),
			new VigorBoost(),
			new ZombieInABox(),
			new TargetFinder(),
			new AbilityCooldownReset()
		}
		orderby gift.Urgency
		select gift).ToList<Scp2536GiftBase>();

		// Token: 0x14000129 RID: 297
		// (add) Token: 0x06005FDE RID: 24542 RVA: 0x00140900 File Offset: 0x0013EB00
		// (remove) Token: 0x06005FDF RID: 24543 RVA: 0x00140934 File Offset: 0x0013EB34
		public static event Action OnServerOpenGift;

		// Token: 0x170013E0 RID: 5088
		// (get) Token: 0x06005FE0 RID: 24544 RVA: 0x00003897 File Offset: 0x00001A97
		public IVerificationRule VerificationRule
		{
			get
			{
				return StandardDistanceVerification.Default;
			}
		}

		// Token: 0x06005FE1 RID: 24545 RVA: 0x00140968 File Offset: 0x0013EB68
		public T ServerGetGift<T>() where T : Scp2536GiftBase
		{
			foreach (Scp2536GiftBase scp2536GiftBase in Scp2536GiftController.Gifts)
			{
				T t = scp2536GiftBase as T;
				if (t != null)
				{
					return t;
				}
			}
			return default(T);
		}

		// Token: 0x06005FE2 RID: 24546 RVA: 0x001409D4 File Offset: 0x0013EBD4
		public void ServerGrantRandomGift(ReferenceHub hub)
		{
			if (!NetworkServer.active)
			{
				return;
			}
			bool flag = hub.IsSCP(false);
			for (int i = 0; i < Scp2536GiftController.Gifts.Count; i++)
			{
				Scp2536GiftBase scp2536GiftBase = Scp2536GiftController.Gifts[i];
				bool flag2 = scp2536GiftBase is Scp2536AnomalousGiftBase;
				if ((!flag || flag2) && (flag || !flag2) && !scp2536GiftBase.IgnoredByRandomness && scp2536GiftBase.CanBeGranted(hub))
				{
					ITimeLimitedGift timeLimitedGift = scp2536GiftBase as ITimeLimitedGift;
					if (timeLimitedGift == null || timeLimitedGift.IsAvailable)
					{
						if (NonStandardWeapons.CanOverrideGift(scp2536GiftBase))
						{
							scp2536GiftBase = this.ServerGetGift<NonStandardWeapons>();
						}
						scp2536GiftBase.ObtainedBy.Add(hub);
						scp2536GiftBase.ServerGrant(hub);
						return;
					}
				}
			}
			this.ServerGetGift<Naughty>().ServerGrant(hub);
		}

		// Token: 0x06005FE3 RID: 24547 RVA: 0x00140A80 File Offset: 0x0013EC80
		public void ServerPrepareGifts(bool resetRewards = true)
		{
			for (int i = 0; i < this.GiftBoxes.Length; i++)
			{
				this.RpcSetGiftState(i, true);
			}
			if (!resetRewards)
			{
				return;
			}
			foreach (Scp2536GiftBase scp2536GiftBase in Scp2536GiftController.Gifts)
			{
				scp2536GiftBase.Reset();
			}
		}

		// Token: 0x06005FE4 RID: 24548 RVA: 0x00140AF0 File Offset: 0x0013ECF0
		public void ServerInteract(ReferenceHub ply, byte colliderId)
		{
			if (!Scp2536Controller.PrevalidateTarget(ply))
			{
				return;
			}
			bool flag = ply.IsSCP(false);
			if (!ply.IsHuman() && !flag)
			{
				return;
			}
			if (flag && Scp2536AnomalousGiftBase.GlobalObtainedBy.Contains(ply))
			{
				return;
			}
			Scp2536GiftController.GiftBox giftBox = this.GiftBoxes[(int)colliderId % this.GiftBoxes.Length];
			if (!giftBox.CanBeOpen)
			{
				return;
			}
			if (ply.inventory.UserInventory.Items.Count >= 8)
			{
				ply.hints.Show(new TranslationHint(HintTranslations.MaxItemsAlreadyReached, new HintParameter[]
				{
					new ByteHintParameter(8)
				}, new HintEffect[]
				{
					HintEffectPresets.TrailingPulseAlpha(0.5f, 1f, 0.5f, 2f, 0f, 3)
				}, 2f));
				return;
			}
			Stopwatch stopwatch;
			if (this._cooldowns.TryGetValue(ply.netId, out stopwatch))
			{
				if (stopwatch.Elapsed.TotalSeconds < 0.699999988079071)
				{
					return;
				}
				stopwatch.Restart();
			}
			else
			{
				this._cooldowns.Add(ply.netId, Stopwatch.StartNew());
			}
			this.RpcSetGiftState((int)colliderId, false);
			Scp2536Controller.Singleton.GiftController.ServerGrantRandomGift(ply);
			giftBox.CanBeOpen = false;
			Action onServerOpenGift = Scp2536GiftController.OnServerOpenGift;
			if (onServerOpenGift == null)
			{
				return;
			}
			onServerOpenGift();
		}

		// Token: 0x06005FE5 RID: 24549 RVA: 0x00140C28 File Offset: 0x0013EE28
		[ClientRpc]
		private void RpcSetGiftState(int giftIndex, bool isWrapped)
		{
			NetworkWriterPooled writer = NetworkWriterPool.Get();
			writer.WriteInt(giftIndex);
			writer.WriteBool(isWrapped);
			this.SendRPCInternal("System.Void Christmas.Scp2536.Scp2536GiftController::RpcSetGiftState(System.Int32,System.Boolean)", -1695853183, writer, 0, true);
			NetworkWriterPool.Return(writer);
		}

		// Token: 0x06005FE7 RID: 24551 RVA: 0x00140C80 File Offset: 0x0013EE80
		static Scp2536GiftController()
		{
			RemoteProcedureCalls.RegisterRpc(typeof(Scp2536GiftController), "System.Void Christmas.Scp2536.Scp2536GiftController::RpcSetGiftState(System.Int32,System.Boolean)", new RemoteCallDelegate(Scp2536GiftController.InvokeUserCode_RpcSetGiftState__Int32__Boolean));
		}

		// Token: 0x06005FE8 RID: 24552 RVA: 0x00002DA5 File Offset: 0x00000FA5
		public override bool Weaved()
		{
			return true;
		}

		// Token: 0x06005FE9 RID: 24553 RVA: 0x00140DE4 File Offset: 0x0013EFE4
		protected void UserCode_RpcSetGiftState__Int32__Boolean(int giftIndex, bool isWrapped)
		{
			Scp2536GiftController.GiftBox giftBox = this.GiftBoxes[giftIndex];
			giftBox.Lid.SetActive(isWrapped);
			giftBox.CanBeOpen = isWrapped;
			if (!isWrapped)
			{
				giftBox.Particles.Play();
				AudioSourcePoolManager.PlayOnTransform(this._openGiftClip, base.transform, 10f, 1f, FalloffType.Exponential, MixerChannel.DefaultSfx, 1f);
				return;
			}
			giftBox.Particles.Stop();
		}

		// Token: 0x06005FEA RID: 24554 RVA: 0x00140E4A File Offset: 0x0013F04A
		protected static void InvokeUserCode_RpcSetGiftState__Int32__Boolean(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
		{
			if (!NetworkClient.active)
			{
				UnityEngine.Debug.LogError("RPC RpcSetGiftState called on server.");
				return;
			}
			((Scp2536GiftController)obj).UserCode_RpcSetGiftState__Int32__Boolean(reader.ReadInt(), reader.ReadBool());
		}

		// Token: 0x040045F1 RID: 17905
		private const bool CanZombiesOpenGifts = false;

		// Token: 0x040045F4 RID: 17908
		public Scp2536GiftController.GiftBox[] GiftBoxes;

		// Token: 0x040045F5 RID: 17909
		[SerializeField]
		private AudioClip _openGiftClip;

		// Token: 0x040045F6 RID: 17910
		private readonly Dictionary<uint, Stopwatch> _cooldowns = new Dictionary<uint, Stopwatch>();

		// Token: 0x040045F7 RID: 17911
		private const float InteractionCooldown = 0.7f;

		// Token: 0x020010D1 RID: 4305
		[Serializable]
		public class GiftBox
		{
			// Token: 0x170013E1 RID: 5089
			// (get) Token: 0x06005FEB RID: 24555 RVA: 0x00140E79 File Offset: 0x0013F079
			// (set) Token: 0x06005FEC RID: 24556 RVA: 0x00140E81 File Offset: 0x0013F081
			public bool CanBeOpen { get; set; }

			// Token: 0x040045F9 RID: 17913
			public ParticleSystem Particles;

			// Token: 0x040045FA RID: 17914
			public GameObject Lid;
		}
	}
}
