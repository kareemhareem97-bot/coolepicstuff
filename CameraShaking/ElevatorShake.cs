using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Interactables.Interobjects;
using UnityEngine;

namespace CameraShaking
{
	// Token: 0x02000889 RID: 2185
	public class ElevatorShake : IShakeEffect
	{
		// Token: 0x060031E4 RID: 12772 RVA: 0x000A795C File Offset: 0x000A5B5C
		[RuntimeInitializeOnLoadMethod]
		private static void Init()
		{
			Action<ReferenceHub> value;
			if ((value = ElevatorShake.<>O.<0>__OnPlayerAdded) == null)
			{
				value = (ElevatorShake.<>O.<0>__OnPlayerAdded = new Action<ReferenceHub>(ElevatorShake.OnPlayerAdded));
			}
			ReferenceHub.OnPlayerAdded += value;
			ElevatorChamber.ElevatorMoved value2;
			if ((value2 = ElevatorShake.<>O.<1>__OnElevatorMoved) == null)
			{
				value2 = (ElevatorShake.<>O.<1>__OnElevatorMoved = new ElevatorChamber.ElevatorMoved(ElevatorShake.OnElevatorMoved));
			}
			ElevatorChamber.OnElevatorMoved += value2;
		}

		// Token: 0x060031E5 RID: 12773 RVA: 0x000A79A9 File Offset: 0x000A5BA9
		private static void OnPlayerAdded(ReferenceHub hub)
		{
			if (!hub.isLocalPlayer)
			{
				return;
			}
			CameraShakeController.AddEffect(new ElevatorShake());
		}

		// Token: 0x060031E6 RID: 12774 RVA: 0x000A79BE File Offset: 0x000A5BBE
		private static void OnElevatorMoved(Bounds elevatorBounds, ElevatorChamber chamber, Vector3 deltaPos, Quaternion deltaRot)
		{
			if (!elevatorBounds.Contains(MainCameraController.CurrentCamera.position))
			{
				return;
			}
			ElevatorShake.LastMovement.Restart();
		}

		// Token: 0x060031E7 RID: 12775 RVA: 0x000A79E0 File Offset: 0x000A5BE0
		public bool GetEffect(ReferenceHub ply, out ShakeEffectValues shakeValues)
		{
			float num = ElevatorShake.LastMovement.IsRunning ? ElevatorShake.FadeCurve.Evaluate((float)ElevatorShake.LastMovement.Elapsed.TotalSeconds) : 0f;
			if (Mathf.Approximately(num, 0f))
			{
				ElevatorShake.RealTime.Restart();
				shakeValues = ShakeEffectValues.None;
			}
			else
			{
				float num2 = (float)ElevatorShake.RealTime.Elapsed.TotalSeconds;
				float num3 = Mathf.Sin(num2 * 9f);
				float num4 = Mathf.Sin(num2 * 3f);
				float num5 = Mathf.Sin(num2 * 75f) * 0.12f;
				shakeValues = new ShakeEffectValues(new Quaternion?(Quaternion.Euler(num3 * num4 * num * num5 * Vector3.up)), null, null, 1f, 0f, 0f);
			}
			return true;
		}

		// Token: 0x040026F4 RID: 9972
		private const float ShakeSpeed = 75f;

		// Token: 0x040026F5 RID: 9973
		private const float ShakeAngle = 0.12f;

		// Token: 0x040026F6 RID: 9974
		private const float PulsePrimarySpeed = 9f;

		// Token: 0x040026F7 RID: 9975
		private const float PulseSecondarySpeed = 3f;

		// Token: 0x040026F8 RID: 9976
		private static readonly Stopwatch RealTime = Stopwatch.StartNew();

		// Token: 0x040026F9 RID: 9977
		private static readonly Stopwatch LastMovement = new Stopwatch();

		// Token: 0x040026FA RID: 9978
		private static readonly AnimationCurve FadeCurve = new AnimationCurve(new Keyframe[]
		{
			new Keyframe(0f, 1f, -0.1f, -0.1f, 0f, 0.5f),
			new Keyframe(0.5f, 0f, 0f, 0f, 0.2f, 0f)
		});

		// Token: 0x0200088A RID: 2186
		[CompilerGenerated]
		private static class <>O
		{
			// Token: 0x040026FB RID: 9979
			public static Action<ReferenceHub> <0>__OnPlayerAdded;

			// Token: 0x040026FC RID: 9980
			public static ElevatorChamber.ElevatorMoved <1>__OnElevatorMoved;
		}
	}
}
