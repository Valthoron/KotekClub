using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public partial class Spawner : MonoBehaviour
{
    // ********************************************************************************
    // Constants
    const float SpawnPeriod = 3.5f;
    const float MaxBaddies = 3.0f;
    const float FoodChance = 0.0667f;
    const int FoodGuarantee = 15;

    // ********************************************************************************
    // Members
    private float _timer = 0.0f;
    private List<GameObject> _baddies = new();
    private int _foodFails = 0;

    // ********************************************************************************
    // Properties
    public bool Active = true;
    public GameObject[] BaddiePrefabs;
    public GameObject[] FoodPrefabs;
    public Rect[] SpawnZones;
    public Dude Player;

    // ********************************************************************************
    // Events
    public UnityEvent<Baddie> BaddieSpawned;

    // ********************************************************************************
    // Unity messagees
    public void Update()
    {
        if (!Active)
            return;

        _timer -= Time.deltaTime;

        if (_timer < 0.0f)
        {
            _timer = Random.Range(0.5f, SpawnPeriod);

            if (_baddies.Count < MaxBaddies)
            {
                Rect zone = SpawnZones[Random.Range(0, SpawnZones.Length)];

                GameObject prefab = BaddiePrefabs[Random.Range(0, BaddiePrefabs.Length)];
                GameObject baddie = Instantiate(prefab, null);
                baddie.transform.position = new Vector3(Random.Range(zone.xMin, zone.xMax), Random.Range(zone.yMin, zone.yMax), 0.0f);

                var baddieComponent = baddie.GetComponent<Baddie>();
                baddieComponent.Target = Player;
                baddieComponent.Defeated.AddListener(OnBaddieDefeated);

                BaddieSpawned?.Invoke(baddieComponent);
                _baddies.Add(baddie);
            }
        }
    }

    // ********************************************************************************
    // Gameplay events
    public void OnBaddieDefeated(Baddie baddie)
    {
        _baddies.Remove(baddie.gameObject);

        if ((Random.value > FoodChance) && (_foodFails < FoodGuarantee))
        {
            _foodFails++;
            return;
        }

        var baddieRenderer = baddie.GetComponent<SpriteRenderer>();

        DropFood(baddie.transform.position, !baddieRenderer.flipX);
        _foodFails = 0;
    }

    // ********************************************************************************
    // Utilities
    private void DropFood(Vector3 position, bool flip)
    {
        GameObject foodPrefab = FoodPrefabs[Random.Range(0, FoodPrefabs.Length)];
        GameObject food = Instantiate(foodPrefab, null);
        food.transform.position = position;
        food.transform.localScale = new Vector3(flip ? -1f : 1f, 1f, 1f);
    }
}
