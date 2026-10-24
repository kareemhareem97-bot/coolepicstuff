using System;
using System.Collections.Generic;
using GameCore;
using MEC;
using Mirror;
using ToggleableMenus;
using UnityEngine;
using Utils.ConfigHandler;

// Token: 0x020000E4 RID: 228
public class PlayerList : SimpleToggleableMenu
{
	// Token: 0x170000BB RID: 187
	// (get) Token: 0x06000452 RID: 1106 RVA: 0x00010834 File Offset: 0x0000EA34
	// (set) Token: 0x06000453 RID: 1107 RVA: 0x00010858 File Offset: 0x0000EA58
	private static string ServerName
	{
		get
		{
			ServerConfigSynchronizer serverConfigSynchronizer = ServerConfigSynchronizer.Singleton;
			if (!(serverConfigSynchronizer == null))
			{
				return serverConfigSynchronizer.ServerName;
			}
			return null;
		}
		set
		{
			if (ServerConfigSynchronizer.Singleton == null)
			{
				return;
			}
			ServerConfigSynchronizer.Singleton.NetworkServerName = value;
		}
	}

	// Token: 0x06000454 RID: 1108 RVA: 0x00010873 File Offset: 0x0000EA73
	private void Update()
	{
		RectTransform component = base.GetComponent<RectTransform>();
		component.localPosition = Vector3.zero;
		component.sizeDelta = Vector2.zero;
	}

	// Token: 0x06000455 RID: 1109 RVA: 0x00010890 File Offset: 0x0000EA90
	private void Start()
	{
		PlayerList._anyAdminOnServer = false;
		if (!NetworkServer.active)
		{
			return;
		}
		ConfigFile.ServerConfig.UpdateConfigValue(PlayerList.RefreshRate);
		ConfigFile.ServerConfig.UpdateConfigValue(PlayerList.Title);
		Timing.RunCoroutine(this._RefreshTitleLoop(), Segment.FixedUpdate);
	}

	// Token: 0x06000456 RID: 1110 RVA: 0x000108CB File Offset: 0x0000EACB
	protected override void Awake()
	{
		base.Awake();
		PlayerList.instances.Clear();
		PlayerList.singleton = this;
		PlayerList.s_parent = this.parent;
		PlayerList.s_template = this.template;
	}

	// Token: 0x06000457 RID: 1111 RVA: 0x000108FC File Offset: 0x0000EAFC
	public static void UpdatePlayerNickname(ReferenceHub instance)
	{
		foreach (PlayerList.Instance instance2 in PlayerList.instances)
		{
			if (!(instance2.owner == null) && !(instance2.owner != instance))
			{
				ReferenceHub hub = ReferenceHub.GetHub(instance2.owner);
				if (instance2.listElementReference != null && hub != null)
				{
					instance2.listElementReference.TextNick.text = hub.nicknameSync.DisplayName;
					break;
				}
				Debug.LogWarning("UpdatePlayerNickname: PlayerList Instance either has a null list element or is updating for an unknown player.");
				break;
			}
		}
	}

	// Token: 0x06000458 RID: 1112 RVA: 0x000109B0 File Offset: 0x0000EBB0
	public static void UpdatePlayerRole(ReferenceHub instance)
	{
		PlayerList._anyAdminOnServer = false;
		bool flag = instance == null;
		foreach (PlayerList.Instance instance2 in PlayerList.instances)
		{
			try
			{
				if (instance2 != null)
				{
					if (!PlayerList._anyAdminOnServer && !string.IsNullOrEmpty(instance.serverRoles.GetUncoloredRoleString()))
					{
						PlayerList._anyAdminOnServer = true;
					}
					if (!flag)
					{
						instance != instance2.owner;
					}
				}
			}
			catch (Exception ex)
			{
				GameCore.Console.AddLog("Exception caught (UpdatePlayerRole in PlayerList): " + ex.Message, Color.red, false, ConsoleLogType.Log);
				Debug.LogError("Exception caught (UpdatePlayerRole in PlayerList): " + ex.Message);
			}
		}
	}

	// Token: 0x06000459 RID: 1113 RVA: 0x00010A80 File Offset: 0x0000EC80
	public void RefreshTitleSafe()
	{
		if (string.IsNullOrEmpty(PlayerList.Title.Value))
		{
			PlayerList.ServerName = ServerConsole.Singleton.RefreshServerNameSafe();
			return;
		}
		string text;
		if (!ServerConsole.Singleton.NameFormatter.TryProcessExpression(PlayerList.Title.Value, "player list title", out text))
		{
			ServerConsole.AddLog(text, ConsoleColor.Gray, false);
			return;
		}
		PlayerList.ServerName = text;
	}

	// Token: 0x0600045A RID: 1114 RVA: 0x00010ADF File Offset: 0x0000ECDF
	public void RefreshTitle()
	{
		PlayerList.ServerName = (string.IsNullOrEmpty(PlayerList.Title.Value) ? ServerConsole.Singleton.RefreshServerName() : ServerConsole.Singleton.NameFormatter.ProcessExpression(PlayerList.Title.Value));
	}

	// Token: 0x0600045B RID: 1115 RVA: 0x00010B1C File Offset: 0x0000ED1C
	private IEnumerator<float> _RefreshTitleLoop()
	{
		while (this != null)
		{
			this.RefreshTitleSafe();
			ushort i = 0;
			while ((float)i < 50f * PlayerList.RefreshRate.Value)
			{
				yield return 0f;
				ushort num = i;
				i = num + 1;
			}
		}
		yield break;
	}

	// Token: 0x040003CC RID: 972
	public static readonly ConfigEntry<float> RefreshRate = new ConfigEntry<float>("player_list_title_rate", 5f, "Player List Title Refresh Rate", "The amount of time (in seconds) between refreshing the title of the player list");

	// Token: 0x040003CD RID: 973
	public static readonly ConfigEntry<string> Title = new ConfigEntry<string>("player_list_title", null, "Player List Title", "The title at the top of the player list menu.");

	// Token: 0x040003CE RID: 974
	public Transform parent;

	// Token: 0x040003CF RID: 975
	public Transform template;

	// Token: 0x040003D0 RID: 976
	public GameObject mainPanel;

	// Token: 0x040003D1 RID: 977
	public GameObject reportForm;

	// Token: 0x040003D2 RID: 978
	public GameObject reportPopup;

	// Token: 0x040003D3 RID: 979
	public static InterfaceColorAdjuster ica;

	// Token: 0x040003D4 RID: 980
	public static PlayerList singleton;

	// Token: 0x040003D5 RID: 981
	private int _timer;

	// Token: 0x040003D6 RID: 982
	private static Transform s_parent;

	// Token: 0x040003D7 RID: 983
	private static Transform s_template;

	// Token: 0x040003D8 RID: 984
	private static bool _anyAdminOnServer;

	// Token: 0x040003D9 RID: 985
	public static readonly List<PlayerList.Instance> instances = new List<PlayerList.Instance>();

	// Token: 0x020000E5 RID: 229
	[Serializable]
	public class Instance
	{
		// Token: 0x040003DA RID: 986
		public ReferenceHub owner;

		// Token: 0x040003DB RID: 987
		public PlayerListElement listElementReference;
	}
}
