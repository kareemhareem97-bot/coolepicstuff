using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using GameCore;
using NorthwoodLib.Pools;
using Scp914;
using Utils.ConfigHandler;

// Token: 0x0200022C RID: 556
public class YamlConfig : ConfigRegister
{
	// Token: 0x17000172 RID: 370
	// (get) Token: 0x06000C5D RID: 3165 RVA: 0x00030764 File Offset: 0x0002E964
	// (set) Token: 0x06000C5E RID: 3166 RVA: 0x0003077B File Offset: 0x0002E97B
	public string[] RawData
	{
		get
		{
			if (!this._virtual)
			{
				return this._rawData;
			}
			return this._rawDataUnfiltered;
		}
		set
		{
			if (this._virtual)
			{
				this._rawDataUnfiltered = value;
				return;
			}
			this._rawData = value;
		}
	}

	// Token: 0x17000173 RID: 371
	// (get) Token: 0x06000C5F RID: 3167 RVA: 0x00030794 File Offset: 0x0002E994
	// (set) Token: 0x06000C60 RID: 3168 RVA: 0x0003079C File Offset: 0x0002E99C
	public bool IsVirtual
	{
		get
		{
			return this._virtual;
		}
		set
		{
			if (!value || this._virtual)
			{
				return;
			}
			this._virtual = true;
			this._rawDataUnfiltered = this.RawData;
		}
	}

	// Token: 0x06000C61 RID: 3169 RVA: 0x000307BD File Offset: 0x0002E9BD
	public YamlConfig()
	{
		this.RawData = new string[0];
	}

	// Token: 0x06000C62 RID: 3170 RVA: 0x000307D1 File Offset: 0x0002E9D1
	public YamlConfig(string path)
	{
		this.Path = path;
		this.LoadConfigFile(path);
	}

	// Token: 0x06000C63 RID: 3171 RVA: 0x000307E8 File Offset: 0x0002E9E8
	public override void UpdateConfigValue(ConfigEntry configEntry)
	{
		if (configEntry == null)
		{
			throw new NullReferenceException("Config type unsupported (Config: Null).");
		}
		ConfigEntry<bool> configEntry2 = configEntry as ConfigEntry<bool>;
		if (configEntry2 != null)
		{
			configEntry2.Value = this.GetBool(configEntry2.Key, configEntry2.Default);
			return;
		}
		ConfigEntry<byte> configEntry3 = configEntry as ConfigEntry<byte>;
		if (configEntry3 != null)
		{
			configEntry3.Value = this.GetByte(configEntry3.Key, configEntry3.Default);
			return;
		}
		ConfigEntry<char> configEntry4 = configEntry as ConfigEntry<char>;
		if (configEntry4 != null)
		{
			configEntry4.Value = this.GetChar(configEntry4.Key, configEntry4.Default);
			return;
		}
		ConfigEntry<decimal> configEntry5 = configEntry as ConfigEntry<decimal>;
		if (configEntry5 != null)
		{
			configEntry5.Value = this.GetDecimal(configEntry5.Key, configEntry5.Default);
			return;
		}
		ConfigEntry<double> configEntry6 = configEntry as ConfigEntry<double>;
		if (configEntry6 != null)
		{
			configEntry6.Value = this.GetDouble(configEntry6.Key, configEntry6.Default);
			return;
		}
		ConfigEntry<float> configEntry7 = configEntry as ConfigEntry<float>;
		if (configEntry7 != null)
		{
			configEntry7.Value = this.GetFloat(configEntry7.Key, configEntry7.Default);
			return;
		}
		ConfigEntry<int> configEntry8 = configEntry as ConfigEntry<int>;
		if (configEntry8 != null)
		{
			configEntry8.Value = this.GetInt(configEntry8.Key, configEntry8.Default);
			return;
		}
		ConfigEntry<long> configEntry9 = configEntry as ConfigEntry<long>;
		if (configEntry9 != null)
		{
			configEntry9.Value = this.GetLong(configEntry9.Key, configEntry9.Default);
			return;
		}
		ConfigEntry<sbyte> configEntry10 = configEntry as ConfigEntry<sbyte>;
		if (configEntry10 != null)
		{
			configEntry10.Value = this.GetSByte(configEntry10.Key, configEntry10.Default);
			return;
		}
		ConfigEntry<short> configEntry11 = configEntry as ConfigEntry<short>;
		if (configEntry11 != null)
		{
			configEntry11.Value = this.GetShort(configEntry11.Key, configEntry11.Default);
			return;
		}
		ConfigEntry<string> configEntry12 = configEntry as ConfigEntry<string>;
		if (configEntry12 != null)
		{
			configEntry12.Value = this.GetString(configEntry12.Key, configEntry12.Default);
			return;
		}
		ConfigEntry<uint> configEntry13 = configEntry as ConfigEntry<uint>;
		if (configEntry13 != null)
		{
			configEntry13.Value = this.GetUInt(configEntry13.Key, configEntry13.Default);
			return;
		}
		ConfigEntry<ulong> configEntry14 = configEntry as ConfigEntry<ulong>;
		if (configEntry14 != null)
		{
			configEntry14.Value = this.GetULong(configEntry14.Key, configEntry14.Default);
			return;
		}
		ConfigEntry<ushort> configEntry15 = configEntry as ConfigEntry<ushort>;
		if (configEntry15 == null)
		{
			ConfigEntry<List<bool>> configEntry16 = configEntry as ConfigEntry<List<bool>>;
			if (configEntry16 == null)
			{
				ConfigEntry<List<byte>> configEntry17 = configEntry as ConfigEntry<List<byte>>;
				if (configEntry17 == null)
				{
					ConfigEntry<List<char>> configEntry18 = configEntry as ConfigEntry<List<char>>;
					if (configEntry18 == null)
					{
						ConfigEntry<List<decimal>> configEntry19 = configEntry as ConfigEntry<List<decimal>>;
						if (configEntry19 == null)
						{
							ConfigEntry<List<double>> configEntry20 = configEntry as ConfigEntry<List<double>>;
							if (configEntry20 == null)
							{
								ConfigEntry<List<float>> configEntry21 = configEntry as ConfigEntry<List<float>>;
								if (configEntry21 == null)
								{
									ConfigEntry<List<int>> configEntry22 = configEntry as ConfigEntry<List<int>>;
									if (configEntry22 == null)
									{
										ConfigEntry<List<long>> configEntry23 = configEntry as ConfigEntry<List<long>>;
										if (configEntry23 == null)
										{
											ConfigEntry<List<sbyte>> configEntry24 = configEntry as ConfigEntry<List<sbyte>>;
											if (configEntry24 == null)
											{
												ConfigEntry<List<short>> configEntry25 = configEntry as ConfigEntry<List<short>>;
												if (configEntry25 == null)
												{
													ConfigEntry<List<string>> configEntry26 = configEntry as ConfigEntry<List<string>>;
													if (configEntry26 == null)
													{
														ConfigEntry<List<uint>> configEntry27 = configEntry as ConfigEntry<List<uint>>;
														if (configEntry27 == null)
														{
															ConfigEntry<List<ulong>> configEntry28 = configEntry as ConfigEntry<List<ulong>>;
															if (configEntry28 == null)
															{
																ConfigEntry<List<ushort>> configEntry29 = configEntry as ConfigEntry<List<ushort>>;
																if (configEntry29 == null)
																{
																	ConfigEntry<Dictionary<string, string>> configEntry30 = configEntry as ConfigEntry<Dictionary<string, string>>;
																	if (configEntry30 == null)
																	{
																		ConfigEntry<Scp914Mode> configEntry31 = configEntry as ConfigEntry<Scp914Mode>;
																		if (configEntry31 == null)
																		{
																			throw new Exception(string.Concat(new string[]
																			{
																				"Config type unsupported (Config: Key = \"",
																				configEntry.Key ?? "Null",
																				"\" Type = \"",
																				configEntry.ValueType.FullName ?? "Null",
																				"\" Name = \"",
																				configEntry.Name ?? "Null",
																				"\" Description = \"",
																				configEntry.Description ?? "Null",
																				"\")."
																			}));
																		}
																		string @string = this.GetString(configEntry31.Key, "");
																		Scp914Mode value;
																		if (@string == "default" || !Enum.TryParse<Scp914Mode>(@string, out value))
																		{
																			configEntry31.Value = configEntry31.Default;
																			return;
																		}
																		configEntry31.Value = value;
																		return;
																	}
																	else
																	{
																		configEntry30.Value = this.GetStringDictionary(configEntry30.Key);
																		if (configEntry30.Value.Count <= 0 && string.Equals(this.GetRawString(configEntry30.Key), "default", StringComparison.OrdinalIgnoreCase))
																		{
																			configEntry30.Value = configEntry30.Default;
																			return;
																		}
																	}
																}
																else
																{
																	configEntry29.Value = this.GetUShortList(configEntry29.Key);
																	if (configEntry29.Value.Count <= 0 && string.Equals(this.GetRawString(configEntry29.Key), "default", StringComparison.OrdinalIgnoreCase))
																	{
																		configEntry29.Value = configEntry29.Default;
																		return;
																	}
																}
															}
															else
															{
																configEntry28.Value = this.GetULongList(configEntry28.Key);
																if (configEntry28.Value.Count <= 0 && string.Equals(this.GetRawString(configEntry28.Key), "default", StringComparison.OrdinalIgnoreCase))
																{
																	configEntry28.Value = configEntry28.Default;
																	return;
																}
															}
														}
														else
														{
															configEntry27.Value = this.GetUIntList(configEntry27.Key);
															if (configEntry27.Value.Count <= 0 && string.Equals(this.GetRawString(configEntry27.Key), "default", StringComparison.OrdinalIgnoreCase))
															{
																configEntry27.Value = configEntry27.Default;
																return;
															}
														}
													}
													else
													{
														configEntry26.Value = this.GetStringList(configEntry26.Key);
														if (configEntry26.Value.Count <= 0 && string.Equals(this.GetRawString(configEntry26.Key), "default", StringComparison.OrdinalIgnoreCase))
														{
															configEntry26.Value = configEntry26.Default;
															return;
														}
													}
												}
												else
												{
													configEntry25.Value = this.GetShortList(configEntry25.Key);
													if (configEntry25.Value.Count <= 0 && string.Equals(this.GetRawString(configEntry25.Key), "default", StringComparison.OrdinalIgnoreCase))
													{
														configEntry25.Value = configEntry25.Default;
														return;
													}
												}
											}
											else
											{
												configEntry24.Value = this.GetSByteList(configEntry24.Key);
												if (configEntry24.Value.Count <= 0 && string.Equals(this.GetRawString(configEntry24.Key), "default", StringComparison.OrdinalIgnoreCase))
												{
													configEntry24.Value = configEntry24.Default;
													return;
												}
											}
										}
										else
										{
											configEntry23.Value = this.GetLongList(configEntry23.Key);
											if (configEntry23.Value.Count <= 0 && string.Equals(this.GetRawString(configEntry23.Key), "default", StringComparison.OrdinalIgnoreCase))
											{
												configEntry23.Value = configEntry23.Default;
												return;
											}
										}
									}
									else
									{
										configEntry22.Value = this.GetIntList(configEntry22.Key);
										if (configEntry22.Value.Count <= 0 && string.Equals(this.GetRawString(configEntry22.Key), "default", StringComparison.OrdinalIgnoreCase))
										{
											configEntry22.Value = configEntry22.Default;
											return;
										}
									}
								}
								else
								{
									configEntry21.Value = this.GetFloatList(configEntry21.Key);
									if (configEntry21.Value.Count <= 0 && string.Equals(this.GetRawString(configEntry21.Key), "default", StringComparison.OrdinalIgnoreCase))
									{
										configEntry21.Value = configEntry21.Default;
										return;
									}
								}
							}
							else
							{
								configEntry20.Value = this.GetDoubleList(configEntry20.Key);
								if (configEntry20.Value.Count <= 0 && string.Equals(this.GetRawString(configEntry20.Key), "default", StringComparison.OrdinalIgnoreCase))
								{
									configEntry20.Value = configEntry20.Default;
									return;
								}
							}
						}
						else
						{
							configEntry19.Value = this.GetDecimalList(configEntry19.Key);
							if (configEntry19.Value.Count <= 0 && string.Equals(this.GetRawString(configEntry19.Key), "default", StringComparison.OrdinalIgnoreCase))
							{
								configEntry19.Value = configEntry19.Default;
								return;
							}
						}
					}
					else
					{
						configEntry18.Value = this.GetCharList(configEntry18.Key);
						if (configEntry18.Value.Count <= 0 && string.Equals(this.GetRawString(configEntry18.Key), "default", StringComparison.OrdinalIgnoreCase))
						{
							configEntry18.Value = configEntry18.Default;
							return;
						}
					}
				}
				else
				{
					configEntry17.Value = this.GetByteList(configEntry17.Key);
					if (configEntry17.Value.Count <= 0 && string.Equals(this.GetRawString(configEntry17.Key), "default", StringComparison.OrdinalIgnoreCase))
					{
						configEntry17.Value = configEntry17.Default;
						return;
					}
				}
			}
			else
			{
				configEntry16.Value = this.GetBoolList(configEntry16.Key);
				if (configEntry16.Value.Count <= 0 && string.Equals(this.GetRawString(configEntry16.Key), "default", StringComparison.OrdinalIgnoreCase))
				{
					configEntry16.Value = configEntry16.Default;
					return;
				}
			}
			return;
		}
		configEntry15.Value = this.GetUShort(configEntry15.Key, configEntry15.Default);
	}

	// Token: 0x06000C64 RID: 3172 RVA: 0x000310D9 File Offset: 0x0002F2D9
	private static string[] Filter(IEnumerable<string> lines)
	{
		return (from line in lines
		where !string.IsNullOrEmpty(line) && !line.StartsWith("#") && (line.StartsWith(" - ") || line.Contains(':'))
		select line).ToArray<string>();
	}

	// Token: 0x06000C65 RID: 3173 RVA: 0x00031108 File Offset: 0x0002F308
	public void LoadConfigFile(string path)
	{
		if (string.IsNullOrEmpty(path))
		{
			return;
		}
		this.Path = path;
		if (!ServerStatic.DisableConfigValidation)
		{
			YamlConfig.RemoveInvalid(path);
		}
		if (!ServerStatic.DisableConfigValidation && this.Path.EndsWith("config_gameplay.txt") && !this._afteradding && FileManager.FileExists("ConfigTemplates/config_gameplay.template.txt"))
		{
			YamlConfig.AddMissingTemplateKeys("ConfigTemplates/config_gameplay.template.txt", path, ref this._afteradding);
		}
		else if (!ServerStatic.DisableConfigValidation && this.Path.EndsWith("config_remoteadmin.txt") && !this._afteradding)
		{
			YamlConfig.AddMissingRoleVars(path);
			YamlConfig.AddMissingPerms(path, ref this._afteradding);
		}
		this._rawDataUnfiltered = FileManager.ReadAllLines(path);
		this.RawData = YamlConfig.Filter(this._rawDataUnfiltered);
		base.UpdateRegisteredConfigValues();
	}

	// Token: 0x06000C66 RID: 3174 RVA: 0x000311C8 File Offset: 0x0002F3C8
	private static void RemoveDeprecated(string path)
	{
		List<string> list = FileManager.ReadAllLinesList(path);
		for (int i = list.Count - 1; i >= 0; i--)
		{
			for (int j = 0; j < YamlConfig._deprecatedconfigs.Length; j++)
			{
				if (list[i].StartsWith(YamlConfig._deprecatedconfigs[j] + ":") && (i == 0 || list[i - 1] != "#REMOVED FROM GAME - REDUNDANT"))
				{
					list.Insert(i, "#REMOVED FROM GAME - REDUNDANT");
				}
			}
		}
		FileManager.WriteToFile(list, path, false);
	}

	// Token: 0x06000C67 RID: 3175 RVA: 0x0003124C File Offset: 0x0002F44C
	private static void AddMissingPerms(string path, ref bool _afteradding)
	{
		string[] perms = YamlConfig.GetStringList("Permissions", path).ToArray();
		string[] array = EnumUtils<PlayerPermissions>.Names.ToArray<string>();
		if (perms.Length == array.Length)
		{
			return;
		}
		List<string> collection = (from permtype in array
		select new
		{
			permtype = permtype,
			inconfig = perms.Any((string perm) => perm.StartsWith(permtype))
		} into t
		where !t.inconfig
		select " - " + t.permtype + ": []").ToList<string>();
		List<string> list = FileManager.ReadAllLinesList(path);
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i] == "Permissions:")
			{
				list.InsertRange(i + 1, collection);
			}
		}
		FileManager.WriteToFile(list, path, false);
		_afteradding = true;
	}

	// Token: 0x06000C68 RID: 3176 RVA: 0x00031334 File Offset: 0x0002F534
	private static void AddMissingRoleVars(string path)
	{
		string text = TimeBehaviour.FormatTime("yyyy/MM/dd HH:mm:ss");
		List<string> stringList = YamlConfig.GetStringList("Roles", path);
		List<string> list = ListPool<string>.Shared.Rent();
		string config = FileManager.ReadAllText(path);
		using (List<string>.Enumerator enumerator = stringList.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				string role = enumerator.Current;
				list.AddRange(from rolevar in YamlConfig._rolevars
				where !config.Contains(role + "_" + rolevar + ":")
				select role + "_" + rolevar + ": default");
			}
		}
		if (list.Count > 0)
		{
			YamlConfig.Write(list, path, ref text);
		}
		ListPool<string>.Shared.Return(list);
	}

	// Token: 0x06000C69 RID: 3177 RVA: 0x00031410 File Offset: 0x0002F610
	private static void AddMissingTemplateKeys(string templatepath, string path, ref bool _afteradding)
	{
		string text = TimeBehaviour.FormatTime("yyyy/MM/dd HH:mm:ss");
		string text2 = FileManager.ReadAllText(path);
		string[] array = FileManager.ReadAllLines(templatepath);
		List<string> list = ListPool<string>.Shared.Rent();
		List<string> list2 = ListPool<string>.Shared.Rent();
		List<string> list3 = ListPool<string>.Shared.Rent();
		for (int i = 0; i < array.Length; i++)
		{
			if (!array[i].StartsWith("#") && !array[i].StartsWith(" -") && array[i].Contains(":") && ((i + 1 < array.Length && array[i + 1].StartsWith(" -")) || array[i].EndsWith("[]")))
			{
				list.Add(array[i]);
			}
			else if (!array[i].StartsWith("#") && array[i].Contains(":") && !array[i].StartsWith(" -"))
			{
				list2.Add(array[i].Substring(0, array[i].IndexOf(':') + 1));
			}
		}
		foreach (string text3 in list2)
		{
			if (!text2.Contains(text3))
			{
				list3.Add(text3 + " default");
			}
		}
		ListPool<string>.Shared.Return(list2);
		YamlConfig.Write(list3, path, ref text);
		ListPool<string>.Shared.Return(list3);
		foreach (string text4 in list)
		{
			if (!text2.Contains(text4))
			{
				bool flag = false;
				List<string> list4 = new List<string>
				{
					"#LIST",
					text4
				};
				foreach (string text5 in array)
				{
					if (text5.StartsWith(text4) && text5.EndsWith("[]"))
					{
						list4.Clear();
						list4.AddRange(new string[]
						{
							"#LIST - [] equals to empty",
							text5
						});
						break;
					}
					if (text5.StartsWith(text4))
					{
						flag = true;
					}
					else if (flag)
					{
						if (text5.StartsWith(" - "))
						{
							list4.Add(text5);
						}
						else if (!text5.StartsWith("#"))
						{
							break;
						}
					}
				}
				YamlConfig.Write(list4, path, ref text);
			}
		}
		ListPool<string>.Shared.Return(list);
		_afteradding = true;
	}

	// Token: 0x06000C6A RID: 3178 RVA: 0x000316BC File Offset: 0x0002F8BC
	private static void Write(IEnumerable<string> text, string path, ref string time)
	{
		string[] array = text.ToArray<string>();
		if (array.Length != 0)
		{
			YamlConfig.Write(string.Join("\r\n", array), path, ref time);
		}
	}

	// Token: 0x06000C6B RID: 3179 RVA: 0x000316E8 File Offset: 0x0002F8E8
	private static void Write(string text, string path, ref string time)
	{
		using (StreamWriter streamWriter = File.AppendText(path))
		{
			streamWriter.Write(string.Concat(new string[]
			{
				"\r\n\r\n#ADDED BY CONFIG VALIDATOR - ",
				time,
				" Game version: ",
				GameCore.Version.VersionString,
				"\r\n",
				text
			}));
		}
	}

	// Token: 0x06000C6C RID: 3180 RVA: 0x00031754 File Offset: 0x0002F954
	private static void RemoveInvalid(string path)
	{
		string[] array = FileManager.ReadAllLines(path);
		bool flag = false;
		for (int i = 0; i < array.Length; i++)
		{
			if (!array[i].StartsWith("#") && !array[i].StartsWith(" -") && !array[i].Contains(":") && !string.IsNullOrEmpty(array[i].Replace(" ", "")))
			{
				flag = true;
				array[i] = "#INVALID - " + array[i];
			}
		}
		if (flag)
		{
			FileManager.WriteToFile(array, path, false);
		}
	}

	// Token: 0x06000C6D RID: 3181 RVA: 0x000317DC File Offset: 0x0002F9DC
	private void CommentInvalid(string key, string type)
	{
		if (this.IsVirtual)
		{
			return;
		}
		for (int i = 0; i < this._rawDataUnfiltered.Length; i++)
		{
			if (this._rawDataUnfiltered[i].StartsWith(key + ": ", StringComparison.Ordinal))
			{
				this._rawDataUnfiltered[i] = "#INVALID " + type + " - " + this._rawDataUnfiltered[i];
			}
		}
		if (!ServerStatic.DisableConfigValidation)
		{
			FileManager.WriteToFile(this._rawDataUnfiltered, this.Path, false);
		}
	}

	// Token: 0x06000C6E RID: 3182 RVA: 0x00031859 File Offset: 0x0002FA59
	public bool Reload()
	{
		if (this.IsVirtual)
		{
			return false;
		}
		if (string.IsNullOrEmpty(this.Path))
		{
			return false;
		}
		this.LoadConfigFile(this.Path);
		return true;
	}

	// Token: 0x06000C6F RID: 3183 RVA: 0x00031884 File Offset: 0x0002FA84
	public bool TryGetString(string key, out string value)
	{
		foreach (string text in this.RawData)
		{
			if (text.StartsWith(key + ": "))
			{
				value = text.Substring(key.Length + 2);
				return true;
			}
		}
		value = "default";
		return false;
	}

	// Token: 0x06000C70 RID: 3184 RVA: 0x000318D8 File Offset: 0x0002FAD8
	private string GetRawString(string key)
	{
		string result;
		if (!this.TryGetString(key, out result))
		{
			return "default";
		}
		return result;
	}

	// Token: 0x06000C71 RID: 3185 RVA: 0x000318F8 File Offset: 0x0002FAF8
	public void SetString(string key, string value = null)
	{
		this.Reload();
		int num = 0;
		List<string> list = null;
		int i = 0;
		while (i < this._rawDataUnfiltered.Length)
		{
			if (this._rawDataUnfiltered[i].StartsWith(key + ": "))
			{
				if (value == null)
				{
					list = this._rawDataUnfiltered.ToList<string>();
					list.RemoveAt(i);
					num = 2;
					break;
				}
				this._rawDataUnfiltered[i] = key + ": " + value;
				num = 1;
				break;
			}
			else
			{
				i++;
			}
		}
		if (this.IsVirtual)
		{
			return;
		}
		switch (num)
		{
		case 0:
			list = this._rawDataUnfiltered.ToList<string>();
			list.Insert(list.Count, key + ": " + value);
			FileManager.WriteToFile(list, this.Path, false);
			break;
		case 1:
			FileManager.WriteToFile(this._rawDataUnfiltered, this.Path, false);
			break;
		case 2:
			if (list != null)
			{
				FileManager.WriteToFile(list, this.Path, false);
			}
			break;
		}
		this.Reload();
	}

	// Token: 0x06000C72 RID: 3186 RVA: 0x000319EC File Offset: 0x0002FBEC
	private static List<string> GetStringList(string key, string path)
	{
		bool flag = false;
		List<string> list = new List<string>();
		foreach (string text in FileManager.ReadAllLines(path))
		{
			if (text.StartsWith(key) && text.EndsWith("[]"))
			{
				break;
			}
			if (text.StartsWith(key + ":"))
			{
				string text2 = text.Substring(key.Length + 1);
				if (text2.Contains("[") && text2.Contains("]"))
				{
					return YamlConfig.ParseCommaSeparatedString(text2).ToList<string>();
				}
				flag = true;
			}
			else if (flag)
			{
				if (text.StartsWith(" - "))
				{
					list.Add(text.Substring(3));
				}
				else if (!text.StartsWith("#"))
				{
					break;
				}
			}
		}
		return list;
	}

	// Token: 0x06000C73 RID: 3187 RVA: 0x00031AC0 File Offset: 0x0002FCC0
	public void SetStringListItem(string key, string value, string newValue)
	{
		this.Reload();
		bool flag = false;
		int num = 0;
		List<string> list = null;
		for (int i = 0; i < this._rawDataUnfiltered.Length; i++)
		{
			string text = this._rawDataUnfiltered[i];
			if (text.StartsWith(key + ":"))
			{
				flag = true;
			}
			else if (flag)
			{
				if (value != null && text == " - " + value)
				{
					if (newValue == null)
					{
						list = this._rawDataUnfiltered.ToList<string>();
						list.RemoveAt(i);
						num = 2;
						break;
					}
					this._rawDataUnfiltered[i] = " - " + newValue;
					num = 1;
					break;
				}
				else if (!text.StartsWith(" - ") && !text.StartsWith("#"))
				{
					if (value != null)
					{
						list = this._rawDataUnfiltered.ToList<string>();
						list.Insert(i, " - " + newValue);
						num = 2;
						break;
					}
					break;
				}
			}
		}
		if (this.IsVirtual)
		{
			return;
		}
		int num2 = num;
		if (num2 != 1)
		{
			if (num2 == 2)
			{
				if (list != null)
				{
					FileManager.WriteToFile(list, this.Path, false);
				}
			}
		}
		else
		{
			FileManager.WriteToFile(this._rawDataUnfiltered, this.Path, false);
		}
		this.Reload();
	}

	// Token: 0x06000C74 RID: 3188 RVA: 0x00031BEA File Offset: 0x0002FDEA
	public IEnumerable<string> StringListToText(string key, List<string> list)
	{
		yield return key + ":";
		foreach (string str in list)
		{
			yield return " - " + str;
		}
		List<string>.Enumerator enumerator = default(List<string>.Enumerator);
		yield break;
		yield break;
	}

	// Token: 0x06000C75 RID: 3189 RVA: 0x00031C04 File Offset: 0x0002FE04
	public Dictionary<string, string> GetStringDictionary(string key)
	{
		this.GetStringCollection(key, YamlConfig.DataBuffer);
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		foreach (string text in YamlConfig.DataBuffer)
		{
			if (!text.Contains(": "))
			{
				ServerConsole.AddLog(string.Concat(new string[]
				{
					"Invalid entry \"",
					text,
					"\" in dictionary ",
					key,
					" in the config file - missing \": \"."
				}), ConsoleColor.Red, false);
			}
			else
			{
				int num = text.IndexOf(": ", StringComparison.Ordinal);
				string text2 = text.Substring(0, num);
				if (!dictionary.ContainsKey(text2))
				{
					dictionary.Add(text2, text.Substring(num + 2));
				}
				else
				{
					ServerConsole.AddLog(string.Concat(new string[]
					{
						"Ignoring duplicated subkey ",
						text2,
						" in dictionary ",
						key,
						" in the config file."
					}), ConsoleColor.Yellow, false);
				}
			}
		}
		return dictionary;
	}

	// Token: 0x06000C76 RID: 3190 RVA: 0x00031D14 File Offset: 0x0002FF14
	public void SetStringDictionaryItem(string key, string subkey, string value)
	{
		this.Reload();
		bool flag = false;
		int num = 0;
		List<string> list = null;
		for (int i = 0; i < this._rawDataUnfiltered.Length; i++)
		{
			string text = this._rawDataUnfiltered[i];
			if (text.StartsWith(key + ":"))
			{
				flag = true;
			}
			else if (flag)
			{
				if (text.StartsWith(" - " + subkey + ": "))
				{
					if (value == null)
					{
						list = this._rawDataUnfiltered.ToList<string>();
						list.RemoveAt(i);
						num = 2;
						break;
					}
					this._rawDataUnfiltered[i] = " - " + subkey + ": " + value;
					num = 1;
					break;
				}
				else if (!text.StartsWith(" - ") && !text.StartsWith("#"))
				{
					if (value != null)
					{
						list = this._rawDataUnfiltered.ToList<string>();
						list.Insert(i, " - " + subkey + ": " + value);
						num = 2;
						break;
					}
					break;
				}
			}
		}
		if (this.IsVirtual)
		{
			return;
		}
		switch (num)
		{
		case 0:
			list = this._rawDataUnfiltered.ToList<string>();
			list.Insert(list.Count, " - " + subkey + ": " + value);
			FileManager.WriteToFile(list, this.Path, false);
			break;
		case 1:
			FileManager.WriteToFile(this._rawDataUnfiltered, this.Path, false);
			break;
		case 2:
			if (list != null)
			{
				FileManager.WriteToFile(list, this.Path, false);
			}
			break;
		}
		this.Reload();
	}

	// Token: 0x06000C77 RID: 3191 RVA: 0x00031E90 File Offset: 0x00030090
	public static string[] ParseCommaSeparatedString(string data)
	{
		data = data.Trim();
		if (!data.StartsWith("[", StringComparison.Ordinal) || !data.EndsWith("]", StringComparison.Ordinal))
		{
			return null;
		}
		data = data.Substring(1, data.Length - 2).Replace(", ", ",");
		return data.Split(',', StringSplitOptions.None);
	}

	// Token: 0x06000C78 RID: 3192 RVA: 0x00031EEC File Offset: 0x000300EC
	public IEnumerable<string> GetKeys()
	{
		return from line in this.RawData
		where line.Contains(":")
		select line.Split(':', StringSplitOptions.None)[0];
	}

	// Token: 0x06000C79 RID: 3193 RVA: 0x00031F48 File Offset: 0x00030148
	public bool IsList(string key)
	{
		bool flag = false;
		foreach (string text in this.RawData)
		{
			if (text.StartsWith(key + ":"))
			{
				flag = true;
			}
			else if (flag)
			{
				if (text.StartsWith(" - "))
				{
					return true;
				}
				if (!text.StartsWith("#"))
				{
					break;
				}
			}
		}
		return false;
	}

	// Token: 0x06000C7A RID: 3194 RVA: 0x00031FA8 File Offset: 0x000301A8
	public void Merge(ref YamlConfig toMerge)
	{
		string[] array = this.GetKeys().ToArray<string>();
		this.IsVirtual = true;
		foreach (string text in toMerge.GetKeys())
		{
			if (!array.Contains(text))
			{
				if (toMerge.IsList(text))
				{
					using (IEnumerator<string> enumerator2 = toMerge.StringListToText(text, toMerge.GetStringList(text)).GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							string element = enumerator2.Current;
							this.RawData.Append(element);
						}
						continue;
					}
				}
				this.SetString(text, toMerge.GetRawString(text));
			}
		}
	}

	// Token: 0x06000C7B RID: 3195 RVA: 0x00032074 File Offset: 0x00030274
	public bool GetBool(string key, bool def = false)
	{
		string text = this.GetRawString(key).ToLower();
		if (text == "default")
		{
			return def;
		}
		bool result;
		if (bool.TryParse(text, out result))
		{
			return result;
		}
		ServerConsole.AddLog(key + " has invalid value, " + text + " is not a valid bool!", ConsoleColor.Gray, false);
		this.CommentInvalid(key, "BOOL");
		return def;
	}

	// Token: 0x06000C7C RID: 3196 RVA: 0x000320D0 File Offset: 0x000302D0
	public byte GetByte(string key, byte def = 0)
	{
		string text = this.GetRawString(key).ToLower();
		if (text == "default")
		{
			return def;
		}
		byte result;
		if (byte.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out result))
		{
			return result;
		}
		ServerConsole.AddLog(key + " has an invalid value, " + text + " is not a valid byte!", ConsoleColor.Gray, false);
		this.CommentInvalid(key, "BYTE");
		return def;
	}

	// Token: 0x06000C7D RID: 3197 RVA: 0x00032134 File Offset: 0x00030334
	public sbyte GetSByte(string key, sbyte def = 0)
	{
		string text = this.GetRawString(key).ToLower();
		if (text == "default")
		{
			return def;
		}
		sbyte result;
		if (sbyte.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out result))
		{
			return result;
		}
		ServerConsole.AddLog(key + " has an invalid value, " + text + " is not a valid signed byte!", ConsoleColor.Gray, false);
		this.CommentInvalid(key, "SBYTE");
		return def;
	}

	// Token: 0x06000C7E RID: 3198 RVA: 0x00032198 File Offset: 0x00030398
	public char GetChar(string key, char def = ' ')
	{
		string rawString = this.GetRawString(key);
		if (rawString == "default")
		{
			return def;
		}
		char result;
		if (char.TryParse(rawString, out result))
		{
			return result;
		}
		ServerConsole.AddLog(key + " has an invalid value, " + rawString + " is not a valid char!", ConsoleColor.Gray, false);
		this.CommentInvalid(key, "CHAR");
		return def;
	}

	// Token: 0x06000C7F RID: 3199 RVA: 0x000321F0 File Offset: 0x000303F0
	public decimal GetDecimal(string key, [Optional] decimal def = 0m)
	{
		string text = this.GetRawString(key).ToLower();
		if (text == "default")
		{
			return def;
		}
		decimal result;
		if (decimal.TryParse(text.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out result))
		{
			return result;
		}
		ServerConsole.AddLog(key + " has invalid value, " + text + " is not a valid decimal!", ConsoleColor.Gray, false);
		this.CommentInvalid(key, "DECIMAL");
		return def;
	}

	// Token: 0x06000C80 RID: 3200 RVA: 0x00032260 File Offset: 0x00030460
	public double GetDouble(string key, double def = 0.0)
	{
		string text = this.GetRawString(key).ToLower();
		if (text == "default")
		{
			return def;
		}
		double result;
		if (double.TryParse(text.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out result))
		{
			return result;
		}
		ServerConsole.AddLog(key + " has invalid value, " + text + " is not a valid double!", ConsoleColor.Gray, false);
		this.CommentInvalid(key, "DOUBLE");
		return def;
	}

	// Token: 0x06000C81 RID: 3201 RVA: 0x000322D0 File Offset: 0x000304D0
	public float GetFloat(string key, float def = 0f)
	{
		string text = this.GetRawString(key).ToLower();
		if (text == "default")
		{
			return def;
		}
		float result;
		if (float.TryParse(text.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out result))
		{
			return result;
		}
		ServerConsole.AddLog(key + " has invalid value, " + text + " is not a valid float!", ConsoleColor.Gray, false);
		this.CommentInvalid(key, "FLOAT");
		return def;
	}

	// Token: 0x06000C82 RID: 3202 RVA: 0x00032340 File Offset: 0x00030540
	public int GetInt(string key, int def = 0)
	{
		string text = this.GetRawString(key).ToLower();
		if (text == "default")
		{
			return def;
		}
		int result;
		if (int.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out result))
		{
			return result;
		}
		ServerConsole.AddLog(key + " has an invalid value, " + text + " is not a valid integer!", ConsoleColor.Gray, false);
		this.CommentInvalid(key, "INT");
		return def;
	}

	// Token: 0x06000C83 RID: 3203 RVA: 0x000323A4 File Offset: 0x000305A4
	public uint GetUInt(string key, uint def = 0U)
	{
		string text = this.GetRawString(key).ToLower();
		if (text == "default")
		{
			return def;
		}
		uint result;
		if (uint.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out result))
		{
			return result;
		}
		ServerConsole.AddLog(key + " has an invalid value, " + text + " is not a valid unsigned integer!", ConsoleColor.Gray, false);
		this.CommentInvalid(key, "UINT");
		return def;
	}

	// Token: 0x06000C84 RID: 3204 RVA: 0x00032408 File Offset: 0x00030608
	public long GetLong(string key, long def = 0L)
	{
		string text = this.GetRawString(key).ToLower();
		if (text == "default")
		{
			return def;
		}
		long result;
		if (long.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out result))
		{
			return result;
		}
		ServerConsole.AddLog(key + " has an invalid value, " + text + " is not a valid long!", ConsoleColor.Gray, false);
		this.CommentInvalid(key, "LONG");
		return def;
	}

	// Token: 0x06000C85 RID: 3205 RVA: 0x0003246C File Offset: 0x0003066C
	public ulong GetULong(string key, ulong def = 0UL)
	{
		string text = this.GetRawString(key).ToLower();
		if (text == "default")
		{
			return def;
		}
		ulong result;
		if (ulong.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out result))
		{
			return result;
		}
		ServerConsole.AddLog(key + " has an invalid value, " + text + " is not a valid unsigned long!", ConsoleColor.Gray, false);
		this.CommentInvalid(key, "ULONG");
		return def;
	}

	// Token: 0x06000C86 RID: 3206 RVA: 0x000324D0 File Offset: 0x000306D0
	public short GetShort(string key, short def = 0)
	{
		string text = this.GetRawString(key).ToLower();
		if (text == "default")
		{
			return def;
		}
		short result;
		if (short.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out result))
		{
			return result;
		}
		ServerConsole.AddLog(key + " has an invalid value, " + text + " is not a valid short!", ConsoleColor.Gray, false);
		this.CommentInvalid(key, "SHORT");
		return def;
	}

	// Token: 0x06000C87 RID: 3207 RVA: 0x00032534 File Offset: 0x00030734
	public ushort GetUShort(string key, ushort def = 0)
	{
		string text = this.GetRawString(key).ToLower();
		if (text == "default")
		{
			return def;
		}
		ushort result;
		if (ushort.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out result))
		{
			return result;
		}
		ServerConsole.AddLog(key + " has an invalid value, " + text + " is not a valid unsigned short!", ConsoleColor.Gray, false);
		this.CommentInvalid(key, "USHORT");
		return def;
	}

	// Token: 0x06000C88 RID: 3208 RVA: 0x00032598 File Offset: 0x00030798
	public string GetString(string key, string def = "")
	{
		string rawString = this.GetRawString(key);
		if (!(rawString == "default"))
		{
			return rawString;
		}
		return def;
	}

	// Token: 0x06000C89 RID: 3209 RVA: 0x000325BD File Offset: 0x000307BD
	public List<bool> GetBoolList(string key)
	{
		this.GetStringCollection(key, YamlConfig.DataBuffer);
		IEnumerable<string> dataBuffer = YamlConfig.DataBuffer;
		Func<string, bool> selector;
		if ((selector = YamlConfig.<>O.<0>__Parse) == null)
		{
			selector = (YamlConfig.<>O.<0>__Parse = new Func<string, bool>(bool.Parse));
		}
		return dataBuffer.Select(selector).ToList<bool>();
	}

	// Token: 0x06000C8A RID: 3210 RVA: 0x000325F5 File Offset: 0x000307F5
	public List<byte> GetByteList(string key)
	{
		this.GetStringCollection(key, YamlConfig.DataBuffer);
		IEnumerable<string> dataBuffer = YamlConfig.DataBuffer;
		Func<string, byte> selector;
		if ((selector = YamlConfig.<>O.<1>__Parse) == null)
		{
			selector = (YamlConfig.<>O.<1>__Parse = new Func<string, byte>(byte.Parse));
		}
		return dataBuffer.Select(selector).ToList<byte>();
	}

	// Token: 0x06000C8B RID: 3211 RVA: 0x0003262D File Offset: 0x0003082D
	public List<sbyte> GetSByteList(string key)
	{
		this.GetStringCollection(key, YamlConfig.DataBuffer);
		IEnumerable<string> dataBuffer = YamlConfig.DataBuffer;
		Func<string, sbyte> selector;
		if ((selector = YamlConfig.<>O.<2>__Parse) == null)
		{
			selector = (YamlConfig.<>O.<2>__Parse = new Func<string, sbyte>(sbyte.Parse));
		}
		return dataBuffer.Select(selector).ToList<sbyte>();
	}

	// Token: 0x06000C8C RID: 3212 RVA: 0x00032665 File Offset: 0x00030865
	public List<char> GetCharList(string key)
	{
		this.GetStringCollection(key, YamlConfig.DataBuffer);
		IEnumerable<string> dataBuffer = YamlConfig.DataBuffer;
		Func<string, char> selector;
		if ((selector = YamlConfig.<>O.<3>__Parse) == null)
		{
			selector = (YamlConfig.<>O.<3>__Parse = new Func<string, char>(char.Parse));
		}
		return dataBuffer.Select(selector).ToList<char>();
	}

	// Token: 0x06000C8D RID: 3213 RVA: 0x0003269D File Offset: 0x0003089D
	public List<decimal> GetDecimalList(string key)
	{
		this.GetStringCollection(key, YamlConfig.DataBuffer);
		IEnumerable<string> dataBuffer = YamlConfig.DataBuffer;
		Func<string, decimal> selector;
		if ((selector = YamlConfig.<>O.<4>__Parse) == null)
		{
			selector = (YamlConfig.<>O.<4>__Parse = new Func<string, decimal>(decimal.Parse));
		}
		return dataBuffer.Select(selector).ToList<decimal>();
	}

	// Token: 0x06000C8E RID: 3214 RVA: 0x000326D5 File Offset: 0x000308D5
	public List<double> GetDoubleList(string key)
	{
		this.GetStringCollection(key, YamlConfig.DataBuffer);
		IEnumerable<string> dataBuffer = YamlConfig.DataBuffer;
		Func<string, double> selector;
		if ((selector = YamlConfig.<>O.<5>__Parse) == null)
		{
			selector = (YamlConfig.<>O.<5>__Parse = new Func<string, double>(double.Parse));
		}
		return dataBuffer.Select(selector).ToList<double>();
	}

	// Token: 0x06000C8F RID: 3215 RVA: 0x0003270D File Offset: 0x0003090D
	public List<float> GetFloatList(string key)
	{
		this.GetStringCollection(key, YamlConfig.DataBuffer);
		IEnumerable<string> dataBuffer = YamlConfig.DataBuffer;
		Func<string, float> selector;
		if ((selector = YamlConfig.<>O.<6>__Parse) == null)
		{
			selector = (YamlConfig.<>O.<6>__Parse = new Func<string, float>(float.Parse));
		}
		return dataBuffer.Select(selector).ToList<float>();
	}

	// Token: 0x06000C90 RID: 3216 RVA: 0x00032745 File Offset: 0x00030945
	public List<int> GetIntList(string key)
	{
		this.GetStringCollection(key, YamlConfig.DataBuffer);
		IEnumerable<string> dataBuffer = YamlConfig.DataBuffer;
		Func<string, int> selector;
		if ((selector = YamlConfig.<>O.<7>__Parse) == null)
		{
			selector = (YamlConfig.<>O.<7>__Parse = new Func<string, int>(int.Parse));
		}
		return dataBuffer.Select(selector).ToList<int>();
	}

	// Token: 0x06000C91 RID: 3217 RVA: 0x0003277D File Offset: 0x0003097D
	public List<uint> GetUIntList(string key)
	{
		this.GetStringCollection(key, YamlConfig.DataBuffer);
		IEnumerable<string> dataBuffer = YamlConfig.DataBuffer;
		Func<string, uint> selector;
		if ((selector = YamlConfig.<>O.<8>__Parse) == null)
		{
			selector = (YamlConfig.<>O.<8>__Parse = new Func<string, uint>(uint.Parse));
		}
		return dataBuffer.Select(selector).ToList<uint>();
	}

	// Token: 0x06000C92 RID: 3218 RVA: 0x000327B5 File Offset: 0x000309B5
	public List<long> GetLongList(string key)
	{
		this.GetStringCollection(key, YamlConfig.DataBuffer);
		IEnumerable<string> dataBuffer = YamlConfig.DataBuffer;
		Func<string, long> selector;
		if ((selector = YamlConfig.<>O.<9>__Parse) == null)
		{
			selector = (YamlConfig.<>O.<9>__Parse = new Func<string, long>(long.Parse));
		}
		return dataBuffer.Select(selector).ToList<long>();
	}

	// Token: 0x06000C93 RID: 3219 RVA: 0x000327ED File Offset: 0x000309ED
	public List<ulong> GetULongList(string key)
	{
		this.GetStringCollection(key, YamlConfig.DataBuffer);
		IEnumerable<string> dataBuffer = YamlConfig.DataBuffer;
		Func<string, ulong> selector;
		if ((selector = YamlConfig.<>O.<10>__Parse) == null)
		{
			selector = (YamlConfig.<>O.<10>__Parse = new Func<string, ulong>(ulong.Parse));
		}
		return dataBuffer.Select(selector).ToList<ulong>();
	}

	// Token: 0x06000C94 RID: 3220 RVA: 0x00032825 File Offset: 0x00030A25
	public List<short> GetShortList(string key)
	{
		this.GetStringCollection(key, YamlConfig.DataBuffer);
		IEnumerable<string> dataBuffer = YamlConfig.DataBuffer;
		Func<string, short> selector;
		if ((selector = YamlConfig.<>O.<11>__Parse) == null)
		{
			selector = (YamlConfig.<>O.<11>__Parse = new Func<string, short>(short.Parse));
		}
		return dataBuffer.Select(selector).ToList<short>();
	}

	// Token: 0x06000C95 RID: 3221 RVA: 0x0003285D File Offset: 0x00030A5D
	public List<ushort> GetUShortList(string key)
	{
		this.GetStringCollection(key, YamlConfig.DataBuffer);
		IEnumerable<string> dataBuffer = YamlConfig.DataBuffer;
		Func<string, ushort> selector;
		if ((selector = YamlConfig.<>O.<12>__Parse) == null)
		{
			selector = (YamlConfig.<>O.<12>__Parse = new Func<string, ushort>(ushort.Parse));
		}
		return dataBuffer.Select(selector).ToList<ushort>();
	}

	// Token: 0x06000C96 RID: 3222 RVA: 0x00032898 File Offset: 0x00030A98
	public List<string> GetStringList(string key)
	{
		List<string> list = new List<string>();
		this.GetStringCollection(key, list);
		return list;
	}

	// Token: 0x06000C97 RID: 3223 RVA: 0x000328B4 File Offset: 0x00030AB4
	public void GetStringCollection(string key, ICollection<string> collection)
	{
		bool flag = false;
		collection.Clear();
		foreach (string text in this.RawData)
		{
			if (text.StartsWith(key) && text.TrimEnd().EndsWith("[]", StringComparison.Ordinal))
			{
				break;
			}
			if (text.StartsWith(key + ":", StringComparison.Ordinal))
			{
				if (text.StartsWith(key + ": ", StringComparison.Ordinal))
				{
					string text2 = text.Substring(key.Length + 2);
					if (text2.Contains("[") && text2.Contains("]"))
					{
						List<string> list = collection as List<string>;
						if (list != null)
						{
							list.AddRange(YamlConfig.ParseCommaSeparatedString(text2));
							return;
						}
						foreach (string item in YamlConfig.ParseCommaSeparatedString(text2))
						{
							collection.Add(item);
						}
						return;
					}
				}
				flag = true;
			}
			else if (flag)
			{
				if (text.StartsWith(" - ", StringComparison.Ordinal))
				{
					collection.Add(text.Substring(3).TrimEnd());
				}
				else if (!text.StartsWith("#", StringComparison.Ordinal))
				{
					break;
				}
			}
		}
	}

	// Token: 0x04000A28 RID: 2600
	private static readonly string[] _rolevars = new string[]
	{
		"color",
		"badge",
		"cover",
		"hidden",
		"kick_power",
		"required_kick_power"
	};

	// Token: 0x04000A29 RID: 2601
	private static readonly string[] _deprecatedconfigs = new string[]
	{
		"administrator_password"
	};

	// Token: 0x04000A2A RID: 2602
	private bool _afteradding;

	// Token: 0x04000A2B RID: 2603
	private bool _virtual;

	// Token: 0x04000A2C RID: 2604
	private string[] _rawDataUnfiltered;

	// Token: 0x04000A2D RID: 2605
	private string[] _rawData;

	// Token: 0x04000A2E RID: 2606
	private static readonly List<string> DataBuffer = new List<string>();

	// Token: 0x04000A2F RID: 2607
	public string Path;

	// Token: 0x0200022D RID: 557
	[CompilerGenerated]
	private static class <>O
	{
		// Token: 0x04000A30 RID: 2608
		public static Func<string, bool> <0>__Parse;

		// Token: 0x04000A31 RID: 2609
		public static Func<string, byte> <1>__Parse;

		// Token: 0x04000A32 RID: 2610
		public static Func<string, sbyte> <2>__Parse;

		// Token: 0x04000A33 RID: 2611
		public static Func<string, char> <3>__Parse;

		// Token: 0x04000A34 RID: 2612
		public static Func<string, decimal> <4>__Parse;

		// Token: 0x04000A35 RID: 2613
		public static Func<string, double> <5>__Parse;

		// Token: 0x04000A36 RID: 2614
		public static Func<string, float> <6>__Parse;

		// Token: 0x04000A37 RID: 2615
		public static Func<string, int> <7>__Parse;

		// Token: 0x04000A38 RID: 2616
		public static Func<string, uint> <8>__Parse;

		// Token: 0x04000A39 RID: 2617
		public static Func<string, long> <9>__Parse;

		// Token: 0x04000A3A RID: 2618
		public static Func<string, ulong> <10>__Parse;

		// Token: 0x04000A3B RID: 2619
		public static Func<string, short> <11>__Parse;

		// Token: 0x04000A3C RID: 2620
		public static Func<string, ushort> <12>__Parse;
	}
}
