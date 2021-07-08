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
	}
}