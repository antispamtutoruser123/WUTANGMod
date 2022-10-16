using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

namespace WUTANGVR
{


    [HarmonyPatch]
    public class CameraPatches
    {
        static float delaycount = 0;
        static bool ingame = false;
        public static Vector3 startpos,startrot,offset;
        public static RenderTexture rt;
        public static GameObject newUI;
        public static GameObject worldcam;
        public static GameObject chin;
        public static CanvasScaler background;
        public static GameObject DummyCamera, VRCamera,VRPlayer;

        private static readonly string[] canvasesToIgnore =
    {
        "com.sinai.unityexplorer_Root", // UnityExplorer.
        "com.sinai.unityexplorer.MouseInspector_Root", // UnityExplorer.
        "com.sinai.universelib.resizeCursor_Root"
    };
        private static readonly string[] canvasesToWorld =
    {
        "OverlayCanvas"
    };

        [HarmonyPostfix]
        [HarmonyPatch(typeof(CameraFade), "Update")]
        private static void recenter(CameraFade __instance)
        {
            if(background && !background.enabled)
            {
                delaycount += Time.deltaTime;
                if (delaycount > .1f)
                {
                    background.enabled = true;
                    background = null;
                }
            }
            if (!chin && SceneManager.GetActiveScene().name == "VideoIntro_SVW 2")
            {

                // move main menu
                var bg = GameObject.Find("Quad");
                var bg2 = GameObject.Find("Quad 2");
                chin = GameObject.Find("SVW 2 chinese");
                if (chin)
                {
                    chin.transform.localPosition = new Vector3(0, 0, .1f);
                    bg.transform.localPosition = new Vector3(0, 0, .2f);
                    bg2.transform.localPosition = new Vector3(0, 0, .2f);

                    //chin.layer = 5;
                    bg.layer = 5;
                    bg2.layer = 5;

                    if (Camera.main.GetComponent<ScionEngine.ScionPostProcess>())
                        Camera.main.GetComponent<ScionEngine.ScionPostProcess>().enabled = false;

                    var cam = GameObject.Find("Camera").GetComponent<Camera>(); 
                    cam.cullingMask = 5;
                    cam.clearFlags = CameraClearFlags.Depth;
                }
            }

            if (Input.GetKeyDown("joystick button 6"))
            {
                MyCameraManager.Recenter();
            }
          
        }
            [HarmonyPostfix]
        [HarmonyPatch(typeof(CameraScript), "DoFixedUpdate")]
        private static void MakeCamera(CameraScript __instance)
        {
            if (!DummyCamera)
            {
                Logs.WriteInfo($"LLLLL: CREATING DUMMY CAMERA:  {__instance.name} {__instance.tag}");

                VRPlayer = __instance.gameObject;

                DummyCamera = new GameObject("DummyCamera");
               
                DummyCamera.transform.parent = __instance.transform;


            }

            if(DummyCamera.transform.childCount == 0)
            foreach (Camera cam in Camera.allCameras)
            {
                if (cam.tag == "MainCamera")
                {
                   
                    VRCamera = cam.gameObject;
                    VRCamera.transform.parent = DummyCamera.transform;

                }

            }

            DummyCamera.transform.localPosition = new Vector3(14.7728f, 0, -9f);
            DummyCamera.transform.localScale = new Vector3(8.0109f, 1f, 1f);
           // newUI.SetActive(false);
        }


        [HarmonyPrefix]
        [HarmonyPatch(typeof(DefaultBattleGUI), "OnShow")]
        private static void MoveIntroCanvas(DefaultBattleGUI __instance)
        {
            __instance.GetComponent<Canvas>().scaleFactor = 0.15f;
            newUI.SetActive(false);
        }

            [HarmonyPrefix]
        [HarmonyPatch(typeof(CanvasScaler), "OnEnable")]
        private static void MoveIntroCanvas(CanvasScaler __instance)
        {
            if (IsCanvasToIgnore(__instance.name)) return;

            Logs.WriteInfo($"Hiding Canvas:  {__instance.name}");
            if (__instance.transform.parent.parent)
            {
                Logs.WriteInfo($"Hiding Canvas Parent:  {__instance.transform.parent.parent.name}");

                if (!background)
                {
                    background = __instance;
                    background.enabled = false;
                    
                }
            }

            var canvas = __instance.GetComponent<Canvas>();

            canvas.renderMode = RenderMode.ScreenSpaceCamera;

            if (!worldcam)
            {
                worldcam = new GameObject("WorldCam");
                worldcam.AddComponent<Camera>();
                
            }

            worldcam.GetComponent<Camera>().enabled = true;
            canvas.worldCamera = worldcam.GetComponent<Camera>();


            if (!rt)
            {
                rt = new RenderTexture(1920, 1080, 24);

                worldcam.GetComponent<Camera>().targetTexture = rt;


                newUI = new GameObject("newUI");
                newUI.AddComponent<Canvas>();
                newUI.AddComponent<RawImage>();
                newUI.layer = 5;


                canvas = newUI.GetComponent<Canvas>();
                canvas.renderMode = RenderMode.WorldSpace;
                newUI.transform.localPosition = new Vector3(-5f, 16f, -1f);
                newUI.transform.localScale = new Vector3(.2462f, .1456f, .1f);
                newUI.transform.eulerAngles = new Vector3(0, -18f, 0);
                newUI.GetComponent<RawImage>().texture = rt;

            }


        }


        private static bool IsCanvasToIgnore(string canvasName)
        {
            foreach (var s in canvasesToIgnore)
                if (Equals(s, canvasName))
                    return true;
            return false;
        }

        private static bool IsCanvasToWorld(string canvasName)
        {
            foreach (var s in canvasesToWorld)
                if (Equals(s, canvasName))
                    return true;
            return false;
        }

    }
}

