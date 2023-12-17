using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AR_ManoMotion
{
    public class FruitSpawner : MonoBehaviour
    {
        public List<GameObject> fruitPrefabs;

        /* Gravity scale - keep it low to have a chance to hit objects */
        [Range(-10f, -0.1f)] public float GravityValue = -0.5f;

        /* Min/max linear spawn forces X and Y */
        [Range(0.1f, 2f)] public float MinLinearSpawnForceY = 0.35f;
        [Range(0.1f, 2f)] public float MaxLinearSpawnForceY = 0.6f;
        [Range(-1f, 1f)] public float MinLinearSpawnForceX = -0.1f;
        [Range(-1f, 1f)] public float MaxLinearSpawnForceX = 0.1f;

        /* Min/max angular (torque) forces */
        [Range(-5f, 5f)] public float MinAngularSpawnForce = -2.5f;
        [Range(-5f, 5f)] public float MaxAngularSpawnForce = 2.5f;

        /* Min/max spawn rates (seconds) */
        [Range(0.1f, 5f)] public float MinSpawnRate = 0.5f;
        [Range(0.1f, 5f)] public float MaxSpawnRate = 1.25f;

        private BoxCollider _boxCollider;
        private bool _spawnerActive = true;
        private float _spawnMultiplier = 1f;

        /* TODO 4.4 Use this to control spawn rate
         *    -> You need to figure out where to use it, changing it's value will not do anything as it is not used in code
         *    -> Hint: Study the spawn coroutine
         */
        public float SpawnMultiplier { get => _spawnMultiplier; set => _spawnMultiplier = value; }

        /* TODO 3.4 Use this to control if the fruits should spawn
         *    -> You need to figure out where to use it, changing it's value will not do anything as it is not used in code
         *    -> Hint: Study the spawn coroutine
         */
        public bool SpawnerActive { get => _spawnerActive; set => _spawnerActive = value; }

        void Start()
        {
            /* Setup */
            _boxCollider = GetComponent<BoxCollider>();
            Physics.gravity = new Vector3(0f, GravityValue, 0f);

            /* Start spawning fruits */
            StartCoroutine(SpawnFruitsCoroutine());
        }

        private IEnumerator SpawnFruitsCoroutine()
        {
            for (; ; )
            {
                if (SpawnerActive) /* TODO 3.6 */
                {
                    /* Get random values */
                    int randFruitIndex = Random.Range(0, fruitPrefabs.Count);
                    float randSpawnTimer = Random.Range(MinSpawnRate, MaxSpawnRate) / SpawnMultiplier; /* TODO 4.6 */
                    float randSpawnForceY = Random.Range(MinLinearSpawnForceY, MaxLinearSpawnForceY);
                    float randSpawnForceDeviationX = Random.Range(MinLinearSpawnForceX, MaxLinearSpawnForceX);
                    Vector3 randSpawnPos = GetRandomPointInBounds(_boxCollider.bounds);
                    Vector3 randAngularForce = new Vector3(
                        Random.Range(MinAngularSpawnForce, MaxAngularSpawnForce),
                        Random.Range(MinAngularSpawnForce, MaxAngularSpawnForce),
                        Random.Range(MinAngularSpawnForce, MaxAngularSpawnForce));

                    /* Spawn a fruit (Note! The instance will be parented to 'GameScene' object, see the forth Instantiate() parameter) */
                    GameObject newFruit = Instantiate(fruitPrefabs[randFruitIndex], randSpawnPos, Quaternion.identity, transform.parent);

                    /* Apply linear and angular forces */
                    newFruit.GetComponent<Rigidbody>().AddForce(new Vector3(randSpawnForceDeviationX, randSpawnForceY, 0), ForceMode.VelocityChange);
                    newFruit.GetComponent<Rigidbody>().AddRelativeTorque(randAngularForce, ForceMode.VelocityChange);

                    /* Wait until next spawn */
                    yield return new WaitForSeconds(randSpawnTimer);
                }
            }
        }

        public Vector3 GetRandomPointInBounds(Bounds b)
        {
            return new Vector3(
                Random.Range(b.min.x, b.max.x),
                Random.Range(b.min.y, b.max.y),
                Random.Range(b.center.z, b.center.z));  /* Keep Z constant (on the same plane) */
        }
    }
}