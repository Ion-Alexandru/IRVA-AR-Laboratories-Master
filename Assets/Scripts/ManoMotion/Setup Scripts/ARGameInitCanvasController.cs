using UnityEngine;

namespace AR_ManoMotion
{
    public class ARGameInitCanvasController : MonoBehaviour
    {
        [Tooltip("GameObject for phase 1 (plane detection)")]
        [SerializeField] private GameObject initPhasePlaneDetection;

        [Tooltip("GameObject for phase 2 (object placement)")]
        [SerializeField] private GameObject initPhaseScenePlacement;

        [Tooltip("GameObject for phase 3 (scene adjustments)")]
        [SerializeField] private GameObject initPhaseSceneConfigs;

        [Tooltip("GameObject for phase 3 (scene adjustments) sliders")]
        [SerializeField] private GameObject initPhaseSceneConfigsSliders;

        public void SetEnabledStatesForAll(bool planeDetectionPhase, bool scenePlacementState, bool sceneConfigsState)
        {
            initPhasePlaneDetection.SetActive(planeDetectionPhase);
            initPhaseScenePlacement.SetActive(scenePlacementState);
            initPhaseSceneConfigs.SetActive(sceneConfigsState);
            initPhaseSceneConfigsSliders.SetActive(sceneConfigsState);
        }
    }
}