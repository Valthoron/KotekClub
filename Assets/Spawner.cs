using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public partial class Spawner : MonoBehaviour
{
    // ********************************************************************************
    // Structs/enums

    // ********************************************************************************
    // Constants
    const float _spawnPeriod = 3.5f;
    const float _maxBaddies = 3.0f;

    // ********************************************************************************
    // Members
    private float _timer = 0.0f;
    private List<GameObject> _baddies = new();

    // ********************************************************************************
    // Properties
    public bool Active = true;
    public GameObject[] BaddiePrefabs;
    public Rect[] SpawnZones;
    public Dude Player;

    // ********************************************************************************
    // Events
    public UnityEvent<Baddie> BaddieSpawned;

    // ********************************************************************************
    // Unity messagees
    void Update()
    {
        if (!Active)
            return;

        _timer -= Time.deltaTime;

        if (_timer < 0.0f)
        {
            _timer = Random.Range(0.5f, _spawnPeriod);

            _baddies.RemoveAll(b => b == null);

            if (_baddies.Count < _maxBaddies)
            {
                Rect zone = SpawnZones[Random.Range(0, SpawnZones.Length)];

                GameObject prefab = BaddiePrefabs[Random.Range(0, BaddiePrefabs.Length)];
                GameObject baddie = Instantiate(prefab, transform);
                baddie.transform.position = new Vector3(Random.Range(zone.xMin, zone.xMax), Random.Range(zone.yMin, zone.yMax), 0.0f);

                if (baddie.TryGetComponent<Baddie>(out var baddieComponent))
                {
                    baddieComponent.Player = Player;
                    BaddieSpawned?.Invoke(baddieComponent);
                }

                _baddies.Add(baddie);
            }
        }
    }
}
