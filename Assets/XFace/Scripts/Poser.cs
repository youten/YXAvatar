using UnityEngine;

namespace XFace
{
    public class Poser
    {
        private readonly Transform _targetRoot;
        private readonly Vector3 _defaultRootPos;
        private readonly Transform[] _targetHeads;
        private readonly Quaternion[] _defaultHeadsRot;

        public Poser(Transform posRoot, params Transform[] rotHeads)
        {
            _targetRoot = posRoot;
            _targetHeads = rotHeads;
            _defaultRootPos = posRoot.localPosition;
            _defaultHeadsRot = new Quaternion[rotHeads.Length];
            for (int i = 0; i < rotHeads.Length; i++)
            {
                _defaultHeadsRot[i] = rotHeads[i].localRotation;
            }
        }

        public void Update(Vector3 pos, Quaternion rot)
        {
            _targetRoot.position = pos;

            if (_targetHeads.Length == 1)
            {
                _targetHeads[0].localRotation = _defaultHeadsRot[0] * rot;
            }
            else if (_targetHeads.Length == 4)
            {
                _targetHeads[0].localRotation =
                    _defaultHeadsRot[0] * Quaternion.Lerp(_defaultHeadsRot[0], rot, 0.5f);
                _targetHeads[1].localRotation =
                    _defaultHeadsRot[1] * Quaternion.Lerp(_defaultHeadsRot[1], rot, 0.35f);
                _targetHeads[2].localRotation =
                    _defaultHeadsRot[2] * Quaternion.Lerp(_defaultHeadsRot[2], rot, 0.1f);
                _targetHeads[3].localRotation =
                    _defaultHeadsRot[3] * Quaternion.Lerp(_defaultHeadsRot[3], rot, 0.05f);
            }
        }
    }
}