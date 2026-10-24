using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GameCore;
using NorthwoodLib.Pools;

// Token: 0x020001DC RID: 476
public static class FileManager
{
	// Token: 0x060009FE RID: 2558 RVA: 0x00027800 File Offset: 0x00025A00
	public static void RefreshAppFolder()
	{
		FileManager._appfolder = ((ServerStatic.IsDedicated && ConfigFile.HosterPolicy != null && ConfigFile.HosterPolicy.GetBool("gamedir_for_configs", false)) ? "AppData" : (Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + FileManager.GetPathSeparator().ToString() + "SCP Secret Laboratory"));
	}

	// Token: 0x060009FF RID: 2559 RVA: 0x00027858 File Offset: 0x00025A58
	public static string GetAppFolder(bool addSeparator = true, bool serverConfig = false, string centralConfig = "")
	{
		if (string.IsNullOrEmpty(FileManager._appfolder))
		{
			FileManager.RefreshAppFolder();
		}
		if (serverConfig && !string.IsNullOrEmpty(FileManager._configfolder) && string.IsNullOrEmpty(centralConfig))
		{
			return FileManager._configfolder + (addSeparator ? FileManager.GetPathSeparator().ToString() : "");
		}
		return FileManager._appfolder + ((addSeparator || serverConfig) ? FileManager.GetPathSeparator().ToString() : "") + (serverConfig ? ("config/" + (string.IsNullOrEmpty(centralConfig) ? (ServerStatic.IsDedicated ? ServerStatic.ServerPort.ToString() : "nondedicated") : centralConfig) + (addSeparator ? FileManager.GetPathSeparator().ToString() : "")) : "");
	}

	// Token: 0x06000A00 RID: 2560 RVA: 0x00027924 File Offset: 0x00025B24
	public static string StripPath(string path)
	{
		path = path.Replace("\"", "").Trim();
		while (path.EndsWith("\\") || path.EndsWith("/") || path.EndsWith(FileManager.GetPathSeparator().ToString()))
		{
			path = path.Substring(0, path.Length - 1);
		}
		return path;
	}

	// Token: 0x06000A01 RID: 2561 RVA: 0x0002798B File Offset: 0x00025B8B
	public static void SetAppFolder(string path)
	{
		path = FileManager.StripPath(path);
		if (!Directory.Exists(path))
		{
			FileManager._appfolder = "";
			return;
		}
		FileManager._appfolder = path;
	}

	// Token: 0x06000A02 RID: 2562 RVA: 0x000279AE File Offset: 0x00025BAE
	public static void SetConfigFolder(string path)
	{
		path = FileManager.StripPath(path);
		if (!Directory.Exists(path))
		{
			FileManager._configfolder = "";
			return;
		}
		FileManager._configfolder = path;
	}

	// Token: 0x06000A03 RID: 2563 RVA: 0x000279D1 File Offset: 0x00025BD1
	public static string ReplacePathSeparators(string path)
	{
		return path.Replace('/', FileManager.GetPathSeparator()).Replace('\\', FileManager.GetPathSeparator());
	}

	// Token: 0x06000A04 RID: 2564 RVA: 0x000279EC File Offset: 0x00025BEC
	public static char GetPathSeparator()
	{
		return Path.DirectorySeparatorChar;
	}

	// Token: 0x06000A05 RID: 2565 RVA: 0x000279F3 File Offset: 0x00025BF3
	public static bool FileExists(string path)
	{
		return File.Exists(path);
	}

	// Token: 0x06000A06 RID: 2566 RVA: 0x000279FB File Offset: 0x00025BFB
	public static bool DictionaryExists(string path)
	{
		return Directory.Exists(path);
	}

	// Token: 0x06000A07 RID: 2567 RVA: 0x00027A03 File Offset: 0x00025C03
	public static void FileCreate(string path)
	{
		File.Create(path).Dispose();
	}

	// Token: 0x06000A08 RID: 2568 RVA: 0x00027A10 File Offset: 0x00025C10
	public static FileStream FileStreamCreate(string path)
	{
		return File.Create(path);
	}

	// Token: 0x06000A09 RID: 2569 RVA: 0x00027A18 File Offset: 0x00025C18
	public static string[] ReadAllLines(string path)
	{
		List<string> list = ListPool<string>.Shared.Rent();
		using (StreamReader streamReader = new StreamReader(path))
		{
			string item;
			while ((item = streamReader.ReadLine()) != null)
			{
				list.Add(item);
			}
		}
		string[] result = list.ToArray();
		ListPool<string>.Shared.Return(list);
		return result;
	}

	// Token: 0x06000A0A RID: 2570 RVA: 0x00027A78 File Offset: 0x00025C78
	public static List<string> ReadAllLinesList(string path)
	{
		List<string> list = new List<string>();
		using (StreamReader streamReader = new StreamReader(path))
		{
			string item;
			while ((item = streamReader.ReadLine()) != null)
			{
				list.Add(item);
			}
		}
		return list;
	}

	// Token: 0x06000A0B RID: 2571 RVA: 0x00027AC4 File Offset: 0x00025CC4
	public static void ReadAllLinesList(string path, List<string> list)
	{
		list.Clear();
		using (StreamReader streamReader = new StreamReader(path))
		{
			string item;
			while ((item = streamReader.ReadLine()) != null)
			{
				list.Add(item);
			}
		}
	}

	// Token: 0x06000A0C RID: 2572 RVA: 0x00027B10 File Offset: 0x00025D10
	public static string[] ReadAllLinesSafe(string path)
	{
		List<string> list = ListPool<string>.Shared.Rent();
		using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
		{
			using (StreamReader streamReader = new StreamReader(fileStream))
			{
				string item;
				while ((item = streamReader.ReadLine()) != null)
				{
					list.Add(item);
				}
			}
		}
		string[] result = list.ToArray();
		ListPool<string>.Shared.Return(list);
		return result;
	}

	// Token: 0x06000A0D RID: 2573 RVA: 0x00027B90 File Offset: 0x00025D90
	public static List<string> ReadAllLinesSafeList(string path)
	{
		List<string> list = new List<string>();
		using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
		{
			using (StreamReader streamReader = new StreamReader(fileStream))
			{
				string item;
				while ((item = streamReader.ReadLine()) != null)
				{
					list.Add(item);
				}
			}
		}
		return list;
	}

	// Token: 0x06000A0E RID: 2574 RVA: 0x00027BFC File Offset: 0x00025DFC
	public static void ReadAllLinesSafeList(string path, List<string> list)
	{
		list.Clear();
		using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
		{
			using (StreamReader streamReader = new StreamReader(fileStream))
			{
				string item;
				while ((item = streamReader.ReadLine()) != null)
				{
					list.Add(item);
				}
			}
		}
	}

	// Token: 0x06000A0F RID: 2575 RVA: 0x00027C68 File Offset: 0x00025E68
	public static string ReadAllText(string path)
	{
		return File.ReadAllText(path);
	}

	// Token: 0x06000A10 RID: 2576 RVA: 0x00027C70 File Offset: 0x00025E70
	public static string ReadAllTextSafe(string path)
	{
		string result;
		using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
		{
			using (StreamReader streamReader = new StreamReader(fileStream))
			{
				result = streamReader.ReadToEnd();
			}
		}
		return result;
	}

	// Token: 0x06000A11 RID: 2577 RVA: 0x00027CC8 File Offset: 0x00025EC8
	public static void WriteToFile(IEnumerable<string> data, string path, bool removeempty = false)
	{
		IEnumerable<string> contents;
		if (!removeempty)
		{
			contents = data;
		}
		else
		{
			contents = from line in data
			where !string.IsNullOrWhiteSpace(line.Replace(Environment.NewLine, "").Replace("\r\n", "").Replace("\n", "").Replace(" ", ""))
			select line;
		}
		File.WriteAllLines(path, contents, Misc.Utf8Encoding);
	}

	// Token: 0x06000A12 RID: 2578 RVA: 0x00027D00 File Offset: 0x00025F00
	public static void WriteStringToFile(string data, string path)
	{
		File.WriteAllText(path, data, Misc.Utf8Encoding);
	}

	// Token: 0x06000A13 RID: 2579 RVA: 0x00027D10 File Offset: 0x00025F10
	public static void WriteToFileSafe(IEnumerable<string> data, string path, bool removeempty = false)
	{
		using (FileStream fileStream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite))
		{
			using (StreamWriter streamWriter = new StreamWriter(fileStream, Misc.Utf8Encoding))
			{
				streamWriter.Write(string.Join("\r\n", data));
			}
		}
	}

	// Token: 0x06000A14 RID: 2580 RVA: 0x00027D78 File Offset: 0x00025F78
	public static void WriteStringToFileSafe(string data, string path)
	{
		using (FileStream fileStream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite))
		{
			using (StreamWriter streamWriter = new StreamWriter(fileStream, Misc.Utf8Encoding))
			{
				streamWriter.Write(data);
			}
		}
	}

	// Token: 0x06000A15 RID: 2581 RVA: 0x00027DD4 File Offset: 0x00025FD4
	public static void AppendFile(string data, string path, bool newLine = true)
	{
		string[] array = FileManager.ReadAllLines(path);
		if (!newLine || array.Length == 0 || array[array.Length - 1].EndsWith(Environment.NewLine) || array[array.Length - 1].EndsWith("\n"))
		{
			File.AppendAllText(path, data, Misc.Utf8Encoding);
			return;
		}
		File.AppendAllText(path, Environment.NewLine + data, Misc.Utf8Encoding);
	}

	// Token: 0x06000A16 RID: 2582 RVA: 0x00027E38 File Offset: 0x00026038
	public static void AppendFileSafe(string data, string path, bool newLine = true)
	{
		using (FileStream fileStream = new FileStream(path, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
		{
			using (StreamWriter streamWriter = new StreamWriter(fileStream, Misc.Utf8Encoding))
			{
				streamWriter.Write(newLine ? ("\r\n" + data) : data);
			}
		}
	}

	// Token: 0x06000A17 RID: 2583 RVA: 0x00027EA4 File Offset: 0x000260A4
	public static void RenameFile(string path, string newpath)
	{
		File.Move(path, newpath);
	}

	// Token: 0x06000A18 RID: 2584 RVA: 0x00027EAD File Offset: 0x000260AD
	public static void DeleteFile(string path)
	{
		File.Delete(path);
	}

	// Token: 0x06000A19 RID: 2585 RVA: 0x00027EB5 File Offset: 0x000260B5
	public static void ReplaceLine(int line, string text, string path)
	{
		string[] array = FileManager.ReadAllLines(path);
		array[line] = text;
		FileManager.WriteToFile(array, path, false);
	}

	// Token: 0x06000A1A RID: 2586 RVA: 0x00027EC8 File Offset: 0x000260C8
	public static void RemoveEmptyLines(string path)
	{
		string[] array = FileManager.ReadAllLines(path);
		string[] array2 = (from s in array
		where !string.IsNullOrWhiteSpace(s.Replace(Environment.NewLine, "").Replace("\r\n", "").Replace("\n", "").Replace(" ", ""))
		select s).ToArray<string>();
		if (array == array2)
		{
			return;
		}
		FileManager.WriteToFile(array2, path, false);
	}

	// Token: 0x06000A1B RID: 2587 RVA: 0x00027F14 File Offset: 0x00026114
	private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs = true, bool overwrite = true)
	{
		DirectoryInfo directoryInfo = new DirectoryInfo(sourceDirName);
		if (!directoryInfo.Exists)
		{
			throw new DirectoryNotFoundException("Source directory does not exist or could not be found: " + sourceDirName);
		}
		DirectoryInfo[] directories = directoryInfo.GetDirectories();
		if (Directory.Exists(destDirName))
		{
			Directory.Delete(destDirName, true);
		}
		Directory.CreateDirectory(destDirName);
		foreach (FileInfo fileInfo in directoryInfo.GetFiles())
		{
			string destFileName = Path.Combine(destDirName, fileInfo.Name);
			fileInfo.CopyTo(destFileName, overwrite);
		}
		if (copySubDirs)
		{
			foreach (DirectoryInfo directoryInfo2 in directories)
			{
				string destDirName2 = Path.Combine(destDirName, directoryInfo2.Name);
				FileManager.DirectoryCopy(directoryInfo2.FullName, destDirName2, overwrite, true);
			}
		}
	}

	// Token: 0x040008AC RID: 2220
	private static string _appfolder = "";

	// Token: 0x040008AD RID: 2221
	private static string _configfolder = "";
}
