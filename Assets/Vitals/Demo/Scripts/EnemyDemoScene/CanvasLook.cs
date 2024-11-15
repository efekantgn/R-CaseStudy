using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Vitals
{
    public class CanvasLook : MonoBehaviour
    {
        [SerializeField] private Transform _cameraTransform;

        private void Awake()
        {
            if (_cameraTransform == null)
                _cameraTransform = Camera.main.transform;

        }
        void Update()
        {
            transform.forward = _cameraTransform.forward;
            transform.right = _cameraTransform.right;
        }
    }
}
