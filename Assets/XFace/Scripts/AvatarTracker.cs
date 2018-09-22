using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.iOS;

namespace XFace
{
    public class AvatarTracker : MonoBehaviour
    {
        private enum Model
        {
            Sana,
            Yuni,
            Haneru,
            Andelte,
            Fencer,
            Shaclo,
            VRoid
        }

        [SerializeField] private Model _targetModel = Model.Sana;
        [SerializeField] private Transform _targetPos;
        [SerializeField] private Transform _targetHeadRot;
        [SerializeField] private Transform _HeadRot2;
        [SerializeField] private Transform _HeadRot3;
        [SerializeField] private Transform _HeadRot4;
        [SerializeField] private SkinnedMeshRenderer _targetBlendShape;
        [SerializeField] private Transform _leftEyeRot;
        [SerializeField] private Transform _rightEyeRot;
        [SerializeField] private AnimationCurve _jawOpenCurve;
        [SerializeField] private AnimationCurve _eyeBlinkCurve;

        private Poser _poser;
        private Dictionary<string, float> _currentBlendShapes;

        // for sana_v1.01 https://536.booth.pm/items/990997
        private const int SanaLeftBlink = 2;
        private const int SanaRightBlink = 3;
        private const int SanaMihiraki = 6; // eyeWide
        private const int SanaMegashiraage = 9; // eyeWide
        private const int SanaA = 10; // jawOpen
        private const int SanaKoukakuage = 16; // mouthSmile
        private const int SanaWear = 20; // sanya

        // for yuni_v1.02 https://536.booth.pm/items/992269
        private const int YuniLeftBlink = 1;
        private const int YuniRightBlink = 2;
        private const int YuniMihiraki = 3; // eyeWide
        private const int YuniMegashiraage = 8; // eyeWide
        private const int YuniA = 10; // jawOpen
        private const int YuniKoukakuage = 16; // mouthSmile
        private const int YuniWear = 18; // megane

        // for InabaHaneru_MMDVer1.0.1 http://3d.nicovideo.jp/works/td41039
        private const int HaneruLeftBlink = 13;
        private const int HaneruRightBlink = 14;
        private const int HaneruMihiraki = 12; // eyeWide
        private const int HaneruA = 0; // jawOpen
        private const int HaneruMouthFlat = 7; // 1 - mouthSmile
        private const int HaneruWear = 28; // sychopath

        // for Andelte v1.6 https://booth.pm/ja/items/999401
        private const int AndelteLeftBlink = 21;
        private const int AndelteRightBlink = 22;
        // eyeWide not supported
        private const int AndelteA = 0; // jawOpen
        private const int AndelteMouthFlat = 10; // 1 - mouthSmile
        private const int AndelteWear = 30; // eye star
        private const int AndelteEyeUe = 25;
        private const int AndelteEyeMigi = 26;
        private const int AndelteEyeHidari = 27;
        private const int AndelteEyeSita = 28;

        // for Fencer / pondero 2018.08.26 https://booth.pm/ja/items/961361
        private const int FencerBlink = 20;
        private const int FencerMehiraki = 26; // eyeWide
        private const int FencerA = 15; // jawOpen
        private const int FencerNikkori = 28; // mouthSmile

        // for Shaclo / tomori_hikage v1.1.2 https://tomori-hikage.booth.pm/items/987296
        private const int ShacloLeftBlink = 20;
        private const int ShacloRightBlink = 19;
        private const int ShacloA = 4;
        private const int ShacloMouthNiko = 40;
        private const int ShacloDark = 30;
        private const int ShacloHighlightOff = 31;

        // for VRMYMei / VRoid Studio v0.2.11
        private const int VRoidLeftBlinkJoy = 16;
        private const int VRoidRightBlinkJoy = 15;
        private const int VRoidMouthJoy = 25; // jawOpen
        private const int VRoidMouthFun = 24; // mouthSmile
        private const int VRoidEyeExtra = 19; // >< 1
        private const int VRoidEyeExtraOn = 39; // >< 2

        private void Start()
        {
            if (_HeadRot2 && _HeadRot3 && _HeadRot4)
            {
                _poser = new Poser(_targetPos, _targetHeadRot, _HeadRot2, _HeadRot3, _HeadRot4);
            }
            else
            {
                _poser = new Poser(_targetPos, _targetHeadRot);
            }
        }

        public void UpdateTracking(Vector3 pos, Vector3 rot, Dictionary<string, float> blendShapes)
        {
            if (_poser != null)
            {
                var p = new Vector3(
                    pos.y,
                    -pos.x,
                    pos.z * 1.5f);
                var r = Quaternion.Euler(
                    -rot.y,
                    -rot.x,
                    -rot.z + 90.0f);

                _poser.Update(p, r);
            }

            _currentBlendShapes = blendShapes;
        }

        private bool _isWearing;

        public void ToggleWear()
        {
            if (!_targetBlendShape)
            {
                return;
            }

            _isWearing = !_isWearing;
            switch (_targetModel)
            {
                case Model.Sana:
                    _targetBlendShape.SetBlendShapeWeight(SanaWear, _isWearing ? 100 : 0);
                    break;
                case Model.Yuni:
                    _targetBlendShape.SetBlendShapeWeight(YuniWear, _isWearing ? 100 : 0);
                    break;
                case Model.Haneru:
                    _targetBlendShape.SetBlendShapeWeight(HaneruWear, _isWearing ? 100 : 0);
                    break;
                case Model.Andelte:
                    _targetBlendShape.SetBlendShapeWeight(AndelteWear, _isWearing ? 100 : 0);
                    break;
                case Model.Fencer:
                    SetActiveByName(_isWearing, transform, "mantle");
                    SetActiveByName(_isWearing, transform, "breastplate");
                    SetActiveByName(_isWearing, transform, "gauntlet");
                    SetActiveByName(_isWearing, transform, "hair_acc");
                    break;
                case Model.Shaclo:
                    _targetBlendShape.SetBlendShapeWeight(ShacloDark, _isWearing ? 100 : 0);
                    _targetBlendShape.SetBlendShapeWeight(ShacloHighlightOff, _isWearing ? 100 : 0);
                    break;
                case Model.VRoid:
                    _targetBlendShape.SetBlendShapeWeight(VRoidEyeExtra, _isWearing ? 100 : 0);
                    _targetBlendShape.SetBlendShapeWeight(VRoidEyeExtraOn, _isWearing ? 100 : 0);
                    break;
            }
        }

        private void LateUpdate()
        {
            if (_poser != null)
            {
                _poser.Apply();
            }
            UpdateBlendShapes();
        }

        private void UpdateBlendShapes()
        {
            if (!_targetBlendShape || _currentBlendShapes == null)
            {
                return;
            }

            float eyeWide = (_currentBlendShapes[ARBlendShapeLocation.EyeWideLeft]
                             + _currentBlendShapes[ARBlendShapeLocation.EyeWideRight]) * 0.5f;
            float mouthSmile = 0.2f + (_currentBlendShapes[ARBlendShapeLocation.MouthSmileLeft]
                                       + _currentBlendShapes[ARBlendShapeLocation.MouthSmileRight]) * 0.5f;
            if (_targetModel == Model.Sana)
            {
                // jawOpen
                _targetBlendShape.SetBlendShapeWeight(SanaA,
                    _jawOpenCurve.Evaluate(_currentBlendShapes[ARBlendShapeLocation.JawOpen]) * 100.0f);
                // eyeWide
                _targetBlendShape.SetBlendShapeWeight(SanaMihiraki, eyeWide * 100.0f);
                _targetBlendShape.SetBlendShapeWeight(SanaMegashiraage, eyeWide * 100.0f);
                // eyeBlink
                _targetBlendShape.SetBlendShapeWeight(SanaLeftBlink,
                    _eyeBlinkCurve.Evaluate(_currentBlendShapes[ARBlendShapeLocation.EyeBlinkLeft]) * 100.0f);
                _targetBlendShape.SetBlendShapeWeight(SanaRightBlink,
                    _eyeBlinkCurve.Evaluate(_currentBlendShapes[ARBlendShapeLocation.EyeBlinkRight]) * 100.0f);
                // mouthSmile
                _targetBlendShape.SetBlendShapeWeight(SanaKoukakuage, mouthSmile * 100.0f);
            }
            else if (_targetModel == Model.Yuni)
            {
                // jawOpen
                _targetBlendShape.SetBlendShapeWeight(YuniA,
                    _jawOpenCurve.Evaluate(_currentBlendShapes[ARBlendShapeLocation.JawOpen]) * 100.0f);
                // eyeWide
                _targetBlendShape.SetBlendShapeWeight(YuniMihiraki, eyeWide * 100.0f);
                _targetBlendShape.SetBlendShapeWeight(YuniMegashiraage, eyeWide * 100.0f);
                // eyeBlink
                _targetBlendShape.SetBlendShapeWeight(YuniLeftBlink,
                    _eyeBlinkCurve.Evaluate(_currentBlendShapes[ARBlendShapeLocation.EyeBlinkLeft]) * 100.0f);
                _targetBlendShape.SetBlendShapeWeight(YuniRightBlink,
                    _eyeBlinkCurve.Evaluate(_currentBlendShapes[ARBlendShapeLocation.EyeBlinkRight]) * 100.0f);
                // mouthSmile
                _targetBlendShape.SetBlendShapeWeight(YuniKoukakuage, mouthSmile * 100.0f);
            }
            else if (_targetModel == Model.Haneru)
            {
                // jawOpen
                _targetBlendShape.SetBlendShapeWeight(HaneruA,
                    _jawOpenCurve.Evaluate(_currentBlendShapes[ARBlendShapeLocation.JawOpen]) * 100.0f);
                // eyeWide
                _targetBlendShape.SetBlendShapeWeight(HaneruMihiraki, eyeWide * 100.0f);
                // eyeBlink
                _targetBlendShape.SetBlendShapeWeight(HaneruLeftBlink,
                    _eyeBlinkCurve.Evaluate(_currentBlendShapes[ARBlendShapeLocation.EyeBlinkLeft]) * 100.0f);
                _targetBlendShape.SetBlendShapeWeight(HaneruRightBlink,
                    _eyeBlinkCurve.Evaluate(_currentBlendShapes[ARBlendShapeLocation.EyeBlinkRight]) * 100.0f);
                // mouthSmile
                _targetBlendShape.SetBlendShapeWeight(HaneruMouthFlat, (0.8f - mouthSmile) * 100.0f);
            }
            else if (_targetModel == Model.Andelte)
            {
                // jawOpen
                _targetBlendShape.SetBlendShapeWeight(AndelteA,
                    _jawOpenCurve.Evaluate(_currentBlendShapes[ARBlendShapeLocation.JawOpen]) * 100.0f);
                // eyeWide not supported
                // eyeBlink
                _targetBlendShape.SetBlendShapeWeight(AndelteLeftBlink,
                    _eyeBlinkCurve.Evaluate(_currentBlendShapes[ARBlendShapeLocation.EyeBlinkLeft]) * 100.0f);
                _targetBlendShape.SetBlendShapeWeight(AndelteRightBlink,
                    _eyeBlinkCurve.Evaluate(_currentBlendShapes[ARBlendShapeLocation.EyeBlinkRight]) * 100.0f);
                // mouthSmile
                _targetBlendShape.SetBlendShapeWeight(AndelteMouthFlat, (0.6f - mouthSmile) * 100.0f);
                // look-right,left,up,down morph
                float lookRight = (_currentBlendShapes[ARBlendShapeLocation.EyeLookInLeft]
                                   + _currentBlendShapes[ARBlendShapeLocation.EyeLookOutRight]) * 0.5f;
                float lookLeft = (_currentBlendShapes[ARBlendShapeLocation.EyeLookOutLeft]
                                  + _currentBlendShapes[ARBlendShapeLocation.EyeLookInRight]) * 0.5f;
                float lookUp = (_currentBlendShapes[ARBlendShapeLocation.EyeLookUpLeft]
                                + _currentBlendShapes[ARBlendShapeLocation.EyeLookUpRight]) * 0.5f;
                float lookDown = (_currentBlendShapes[ARBlendShapeLocation.EyeLookDownLeft]
                                  + _currentBlendShapes[ARBlendShapeLocation.EyeLookDownLeft]) * 0.5f;
                _targetBlendShape.SetBlendShapeWeight(AndelteEyeMigi, lookRight * 100.0f);
                _targetBlendShape.SetBlendShapeWeight(AndelteEyeHidari, lookLeft * 100.0f);
                _targetBlendShape.SetBlendShapeWeight(AndelteEyeUe, lookUp * 80.0f);
                _targetBlendShape.SetBlendShapeWeight(AndelteEyeSita, lookDown * 80.0f);
            }
            else if (_targetModel == Model.Fencer)
            {
                // jawOpen
                _targetBlendShape.SetBlendShapeWeight(FencerA,
                    _jawOpenCurve.Evaluate(_currentBlendShapes[ARBlendShapeLocation.JawOpen]) * 100.0f);
                // eyeWide
                _targetBlendShape.SetBlendShapeWeight(FencerMehiraki, eyeWide * 100.0f);
                // eyeBlink
                float eyeBlink = (_currentBlendShapes[ARBlendShapeLocation.EyeBlinkLeft] +
                                  _currentBlendShapes[ARBlendShapeLocation.EyeBlinkRight]) * 0.5f;
                _targetBlendShape.SetBlendShapeWeight(FencerBlink,
                    _eyeBlinkCurve.Evaluate(eyeBlink) * 100.0f);
                // mouthSmile
                _targetBlendShape.SetBlendShapeWeight(FencerNikkori, mouthSmile * 100.0f);
            }
            else if (_targetModel == Model.Shaclo)
            {
                // jawOpen
                _targetBlendShape.SetBlendShapeWeight(ShacloA,
                    _jawOpenCurve.Evaluate(_currentBlendShapes[ARBlendShapeLocation.JawOpen]) * 100.0f);
                // eyeWide not supported
                // eyeBlink
                _targetBlendShape.SetBlendShapeWeight(ShacloLeftBlink,
                    _eyeBlinkCurve.Evaluate(_currentBlendShapes[ARBlendShapeLocation.EyeBlinkLeft]) * 100.0f);
                _targetBlendShape.SetBlendShapeWeight(ShacloRightBlink,
                    _eyeBlinkCurve.Evaluate(_currentBlendShapes[ARBlendShapeLocation.EyeBlinkRight]) * 100.0f);
                // mouthSmile
                _targetBlendShape.SetBlendShapeWeight(ShacloMouthNiko, mouthSmile * 100.0f);
            }
            else if (_targetModel == Model.VRoid)
            {
                // jawOpen
                _targetBlendShape.SetBlendShapeWeight(VRoidMouthJoy,
                    _jawOpenCurve.Evaluate(_currentBlendShapes[ARBlendShapeLocation.JawOpen]) * 100.0f);
                // eyeWide not supported
                // eyeBlink
                _targetBlendShape.SetBlendShapeWeight(VRoidLeftBlinkJoy,
                    _eyeBlinkCurve.Evaluate(_currentBlendShapes[ARBlendShapeLocation.EyeBlinkLeft]) * 100.0f);
                _targetBlendShape.SetBlendShapeWeight(VRoidRightBlinkJoy,
                    _eyeBlinkCurve.Evaluate(_currentBlendShapes[ARBlendShapeLocation.EyeBlinkRight]) * 100.0f);
                // mouthSmile
                _targetBlendShape.SetBlendShapeWeight(VRoidMouthFun, mouthSmile * 100.0f);                
            }

            // hidarime
            if (_leftEyeRot)
            {
                float lookInLeft = _currentBlendShapes[ARBlendShapeLocation.EyeLookInLeft];
                float lookOutLeft = _currentBlendShapes[ARBlendShapeLocation.EyeLookOutLeft];
                float lookUpLeft = _currentBlendShapes[ARBlendShapeLocation.EyeLookUpLeft];
                float lookDownLeft = _currentBlendShapes[ARBlendShapeLocation.EyeLookDownLeft];
                _leftEyeRot.transform.localEulerAngles = new Vector3(
                    (lookDownLeft - lookUpLeft) * 10.0f,
                    (lookInLeft - lookOutLeft) * 15.0f,
                    0);
            }

            // migime
            if (_rightEyeRot)
            {
                float lookInRight = _currentBlendShapes[ARBlendShapeLocation.EyeLookInRight];
                float lookOutRight = _currentBlendShapes[ARBlendShapeLocation.EyeLookOutRight];
                float lookUpRight = _currentBlendShapes[ARBlendShapeLocation.EyeLookUpRight];
                float lookDownRight = _currentBlendShapes[ARBlendShapeLocation.EyeLookDownRight];
                _rightEyeRot.transform.localEulerAngles = new Vector3(
                    (lookDownRight - lookUpRight) * 10.0f,
                    (lookOutRight - lookInRight) * 15.0f,
                    0);
            }
        }

        private void SetActiveByName(bool isActive, Transform root, string name)
        {
            var target = FindDeep(root, name);
            if (target)
            {
                target.gameObject.SetActive(isActive);
            }
        }

        private Transform FindDeep(Transform root, string name)
        {
            return System.Array.Find(root.GetComponentsInChildren<Transform>(true), c => c.name == name);
        }
    }
}