using UnityEngine;
using UnityEngine.XR.iOS;

namespace XFace
{
    public class ARKitTracker : MonoBehaviour
    {
        private UnityARSessionNativeInterface m_session;

        [SerializeField] private AvatarTracker[] _avatarList;

        private void Start()
        {
            m_session = UnityARSessionNativeInterface.GetARSessionNativeInterface();

            Application.targetFrameRate = 60;
            ARKitFaceTrackingConfiguration config = new ARKitFaceTrackingConfiguration();
            // ARではなくてAvatarシステムとかだと非Gravityが楽
            config.alignment = UnityARAlignment.UnityARAlignmentCamera;
            config.enableLightEstimation = true;

            if (config.IsSupported)
            {
                m_session.RunWithConfig(config);
                UnityARSessionNativeInterface.ARFaceAnchorUpdatedEvent += FaceUpdated;
            }
        }

        private void FaceUpdated(ARFaceAnchor anchorData)
        {
            foreach (var avatar in _avatarList)
            {
                if (avatar.gameObject.activeInHierarchy)
                {
                    var pos = UnityARMatrixOps.GetPosition(anchorData.transform);
                    var rot = UnityARMatrixOps.GetRotation(anchorData.transform).eulerAngles;
                    avatar.UpdateTracking(pos, rot, anchorData.blendShapes);
                    break;
                }
            }
        }

        public void SwitchAvatar(bool isForward = true)
        {
            int now = 0;
            for (int i = 0; i < _avatarList.Length; i++)
            {
                if (_avatarList[i].gameObject.activeInHierarchy)
                {
                    now = i;
                    break;
                }
            }
            if (isForward)
            {
                now++;
                if (now >= _avatarList.Length)
                {
                    now = 0;
                }
            }
            else
            {
                now--;
                if (now < 0)
                {
                    now = _avatarList.Length - 1;
                }
            }
            for (int i = 0; i < _avatarList.Length; i++)
            {
                if (i == now)
                {
                    _avatarList[i].gameObject.SetActive(true);
                }
                else
                {
                    _avatarList[i].gameObject.SetActive(false);
                }
            }
        }

        public void ToggleActiveAvatarWear()
        {
            foreach (var avatar in _avatarList)
            {
                if (avatar.gameObject.activeInHierarchy)
                {
                    avatar.ToggleWear();
                    break;
                }
            }
        }
    }
}