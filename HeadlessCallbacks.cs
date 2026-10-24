using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using UnityEngine;

// Token: 0x02000018 RID: 24
public class HeadlessCallbacks : Attribute
{
	// Token: 0x0600006C RID: 108 RVA: 0x00003190 File Offset: 0x00001390
	public static void FindCallbacks()
	{
		if (HeadlessCallbacks.callbackRegistry != null)
		{
			return;
		}
		try
		{
			HeadlessCallbacks.callbackRegistry = from t in Assembly.GetExecutingAssembly().GetTypes()
			let attributes = t.GetCustomAttributes(typeof(HeadlessCallbacks), true)
			where attributes != null && attributes.Length != 0
			select t;
		}
		catch (ReflectionTypeLoadException ex)
		{
			try
			{
				HeadlessCallbacks.callbackRegistry = from t in ex.Types
				where t != null
				select t;
			}
			catch (Exception ex2)
			{
				Debug.Log("Headless Builder could not find callbacks (" + ex2.GetType().Name + "), but will still continue as planned");
				HeadlessCallbacks.callbackRegistry = Enumerable.Empty<Type>();
			}
		}
		catch (Exception ex3)
		{
			Debug.Log("Headless Builder could not find callbacks (" + ex3.GetType().Name + "), but will still continue as planned");
			HeadlessCallbacks.callbackRegistry = Enumerable.Empty<Type>();
		}
	}

	// Token: 0x0600006D RID: 109 RVA: 0x000032DC File Offset: 0x000014DC
	public static void InvokeCallbacks(string callbackName)
	{
		HeadlessCallbacks.FindCallbacks();
		foreach (object obj in HeadlessCallbacks.callbackRegistry)
		{
			Type type = (Type)obj;
			MethodInfo method = type.GetMethod(callbackName);
			if (method != null)
			{
				try
				{
					method.Invoke(type, null);
				}
				catch (Exception message)
				{
					Debug.LogError(message);
				}
			}
		}
	}

	// Token: 0x04000030 RID: 48
	private static IEnumerable callbackRegistry;
}
