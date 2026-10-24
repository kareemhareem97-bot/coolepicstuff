using System;
using UnityEngine;
using UserSettings;
using UserSettings.VideoSettings;

namespace Decals
{
	// Token: 0x02001106 RID: 4358
	public class DecalPoolManager : MonoBehaviour
	{
		// Token: 0x060060FF RID: 24831 RVA: 0x00143686 File Offset: 0x00141886
		private void Awake()
		{
			DecalPoolManager._singleton = this;
			DecalPoolManager._singletonSet = true;
			this._collections.ForEach(delegate(DecalPoolManager.DecalCollection x)
			{
				x.Setup();
			});
		}

		// Token: 0x06006100 RID: 24832 RVA: 0x001436BE File Offset: 0x001418BE
		private void OnDestroy()
		{
			DecalPoolManager._singletonSet = false;
			this._collections.ForEach(delegate(DecalPoolManager.DecalCollection x)
			{
				x.UnlinkListeners();
			});
		}

		// Token: 0x06006101 RID: 24833 RVA: 0x001436F0 File Offset: 0x001418F0
		public static bool TryGet(DecalPoolType poolType, out Decal decal)
		{
			if (DecalPoolManager._singletonSet)
			{
				DecalPoolManager.DecalCollection[] collections = DecalPoolManager._singleton._collections;
				for (int i = 0; i < collections.Length; i++)
				{
					if (collections[i].TryGet(poolType, out decal))
					{
						return true;
					}
				}
			}
			decal = null;
			return false;
		}

		// Token: 0x06006102 RID: 24834 RVA: 0x00143730 File Offset: 0x00141930
		public static void ClientClear(DecalPoolType decalPoolType, int amount)
		{
			DecalPoolManager.DecalCollection[] collections = DecalPoolManager._singleton._collections;
			for (int i = 0; i < collections.Length; i++)
			{
				collections[i].Clear(decalPoolType, amount);
			}
		}

		// Token: 0x04004687 RID: 18055
		private static DecalPoolManager _singleton;

		// Token: 0x04004688 RID: 18056
		private static bool _singletonSet;

		// Token: 0x04004689 RID: 18057
		[SerializeField]
		private DecalPoolManager.DecalCollection[] _collections;

		// Token: 0x02001107 RID: 4359
		[Serializable]
		private class DecalCollection
		{
			// Token: 0x06006104 RID: 24836 RVA: 0x00143760 File Offset: 0x00141960
			public void Setup()
			{
				this._pools = new DecalPool[this._templates.Length];
				for (int i = 0; i < this._pools.Length; i++)
				{
					this._pools[i] = new DecalPool(this._templates[i]);
				}
				UserSetting<bool>.AddListener<PerformanceVideoSetting>(this._enabledSetting, new Action<bool>(this.OnSettingToggled));
				UserSetting<float>.AddListener<PerformanceVideoSetting>(this._limitSetting, new Action<float>(this.OnLimitChanged));
				this._decalsEnabled = UserSetting<bool>.Get<PerformanceVideoSetting>(this._enabledSetting);
				this._decalLimit = (int)UserSetting<float>.Get<PerformanceVideoSetting>(this._limitSetting);
			}

			// Token: 0x06006105 RID: 24837 RVA: 0x001437F9 File Offset: 0x001419F9
			public void UnlinkListeners()
			{
				UserSetting<bool>.RemoveListener<PerformanceVideoSetting>(this._enabledSetting, new Action<bool>(this.OnSettingToggled));
				UserSetting<float>.RemoveListener<PerformanceVideoSetting>(this._limitSetting, new Action<float>(this.OnLimitChanged));
			}

			// Token: 0x06006106 RID: 24838 RVA: 0x0014382C File Offset: 0x00141A2C
			public bool TryGet(DecalPoolType type, out Decal decal)
			{
				int i = 0;
				int num = 0;
				DecalPool decalPool = null;
				DecalPool decalPool2 = null;
				for (int j = 0; j < this._pools.Length; j++)
				{
					DecalPool decalPool3 = this._pools[j];
					int instances = decalPool3.Instances;
					i += instances;
					if (decalPool3.Type == type)
					{
						decalPool = decalPool3;
					}
					else if (instances > num)
					{
						decalPool2 = decalPool3;
						num = instances;
					}
				}
				if (decalPool == null)
				{
					decal = null;
					return false;
				}
				while (i >= this._decalLimit)
				{
					if (decalPool2 == null || decalPool2.Instances == 0)
					{
						decalPool.DisableLast();
					}
					else
					{
						decalPool2.DisableLast();
					}
					i--;
				}
				decal = decalPool.Get();
				return true;
			}

			// Token: 0x06006107 RID: 24839 RVA: 0x001438C4 File Offset: 0x00141AC4
			internal void Clear(DecalPoolType decalPoolType, int amount)
			{
				foreach (DecalPool decalPool in this._pools)
				{
					if (decalPool.Type == decalPoolType)
					{
						decalPool.SetLimit(Mathf.Max(decalPool.Instances - amount, 0));
					}
				}
			}

			// Token: 0x06006108 RID: 24840 RVA: 0x00143907 File Offset: 0x00141B07
			private void OnLimitChanged(float limit)
			{
				this._decalLimit = (int)limit;
				this.RefreshLimits();
			}

			// Token: 0x06006109 RID: 24841 RVA: 0x00143917 File Offset: 0x00141B17
			private void OnSettingToggled(bool status)
			{
				this._decalsEnabled = status;
				this.RefreshLimits();
			}

			// Token: 0x0600610A RID: 24842 RVA: 0x00143928 File Offset: 0x00141B28
			private void RefreshLimits()
			{
				int limit = this._decalsEnabled ? this._decalLimit : 0;
				this._pools.ForEach(delegate(DecalPool x)
				{
					x.SetLimit(limit);
				});
			}

			// Token: 0x0400468A RID: 18058
			[SerializeField]
			private Decal[] _templates;

			// Token: 0x0400468B RID: 18059
			[SerializeField]
			private PerformanceVideoSetting _enabledSetting;

			// Token: 0x0400468C RID: 18060
			[SerializeField]
			private PerformanceVideoSetting _limitSetting;

			// Token: 0x0400468D RID: 18061
			private DecalPool[] _pools;

			// Token: 0x0400468E RID: 18062
			private int _decalLimit;

			// Token: 0x0400468F RID: 18063
			private bool _decalsEnabled;
		}
	}
}
