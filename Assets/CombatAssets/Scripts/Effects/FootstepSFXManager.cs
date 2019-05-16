/********************************************
 * Copyright(c): 2016 Victor Klepikov       *
 *                                          *
 * Site: 	     http://u3d.as/5Fb		    *
 * Support: 	 http://bit.ly/vk-Support   *
 ********************************************/


using UnityEngine;
using AdvancedShooterKit.Utils;


namespace AdvancedShooterKit
{
    public class FootstepSFXManager : MonoBehaviour, IFootstepSFXManager
    {
        [System.Serializable]
        public sealed class SurfaceData
        {
            public string name = string.Empty;
            public int index = 0;            
            public AudioClip jumpingSFX, landingSFX;
            public AudioClip[] footstepSounds = null;
        }

        [SerializeField]
        private SurfaceData generic = null;
        [SerializeField]
        private SurfaceData[] surfaces = null;


        private AudioSource m_Audio = null;


        // Use this for initialization
        void Awake()
        {
            m_Audio = this.GetComponent<AudioSource>();
            m_Audio.SetupForSFX();
        }


        // Play JumpingSound
        public void PlayJumpingSound( RaycastHit hit )
        {
            m_Audio.PlayOneShot( GetSurfaceByHit( hit ).jumpingSFX );
        }

        // Play LandingSound
        public void PlayLandingSound( RaycastHit hit )
        {
            m_Audio.PlayOneShot( GetSurfaceByHit( hit ).landingSFX );
        }

        // Play FootStepAudio
        public void PlayFootStepSound( RaycastHit hit )
        {
            AudioClip[] stepSounds = GetSurfaceByHit( hit ).footstepSounds;

            //Play RandomStepSound
            int index = Random.Range( 1, stepSounds.Length );
            m_Audio.clip = stepSounds[ index ];
            m_Audio.PlayOneShot( m_Audio.clip );
            stepSounds[ index ] = stepSounds[ 0 ];
            stepSounds[ 0 ] = m_Audio.clip;
        }


        // GetSurface ByHit
        private SurfaceData GetSurfaceByHit( RaycastHit hit )
        {
            m_Audio.outputAudioMixerGroup = GameSettings.SFXOutput;
            m_Audio.pitch = Time.timeScale;

            int tmpIndex = SurfaceDetector.GetSurfaceIndexByHit( hit );
            foreach( SurfaceData sur in surfaces )            
                if( sur.index == tmpIndex )                
                    return sur;

            return generic;
        }
    }
}