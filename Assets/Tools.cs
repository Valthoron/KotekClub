using System.Collections.Generic;
using UnityEngine;

public static class Tools
{
	public static GameObject GetClosestDamageable(Collider2D source)
	{
		List<Collider2D> results = new();

		if (source.OverlapCollider(new ContactFilter2D(), results) != 0)
		{
			GameObject nearestDamageable = null;
			float nearestDistanceSquared = 0.0f;

			foreach (var result in results)
			{
				if (result.gameObject == source.gameObject)
					continue;

				if (!result.gameObject.TryGetComponent<IDamageable>(out _))
					continue;
				
				float distanceSquared = (result.transform.root.position - source.transform.root.position).sqrMagnitude;

				if (nearestDamageable == null || distanceSquared < nearestDistanceSquared)
				{
					nearestDamageable = result.gameObject;
					nearestDistanceSquared = distanceSquared;
				}
			}

			if (nearestDamageable != null)
				return nearestDamageable;
		}

		return null;
	}

	public static void SetRenderersEnabled(GameObject gameObject, bool enabled)
	{
		if (gameObject.TryGetComponent<SpriteRenderer>(out var spriteRenderer))
			spriteRenderer.enabled = enabled;

		if (gameObject.TryGetComponent<MeshRenderer>(out var meshRenderer))
			meshRenderer.enabled = enabled;

		for (int i = 0; i < gameObject.transform.childCount; i++)
		{
			GameObject child = gameObject.transform.GetChild(i).gameObject;
			SetRenderersEnabled(child, enabled);
		}
	}
}
