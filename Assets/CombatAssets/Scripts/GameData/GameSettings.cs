/********************************************
 * Copyright(c): 2016 Victor Klepikov       *
 *                                          *
 * Site: 	     http://u3d.as/5Fb		    *
 * Support: 	 http://bit.ly/vk-Support   *
 ********************************************/


using UnityEngine;
using UnityEngine.Audio;


namespace AdvancedShooterKit
{
    public class GameSettings : ScriptableObject
    {
        [SerializeField]
        private bool showHud = true;
        [SerializeField]
        private bool showCrosshair = true;
        [SerializeField]
        private EDamageIndication damageIndication = EDamageIndication.ForAll;

        [SerializeField]
        private EDifficultyLevel difficultyLevel = EDifficultyLevel.Normal;
        [SerializeField]
        private bool invertLookX, invertLookY;
        [Range( 1f, 10f )][SerializeField]
        private float lookSensitivity = 1f;

        [Range( -80f, 0f )][SerializeField]
        private float masterVolume, musicVolume, sfxVolume, voiceVolume;
        [SerializeField]
        private AudioMixer masterMixer = null;
        [SerializeField]
        private AudioMixerGroup sfxOutput = null, musicOutput = null, voiceOutput = null;


        private static GameSettings instance = null;
        private static GameSettings m_Instance
        {
            get
            {
                if( instance == null )
                    instance = Resources.Load<GameSettings>( "GameSettings" );

                return instance;
            }
        }


        
        // Show Hud
        public static bool ShowHud
        {
            get { return m_Instance.showHud; }
            set
            {
                if( m_Instance.showHud == value || !Application.isPlaying )
                    return;

                m_Instance.showHud = value;

                if( !ASKInputManager.GameIsPaused )
                    HudElements.SetActive( value );
            }
        }
        // Show Crosshair
        public static bool ShowCrosshair
        {
            get { return m_Instance.showCrosshair; }
            set
            {
                if( m_Instance.showCrosshair == value || !Application.isPlaying )
                    return;

                m_Instance.showCrosshair = value;

                if( value )
                    WeaponsManager.UpdateHud( true );                
                else
                    HudElements.Crosshair.SetActive( ECrosshairView.None );
            }
        }
        // Damage Indication
        public static EDamageIndication DamageIndication
        {
            get { return m_Instance.damageIndication; }
            set
            {
                m_Instance.damageIndication = value;

                if( value == EDamageIndication.OFF )
                    HudElements.Crosshair.SetColor( ECrosshairColor.Normal );
            }
        }

        // Difficulty Level
        public static EDifficultyLevel DifficultyLevel
        {
            get { return m_Instance.difficultyLevel; }
            set { m_Instance.difficultyLevel = value; }
        }

        // Invert Look X
        public static bool InvertLookX
        {
            get { return m_Instance.invertLookX; }
            set { m_Instance.invertLookX = value; }
        }
        // Invert Look Y
        public static bool InvertLookY
        {
            get { return m_Instance.invertLookY; }
            set { m_Instance.invertLookY = value; }
        }

        // InvertLook GetFloatValue
        public static float InvertLookPosNegValue_X
        {
            get { return ( m_Instance.invertLookX ? -1f : 1f ); }
        }
        public static float InvertLookPosNegValue_Y
        {
            get { return ( m_Instance.invertLookY ? -1f : 1f ); }
        }

        // Look Sensitivity
        public static float LookSensitivity
        {
            get { return m_Instance.lookSensitivity; }
            set { m_Instance.lookSensitivity = value; }
        }

        // Look Sensitivity GetByInvert X
        public static float GetLookSensitivityByInvert_X
        {
            get { return ( m_Instance.invertLookX ? -m_Instance.lookSensitivity : m_Instance.lookSensitivity ); }
        }

        // Look Sensitivity GetByInvert Y
        public static float GetLookSensitivityByInvert_Y
        {
            get { return ( m_Instance.invertLookY ? -m_Instance.lookSensitivity : m_Instance.lookSensitivity ); }
        }

        
        // Master Volume
        public static float MasterVolume
        {
            get { return m_Instance.masterVolume; }
            set
            {
                if( m_Instance.masterVolume == value )
                    return;
                
                m_Instance.masterVolume = value;
                SetVolumeByType( EVolumeType.Master, value );
            }
        }

        // Music Volume
        public static float MusicVolume
        {
            get { return m_Instance.musicVolume; }
            set
            {
                if( m_Instance.musicVolume == value )
                    return;

                m_Instance.musicVolume = value;
                SetVolumeByType( EVolumeType.Music, value );
            }
        }
        // SFX[Sounds] Volume
        public static float SFXVolume
        {
            get { return m_Instance.sfxVolume; }
            set
            {
                if( m_Instance.sfxVolume == value )
                    return;

                m_Instance.sfxVolume = value;
                SetVolumeByType( EVolumeType.SFX, value );
            }
        }        
        // Voice Volume
        public static float VoiceVolume
        {
            get { return m_Instance.voiceVolume; }
            set
            {
                if( m_Instance.voiceVolume == value )
                    return;

                m_Instance.voiceVolume = value;
                SetVolumeByType( EVolumeType.Voice, value );
            }
        }


        public static AudioMixer MasterMixer { get { return m_Instance.masterMixer; } }
        public static AudioMixerGroup MusicOutput { get { return m_Instance.musicOutput; } }
        public static AudioMixerGroup SFXOutput { get { return m_Instance.sfxOutput; } }        
        public static AudioMixerGroup VoiceOutput { get { return m_Instance.voiceOutput; } }


        // SetVolume ByType
        public static void SetVolumeByType( EVolumeType volumeType, float value )
        {
            m_Instance.masterMixer.SetFloat( volumeType + "Volume", value );
        }


        // Update MixerVolumes
        internal static void UpdateMixerVolumes()
        {
            SetVolumeByType( EVolumeType.Master, m_Instance.masterVolume );
            SetVolumeByType( EVolumeType.Music, m_Instance.musicVolume );
            SetVolumeByType( EVolumeType.SFX, m_Instance.sfxVolume );
            SetVolumeByType( EVolumeType.Voice, m_Instance.voiceVolume );
        }                
    }
}