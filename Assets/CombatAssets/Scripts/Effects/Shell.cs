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
    [RequireComponent( typeof( CapsuleCollider ) )]
    [RequireComponent( typeof( Rigidbody ) )]
    [RequireComponent( typeof( AudioSource ) )]       
    public class Shell : MonoBehaviour
    {
        [Range( 1f, 120f )]
        public float lifetime = 5f;
        public AudioClip hitSFX = null;

        private AudioSource m_Audio = null;
        private Rigidbody m_Rigidbody = null;


        // Spawn FromWeapon
        internal static void SpawnFromWeapon( Firearms weapon )
        {
            Shell tmpShell = weapon.currentSlot.projectileObject.shellObject;  
            if( tmpShell != null )
            {
                if( weapon.shellOuter == null )
                {
                    Debug.LogError( "ERROR: Shell Outer is not setup! Error in: " + weapon.name );
                    return;
                }

                tmpShell = tmpShell.SpawnCopy( weapon.shellOuter.position, Random.rotation );                
                tmpShell.Configure( weapon.shellOuter.forward * Random.Range( weapon.shelloutForce * .75f, weapon.shelloutForce * 1.25f ) / 45f );
            }
        }


        // Configure
        private void Configure( Vector3 outForce )
        {
            this.MoveToCache();

            m_Audio = GetComponent<AudioSource>();
            m_Audio.SetupForSFX();

            m_Rigidbody = GetComponent<Rigidbody>();
            m_Rigidbody.AddForce( outForce, ForceMode.Impulse );
            m_Rigidbody.maxAngularVelocity = Random.Range( 26f, 38f );
            m_Rigidbody.velocity = m_Rigidbody.angularVelocity = Vector3.up * 2.5f;
            m_Rigidbody.constraints = RigidbodyConstraints.None;

            AddTorque( Random.Range( -.25f, .4f ) );

            StartCoroutine( RemoveShell() );
        }

        // Remove Shell
        private IEnumerator RemoveShell()
        {
            float elapsed = 0f;
            while( elapsed < lifetime )
            {
                elapsed += Time.deltaTime;
                yield return null;
            }

            GetComponent<Collider>().enabled = false;
            Destroy( gameObject, .32f );
        }

        // OnCollisionEnter
        void OnCollisionEnter( Collision collision )
        {
            if( collision.relativeVelocity.magnitude < .5f )
                return;

            m_Rigidbody.maxAngularVelocity = Random.Range( 12f, 18f );
            AddTorque( Random.Range( -.35f, .5f ) );
            m_Audio.pitch = Time.timeScale;
            m_Audio.PlayOneShot( hitSFX );
        }

        // AddTorque
        private void AddTorque( float value )
        {
            bool isPositive = ( Random.value > .5f );
            m_Rigidbody.AddRelativeTorque( Random.rotation.eulerAngles * ( isPositive ? value : -value ) );            
        }
    };
}