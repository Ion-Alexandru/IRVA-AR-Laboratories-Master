using UnityEngine;

namespace AR_ManoMotion
{
    /* TODO 1.1. Test the ManoMotion demo scene from Manomotion AR Foundation -> Scenes -> ManoMotionSDKProFeatures */
    /* TODO 1.2. Buid the Fruit Ninja scene */
    public class CursorPositionController : MonoBehaviour
    {
        [Tooltip("Object which will follow the cursor (mouse on desktop or hand position in AR")]
        [SerializeField]
        private GameObject CursorPrefab;

        private Vector3 _cursorScreenPos;
        private Camera _mainCamera;
        private const float piPer180 = Mathf.PI / 180f;

        void Start()
        {
            if (_mainCamera == null)
            {
                _mainCamera = Camera.main;
            }
        }

        void Update()
        {
            if (Globals.CurrentDeviceMode == Globals.DeviceMode.Desktop)
            {
                /* If playing on computer, use the in-game cursor position in the mouse position */
                _cursorScreenPos = Input.mousePosition;
            }
            else if (Globals.CurrentDeviceMode == Globals.DeviceMode.AR)
            {
                /*    TODO 2. The 'cursorScreenPos' needs to be set to the on-screen hand position detected
                 *    by ManoMotion -> more precisely, the palm center position
                 *    To do this, get the hand information, from which the palm center position can be accessed
                 *    (!) NOTE:
                 *        -> Palm center is returned in normalized coordinates [0...1], 
                 *        -> 'cursorScreenPos' needs to be in screen coordinates (pixels)
                 *        -> Use screen.width & Screen.height to compute position in pixels
                 *        -> Z coord can be set to 0
                 */

                HandInfo handInfo = ManomotionManager.Instance.Hand_infos[0].hand_info;
                Vector2 palm_position = handInfo.tracking_info.palm_center;
                _cursorScreenPos = new Vector3(palm_position.x * Screen.width, palm_position.y * Screen.height, 0);

            }

            UpdateCursorPosition(_cursorScreenPos);
        }

        /// <summary>
        ///     This keeps the 3D cursor object on an imaginary plane positioned at PlayPlane's position with its normal aligned with PlayPlane's forward vector
        ///     Why? We can easily find the cursor position in screen space, but we need to make a 3D object follow the hand's position in world space.
        ///     As we de not have depth info (except some ManoMotion approximation and ignoring LiDAR sensors), we need to position that object somewhere.
        ///     This 'somewhere' is in this case on the above-described plane. Take note that all fruits are spawned on the same plane as well.
        ///     We basically remove one movement axis and force everything to move around on the same plane
        /// </summary>
        private void UpdateCursorPosition(Vector3 screenPointPos)
        {
            /* Create ray through the screen position towards world */
            Ray cameraRay = _mainCamera.ScreenPointToRay(screenPointPos);
            /* Compute angle between ray and play scene forward vector */
            var playSceneCamAngleRad = Vector3.Angle(transform.forward, cameraRay.direction) * piPer180;
            /* Compute camera <-> play plane distance (z-axis only) */
            float camPlaySceneDistZ = Mathf.Abs(_mainCamera.transform.position.z - transform.position.z);
            /* Position the cursor on a plane 'cameraPlayPlaneDist' distance away using some trig */
            Vector3 targetPos = _mainCamera.transform.position + (cameraRay.direction * (camPlaySceneDistZ / Mathf.Cos(playSceneCamAngleRad)));

            if (targetPos == Vector3.zero)
            {
                return;
            }

            CursorPrefab.transform.position = targetPos;
        }
    }
}