using System;
using UnityEngine;
using TouchScript.Gestures;
using TouchScript.Gestures.TransformGestures;

namespace XFace
{
    // Touch Script 9.0 https://github.com/TouchScript/TouchScript/releases
    [RequireComponent(typeof(TransformGesture))]
    [RequireComponent(typeof(FlickGesture))]
    [RequireComponent(typeof(LongPressGesture))]
    public class TouchEventHandler : MonoBehaviour
    {
        private TransformGesture _transformGesture;
        private FlickGesture _flickGesture;
        private LongPressGesture _longPressGesture;

        [SerializeField] private Transform _targetCamera;
        [SerializeField] private ARKitTracker _targetArKitTracker;
        [SerializeField] private Canvas _uiCanvas;

        private void OnEnable()
        {
            if (_targetCamera)
            {
                transform.SetParent(_targetCamera.transform);
            }

            _transformGesture = GetComponent<TransformGesture>();
            _transformGesture.Transformed += OnTransformed;

            _flickGesture = GetComponent<FlickGesture>();
            _flickGesture.Flicked += OnFlicked;

            _longPressGesture = GetComponent<LongPressGesture>();
            _longPressGesture.LongPressed += OnLongPressed;
        }

        private void OnDisable()
        {
            _transformGesture.Transformed -= OnTransformed;

            _flickGesture.Flicked -= OnFlicked;

            _longPressGesture.LongPressed -= OnLongPressed;
        }

        private void OnTransformed(object sender, EventArgs e)
        {
            // Debug.Log("OnTransformed " + _transformGesture.DeltaPosition.y + " " + _transformGesture.DeltaPosition.x);
            if (_targetCamera)
            {
                _targetCamera.localPosition = new Vector3(
                    _targetCamera.localPosition.x,
                    _targetCamera.localPosition.y,
                    _targetCamera.localPosition.z + _transformGesture.DeltaPosition.y * 2.0f);
            }

            if (_targetArKitTracker)
            {
                _targetArKitTracker.transform.localEulerAngles = new Vector3(
                    _targetArKitTracker.transform.localEulerAngles.x,
                    _targetArKitTracker.transform.localEulerAngles.y - _transformGesture.DeltaPosition.x * 200.0f,
                    _targetArKitTracker.transform.localEulerAngles.z);
            }
        }

        private void OnFlicked(object sender, EventArgs e)
        {
            // Debug.Log("OnFlicked " + _flickGesture.ScreenFlickVector.x + " " + _flickGesture.ScreenFlickVector.y);
            if (!_targetArKitTracker)
            {
                return;
            }
            if (_flickGesture.State != Gesture.GestureState.Recognized)
            {
                return;
            }

            var vec = _flickGesture.ScreenFlickVector;
            if (Mathf.Abs(vec.y) > Mathf.Abs(vec.x))
            {
                if (vec.y > 0)
                {
                    // toggle UI show/hide
                    if (_uiCanvas)
                    {
                        _uiCanvas.enabled = !_uiCanvas.enabled;
                    }
                }
            }
            else
            {
                _targetArKitTracker.SwitchAvatar(vec.x > 0);
            }
        }

        private void OnLongPressed(object sender, EventArgs e)
        {
            _targetArKitTracker.ToggleActiveAvatarWear();
        }
    }
}