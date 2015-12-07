using UnityEngine;
using System;
using System.Collections;

public static class MyExtensionMethods
{
	public static float ConvertBetweenRanges(this float oldValue, float oldMin, float oldMax,
	                                         float newMin, float newMax)
	{
		float newValue = (((oldValue - oldMin) * (newMax - newMin))/(oldMax - oldMin) + newMin);
		return newValue;
	}

	public static bool IsBetween <T>(this T figure, T lower, T upper) where T : IComparable<T>
	{
		return figure.CompareTo(lower) >= 0 && figure.CompareTo(upper) < 0;
	}

	public static Vector3 GroundPosition(this Transform transf, float distanceToCheck, int groundLayerMask)
	{
		var groundHit = new RaycastHit();
		
		if(Physics.Raycast(transf.position, Vector3.down, out groundHit, distanceToCheck, groundLayerMask))
		{
			return groundHit.point;
		}
		else
		{
			Debug.Log("Ground Not Found!");
			return Vector3.zero;
		}
	}

	public static int LayerNumber (this LayerMask layerMask)
	{
		int result = (int)Mathf.Sqrt(layerMask.value)/2;
		return result;
	}
}

