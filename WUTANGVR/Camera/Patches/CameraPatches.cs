
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace WUTANGVR
{


    [HarmonyPatch]
    public class CameraPatches
    {
        public static GameObject loadingscreen;
        public static string StageName;
        static float delaycount = 0;
        static bool camera_angle = false;
        public static Vector3 startpos,startrot,offset,campos,camscale;
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
            if (background && !background.enabled)
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


            if(loadingscreen)
            if (GameObject.Find("Game") && loadingscreen.activeSelf)
            {

                loadingscreen.SetActive(false);
            }

        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(UFE), "Update")]
        private static void CheckStage(UFE __instance)
        {

            if (Input.GetKeyDown("joystick button 6"))
            {
                var stage = UFE.GetStage();

                camera_angle = !camera_angle;
                if (camera_angle)
                {
                    if (stage.stageName == "Sam's Hut")
                        campos = new Vector3(48f, 0, -1.8f);
                    if (stage.stageName == "Wuxíng Waterfall")
                        campos = new Vector3(63f, 0, -58f);
                    if (stage.stageName == "City Streets")
                        campos = new Vector3(64f, 0, -9f);
                    if (stage.stageName == "English Ruins")
                        campos = new Vector3(22f, -2.8f, -4.1f);
                    if (stage.stageName == "Restaurant")
                    {
                        campos = new Vector3(10f, 0, -10f);
                        camscale = new Vector3(1f, 1f, 1f);
                    }
                    if (stage.stageName == "Japan Town")
                        campos = new Vector3(23f, 0, -9f);
                    if (stage.stageName == "Kung fu school")
                    {
                        campos = new Vector3(4.5891f, 0, -9f);
                        camscale = new Vector3(1f, 1f, 1f);
                    }
                    if (stage.stageName == "Hong kong")
                    {
                        campos = new Vector3(7.4727f, 0, 0f);
                        camscale = new Vector3(1f, 1f, 1f);
                    }
                    if (stage.stageName == "Shaolin Yard")
                    {
                        campos = new Vector3(-1.22f, 0, -5.8f);
                        camscale = new Vector3(2.4109f, 1f, 1f);
                    }
                    if (stage.stageName == "Tournament")
                    {
                        campos = new Vector3(14.7728f, 0, -9f);
                        camscale = new Vector3(8.0109f, 1f, 1f);
                    }
                }
                else  // default view
                {
                    if (stage.stageName == "Sam's Hut")
                        campos = new Vector3(48f, 0, -1.8f);
                    if (stage.stageName == "Wuxíng Waterfall")
                        campos = new Vector3(63f, 0, -58f);
                    if (stage.stageName == "City Streets")
                        campos = new Vector3(64f, 0, -9f);
                    if (stage.stageName == "English Ruins")
                        campos = new Vector3(22f, -2.8f, -4.1f);
                    if (stage.stageName == "Restaurant")
                    {
                        campos = new Vector3(10f, 0, -10f);
                        camscale = new Vector3(3.1109f, 1f, 1f);
                    }
                    if (stage.stageName == "Japan Town")
                        campos = new Vector3(23f, 0, -9f);
                    if (stage.stageName == "Kung fu school")
                    {
                        campos = new Vector3(4.5891f, 0, -9f);
                        camscale = new Vector3(2.4109f, 1f, 1f);
                    }
                    if (stage.stageName == "Hong kong")
                    {
                        campos = new Vector3(7.4727f, 0, -5f);
                        camscale = new Vector3(3.9109f, 1f, 1f);
                    }
                    if (stage.stageName == "Shaolin Yard")
                    {
                        campos = new Vector3(-1.22f, 0, -5.8f);
                        camscale = new Vector3(1f, 1f, 1f);
                    }
                    if (stage.stageName == "Tournament")
                    {
                        campos = new Vector3(1.7728f, 0, 22.22f);
                        camscale = new Vector3(5f, 1f, 1f);

                    }
                }
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(DefaultLoadingBattleScreen), "OnShow")]
        private static void MakeCamera1(DefaultLoadingBattleScreen __instance)
        {
            loadingscreen = __instance.gameObject;
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

            DummyCamera.transform.localPosition = campos;
            DummyCamera.transform.localScale = camscale;
            VRCamera.transform.localScale = new Vector3(2.4401f, 1.0915f, 3.5483f);

           
           // var sunshafts = VRCamera.GetComponents<UnityStandardAssets.ImageEffects.SunShafts>();
           // sunshafts[0].enabled = true;
           // sunshafts[1].enabled = true;

            foreach(Component fx in VRCamera.GetComponents(typeof(Component))){

                if (fx.GetType().Name == "GlobalFog")
                    UnityEngine.Object.Destroy(fx);
            }

        }


        [HarmonyPrefix]
        [HarmonyPatch(typeof(BattleGUI), "GetStageMusic")]
        private static void GetStage(StageOptions stage)
        {
            campos = new Vector3(14.7728f, 0, -9f);
            camscale = new Vector3(8.0109f, 1f, 1f);

            Logs.WriteInfo($"LLLLL: STAGE MUSIC:{stage.stageName}");
            if (stage.stageName == "Sam's Hut")
                campos = new Vector3(48f, 0, -1.8f);
            if (stage.stageName == "Wuxíng Waterfall")
                campos = new Vector3(63f, 0, -58f);
            if (stage.stageName == "City Streets")
                campos = new Vector3(64f, 0, -9f);
            if (stage.stageName == "English Ruins")
                campos = new Vector3(22f, -2.8f, -4.1f);
            if (stage.stageName == "Restaurant"){
                campos = new Vector3(10f, 0, -10f);
                camscale = new Vector3(3.1109f, 1f, 1f);
            }
            if (stage.stageName == "Japan Town")
                campos = new Vector3(23f, 0, -9f);
            if (stage.stageName == "Kung fu school")
            {
                campos = new Vector3(4.5891f, 0, -9f);
                camscale = new Vector3(2.4109f, 1f, 1f);
            }
            if (stage.stageName == "Hong kong")
            {
                campos = new Vector3(7.4727f, 0, -5f);
                camscale = new Vector3(3.9109f, 1f, 1f);
            }
            if (stage.stageName == "Shaolin Yard")
            {
                campos = new Vector3(-1.22f, 0, -5.8f);
                camscale = new Vector3(1f, 1f, 1f);
            }
            if (stage.stageName == "Tournament")
            {
                campos = new Vector3(1.7728f, 0, 22.22f);
                camscale = new Vector3(5f, 1f, 1f);

            }
            if (stage.stageName == "Town Market")
            {
                campos = new Vector3(38.43f, 0, -9f);
                camscale = new Vector3(5.8109f, 1f, 1f);

            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(DefaultBattleGUI), "OnShow")]
        private static void MoveIntroCanvas(DefaultBattleGUI __instance)
        {
            __instance.GetComponent<Canvas>().scaleFactor = 0.15f;
            //newUI.SetActive(false);
           // GameObject.Find("BattleGUI Camera").SetActive(false);
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
                worldcam.GetComponent<Camera>().cullingMask = 100000;
            }

            worldcam.GetComponent<Camera>().enabled = true;
            canvas.worldCamera = worldcam.GetComponent<Camera>();

            if(canvas.transform.parent)
                if (canvas.transform.parent.name == "CanvasGroup")
                    canvas.gameObject.layer = 5;

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

