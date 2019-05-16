/********************************************
 * Copyright(c): 2016 Victor Klepikov       *
 *                                          *
 * Site: 	     http://u3d.as/5Fb		    *
 * Support: 	 http://bit.ly/vk-Support   *
 ********************************************/


#define UNITY_5_4_PLUS
#if UNITY_5_3 || UNITY_5_4
#undef UNITY_5_4_PLUS
#endif

using System.Collections;
using UnityEngine;
using AdvancedShooterKit.Utils;


namespace AdvancedShooterKit
{
    public class Explosion : MonoBehaviour, IExplosion
    {
        [SerializeField]
        private LayerMask hitMask = 1;

        [Range( 0f, 250f )]
        [SerializeField]
        private float damage = 50f;

        [Range( 1f, 15f )]
        [SerializeField]
        private float radius = 7f;

        [Range( 5f, 350f )]
        [SerializeField]
        private float force = 100f;

        [Range( 0f, 2f )]
        [SerializeField]
        private float upwardsModifier = .28f;
        
        [SerializeField]
        private GameObject fragments = null;
        [SerializeField]
        private AudioClip[] explosionSounds = null;


        private ICharacter owner = null;

        private bool exploded = false;


        void Awake()
        {
            if( enabled == false )
                enabled = true;
        }

        void Update()
        {
            if( owner != null )
            {
                enabled = false;
                Explode( owner );
            }                
        }        


        // SetOwner
        void SetOwner( ICharacter owner )
        {
            this.owner = owner;
        }

        // Set HitMask
        public void SetHitMask( LayerMask mask )
        {
            hitMask = mask;
        }

        void SetOwnerAndHitMask( ICharacter owner, LayerMask mask )
        {
            this.owner = owner;
            hitMask = mask;
        }


        // Start Explode
        public void StartExplode()
        {
            Explode( null );
        }
        // Start Explode
        public void StartExplode( ICharacter owner )
        {
            Explode( owner );
        }
        // Start Explode
        public void StartExplode( ICharacter owner, float delay )
        {
            StartCoroutine( StartExplodeWithDelay( Mathf.Max( 0f, delay ), owner ) );
        }
        // Start Explode
        public void StartExplode( float delay )
        {
            StartCoroutine( StartExplodeWithDelay( Mathf.Max( 0f, delay ), null ) );
        }

        // StartExplode WithDelay
        private IEnumerator StartExplodeWithDelay( float delay, ICharacter owner )
        {
            for( float el = 0f; el < delay; el += Time.deltaTime )
                yield return null;

            Explode( owner );
        }        
        // Explode
        private void Explode( ICharacter owner )
        {
            if( exploded )
                return;

            exploded = true;            

            Transform m_Transform = transform;

            if( fragments != null )
                fragments.SpawnCopy( m_Transform.position, m_Transform.rotation );

            Collider m_Collider = GetComponent<Collider>();
            Collider[] overlapColliders = Physics.OverlapSphere( m_Transform.position, radius, hitMask );

            Vector3 direction = Vector3.zero;
            RaycastHit hitInfo;
            DamageInfo damageInfo = new DamageInfo();

            foreach( Collider hitCollider in overlapColliders )
            {
                if( hitCollider == m_Collider || hitCollider.isTrigger )
                    continue;

                direction = hitCollider.bounds.center - m_Transform.position;

                if( Physics.Raycast( m_Transform.position, direction, out hitInfo, radius, hitMask ) == false || hitCollider != hitInfo.collider )
                    continue;
                
                float distance = Vector3.Distance( m_Transform.position, hitInfo.point );
                float percent = ( 100f - ( distance / radius * 100f ) ) / 100f;

                IDamageHandler handler = hitCollider.GetComponent<IDamageHandler>();
                if( handler != null )
                {
                    handler.TakeDamage( damageInfo.GetItByInfo( damage * percent, m_Transform, owner, EDamageType.Explosion ) );

                    if( handler.isPlayer )                   
                        PlayerCamera.Shake( percent / 2f, percent );
                }

                Rigidbody tmpRb = hitCollider.attachedRigidbody;
                if( tmpRb != null && tmpRb.isKinematic == false )
                {
                    percent *= 1.75f;
                    tmpRb.AddExplosionForce( force * percent / ( tmpRb.mass > 1f ? tmpRb.mass : 1f ), m_Transform.position, radius, upwardsModifier, ForceMode.Impulse );
                }
            }


            float lifetime = 0f;


            // Play SFX
            AudioSource m_Audio = GetComponent<AudioSource>();
            if( m_Audio != null )
            {
                m_Audio.SetupForSFX();

                AudioClip sfxClip = Audio.GetRandomClip( explosionSounds );
                m_Audio.PlayOneShot( sfxClip );

                if( sfxClip != null )
                    lifetime = Mathf.Max( lifetime, sfxClip.length );
            }
            else
            {
                Audio.PlayClipAtPoint( Audio.GetRandomClip( explosionSounds ), m_Transform.position );
            }


            // Spawn FX
            ParticleSystem explosionFX = GetComponent<ParticleSystem>();
            if( explosionFX != null )
            {
#if UNITY_5_4_PLUS
                var main = explosionFX.main;
                main.playOnAwake = main.loop = false;
                float startLifetime = main.startLifetime.constant;
#else
                explosionFX.playOnAwake = explosionFX.loop = false;
                explosionFX.playbackSpeed = Time.timeScale;
                float startLifetime = explosionFX.startLifetime;
#endif
                explosionFX.Play();

                lifetime = Mathf.Max( lifetime, startLifetime );
            }


            Destroy( gameObject.MoveToCache(), lifetime );
        }
    };
}