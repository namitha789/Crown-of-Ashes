using UnityEngine;

public class CommandCenter : MonoBehaviour
{
    [Header("Command Center Settings")]
    [SerializeField] private int _maxHealth = 300;
    [SerializeField] private bool _isPlayerCommandCenter = true;

    private int _currentHealth;
    private bool _isDestroyed = false;

    // Singleton for player Command Center
    public static CommandCenter PlayerInstance { get; private set; }

    public int CurrentHealth => _currentHealth;
    public int MaxHealth => _maxHealth;
    public bool IsDestroyed => _isDestroyed;
    public bool IsPlayerCommandCenter => _isPlayerCommandCenter;
    public Vector3 Position => transform.position;

    private void Awake()
    {
        if (_isPlayerCommandCenter)
        {
            if (PlayerInstance != null && PlayerInstance != this)
            {
                Debug.LogWarning("Multiple player Command Centers found! Destroying duplicate.");
                Destroy(gameObject);
                return;
            }
            PlayerInstance = this;
        }
    }

    private void Start()
    {
        _currentHealth = _maxHealth;
        Debug.Log($"Command Center initialized with {_maxHealth} HP");
    }

    public void TakeDamage(int damage)
    {
        if (_isDestroyed) return;

        _currentHealth -= damage;
        _currentHealth = Mathf.Max(_currentHealth, 0);

        Debug.Log($"Command Center took {damage} damage! HP: {_currentHealth}/{_maxHealth}");

        // Trigger event for UI update
        EventManager.TriggerEvent("CommandCenterDamaged", this);

        if (_currentHealth <= 0)
        {
            DestroyCommandCenter();
        }
    }

    private void DestroyCommandCenter()
    {
        if (_isDestroyed) return;

        _isDestroyed = true;

        Debug.Log($"Command Center DESTROYED! Player: {_isPlayerCommandCenter}");

        if (_isPlayerCommandCenter)
        {
            // Player loses!
            EventManager.TriggerEvent("GameDefeat", this);
        }
        else
        {
            // Enemy Command Center destroyed (if you add enemy base later)
            EventManager.TriggerEvent("EnemyBaseDestroyed", this);
        }

        // Visual feedback (you can add explosion VFX here later)

        // Destroy after delay to allow VFX
        Destroy(gameObject, 2f);
    }

    private void OnDestroy()
    {
        if (_isPlayerCommandCenter && PlayerInstance == this)
        {
            PlayerInstance = null;
        }
    }

    // For debugging
    private void OnDrawGizmos()
    {
        if (_isPlayerCommandCenter)
        {
            Gizmos.color = Color.blue;
        }
        else
        {
            Gizmos.color = Color.red;
        }
        Gizmos.DrawWireSphere(transform.position, 3f);
    }
    
    // private void Update()
    // {

    //     if (Input.GetKeyDown(KeyCode.T))
    //     {
    //         TakeDamage(40);
    //     }

    // }
}