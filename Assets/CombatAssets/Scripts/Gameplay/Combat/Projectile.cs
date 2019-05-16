/********************************************
 * Copyright(c): 2016 Victor Klepikov       *
 *                                          *
 * Site: 	     http://u3d.as/5Fb		    *
 * Support: 	 http://bit.ly/vk-Support   *
 ********************************************/


using System.Collections;
using UnityEngine;
using AdvancedShooterKit.Utils;


namespace AdvancedShooterKit
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField]
        private EProjectileType type = EProjectileType.Simple;
        [SerializeField]
        private EProjectileSubType subType = EProjectileSubType.Bullet;

        [SerializeField]
        [Range( 0f, 125f )]
        private float damage = 65f;
        [SerializeField]
        [Range( 0f, 900f )]
        private float speed = 75f;
        [SerializeField]
        [Range( .1f, 30f )]
        private float lifetime = 2f;

        [SerializeField]
        private Explosion explosionObject = null;

        [SerializeField]
        private HitEffect decalObject = null;        
        public Shell shellObject = null;
        [SerializeField]
        private AudioClip sound = null;
        [SerializeField]
        private bool impactAfterHit = false;
        [SerializeField]
        private Vector3 noise = Vector3.zero;
        [SerializeField]
        private EResetModes resetAngles = EResetModes.UpdateStart;

        //
        private bool movement = false;
        private LayerMask hitMask = 1;
        private ICharacter owner = null;
        private Transform m_Transform = null;
        private RaycastHit hitInfo;
        private Vector3 nextPosition, prevPosition, defaultAnges;
        private AudioSource m_Audio = null;        


        // Spawn FromWeapon
        internal static void SpawnFromWeapon( Firearms weapon )
        {
            Projectile tmpProjectile = weapon.currentSlot.projectileObject.SpawnCopy( weapon.projectileOuter.position, weapon.projectileOuter.rotation );
            //
            tmpProjectile.hitMask = weapon.hitMask;
            tmpProjectile.owner = weapon.owner;
            tmpProjectile.damage += weapon.addDamage;
            tmpProjectile.speed += weapon.addSpeed;
            //
            tmpProjectile.Configure();
        }

        // Configure
        private void Configure()
        {
            m_Transform = transform;
            nextPosition = m_Transform.position;
            prevPosition = nextPosition;

            switch( subType )
            {
                case EProjectileSubType.Arrow:
                    Pickup tmpPickup = GetComponent<Pickup>();
                    if( tmpPickup != null )
                        tmpPickup.enabled = false;

                    Collider tmpCol = GetComponent<Collider>();
                    if( tmpCol != null )
                        tmpCol.enabled = false;
                    break;

                case EProjectileSubType.Throw:
                case EProjectileSubType.Rocket:
                    m_Audio = GetComponent<AudioSource>();
                    if( m_Audio == null )
                    {
                        Debug.LogWarning( "AudioSource is not found! Warning in " + name );
                        m_Audio = gameObject.AddComponent<AudioSource>();
                    }

                    m_Audio.SetupForSFX();

                    if( subType == EProjectileSubType.Rocket )
                    {
                        m_Audio.pitch = Time.timeScale;
                        m_Audio.loop = true;
                        m_Audio.clip = sound;
                        m_Audio.Play();
                        defaultAnges = m_Transform.localEulerAngles;
                    }

                    type = ( subType == EProjectileSubType.Throw ) ? EProjectileType.Ballistic : EProjectileType.Simple;
                    break;
            }

            if( type == EProjectileType.Ballistic )
            {
                Rigidbody tmpRb = GetComponent<Rigidbody>();
                if( tmpRb != null )
                {
                    tmpRb.isKinematic = false;
                    tmpRb.useGravity = true;
                    tmpRb.AddForce( m_Transform.forward * Random.Range( speed * .85f, speed * 1.15f ), ForceMode.Impulse );
                }
            }

            movement = true;
            StartCoroutine( RemoveProjectile() );
        }

        // Remove Projectile
        private IEnumerator RemoveProjectile()
        {
            for( float el = 0f; el < lifetime; el += Time.deltaTime )
                yield return null;

            if( IsExplosionObject() == false )
                Destroy( gameObject );
        }



        // Fixed Update
        void FixedUpdate()
        {
            if( movement == false )
                return;

            prevPosition = nextPosition;

            if( subType == EProjectileSubType.Throw )
            {                
                nextPosition = m_Transform.position;
                return;
            }                

            if( type == EProjectileType.Simple && subType != EProjectileSubType.Rocket )
                nextPosition += m_Transform.forward * speed * Time.fixedDeltaTime;
            else
                nextPosition = m_Transform.position;                

            Vector3 direction = nextPosition - prevPosition;
            float distance = direction.magnitude;

            if( distance > 0f )
            {
                //**Debug**//
                //Debug.DrawLine( prevPosition, nextPosition, Color.yellow );
                //

                direction /= distance;

                if( Physics.Raycast( prevPosition, direction, out hitInfo, distance, hitMask ) && !hitInfo.collider.isTrigger )
                {
                    Impact();
                    return;
                }
            }

            if( type == EProjectileType.Ballistic )
                return;
            
            if( subType == EProjectileSubType.Rocket )
            {
                if( resetAngles == EResetModes.UpdateStart )
                    m_Transform.localEulerAngles = defaultAnges;

                if( Random.value > .5f )
                    m_Transform.Rotate( noise / 4f );
                else
                    m_Transform.Rotate( noise / -4f );

                m_Transform.position += m_Transform.forward * speed * Time.fixedDeltaTime;

                if( resetAngles == EResetModes.UpdateEnd )
                    m_Transform.localEulerAngles = defaultAnges;
            }
            else
            {
                m_Transform.position = nextPosition;
            }
        }
        

        // Impact
        private void Impact()
        {
            movement = false;
            nextPosition = hitInfo.point;

            if( IsExplosionObject() )
                return;

            m_Transform.position = nextPosition;

            bool showHitTexture = ( subType == EProjectileSubType.Bullet );

            IDamageHandler handler = hitInfo.collider.GetComponent<IDamageHandler>();
            if( handler != null )
            {
                handler.TakeDamage( new DamageInfo( damage, m_Transform, owner, EDamageType.Impact ) );
                showHitTexture = ( handler.isPlayer == false && handler.isNPC == false );
            }

            HitEffect.SpawnHitEffect( decalObject, hitInfo, showHitTexture );

            Rigidbody tmpRb = hitInfo.collider.attachedRigidbody;
            if( tmpRb != null && tmpRb.isKinematic == false )
                tmpRb.AddForce( m_Transform.forward * ( ( damage / 10f ) * ( damage * 20f ) / 100f / ( tmpRb.mass > 1f ? tmpRb.mass : 1f ) ), ForceMode.Impulse );


            if( subType == EProjectileSubType.Arrow )
            {
                Collider tmpCollider = GetComponent<Collider>();
                if( tmpCollider == null )
                {
                    Debug.LogWarning( "Collider is not found! Projectile(Arrow) has been destroyed! Warning in " + this.name );
                    Destroy( gameObject );
                    return;
                }

                tmpCollider.enabled = true;
                tmpRb = tmpCollider.attachedRigidbody;

                if( type == EProjectileType.Ballistic && tmpRb != null )
                {                    
                    tmpRb.velocity = Vector3.zero;
                    tmpRb.useGravity = false;
                    tmpRb.isKinematic = true;
                    tmpRb.Sleep();
                }                

                Pickup tmpPickup = GetComponent<Pickup>();

                if( tmpPickup != null )
                    tmpPickup.enabled = true;

                m_Transform.SetParent( hitInfo.transform );
            }
            else
            {
                Destroy( gameObject );
            }
        }

        // IsExplosion Object
        private bool IsExplosionObject()
        {
            if( explosionObject != null )
            {
                m_Transform.position = nextPosition - m_Transform.forward; //nextPosition - prevPosition;

                IExplosion tmpExplosion = explosionObject.SpawnCopy( m_Transform.position, Random.rotation );

                tmpExplosion.SetHitMask( hitMask );
                tmpExplosion.StartExplode( owner );

                Destroy( gameObject );

                return true;
            }

            return false;
        }


        
        // OnCollision Enter
        void OnCollisionEnter( Collision collision )
        {
            if( subType != EProjectileSubType.Throw )
                return;

            if( impactAfterHit )
            {
                IsExplosionObject();
            }
            else if( impactAfterHit == false && collision.relativeVelocity.magnitude > .5f )
            {
                m_Audio.pitch = Time.timeScale;
                m_Audio.PlayOneShot( sound );
            }
        }


        // OnDisable
        void OnDisable()
        {
            Destroy( gameObject );
        }
    };
}