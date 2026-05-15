using UnityEngine;

public class HeavyChargeEffect : MonoBehaviour
{
    [Header("References")]
    [Tooltip("The visual mesh child to shake — NOT the root capsule.")]
    public Transform visualRoot;

    [Tooltip("All renderers on the player that should tint red.")]
    public Renderer[] renderers;

    [Header("Shake")]
    public float maxShakeAmount = 0.04f;

    [Header("Tint")]
    public Color chargeColor = new Color(1f, 0.15f, 0.15f, 1f);
    public float tintSpeed = 4f;

    private Weapon _weapon;
    private Material[][] _materials;
    private Color[] _baseColors;
    private Vector3 _visualOrigin;
    private bool _initialized;

    private void Start()
    {
        _weapon = GetComponentInChildren<Weapon>();

        if (_weapon == null)
        {
            Debug.LogWarning("[HeavyChargeEffect] No Weapon found in children.");
            enabled = false;
            return;
        }

        if (visualRoot == null)
        {
            Debug.LogWarning("[HeavyChargeEffect] visualRoot not assigned.");
            enabled = false;
            return;
        }

        _visualOrigin = visualRoot.localPosition;

        // Cache instanced materials so we don't dirty shared assets
        _materials = new Material[renderers.Length][];
        int totalMats = 0;
        for (int i = 0; i < renderers.Length; i++)
        {
            _materials[i] = renderers[i].materials; // creates instances
            totalMats += _materials[i].Length;
        }

        _baseColors = new Color[totalMats];
        int idx = 0;
        foreach (var matArr in _materials)
            foreach (var mat in matArr)
                _baseColors[idx++] = mat.color;

        _initialized = true;
    }

    private void Update()
    {
        if (!_initialized) return;

        float t = _weapon.HeavyChargeNormalized;

        // -- Shake --
        if (t > 0f)
        {
            float mag = t * maxShakeAmount;
            visualRoot.localPosition = _visualOrigin + new Vector3(
                Random.Range(-mag, mag),
                Random.Range(-mag, mag),
                Random.Range(-mag, mag)
            );
        }
        else
        {
            visualRoot.localPosition = _visualOrigin;
        }

        // -- Tint --
        int idx = 0;
        foreach (var matArr in _materials)
        {
            foreach (var mat in matArr)
            {
                mat.color = Color.Lerp(mat.color,
                    t > 0f ? Color.Lerp(_baseColors[idx], chargeColor, t) : _baseColors[idx],
                    Time.deltaTime * tintSpeed);
                idx++;
            }
        }
    }

    private void OnDestroy()
    {
        // Restore originals if object is destroyed mid-charge
        if (!_initialized) return;
        int idx = 0;
        foreach (var matArr in _materials)
            foreach (var mat in matArr)
                mat.color = _baseColors[idx++];
    }
}