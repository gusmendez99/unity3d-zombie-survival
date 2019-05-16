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


#pragma warning disable 169

namespace AdvancedShooterKit
{
    public sealed class HitEffect : MonoBehaviour
    {      
        [System.Serializable]
        public sealed class SurfaceData
        {
            public string name = string.Empty;
            public int index = 0;            
            public Texture hitTexture = null;
            public AudioClip hitSound = null;
            public ParticleSystem hitParticle = null;
        };


        [SerializeField]
        private float lifetime = 3.75f;
        [SerializeField]
        private int framesX = 2, framesY = 2;

        [SerializeField]
        private SurfaceData generic = null;
        [SerializeField]
        private SurfaceData[] surfaces = null;


        [SerializeField]
        private bool mainFo, surfacesFo;


        // Spawn HitEffect
        public static void SpawnHitEffect( HitEffect hitObject, RaycastHit hit, bool showHitTexture = false )
        {
            if( hit.collider == null || hit.collider.isTrigger )
                return;

            if( hitObject == null )
            {
                Debug.LogError( "ERROR: Decal Object is not setup!" );
                return;
            }
                       
            Texture hitTexture = hitObject.generic.hitTexture;
            AudioClip hitSound = hitObject.generic.hitSound;
            ParticleSystem hitParticle = hitObject.generic.hitParticle;

            int tmpIndex = SurfaceDetector.GetSurfaceIndexByHit( hit );
            foreach( SurfaceData sur in hitObject.surfaces )
            {
                if( sur.index == tmpIndex )
                {
                    hitTexture = sur.hitTexture;
                    hitSound = sur.hitSound;
                    hitParticle = sur.hitParticle;
                    break;
                }
            }

            HitEffect tmpHitObject = hitObject.SpawnCopy( hit.point + hit.normal * .0003f, Quaternion.FromToRotation( Vector3.forward, hit.normal ) );
            tmpHitObject.Configure( showHitTexture ? hitTexture : null, hitSound, hitParticle );
            tmpHitObject.transform.SetParent( hit.transform );
        }


        // Configure
        private void Configure( Texture texture, AudioClip sound, ParticleSystem particle )
        {
            Transform m_Transform = transform;
            m_Transform.Rotate( 0f, 0f, Random.Range( -180f, 180f ), Space.Self ); //Random Rotation

            // Play SFX
            Audio.PlayClipAtPoint( sound, m_Transform.position ).SetParent( m_Transform );

            // Spawn FX
            if( particle != null )
            {
                ParticleSystem tmpParticle = particle.SpawnCopy( m_Transform.position, m_Transform.rotation );

#if UNITY_5_4_PLUS
                var main = tmpParticle.main;
                main.playOnAwake = main.loop = false;
                float startLifetime = main.startLifetime.constant;
#else
		        tmpParticle.playOnAwake = tmpParticle.loop = false;
                tmpParticle.playbackSpeed = Time.timeScale;
                float startLifetime = particle.startLifetime;
#endif
                tmpParticle.Play();
                tmpParticle.transform.SetParent( m_Transform );
                Destroy( tmpParticle.gameObject, startLifetime );
            }

            // Show Texture
            if( texture != null )
            {
                MeshFilter tmpMeshFilter = GetComponent<MeshFilter>();
                if( tmpMeshFilter != null && tmpMeshFilter.mesh != null )
                {
                    //Random UVs
                    int random = Random.Range( 0, framesX * framesY );
                    int x = Mathf.RoundToInt( random % framesX );
                    int y = Mathf.RoundToInt( random / framesY );

                    Vector2[] quadUVs = new Vector2[] { Vector2.zero, Vector2.up, Vector2.right, Vector2.one };
                    Vector2[] meshUVs = new Vector2[ 4 ];

                    //Debug.Log( "x: " + x + " y: " + y );
                    for( int i = 0; i < 4; i++ )
                    {
                        meshUVs[ i ].x = ( quadUVs[ i ].x + x ) * ( 1f / framesX );
                        meshUVs[ i ].y = ( quadUVs[ i ].y + y ) * ( 1f / framesY );
                    }

                    tmpMeshFilter.mesh.uv = meshUVs;
                }
            }
            else
            {
                if( particle != null && sound != null )
                {
                    lifetime = Mathf.Max( sound.length,
#if UNITY_5_4_PLUS
                                        particle.main.startLifetime.constant
#else
		                                particle.startLifetime 
#endif
                                        );
                }
                else
                {
                    if( particle != null )
#if UNITY_5_4_PLUS
                        lifetime = particle.main.startLifetime.constant;
#else
		                lifetime = particle.startLifetime;
#endif

                    if( sound != null )
                        lifetime = sound.length;
                }
            }

            StartCoroutine( RemoveDecal( texture ) );
        }

        // Remove Decal
        private IEnumerator RemoveDecal( Texture texture )
        {
            Renderer tmpRenderer = GetComponent<Renderer>();
            Material tmpMat = ( tmpRenderer != null ) ? tmpRenderer.material : null;

            if( tmpMat != null )
            {
                if( texture != null )
                    tmpMat.mainTexture = texture;
                else
                    tmpMat.color = Color.clear;
            }

            float elapsed = 0f;
            while( elapsed < lifetime )
            {
                elapsed += Time.deltaTime;
                yield return null;
            }

            // useFade
            if( texture != null && tmpMat != null )
            {                
                Color tmpColor = tmpMat.color;
                while( tmpColor.a > 0f )
                {
                    tmpColor.a -= Time.smoothDeltaTime;
                    tmpMat.color = tmpColor;
                    yield return null;
                }
            }

            Destroy( gameObject );
        }
        

        // OnDisable
        void OnDisable()
        {
            StopCoroutine( "RemoveDecal" );
            Destroy( gameObject );
        }
    };
}