using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.XR.iOS;

namespace XFace
{
    public static class ARKitFace
    {
        private const string Header = "ARKF0001"; // ARKit Face ver.1
        private const char Comma = ',';

        private static class Index
        {
            public const int PosX = 1;
            public const int PosY = 2;
            public const int PosZ = 3;
            public const int RotX = 4;
            public const int RotY = 5;
            public const int RotZ = 6;
            public const int EyeWideLeft = 7;
            public const int EyeWideRight = 8;
            public const int MouthSmileLeft = 9;
            public const int MouthSmileRight = 10;
            public const int JawOpen = 11;
            public const int EyeBlinkLeft = 12;
            public const int EyeBlinkRight = 13;
            public const int EyeLookInLeft = 14;
            public const int EyeLookOutLeft = 15;
            public const int EyeLookUpLeft = 16;
            public const int EyeLookDownLeft = 17;
            public const int EyeLookInRight = 18;
            public const int EyeLookOutRight = 19;
            public const int EyeLookUpRight = 20;
            public const int EyeLookDownRight = 21;
            public const int Max = EyeLookDownRight;
        }

        public static float[] ToArray(Vector3 pos, Vector3 rot, Dictionary<string, float> blendShapes)
        {
            if (pos == null || rot == null || blendShapes == null)
            {
                throw new ArgumentException("null parameter(s)");
            }

            var array = new float[Index.Max + 1];
            array[Index.PosX] = pos.x;
            array[Index.PosY] = pos.y;
            array[Index.PosZ] = pos.z;
            array[Index.RotX] = rot.x;
            array[Index.RotY] = rot.y;
            array[Index.RotZ] = rot.z;
            array[Index.EyeWideLeft] = blendShapes[ARBlendShapeLocation.EyeWideLeft];
            array[Index.EyeWideRight] = blendShapes[ARBlendShapeLocation.EyeWideRight];
            array[Index.MouthSmileLeft] = blendShapes[ARBlendShapeLocation.MouthSmileLeft];
            array[Index.MouthSmileRight] = blendShapes[ARBlendShapeLocation.MouthSmileRight];
            array[Index.JawOpen] = blendShapes[ARBlendShapeLocation.JawOpen];
            array[Index.EyeBlinkLeft] = blendShapes[ARBlendShapeLocation.EyeBlinkLeft];
            array[Index.EyeBlinkRight] = blendShapes[ARBlendShapeLocation.EyeBlinkRight];
            array[Index.EyeLookInLeft] = blendShapes[ARBlendShapeLocation.EyeLookInLeft];
            array[Index.EyeLookOutLeft] = blendShapes[ARBlendShapeLocation.EyeLookOutLeft];
            array[Index.EyeLookUpLeft] = blendShapes[ARBlendShapeLocation.EyeLookUpLeft];
            array[Index.EyeLookDownLeft] = blendShapes[ARBlendShapeLocation.EyeLookDownLeft];
            array[Index.EyeLookInRight] = blendShapes[ARBlendShapeLocation.EyeLookInRight];
            array[Index.EyeLookOutRight] = blendShapes[ARBlendShapeLocation.EyeLookOutRight];
            array[Index.EyeLookUpRight] = blendShapes[ARBlendShapeLocation.EyeLookUpRight];
            array[Index.EyeLookDownRight] = blendShapes[ARBlendShapeLocation.EyeLookDownRight];

            return array;
        }

        public static string ToString(float[] array)
        {
            if (array == null || array.Length < Index.Max + 1)
            {
                throw new ArgumentException("array is null or too short");
            }

            var sb = new StringBuilder(Header);
            for (int i = Index.PosX; i < Index.Max + 1; i++)
            {
                sb.Append(Comma).Append(array[i]);
            }

            return sb.ToString();
        }

        public static float[] FromString(string serialized)
        {
            if (serialized == null || !serialized.StartsWith(Header))
            {
                throw new ArgumentException();
            }

            var values = serialized.Split(Comma);
            if (values == null || values.Length < Index.Max + 1)
            {
                throw new ArgumentException("parameters too short or not comma-separated");
            }

            var array = new float[values.Length];
            for (int i = Index.PosX; i < Index.Max + 1; i++)
            {
                array[i] = float.Parse(values[i]);
            }

            return array;
        }
    }
}