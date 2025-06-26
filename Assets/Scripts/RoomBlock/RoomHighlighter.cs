using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class RoomHighlighter : MonoBehaviour
{
    [Header("高亮渐变时间（秒）")]
    public float transitionTime = 0.2f;
    
    [Header("高亮强度")]
    public float hightLightStrength = 0.2f;

    private Material _mat;
    private float _targetValue = 0f;
    private float _currentValue = 0f;

    private float _transitionVelocity;

    private void Awake()
    {
        var sr = GetComponent<SpriteRenderer>();
        _mat = sr.material;
        if (_mat.HasProperty("_HightLightStrength"))
        {
            _currentValue = _mat.GetFloat("_HightLightStrength");
        }
    }

    private void OnMouseEnter()
    {
        hightLightStrength = 0.15f;
        _targetValue = hightLightStrength;
    }

    private void OnMouseExit()
    {
        _targetValue = 0f;
    }

    private void Update()
    {
        if (_mat == null || !_mat.HasProperty("_HightLightStrength"))
            return;

        // 平滑插值（基于时间）
        _currentValue = Mathf.MoveTowards(_currentValue, _targetValue, Time.deltaTime / transitionTime);
        _mat.SetFloat("_HightLightStrength", _currentValue);
    }
}