/********************************************
 * Copyright(c): 2016 Victor Klepikov       *
 *                                          *
 * Site: 	     http://u3d.as/5Fb		    *
 * Support: 	 http://bit.ly/vk-Support   *
 ********************************************/


using System.Collections;
using UnityEngine;
using AdvancedShooterKit.Utils;
using AdvancedShooterKit.Events;


namespace AdvancedShooterKit
{
    public class Respawner : MonoBehaviour
    {
        [SerializeField]
        private ESpawnMode spawnMode = ESpawnMode.StartPosition;

        //[Range( 1f, 120f )]
        [SerializeField]
        private float
            minRespawnTime = 15f
            , maxRespawnTime = 20f;

        [SerializeField]
        private AudioClip spawnSFX = null;

        [SerializeField]
        private bool smoothScale = false;
        [SerializeField, Range( 1f, 10f )]
        private float scaleSpeed = 5f;

        // Events
        [SerializeField]
        private ASKEvent 
            RespawnStarted = null
            , RespawnEnded = null;        

        //
        private Transform m_Transform = null;
        private Vector3 nativePosition = Vector3.zero;
        private Quaternion nativeRotation = Quaternion.identity;

        public float progress = 1f;
        public int seconds = 0;


        // Awake
        void Awake()
        {
            m_Transform = transform;            
            nativePosition = m_Transform.position;
            nativeRotation = m_Transform.rotation;
        }


        // Start Respawn
        public static void StartRespawn( GameObject bodyObj, float delay = 0f )
        {
            Respawner respawner = bodyObj.GetComponent<Respawner>();
            if( respawner == null )
            {
                Destroy( bodyObj, delay );
                return;
            }

            GameObject timerGO = new GameObject( "respawTimerOf-" + bodyObj.name, typeof( EmptyMono ) );

            timerGO.MoveToCache()
                   .GetComponent<MonoBehaviour>()
                   .StartCoroutine( StartPawn( respawner, timerGO, delay ) );
        }

        // Start Pawn
        private static IEnumerator StartPawn( Respawner respawner, GameObject timerGO, float delay )
        {
            if( delay > 0f )
                yield return new WaitForSeconds( delay );

            respawner.RespawnStarted.Invoke();
            respawner.gameObject.SetActive( false );


            float respawnTime = Random.Range( respawner.minRespawnTime, respawner.maxRespawnTime );

            for( float crTime = 0f; crTime < respawnTime; crTime += Time.deltaTime )
            {
                respawner.seconds = Mathf.RoundToInt( respawnTime - crTime );
                respawner.progress = crTime / respawnTime;
                yield return null;
            }

            respawner.seconds = 0;
            respawner.progress = 1f;

            respawner.gameObject.SetActive( true );                        

            if( respawner.spawnMode == ESpawnMode.StartPosition )
            {
                respawner.m_Transform.position = respawner.nativePosition;
                respawner.m_Transform.rotation = respawner.nativeRotation;
            }

            // OnRespawn
            respawner.Send( "OnRespawn" )
                     .RespawnEnded.Invoke();
                        
            if( respawner.smoothScale )
            {
                float smoothTime = .1f;
                Vector3 nativeScale = respawner.m_Transform.localScale;
                //
                while( smoothTime < 1f )
                {
                    smoothTime += Time.smoothDeltaTime * respawner.scaleSpeed;
                    respawner.m_Transform.localScale = nativeScale * smoothTime;
                    yield return null;
                }
                
                respawner.m_Transform.localScale = nativeScale;
            }

            // Remove timer
            if( respawner.spawnSFX == null )
                Destroy( timerGO );
            else
                timerGO.transform.SetParent( Audio.PlayClipAtPoint( respawner.spawnSFX, respawner.m_Transform.position ) );
        }
    };
}

//RaycastHit floorHit;
//Physics.SphereCast( position, 5f, Vector3.down, out floorHit, 10f );