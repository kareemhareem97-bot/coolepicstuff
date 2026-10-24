using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using GameCore;
using NorthwoodLib.Pools;
using UnityEngine;

namespace Cmdbinding
{
	// Token: 0x02000780 RID: 1920
	public class CmdBinding : MonoBehaviour
	{
		// Token: 0x17000878 RID: 2168
		// (get) Token: 0x06002D0F RID: 11535 RVA: 0x00098A71 File Offset: 0x00096C71
		// (set) Token: 0x06002D10 RID: 11536 RVA: 0x00098A78 File Offset: 0x00096C78
		public static string FilePath { get; private set; } = CmdBinding.GeneratePath();

		// Token: 0x06002D11 RID: 11537 RVA: 0x00098A80 File Offset: 0x00096C80
		static CmdBinding()
		{
			CmdBinding.LoadFile();
		}

		// Token: 0x06002D12 RID: 11538 RVA: 0x00098A9C File Offset: 0x00096C9C
		public static void AddBinding(KeyCode key, string command, bool allowReplacing = true)
		{
			Bind bind = new Bind(key, command);
			int i = 0;
			while (i < CmdBinding.Bindings.Count)
			{
				if (CmdBinding.Bindings[i].Key == key)
				{
					if (!allowReplacing)
					{
						return;
					}
					CmdBinding.Bindings[i] = bind;
					return;
				}
				else
				{
					i++;
				}
			}
			CmdBinding.Bindings.Add(bind);
		}

		// Token: 0x06002D13 RID: 11539 RVA: 0x00098AF8 File Offset: 0x00096CF8
		public static void RemoveBinding(KeyCode key)
		{
			for (int i = 0; i < CmdBinding.Bindings.Count; i++)
			{
				if (CmdBinding.Bindings[i].Key == key)
				{
					CmdBinding.Bindings.RemoveAt(i);
					return;
				}
			}
		}

		// Token: 0x06002D14 RID: 11540 RVA: 0x00098B39 File Offset: 0x00096D39
		public static void RemoveAllBindings()
		{
			CmdBinding.Bindings.Clear();
			CmdBinding.SaveChanges();
		}

		// Token: 0x06002D15 RID: 11541 RVA: 0x00098B4C File Offset: 0x00096D4C
		public static bool TryGetBind(KeyCode key, out Bind bind)
		{
			for (int i = 0; i < CmdBinding.Bindings.Count; i++)
			{
				bind = CmdBinding.Bindings[i];
				if (bind.Key == key)
				{
					return true;
				}
			}
			bind = default(Bind);
			return false;
		}

		// Token: 0x06002D16 RID: 11542 RVA: 0x00098B94 File Offset: 0x00096D94
		public static void SaveChanges()
		{
			StringBuilder stringBuilder = StringBuilderPool.Shared.Rent();
			for (int i = 0; i < CmdBinding.Bindings.Count; i++)
			{
				Bind bind = CmdBinding.Bindings[i];
				stringBuilder.Append((int)bind.Key);
				stringBuilder.Append(':');
				stringBuilder.Append(bind.Command);
				if (i != CmdBinding.Bindings.Count - 1)
				{
					stringBuilder.AppendLine();
				}
			}
			StreamWriter streamWriter = new StreamWriter(CmdBinding.FilePath);
			streamWriter.WriteLine(StringBuilderPool.Shared.ToStringReturn(stringBuilder));
			streamWriter.Close();
		}

		// Token: 0x06002D17 RID: 11543 RVA: 0x00098C28 File Offset: 0x00096E28
		public static void LoadFile()
		{
			GameCore.Console.AddLog("Loading cmd bindings...", Color.grey, false, ConsoleLogType.Log);
			try
			{
				CmdBinding.Bindings.Clear();
				if (!File.Exists(CmdBinding.FilePath))
				{
					CmdBinding.ResetBindings();
				}
				StreamReader streamReader = new StreamReader(CmdBinding.FilePath);
				string text;
				while ((text = streamReader.ReadLine()) != null)
				{
					if (!string.IsNullOrEmpty(text) && text.Contains(':'))
					{
						string[] array = text.Split(':', StringSplitOptions.None);
						int key;
						if (int.TryParse(array[0], out key))
						{
							Bind item = new Bind((KeyCode)key, array[1]);
							CmdBinding.Bindings.Add(item);
						}
					}
				}
				streamReader.Close();
			}
			catch (Exception ex)
			{
				Debug.Log("Error occurred whilst loading Cmdbindings: " + ex.StackTrace + " - " + ex.Message);
			}
		}

		// Token: 0x06002D18 RID: 11544 RVA: 0x00002E9A File Offset: 0x0000109A
		[Obsolete("This has now been made obsolete by the SSS system.")]
		internal static void SynchronizeKeybind(KeyCode code, string cmd)
		{
		}

		// Token: 0x06002D19 RID: 11545 RVA: 0x00098CF8 File Offset: 0x00096EF8
		private static void ResetBindings()
		{
			Debug.Log("Resetting Cmdbindings!");
			new StreamWriter(CmdBinding.FilePath).Close();
		}

		// Token: 0x06002D1A RID: 11546 RVA: 0x00098D13 File Offset: 0x00096F13
		private static string GeneratePath()
		{
			return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SCP Secret Laboratory/cmdbinding.txt");
		}

		// Token: 0x06002D1B RID: 11547 RVA: 0x00098D28 File Offset: 0x00096F28
		private void Update()
		{
			if (CmdBinding.Bindings.Count <= 0)
			{
				return;
			}
			if (GameCore.Console.Singleton == null)
			{
				return;
			}
			foreach (Bind bind in CmdBinding.Bindings)
			{
				if (Input.GetKeyDown(bind.Key))
				{
					GameCore.Console.Singleton.TypeCommand(bind.Command, null);
				}
			}
		}

		// Token: 0x04002322 RID: 8994
		private const string CmdbindingFileName = "SCP Secret Laboratory/cmdbinding.txt";

		// Token: 0x04002323 RID: 8995
		public static readonly List<Bind> Bindings = new List<Bind>();
	}
}
