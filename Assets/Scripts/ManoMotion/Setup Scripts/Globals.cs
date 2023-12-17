namespace AR_ManoMotion
{
    public static class Globals
    {
        /* Enum for setting device play mode */
        public enum DeviceMode { Desktop, AR }

        /* Enum for setting AR init phases */
        public enum ARInitPhase
        {
            PlaneDetection,
            ScenePlacement,
            SceneAdjustments,
            Done,
            UNDEFINED
        }

        public static DeviceMode CurrentDeviceMode = DeviceMode.AR;
    }

}
