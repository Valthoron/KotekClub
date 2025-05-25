using UnityEditor;
using UnityEngine;

public partial class Spawner : MonoBehaviour
{
	// ********************************************************************************
	// Editor messagees
#if UNITY_EDITOR
	void OnDrawGizmosSelected()
	{
		foreach (var zone in SpawnZones)
		{
			Handles.DrawSolidRectangleWithOutline(zone, new Color(0, 1, 0, 0.2f), Color.green);
		}
	}
#endif
}
