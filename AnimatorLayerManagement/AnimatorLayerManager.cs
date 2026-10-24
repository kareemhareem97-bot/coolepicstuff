using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace AnimatorLayerManagement
{
	// Token: 0x02000AD0 RID: 2768
	public class AnimatorLayerManager : MonoBehaviour
	{
		// Token: 0x17000E1C RID: 3612
		// (get) Token: 0x06003E52 RID: 15954 RVA: 0x000CE56E File Offset: 0x000CC76E
		private Animator Anim
		{
			get
			{
				if (this._animFound)
				{
					return this._animCache;
				}
				this._animCache = base.GetComponent<Animator>();
				this._animFound = true;
				return this._animCache;
			}
		}

		// Token: 0x06003E53 RID: 15955 RVA: 0x000CE598 File Offset: 0x000CC798
		private void Awake()
		{
			if (this._rebuildOnAwake)
			{
				this.RebuildPairCache();
			}
		}

		// Token: 0x06003E54 RID: 15956 RVA: 0x000CE5A8 File Offset: 0x000CC7A8
		public void RebuildPairCache()
		{
			this._lookupCache.Clear();
			int layerCount = this.Anim.layerCount;
			if (this._pairs == null)
			{
				this._pairs = new List<RefIdIndexPair>(layerCount);
			}
			else
			{
				this._pairs.Clear();
				this._pairs.EnsureCapacity(layerCount);
			}
			for (int i = 0; i < layerCount; i++)
			{
				string text;
				LayerRefId refId;
				if (AnimatorLayerManager.TryUnpackFormat(this.Anim.GetLayerName(i), out text, out refId))
				{
					this._pairs.Add(new RefIdIndexPair(refId, i));
				}
			}
		}

		// Token: 0x06003E55 RID: 15957 RVA: 0x000CE630 File Offset: 0x000CC830
		public int GetLayerIndex(LayerRefId refId)
		{
			int result;
			if (this._lookupCache.TryGetValue(refId.Value, out result))
			{
				return result;
			}
			for (int i = 0; i < this._pairs.Count; i++)
			{
				RefIdIndexPair refIdIndexPair = this._pairs[i];
				if (refIdIndexPair.RefId.Value == refId.Value)
				{
					this._lookupCache[refId.Value] = refIdIndexPair.LayerIndex;
					return refIdIndexPair.LayerIndex;
				}
			}
			throw new InvalidOperationException(string.Format("{0} with value {1} not detected on the animator controller.", "LayerRefId", refId.Value));
		}

		// Token: 0x06003E56 RID: 15958 RVA: 0x000CE6C7 File Offset: 0x000CC8C7
		public float GetLayerWeight(LayerRefId refId)
		{
			return this.Anim.GetLayerWeight(this.GetLayerIndex(refId));
		}

		// Token: 0x06003E57 RID: 15959 RVA: 0x000CE6DB File Offset: 0x000CC8DB
		public void SetLayerWeight(LayerRefId refId, float weight)
		{
			this.Anim.SetLayerWeight(this.GetLayerIndex(refId), weight);
		}

		// Token: 0x06003E58 RID: 15960 RVA: 0x000CE6F0 File Offset: 0x000CC8F0
		public static bool TryUnpackFormat(string layerName, out string originalName, out LayerRefId refId)
		{
			Match match = AnimatorLayerManager.RegexUnpacker.Match(layerName);
			if (!match.Success)
			{
				originalName = null;
				refId = default(LayerRefId);
				return false;
			}
			originalName = match.Groups[1].Value;
			int refId2;
			if (int.TryParse(match.Groups[2].Value, out refId2))
			{
				refId = new LayerRefId(refId2);
				return true;
			}
			refId = default(LayerRefId);
			return false;
		}

		// Token: 0x06003E59 RID: 15961 RVA: 0x000CE760 File Offset: 0x000CC960
		public static string PackFormat(string originalName, LayerRefId refId)
		{
			return string.Format("{0} - (RefId={1})", originalName, refId.Value);
		}

		// Token: 0x04002DAD RID: 11693
		public const string LayerFormat = "{0} - (RefId={1})";

		// Token: 0x04002DAE RID: 11694
		public const string RegexPattern = "^(.+)\\s-\\s\\(RefId=(\\d+)\\)$";

		// Token: 0x04002DAF RID: 11695
		private static readonly Regex RegexUnpacker = new Regex("^(.+)\\s-\\s\\(RefId=(\\d+)\\)$");

		// Token: 0x04002DB0 RID: 11696
		[SerializeField]
		private bool _rebuildOnAwake;

		// Token: 0x04002DB1 RID: 11697
		[SerializeField]
		private List<RefIdIndexPair> _pairs;

		// Token: 0x04002DB2 RID: 11698
		private readonly Dictionary<int, int> _lookupCache = new Dictionary<int, int>();

		// Token: 0x04002DB3 RID: 11699
		private Animator _animCache;

		// Token: 0x04002DB4 RID: 11700
		private bool _animFound;
	}
}
