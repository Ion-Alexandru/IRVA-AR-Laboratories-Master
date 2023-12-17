using UnityEngine;

namespace AR_ManoMotion
{
    public class HandGestureController : MonoBehaviour
    {
        private FruitSpawner _fruitSpawner;

        void Start() => _fruitSpawner = GetComponentInChildren<FruitSpawner>();

        void Update()
        {
            /* TODO 3.1, 4.1, 5.1 Get the hand info */
            HandInfo handInfo = ManomotionManager.Instance.Hand_infos[0].hand_info;
            
            /* TODO 3.2, 4.2, 5.2 Get the gesture info */
            GestureInfo gestureInfo = handInfo.gesture_info;

            /* TODO 3.3. Get the hand side information */
            HandSide handSide = gestureInfo.hand_side;

            /* TODO 4.3. Get the continuous gestures */
            ManoGestureContinuous continuousGesture = gestureInfo.mano_gesture_continuous;

            /* TODO 5.3. Get the trigger gestures */
            ManoGestureTrigger triggerGesture = gestureInfo.mano_gesture_trigger;
            
            /* TODO 3.5. When the hand side is 'HandSide.Palmside', disable the fruit spawning altogether.
             * If it's 'HandSide.Backside' spawning should be active (default behavior)
             */
            if (handSide == HandSide.Palmside)
            {
                _fruitSpawner.SpawnerActive = false;
            }
            else
            {
                if(handSide == HandSide.Backside)
                {
                    _fruitSpawner.SpawnerActive = true;
                }
            }
            
            /* TODO 4.5. For each frame in which the continuous gesture 'CLOSED_HAND_GESTURE' is active, 
             * increase the spawn rate by 20 (or some number to see a difference)
             * For any other continuous gesture the spawn rate should be kept as default
             * Hint! Use the spawn rate variable found in the fruit spawner script
             * You need to figure out where to use it, changing it's value won't do anything as it's not used in code
             */
            if (continuousGesture == ManoGestureContinuous.CLOSED_HAND_GESTURE)
            {
                _fruitSpawner.SpawnMultiplier = 20.0f;
            }
            else
            {
                _fruitSpawner.SpawnMultiplier = 1.0f;
            }
            
            /* TODO 5.4. When the trigger gesture 'PICK' is detected, destroy all fruit instances which are on-screen.
             * Hint! Use 'DestroyFruitInstance()' function found on fruit controller script,
             * as it plays particle and sound effect as well
             */
            if(triggerGesture == ManoGestureTrigger.PICK)
            {
                FruitController[] allFruits = FindObjectsOfType<FruitController>();

                foreach(FruitController fruit in allFruits)
                {
                    fruit.DestroyFruitInstance();
                }
            }

        }
    }
}