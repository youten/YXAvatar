using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XFace
{
    public class AvatarTracker : MonoBehaviour
    {
        private bool _isReceiver;

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
        [SerializeField] private Transform _headRot2;
        [SerializeField] private Transform _headRot3;
        [SerializeField] private Transform _headRot4;
        [SerializeField] private SkinnedMeshRenderer _targetBlendShape;
        [SerializeField] private Transform _leftEyeRot;
        [SerializeField] private Transform _rightEyeRot;
        [SerializeField] private AnimationCurve _jawOpenCurve;
        [SerializeField] private AnimationCurve _eyeBlinkCurve;

        private Poser _poser;
        private float[] _pose;

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
            if (_headRot2 && _headRot3 && _headRot4)
            {
                _poser = new Poser(_targetPos, _targetHeadRot, _headRot2, _headRot3, _headRot4);
            }
            else
            {
                _poser = new Poser(_targetPos, _targetHeadRot);
            }
#if UNITY_EDITOR || !UNITY_IOS
            // receive
            StartCoroutine(ReceiveLoop());
            _isReceiver = true;
#endif
        }

        public void UpdateTracking(Vector3 pos, Vector3 rot, Dictionary<string, float> blendShapes)
        {
            _pose = ARKitPose.ToArray(pos, rot, blendShapes);
            if (!_isReceiver)
            {
                const string server = "192.168.11.255";
                StartCoroutine(Send(server, ARKitPose.ToString(_pose)));
            }
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
            UpdatePose();
        }

        private void UpdatePose()
        {
            if (!_targetBlendShape || _pose == null)
            {
                return;
            }

            if (_poser != null)
            {
                var p = new Vector3(
                    _pose[ARKitPose.Index.PosY],
                    -_pose[ARKitPose.Index.PosX],
                    _pose[ARKitPose.Index.PosZ] * 1.5f);
                var r = Quaternion.Euler(
                    -_pose[ARKitPose.Index.RotY],
                    -_pose[ARKitPose.Index.RotX],
                    -_pose[ARKitPose.Index.RotZ] + 90.0f);

                _poser.Update(p, r);
            }

            float eyeWide = (_pose[ARKitPose.Index.EyeWideLeft]
                             + _pose[ARKitPose.Index.EyeWideRight]) * 0.5f;
            float mouthSmile = 0.2f + (_pose[ARKitPose.Index.MouthSmileLeft]
                                       + _pose[ARKitPose.Index.MouthSmileRight]) * 0.5f;
            if (_targetModel == Model.Sana)
            {
                // jawOpen
                _targetBlendShape.SetBlendShapeWeight(SanaA,
                    _jawOpenCurve.Evaluate(_pose[ARKitPose.Index.JawOpen]) * 100.0f);
                // eyeWide
                _targetBlendShape.SetBlendShapeWeight(SanaMihiraki, eyeWide * 100.0f);
                _targetBlendShape.SetBlendShapeWeight(SanaMegashiraage, eyeWide * 100.0f);
                // eyeBlink
                _targetBlendShape.SetBlendShapeWeight(SanaLeftBlink,
                    _eyeBlinkCurve.Evaluate(_pose[ARKitPose.Index.EyeBlinkLeft]) * 100.0f);
                _targetBlendShape.SetBlendShapeWeight(SanaRightBlink,
                    _eyeBlinkCurve.Evaluate(_pose[ARKitPose.Index.EyeBlinkRight]) * 100.0f);
                // mouthSmile
                _targetBlendShape.SetBlendShapeWeight(SanaKoukakuage, mouthSmile * 100.0f);
            }
            else if (_targetModel == Model.Yuni)
            {
                // jawOpen
                _targetBlendShape.SetBlendShapeWeight(YuniA,
                    _jawOpenCurve.Evaluate(_pose[ARKitPose.Index.JawOpen]) * 100.0f);
                // eyeWide
                _targetBlendShape.SetBlendShapeWeight(YuniMihiraki, eyeWide * 100.0f);
                _targetBlendShape.SetBlendShapeWeight(YuniMegashiraage, eyeWide * 100.0f);
                // eyeBlink
                _targetBlendShape.SetBlendShapeWeight(YuniLeftBlink,
                    _eyeBlinkCurve.Evaluate(_pose[ARKitPose.Index.EyeBlinkLeft]) * 100.0f);
                _targetBlendShape.SetBlendShapeWeight(YuniRightBlink,
                    _eyeBlinkCurve.Evaluate(_pose[ARKitPose.Index.EyeBlinkRight]) * 100.0f);
                // mouthSmile
                _targetBlendShape.SetBlendShapeWeight(YuniKoukakuage, mouthSmile * 100.0f);
            }
            else if (_targetModel == Model.Haneru)
            {
                // jawOpen
                _targetBlendShape.SetBlendShapeWeight(HaneruA,
                    _jawOpenCurve.Evaluate(_pose[ARKitPose.Index.JawOpen]) * 100.0f);
                // eyeWide
                _targetBlendShape.SetBlendShapeWeight(HaneruMihiraki, eyeWide * 100.0f);
                // eyeBlink
                _targetBlendShape.SetBlendShapeWeight(HaneruLeftBlink,
                    _eyeBlinkCurve.Evaluate(_pose[ARKitPose.Index.EyeBlinkLeft]) * 100.0f);
                _targetBlendShape.SetBlendShapeWeight(HaneruRightBlink,
                    _eyeBlinkCurve.Evaluate(_pose[ARKitPose.Index.EyeBlinkRight]) * 100.0f);
                // mouthSmile
                _targetBlendShape.SetBlendShapeWeight(HaneruMouthFlat, (0.8f - mouthSmile) * 100.0f);
            }
            else if (_targetModel == Model.Andelte)
            {
                // jawOpen
                _targetBlendShape.SetBlendShapeWeight(AndelteA,
                    _jawOpenCurve.Evaluate(_pose[ARKitPose.Index.JawOpen]) * 100.0f);
                // eyeWide not supported
                // eyeBlink
                _targetBlendShape.SetBlendShapeWeight(AndelteLeftBlink,
                    _eyeBlinkCurve.Evaluate(_pose[ARKitPose.Index.EyeBlinkLeft]) * 100.0f);
                _targetBlendShape.SetBlendShapeWeight(AndelteRightBlink,
                    _eyeBlinkCurve.Evaluate(_pose[ARKitPose.Index.EyeBlinkRight]) * 100.0f);
                // mouthSmile
                _targetBlendShape.SetBlendShapeWeight(AndelteMouthFlat, (0.6f - mouthSmile) * 100.0f);
                // look-right,left,up,down morph
                float lookRight = (_pose[ARKitPose.Index.EyeLookInLeft]
                                   + _pose[ARKitPose.Index.EyeLookOutRight]) * 0.5f;
                float lookLeft = (_pose[ARKitPose.Index.EyeLookOutLeft]
                                  + _pose[ARKitPose.Index.EyeLookInRight]) * 0.5f;
                float lookUp = (_pose[ARKitPose.Index.EyeLookUpLeft]
                                + _pose[ARKitPose.Index.EyeLookUpRight]) * 0.5f;
                float lookDown = (_pose[ARKitPose.Index.EyeLookDownLeft]
                                  + _pose[ARKitPose.Index.EyeLookDownLeft]) * 0.5f;
                _targetBlendShape.SetBlendShapeWeight(AndelteEyeMigi, lookRight * 100.0f);
                _targetBlendShape.SetBlendShapeWeight(AndelteEyeHidari, lookLeft * 100.0f);
                _targetBlendShape.SetBlendShapeWeight(AndelteEyeUe, lookUp * 80.0f);
                _targetBlendShape.SetBlendShapeWeight(AndelteEyeSita, lookDown * 80.0f);
            }
            else if (_targetModel == Model.Fencer)
            {
                // jawOpen
                _targetBlendShape.SetBlendShapeWeight(FencerA,
                    _jawOpenCurve.Evaluate(_pose[ARKitPose.Index.JawOpen]) * 100.0f);
                // eyeWide
                _targetBlendShape.SetBlendShapeWeight(FencerMehiraki, eyeWide * 100.0f);
                // eyeBlink
                float eyeBlink = (_pose[ARKitPose.Index.EyeBlinkLeft] +
                                  _pose[ARKitPose.Index.EyeBlinkRight]) * 0.5f;
                _targetBlendShape.SetBlendShapeWeight(FencerBlink,
                    _eyeBlinkCurve.Evaluate(eyeBlink) * 100.0f);
                // mouthSmile
                _targetBlendShape.SetBlendShapeWeight(FencerNikkori, mouthSmile * 100.0f);
            }
            else if (_targetModel == Model.Shaclo)
            {
                // jawOpen
                _targetBlendShape.SetBlendShapeWeight(ShacloA,
                    _jawOpenCurve.Evaluate(_pose[ARKitPose.Index.JawOpen]) * 100.0f);
                // eyeWide not supported
                // eyeBlink
                _targetBlendShape.SetBlendShapeWeight(ShacloLeftBlink,
                    _eyeBlinkCurve.Evaluate(_pose[ARKitPose.Index.EyeBlinkLeft]) * 100.0f);
                _targetBlendShape.SetBlendShapeWeight(ShacloRightBlink,
                    _eyeBlinkCurve.Evaluate(_pose[ARKitPose.Index.EyeBlinkRight]) * 100.0f);
                // mouthSmile
                _targetBlendShape.SetBlendShapeWeight(ShacloMouthNiko, mouthSmile * 100.0f);
            }
            else if (_targetModel == Model.VRoid)
            {
                // jawOpen
                _targetBlendShape.SetBlendShapeWeight(VRoidMouthJoy,
                    _jawOpenCurve.Evaluate(_pose[ARKitPose.Index.JawOpen]) * 100.0f);
                // eyeWide not supported
                // eyeBlink
                _targetBlendShape.SetBlendShapeWeight(VRoidLeftBlinkJoy,
                    _eyeBlinkCurve.Evaluate(_pose[ARKitPose.Index.EyeBlinkLeft]) * 100.0f);
                _targetBlendShape.SetBlendShapeWeight(VRoidRightBlinkJoy,
                    _eyeBlinkCurve.Evaluate(_pose[ARKitPose.Index.EyeBlinkRight]) * 100.0f);
                // mouthSmile
                _targetBlendShape.SetBlendShapeWeight(VRoidMouthFun, mouthSmile * 100.0f);
            }

            // hidarime
            if (_leftEyeRot)
            {
                float lookInLeft = _pose[ARKitPose.Index.EyeLookInLeft];
                float lookOutLeft = _pose[ARKitPose.Index.EyeLookOutLeft];
                float lookUpLeft = _pose[ARKitPose.Index.EyeLookUpLeft];
                float lookDownLeft = _pose[ARKitPose.Index.EyeLookDownLeft];
                _leftEyeRot.transform.localEulerAngles = new Vector3(
                    (lookDownLeft - lookUpLeft) * 10.0f,
                    (lookInLeft - lookOutLeft) * 15.0f,
                    0);
            }

            // migime
            if (_rightEyeRot)
            {
                float lookInRight = _pose[ARKitPose.Index.EyeLookInRight];
                float lookOutRight = _pose[ARKitPose.Index.EyeLookOutRight];
                float lookUpRight = _pose[ARKitPose.Index.EyeLookUpRight];
                float lookDownRight = _pose[ARKitPose.Index.EyeLookDownRight];
                _rightEyeRot.transform.localEulerAngles = new Vector3(
                    (lookDownRight - lookUpRight) * 10.0f,
                    (lookOutRight - lookInRight) * 15.0f,
                    0);
            }
        }

        private static void SetActiveByName(bool isActive, Transform root, string targetName)
        {
            var target = FindDeep(root, targetName);
            if (target)
            {
                target.gameObject.SetActive(isActive);
            }
        }

        private static Transform FindDeep(Transform root, string targetName)
        {
            return System.Array.Find(root.GetComponentsInChildren<Transform>(true), c => c.name == targetName);
        }

        // Ref. https://github.com/dsedb/UDPSampleForUnity/        
        private const int ListenPort = 11340;
        private static bool received;
        private static bool sent;
        private Coroutine sender;

        private struct UdpState
        {
            public System.Net.IPEndPoint e;
            public System.Net.Sockets.UdpClient u;
        }

        public void SendCallback(System.IAsyncResult ar)
        {
            // System.Net.Sockets.UdpClient u = (System.Net.Sockets.UdpClient)ar.AsyncState;
            sent = true;
        }

        private IEnumerator Send(string server, string message)
        {
            // Debug.Log("sending..");
            var u = new System.Net.Sockets.UdpClient();
            u.EnableBroadcast = true;
            u.Connect(server, ListenPort);
            var sendBytes = System.Text.Encoding.ASCII.GetBytes(message);
            sent = false;
            u.BeginSend(sendBytes, sendBytes.Length, SendCallback, u);
            while (!sent)
            {
                yield return null;
            }
            u.Close();
            sender = null;
            // Debug.Log("done.");
        }

        public void ReceiveCallback(System.IAsyncResult ar)
        {
            var u = ((UdpState) ar.AsyncState).u;
            var e = ((UdpState) ar.AsyncState).e;
            var receiveBytes = u.EndReceive(ar, ref e);
            var receiveString = System.Text.Encoding.ASCII.GetString(receiveBytes);
            received = true;

            _pose = ARKitPose.FromString(receiveString);
        }

        private IEnumerator ReceiveLoop()
        {
            var e = new System.Net.IPEndPoint(System.Net.IPAddress.Any, ListenPort);
            var u = new System.Net.Sockets.UdpClient(e);
            u.EnableBroadcast = true;
            var s = new UdpState();
            s.e = e;
            s.u = u;
            for (;;)
            {
                received = false;
                u.BeginReceive(ReceiveCallback, s);
                while (!received)
                {
                    yield return null;
                }
            }
        }
    }
}