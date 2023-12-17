using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.UI;
using TMPro;

namespace AR_ManoMotion
{
    public class ARInitController : MonoBehaviour
    {
        [Tooltip("Set the device to play on (desktop or AR)")]
        [field: NonSerialized]
        private Globals.DeviceMode deviceMode;

        [Tooltip("Enable or disable ARFoundation's plane detection")]
        [SerializeField]
        private bool enablePlaneDetection = true;

        [Tooltip("Tracking smoothing value")]
        [SerializeField]
        [Range(0f, 1f)]
        private float trackingSmoothing = 0.5f;

        [Tooltip("AR Session")]
        [SerializeField]
        private ARSession ARSession;

        [Tooltip("AR Session Origin")]
        [SerializeField]
        private ARSessionOrigin ARSessionOrigin;

        [Tooltip("This prebaf will be instantiated in phase 2")]
        [SerializeField]
        private GameObject ARScenePrefab;

        [Header("Phase 3 - Scene adjustments")]
        [Tooltip("Slider used to adjust the instantiated scene's rotation")]
        [SerializeField]
        private Slider sliderRotation;

        [Tooltip("Slider used to adjust the instantiated scene's scale")]
        [SerializeField]
        private Slider sliderScale;

        [Tooltip("Text used to show rotation value (angles)")]
        [SerializeField]
        private TextMeshProUGUI textRotation;

        [Tooltip("Text used to show scale value")]
        [SerializeField]
        private TextMeshProUGUI textScale;

        private ARPlaneManager _arPlaneManager;
        private ARRaycastManager _arRaycastManager;
        private ARSetupPhasesController _arSetupPhasesController;
        private bool _sceneInstantiated = false;
        private GameObject _arSceneInst = null;
        private Vector3 _currentScale = Vector3.one;
        private Quaternion _currentRot = Quaternion.identity;

        private void Awake() => InitDeviceMode();

        void Start()
        {
            _arPlaneManager = ARSessionOrigin.GetComponent<ARPlaneManager>();
            _arRaycastManager = ARSessionOrigin.GetComponent<ARRaycastManager>();
            _arSetupPhasesController = GetComponent<ARSetupPhasesController>();

            /* Enable or disable ARFoundation's plane detection */
            _arPlaneManager.requestedDetectionMode = enablePlaneDetection ? PlaneDetectionMode.Horizontal : PlaneDetectionMode.None;

            /* Set the amount of hand motion tracking smoothing */
            ManomotionManager.Instance.SetManoMotionSmoothingValueFloat(trackingSmoothing);

            textRotation.text = sliderRotation.value.ToString("0");
            textScale.text = sliderScale.value.ToString("0.00");
        }

        private void Update()
        {
            /* If no scene has been instantiated and we're in the appropriate phase */
            if (!_sceneInstantiated && _arSetupPhasesController.CurrentGamePhase == Globals.ARInitPhase.ScenePlacement)
            {
                InstatiateARGameObjects();
            }
        }

        private void InitDeviceMode()
        {
            deviceMode = Application.isMobilePlatform ? Globals.DeviceMode.AR : Globals.DeviceMode.Desktop;
            Globals.CurrentDeviceMode = deviceMode;
        }

        /* PHASE 1 - Confirm button action */
        public void ConfirmPlaneDetection()
        {
            DisablePlaneDetection();
            /* Advance from phase 1 to phase 2 */
            _arSetupPhasesController.AdvanceToPhase(Globals.ARInitPhase.ScenePlacement);
        }

        /* PHASE 1 - Reset button action */
        public void ResetPlaneDetection()
        {
            HideDetectedPlanes();
            /* Restart the AR session */
            ARSession.Reset();
        }

        /* PHASE 2 - Confirm button action */
        public void ConfirmScenePlacement()
        {
            /* Advance from phase 2 to phase 3 */
            _arSetupPhasesController.AdvanceToPhase(Globals.ARInitPhase.SceneAdjustments);
            HideDetectedPlanes();
        }

        /* PHASE 2 - Reset button action */
        public void ResetScenePlacement()
        {
            if (_arSceneInst != null)
            {
                Destroy(_arSceneInst);
            }
            _sceneInstantiated = false;
        }

        /* PHASE 3 - Rotation slider change action */
        public void SliderRotationChange()
        {
            if (_arSceneInst != null)
            {
                float sliderValue = sliderRotation.value;
                textRotation.text = sliderValue.ToString("0");

                _currentRot = Quaternion.Euler(0, sliderValue * -1, 0);
                _arSceneInst.transform.rotation = _currentRot;

                ARSessionOrigin.MakeContentAppearAt(
                    _arSceneInst.transform,
                    _arSceneInst.transform.position,
                    _arSceneInst.transform.rotation);
            }
        }

        /* PHASE 3 - Scale slider change action */
        public void SliderScaleChange()
        {
            if (_arSceneInst != null)
            {
                float sliderValue = sliderScale.value;
                textScale.text = sliderValue.ToString("0.00");

                _currentScale = Vector3.one * sliderValue;
                _arSceneInst.transform.localScale = _currentScale;

                ARSessionOrigin.MakeContentAppearAt(
                    _arSceneInst.transform,
                    _arSceneInst.transform.position,
                    _arSceneInst.transform.rotation);
            }
        }

        /* PHASE 3 - Confirm button action */
        public void ConfirmSceneAdjustments()
        {
            /* Advance from phase 3 to phase 4 */
            _arSetupPhasesController.AdvanceToPhase(Globals.ARInitPhase.Done);

            if (_arSceneInst != null)
            {
                /* Enable game controller scripts */
                _arSceneInst.GetComponent<CursorPositionController>().enabled = true;
                _arSceneInst.GetComponent<HandGestureController>().enabled = true;
                _arSceneInst.GetComponentInChildren<FruitSpawner>().enabled = true;
            }
        }

        public void DisablePlaneDetection() => _arPlaneManager.requestedDetectionMode = PlaneDetectionMode.None;

        public void HideDetectedPlanes()
        {
            foreach (ARPlane plane in _arPlaneManager.trackables)
            {
                plane.gameObject.SetActive(false);
            }
        }

        private void InstatiateARGameObjects()
        {
            /* Get hand and gesture information -> used to get trigger-type gesture */
            HandInfo handInfo = ManomotionManager.Instance.Hand_infos[0].hand_info;
            GestureInfo gestureInfo = handInfo.gesture_info;
            ManoGestureTrigger manoGestureTrigger = gestureInfo.mano_gesture_trigger;

            /* Check if there is a grab gesture -> if true, instantiate scene */
            if (manoGestureTrigger == ManoGestureTrigger.RELEASE_GESTURE)
            {
                List<ARRaycastHit> hits = new List<ARRaycastHit>();
                float screenCoordX = handInfo.tracking_info.palm_center.x * Screen.width;
                float screenCoordY = handInfo.tracking_info.palm_center.y * Screen.height;

                /* Ray from camera (based on screen coords) towards world */
                if (handInfo.tracking_info.palm_center.x != -1 && handInfo.tracking_info.palm_center.y != -1)
                {
                    Ray ray = Camera.main.ScreenPointToRay(new Vector3(screenCoordX, screenCoordY, 0));
                    if (_arRaycastManager.Raycast(ray, hits))
                    {
                        foreach (ARRaycastHit hit in hits)
                        {
                            /* Instatiate only once */
                            _arSceneInst = Instantiate(ARScenePrefab, hit.pose.position, Quaternion.identity, GameObject.Find("SceneRoot").transform);
                            _sceneInstantiated = true;
                            break;
                        }
                        Handheld.Vibrate();
                    }
                }
            }
        }
    }
}