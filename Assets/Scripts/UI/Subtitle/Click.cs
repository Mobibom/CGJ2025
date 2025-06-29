using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Click : MonoBehaviour
{
    [SerializeField] private UnityEngine.Events.UnityEvent onFinished;
    private void OnMouseDown()
    {
        onFinished?.Invoke();
    }
}
