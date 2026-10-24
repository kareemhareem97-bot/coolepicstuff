using System;
using System.Collections.Generic;
using Interactables.Interobjects.DoorUtils;
using PlayerRoles.FirstPersonControl;
using ProgressiveCulling;
using UnityEngine;

// Token: 0x0200015E RID: 350
public static class SafeLocationFinder
{
	// Token: 0x060006BF RID: 1727 RVA: 0x0001946D File Offset: 0x0001766D
	public static List<Pose> GetLocations(Predicate<RoomCullingConnection> connectionFilter, Predicate<DoorVariant> doorFilter)
	{
		SafeLocationFinder.ResultsNonAlloc.Clear();
		SafeLocationFinder.GetLocations(SafeLocationFinder.ResultsNonAlloc, connectionFilter, doorFilter);
		return SafeLocationFinder.ResultsNonAlloc;
	}

	// Token: 0x060006C0 RID: 1728 RVA: 0x0001948C File Offset: 0x0001768C
	public static void GetLocations(List<Pose> results, Predicate<RoomCullingConnection> connectionFilter, Predicate<DoorVariant> doorFilter)
	{
		SafeLocationFinder.DuplicatePreventer.Clear();
		foreach (RoomCullingConnection roomCullingConnection in RoomCullingConnection.AllInstances)
		{
			if (connectionFilter == null || connectionFilter(roomCullingConnection))
			{
				SafeLocationFinder.AddTransform(roomCullingConnection.transform, results);
			}
		}
		foreach (DoorVariant doorVariant in DoorVariant.AllDoors)
		{
			if (doorFilter == null || doorFilter(doorVariant))
			{
				SafeLocationFinder.AddTransform(doorVariant.transform, results);
			}
		}
	}

	// Token: 0x060006C1 RID: 1729 RVA: 0x0001954C File Offset: 0x0001774C
	public static Vector3 GetSafePosition(Vector3 position, Vector3 direction, float range, CharacterController charController)
	{
		direction = Quaternion.Euler(0f, UnityEngine.Random.Range(-30f, 30f), 0f) * direction;
		float radius = charController.radius;
		float num = Mathf.Lerp(radius, range, UnityEngine.Random.value);
		Vector3 b = Vector3.up * charController.height / 2f;
		Vector3 vector = position + charController.center + SafeLocationFinder.GroundOffset + Vector3.up * radius;
		RaycastHit raycastHit;
		if (!Physics.SphereCast(vector, radius, direction, out raycastHit, num + radius, FpcStateProcessor.Mask))
		{
			return vector + direction * num + b;
		}
		return raycastHit.point + raycastHit.normal * radius + b;
	}

	// Token: 0x060006C2 RID: 1730 RVA: 0x00019624 File Offset: 0x00017824
	public static Vector3 GetSafePositionForPose(Pose pose, float range, CharacterController charController, bool randomizeDir = true)
	{
		Vector3 vector = pose.forward;
		if (randomizeDir && UnityEngine.Random.value > 0.5f)
		{
			vector = -vector;
		}
		return SafeLocationFinder.GetSafePosition(pose.position, vector, range, charController);
	}

	// Token: 0x060006C3 RID: 1731 RVA: 0x00019660 File Offset: 0x00017860
	private static void AddTransform(Transform tr, List<Pose> target)
	{
		Vector3 position;
		Quaternion rotation;
		tr.GetPositionAndRotation(out position, out rotation);
		Pose item = new Pose(position, rotation);
		if (!SafeLocationFinder.DuplicatePreventer.Add(item))
		{
			return;
		}
		target.Add(item);
	}

	// Token: 0x04000623 RID: 1571
	private static readonly List<Pose> ResultsNonAlloc = new List<Pose>();

	// Token: 0x04000624 RID: 1572
	private static readonly HashSet<Pose> DuplicatePreventer = new HashSet<Pose>();

	// Token: 0x04000625 RID: 1573
	private static readonly Vector3 GroundOffset = new Vector3(0f, 0.25f, 0f);

	// Token: 0x04000626 RID: 1574
	private const float AngleVariation = 30f;
}
