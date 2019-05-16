/********************************************
 * Copyright(c): 2016 Victor Klepikov       *
 *                                          *
 * Site: 	     http://u3d.as/5Fb		    *
 * Support: 	 http://bit.ly/vk-Support   *
 ********************************************/


using AdvancedShooterKit.Utils;
using UnityEngine;
using System.Collections;


namespace AdvancedShooterKit
{
    public class Debris : MonoBehaviour
    {
        [Range( 2f, 10f )]
        public float lifetime = 7f;

        [Range( 1f, 5f )]
        public float radius = 4f;

        [Range( .1f, 5f )]
        public float force = 2f;

        [Range( 2f, 35f )]
        public float upwards = 5f;

        public AudioClip crashSound = null;


        // OnEnable
        void OnEnable()
        {
            Transform m_Transform = transform.MoveToCache();
            Audio.PlayClipAtPoint( crashSound, m_Transform.position ).SetParent( m_Transform );

            Collider[] tmpColliders = this.GetComponentsInChildren<Collider>();
            foreach( Collider col in tmpColliders )
            {
                Rigidbody tmpRb = col.attachedRigidbody;
                if( tmpRb != null && !tmpRb.isKinematic )
                {
                    float randomValue = Random.value;
                    tmpRb.velocity = Vector3.zero;
                    tmpRb.angularVelocity = Vector3.one * randomValue;
                    tmpRb.angularDrag = randomValue;
                    tmpRb.inertiaTensorRotation = Random.rotation;
                    tmpRb.AddExplosionForce( force * 1.75f / ( tmpRb.mass > 1f ? tmpRb.mass : 1f ), m_Transform.position, radius, upwards / 15f, ForceMode.Impulse );
                }
            }

            StartCoroutine( RemoveDebris( tmpColliders ) );
        }

        // Remove Debris
        private IEnumerator RemoveDebris( Collider[] tmpColliders )
        {
            float elapsed = 0f;
            while( elapsed < lifetime )
            {
                elapsed += Time.deltaTime;
                yield return null;
            }

            foreach( Collider col in tmpColliders )
                col.enabled = false;

            Destroy( gameObject, .75f );
        }
    };
}