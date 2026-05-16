using System.Collections;
using UnityEngine;

public class EnemyShooter : MonoBehaviour
{
    [Header("Shooting")]
    [SerializeField] private EnemyProjectileBase projectilePrefab;
    [SerializeField] private Transform firePoint;
    //[SerializeField] private Transform target;

    [Header("Timing")]
    [SerializeField] private float fireCooldown = 1.5f;
    [SerializeField] private bool fireAutomatically = true;

    private Material[] _materials;
    private Color[] _baseColors;
    private readonly Color _telegraphColor = new Color(1f, 0.15f, 0.15f, 1f);
    private const float _telegraphDuration = 0.18f;
    private const float _tintFadeSpeed = 6f;

    private bool canFire = true;
    public bool enable = false;
    private EnemyStatus status;

    private void Awake()
    {
        status = GetComponent<EnemyStatus>();
        CacheMaterialsForAtkIndicator();
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
        StartCoroutine(ThingyFire());
    }

    private IEnumerator ThingyFire()
    {
        canFire = false;

        foreach (var mat in _materials)
            SetMaterialColor(mat, _telegraphColor);

        yield return new WaitForSeconds(_telegraphDuration);

        Vector3 direction = firePoint.forward;
        direction.y = 0f;

        EnemyProjectileBase projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        projectile.Setup(direction, gameObject);
        SoundManager.PlaySound("Fire");

        StartCoroutine(FadeBackToBase());
        StartCoroutine(FireCooldownRoutine());
    }

    private IEnumerator FadeBackToBase()
    {
        float elapsed = 0f;
        Color[] startColors = new Color[_materials.Length];

        for (int i = 0; i < _materials.Length; i++)
            startColors[i] = GetMaterialColor(_materials[i]);

        while (elapsed < 1f)
        {
            elapsed += Time.deltaTime * _tintFadeSpeed;

            for (int i = 0; i < _materials.Length; i++)
                SetMaterialColor(_materials[i], Color.Lerp(startColors[i], _baseColors[i], elapsed));

            yield return null;
        }

        for (int i = 0; i < _materials.Length; i++)
            SetMaterialColor(_materials[i], _baseColors[i]);
    }

    private IEnumerator FireCooldownRoutine()
    {
        canFire = false;

        yield return new WaitForSeconds(fireCooldown);

        canFire = true;
    }

    //public void SetTarget(Transform t) => target = t;

    private void OnDestroy()
    {
        if (_materials == null) return;

        for (int i = 0; i < _materials.Length; i++)
        {
            if (_materials[i] != null)
                SetMaterialColor(_materials[i], _baseColors[i]);
        }
    }

    private Color GetMaterialColor(Material mat)
    {
        if (mat == null) return Color.white;

        if (mat.HasProperty("_Color"))
            return mat.GetColor("_Color");

        if (mat.HasProperty("_BaseColor"))
            return mat.GetColor("_BaseColor");

        return Color.white;
    }

    private void SetMaterialColor(Material mat, Color color)
    {
        if (mat == null) return;

        if (mat.HasProperty("_Color"))
            mat.SetColor("_Color", color);
        else if (mat.HasProperty("_BaseColor"))
            mat.SetColor("_BaseColor", color);
    }

    private void CacheMaterialsForAtkIndicator()
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        int total = 0;

        foreach (var r in renderers)
            total += r.materials.Length;

        _materials = new Material[total];
        _baseColors = new Color[total];

        int idx = 0;

        foreach (var r in renderers)
        {
            foreach (var mat in r.materials)
            {
                _materials[idx] = mat;
                _baseColors[idx] = GetMaterialColor(mat);
                idx++;
            }
        }
    }
}
