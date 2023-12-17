using UnityEngine;

namespace AR_ManoMotion
{
    public class FruitController : MonoBehaviour
    {
        [Tooltip("Destroy at Y value")]
        [SerializeField] private float DestroyAtY = -0.1f;

        [Tooltip("Particle prefab instantiated after cursor-fruit contact")]
        [SerializeField] private GameObject hitParticlesPrefab;

        [Tooltip("Sound effect played after cursor-fruit contact")]
        [SerializeField] private AudioClip hitSoundClip;

        private ScoreManager scoreManager;

        void Start ()
        {
            scoreManager = FindObjectOfType<ScoreManager>();
        }

        void Update()
        {
            /* Destroy if y-coord is under some value. It works because fruits' parent is 'GameScene' */
            if (transform.position.y - transform.parent.position.y < DestroyAtY)
            {
                Destroy(gameObject);
            }
        }

        private void OnCollisionEnter(Collision collision) => DestroyFruitInstance();

        public void DestroyFruitInstance()
        {
            /* Instantiate the particle effect:
             *    -> The effect is automatically played (see 'Play On Awake' property on particle in Inspector)
             *    -> The effect is automatically destroyed after playing (see 'Stop Action' property on particle effect in Inspector)
             */
            Instantiate(hitParticlesPrefab, transform.position, Quaternion.identity);

            /* Play the hit sound effect:
             *    -> This function creates an audio source but automatically disposes of it once the clip has finished playing
             */
            AudioSource.PlayClipAtPoint(hitSoundClip, transform.position);

            scoreManager.IncreaseScore();

            Destroy(gameObject);
        }
    }
}