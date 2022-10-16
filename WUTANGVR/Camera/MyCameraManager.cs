using System;
using System.Collections;
using System.Text;
using UnityEngine;

namespace WUTANGVR
{
    class MyCameraManager : MonoBehaviour
    {
        static int cameramode;
        static Vector3 roffsetprev= Vector3.zero;
        static Vector3 prevpos = Vector3.zero;


        public static void RecenterRotation()
        {
            if (!CameraPatches.VRCamera) return;

            Vector3 offset = CameraPatches.startpos - CameraPatches.VRCamera.transform.localPosition;
            var angleOffset = CameraPatches.VRPlayer.transform.parent.eulerAngles.y - CameraPatches.VRCamera.transform.eulerAngles.y;
            CameraPatches.DummyCamera.transform.RotateAround(CameraPatches.VRCamera.transform.position, Vector3.up, angleOffset);

        }

        public static void Recenter()
        {
            Logs.WriteInfo($"LLLL: RECENTERING");
            if (!CameraPatches.VRCamera) return;

            Vector3 offset = CameraPatches.startpos - CameraPatches.VRCamera.transform.localPosition;
            Vector3 roffset = CameraPatches.startrot - CameraPatches.VRCamera.transform.localEulerAngles;

            

            CameraPatches.DummyCamera.transform.Translate(offset - prevpos);

          //  RecenterRotation();

            roffsetprev = roffset;
            prevpos = offset;

        }
     
    }
}
