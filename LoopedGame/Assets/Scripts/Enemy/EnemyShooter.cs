using System.Collections;
using UnityEngine;

public class EnemyShooter : MonoBehaviour
{
    [Header("Shooting")]
    [SerializeField] private EnemyProjectileBase projectilePrefab;
    [SerializeField] private Transform firePoint;

    [Header("Timing")]
    [SerializeField] private float fireCooldown = 1.5f;
    [SerializeField] private bool fireAutomatically = true;

    private bool canFire = true;
    public bool enable = false;

    private EnemyStatus status;

    private Material[] _materials;
    private Color[] _baseColors;
    private readonly Color _telegraphColor = new Color(1f, 0.15f, 0.15f, 1f);
    private const float _telegraphDuration = 0.18f;
    private const float _tintFadeSpeed = 6f;

    private void Awake()
    {
        status = GetComponent<EnemyStatus>();
        CacheMaterials();
    }

    private void CacheMaterials()
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        int total = 0;
        foreach (var r in renderers) total += r.materials.Length;

        _materials = new Material[total];
        _baseColors = new Color[total];

        int idx = 0;
        foreach (var r in renderers)
            foreach (var mat in r.materials)
            {
                _materials[idx] = mat;
                _baseColors[idx] = mat.color;
                idx++;
            }
    }

    private void Update()
    {
        if (enable)
        {
            if (!fireAutomatically) return;
            if (status != null && status.IsStunned) return;
            if (canFire)
            {
                FireAtTarget();
            }
        }
    }

    public void FireAtTarget()
    {
        if (projectilePrefab == null) { Debug.LogWarning("[EnemyShooter] No projectile prefab assigned."); return; }
        if (firePoint == null) { Debug.LogWarning("[EnemyShooter] No fire point assigned."); return; }
        StartCoroutine(TelegraphAndFire());
    }

    private IEnumerator TelegraphAndFire()
    {
        canFire = false;

        foreach (var mat in _materials) mat.color = _telegraphColor;

        yield return new WaitForSeconds(_telegraphDuration);

        Vector3 direction = firePoint.forward;
        direction.y = 0f;

        EnemyProjectileBase projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        projectile.Setup(direction, gameObject);

        StartCoroutine(FadeBackToBase());
        StartCoroutine(FireCooldownRoutine());
    }

    private IEnumerator FadeBackToBase()
    {
        float elapsed = 0f;
        Color[] startColors = new Color[_materials.Length];
        for (int i = 0; i < _materials.Length; i++) startColors[i] = _materials[i].color;

        while (elapsed < 1f)
        {
            elapsed += Time.deltaTime * _tintFadeSpeed;
            for (int i = 0; i < _materials.Length; i++)
                _materials[i].color = Color.Lerp(startColors[i], _baseColors[i], elapsed);
            yield return null;
        }

        for (int i = 0; i < _materials.Length; i++) _materials[i].color = _baseColors[i];
    }

    private IEnumerator FireCooldownRoutine()
    {
        yield return new WaitForSeconds(fireCooldown);
        canFire = true;
    }

    private void OnDestroy()
    {
        if (_materials == null) return;
        for (int i = 0; i < _materials.Length; i++)
            if (_materials[i] != null) _materials[i].color = _baseColors[i];
    }
}