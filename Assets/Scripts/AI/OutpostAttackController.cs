using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutpostAttackController : MonoBehaviour
{
    [Header("Attack Settings")]
    [SerializeField] private GameObject _enemyUnitPrefab;
    [SerializeField] private Transform[] _spawnPoints;
    [SerializeField] private float _raidInterval = 90f; // Periodic raids every 90 seconds
    [SerializeField] private int _raidsPerInterval = 1; // How many units to send per raid
    [SerializeField] private int _counterattackUnits = 3; // Units sent when damaged
    [SerializeField] private float _counterattackCooldown = 15f; // Prevent spam
    [SerializeField] private int _finalAssaultUnits = 5; // All-in attack at low HP

    [Header("Status")]
    [SerializeField] private bool _attacksEnabled = true;

    private SimpleOutpost _outpost;
    private float _nextRaidTime;
    private float _lastCounterattackTime;
    private bool _finalAssaultTriggered = false;
    private int _spawnPointIndex = 0;

    private void Start()
    {
        _outpost = GetComponent<SimpleOutpost>();

        if (_outpost == null)
        {
            enabled = false;
            return;
        }

        if (_enemyUnitPrefab == null)
        {
            enabled = false;
            return;
        }

        if (_spawnPoints == null || _spawnPoints.Length == 0)
        {
            enabled = false;
            return;
        }

        // Schedule first raid
        _nextRaidTime = Time.time + _raidInterval;
        _lastCounterattackTime = Time.time - _counterattackCooldown; // Allow immediate first counterattack

        // Listen for damage events
        EventManager.StartListening("OutpostDamaged", OnOutpostDamaged);
    }

    private void OnDestroy()
    {
        EventManager.StopListening("OutpostDamaged", OnOutpostDamaged);
    }

    private void Update()
    {
        if (!_attacksEnabled || _outpost == null || _outpost.IsDestroyed)
            return;

        // Check for periodic raids
        if (Time.time >= _nextRaidTime)
        {
            LaunchPeriodicRaid();
            _nextRaidTime = Time.time + _raidInterval;
        }

        // Check for final assault trigger (below 50% HP)
        if (!_finalAssaultTriggered && _outpost.CurrentHealth < (_outpost.MaxHealth * 0.5f))
        {
            LaunchFinalAssault();
            _finalAssaultTriggered = true;
        }
    }

    private void OnOutpostDamaged(object data)
    {
        // Check if this is OUR outpost that was damaged
        if (data is SimpleOutpost damagedOutpost && damagedOutpost == _outpost)
        {
            // Launch counterattack if cooldown expired
            if (Time.time >= _lastCounterattackTime + _counterattackCooldown)
            {
                LaunchCounterattack();
                _lastCounterattackTime = Time.time;
            }
        }
    }

    private void LaunchPeriodicRaid()
    {
        if (CommandCenter.PlayerInstance == null || CommandCenter.PlayerInstance.IsDestroyed)
        {
            return;
        }

        for (int i = 0; i < _raidsPerInterval; i++)
        {
            SpawnAttacker();
        }
    }

    private void LaunchCounterattack()
    {
        if (CommandCenter.PlayerInstance == null || CommandCenter.PlayerInstance.IsDestroyed)
        {
            return;
        }

        for (int i = 0; i < _counterattackUnits; i++)
        {
            SpawnAttacker();
        }
    }

    private void LaunchFinalAssault()
    {
        if (CommandCenter.PlayerInstance == null || CommandCenter.PlayerInstance.IsDestroyed)
        {
            return;
        }

        for (int i = 0; i < _finalAssaultUnits; i++)
        {
            SpawnAttacker();
        }
    }

    private void SpawnAttacker()
    {
        if (CommandCenter.PlayerInstance == null)
            return;

        // Get spawn position
        Vector3 spawnPos = _spawnPoints[_spawnPointIndex].position;
        _spawnPointIndex = (_spawnPointIndex + 1) % _spawnPoints.Length;

        // Spawn enemy unit
        GameObject enemyObj = Instantiate(_enemyUnitPrefab, spawnPos, Quaternion.identity);

        // Make it attack Command Center
        Unit enemyUnit = enemyObj.GetComponent<Unit>();
        if (enemyUnit != null)
        {
            // Give unit the attack order via AI
            AttackCommandCenterAI attackAI = enemyObj.AddComponent<AttackCommandCenterAI>();
            attackAI.Initialize(CommandCenter.PlayerInstance.gameObject);
        }
    }
}
