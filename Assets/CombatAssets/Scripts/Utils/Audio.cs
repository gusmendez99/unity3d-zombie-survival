/********************************************
 * Copyright(c): 2016 Victor Klepikov       *
 *                                          *
 * Site: 	     http://u3d.as/5Fb		    *
 * Support: 	 http://bit.ly/vk-Support   *
 ********************************************/


using UnityEngine;


namespace AdvancedShooterKit.Utils
{
    public static class Audio
    {
        // PlayClip AtPoint
        public static Transform PlayClipAtPoint( AudioClip clip, Vector3 position, float lifetime = 0f ) 
        {
            GameObject gameObject = new GameObject( "ASK_OneShotSound" );
            Transform transform = gameObject.transform.MoveToCache();

            if( clip != null )
            {
                transform.position = position;
                AudioSource tmpAudio = gameObject.AddComponent<AudioSource>();
                tmpAudio.SetupForSFX();
                tmpAudio.PlayOneShot( clip );
                Object.Destroy( gameObject, ( lifetime > 0f ) ? lifetime : clip.length );
                return transform;
            }

            Object.Destroy( gameObject, lifetime );
            return transform;
        }

        // Get RandomClip
        public static AudioClip GetRandomClip( AudioClip[] clipsArray )
        {
            if( clipsArray.Length == 0 )
                return null;

            return clipsArray[ Random.Range( 0, clipsArray.Length ) ];
        }

        // Setup SFX AudioSource
        public static void SetupForSFX( this AudioSource m_Audio )
        {
            m_Audio.outputAudioMixerGroup = GameSettings.SFXOutput;
            m_Audio.playOnAwake = false;
            m_Audio.loop = false;
            m_Audio.spatialBlend = 1f;
            m_Audio.pitch = Time.timeScale;
        }
    }
}