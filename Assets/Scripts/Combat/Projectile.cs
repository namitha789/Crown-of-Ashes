using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float _speed = 12f;
    private Vector3 _targetPosition;

    private void Start()
    {
        Destroy(gameObject, 2f); // failsafe
    }

    private void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, _targetPosition, _speed * Time.deltaTime);
        if (Vector3.Distance(transform.position, _targetPosition) < 0.05f)
        {
            Destroy(gameObject);
        }
    }

    public void SetTarget(Vector3 target)
    {
        _targetPosition = target;
        transform.LookAt(target);
    }
}
