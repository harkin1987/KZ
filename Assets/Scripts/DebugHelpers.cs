using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DebugTools
{
    public static class DebugHelpers
    {

		public static RaycastHit DebugRayCast(Vector3 rayOriginPoint, Vector3 rayDirection, float rayDistance, LayerMask mask, Color color, bool drawGizmo = false)
		{
			RaycastHit hitInfo;
			if (drawGizmo)
			{
				Debug.DrawRay(rayOriginPoint, rayDirection * rayDistance, color);
			}
			Physics.Raycast(rayOriginPoint, rayDirection, out hitInfo, rayDistance, mask);
			return hitInfo;
		}

		public static Vector3 SphereOrCapsuleCastCenterOnCollision(Vector3 origin, Vector3 directionCast, float hitInfoDistance)
		{
			return origin + (directionCast.normalized * hitInfoDistance);
		}

		public static int ToLayer(int bitmask)
		{
			int result = bitmask > 0 ? 0 : 31;
			while (bitmask > 1)
			{
				bitmask = bitmask >> 1;
				result++;
			}
			return result;
		}

	}
}