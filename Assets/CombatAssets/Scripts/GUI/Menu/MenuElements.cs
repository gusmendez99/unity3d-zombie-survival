/********************************************
 * Copyright(c): 2016 Victor Klepikov       *
 *                                          *
 * Site: 	     http://u3d.as/5Fb		    *
 * Support: 	 http://bit.ly/vk-Support   *
 ********************************************/


using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


namespace AdvancedShooterKit
{
    public class MenuElements : MonoBehaviour
    {
        public Slider
            lookSens,
            masterVol, musicVol, SFXVol, voiceVol;

        public Toggle
            showHud, showCrosshair,
            invLookX, invLookY;

        public enum EFirstPanel { Gameplay, Audio }
        public EFirstPanel firstPanel = EFirstPanel.Gameplay;

        public GameObject gameplayPanel, audioPanel;

        private static MenuElements instance = null;


        // SetActive
        public static void SetActive( bool value )
        {          
            if( value )            
                ASKInputManager.UnblockCursor();            
            else            
                ASKInputManager.BlockCursor();            

            instance.gameObject.SetActive( value );
        }

        // Awake
        internal void AwakeMENU()
        {
            instance = this;
            //GameSettings.UpdateMixerVolumes();
        }

        // Start
        void Start()
        {
            if( firstPanel == EFirstPanel.Audio )
                gameplayPanel.SetActive( false );
            else
                audioPanel.SetActive( false );
        }

        // OnEnable
        void OnEnable()
        {
            if( !Application.isPlaying )
                return;

            showHud.isOn = GameSettings.ShowHud;
            showCrosshair.isOn = GameSettings.ShowCrosshair;
            invLookX.isOn = GameSettings.InvertLookX;
            invLookY.isOn = GameSettings.InvertLookY;
            //
            lookSens.value = GameSettings.LookSensitivity;
            //
            masterVol.value = GameSettings.MasterVolume;
            musicVol.value = GameSettings.MusicVolume;
            SFXVol.value = GameSettings.SFXVolume;
            voiceVol.value = GameSettings.VoiceVolume;
        }

        // Set Hud IsOn
        public void SetHudIsOn( bool value )
        {
            GameSettings.ShowHud = value;
        }
        // Set Crosshair IsOn
        public void SetCrosshairIsOn( bool value )
        {
            GameSettings.ShowCrosshair = value;
        }
        // Set InvLookX IsOn
        public void SetInvLookXIsOn( bool value )
        {
            GameSettings.InvertLookX = value;
        }
        // Set InvLookY IsOn
        public void SetInvLookYIsOn( bool value )
        {
            GameSettings.InvertLookY = value;
        }

        // Set LookSens
        public void SetLookSens( float value )
        {
            GameSettings.LookSensitivity = value;
        }

        // Set MasterVolume
        public void SetMasterVolume( float value )
        {
            GameSettings.MasterVolume = value;
        }
        // Set MusicVolume
        public void SetMusicVolume( float value )
        {
            GameSettings.MusicVolume = value;
        }
        // Set SFXVolume
        public void SetSFXVolume( float value )
        {
            GameSettings.SFXVolume = value;
        }
        // Set VoiceVolume
        public void SetVoiceVolume( float value )
        {
            GameSettings.VoiceVolume = value;
        }


        // UnPause
        public void UnPause()
        {
            ASKInputManager.Pause();
        }


        // Quit Game
        public void QuitGame()
        {
            #if UNITY_EDITOR
		    UnityEditor.EditorApplication.isPlaying = false;
            #else
            Application.Quit();
            #endif
        }

        // Restart Level
        public void StartReloadScene()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene( "Scene", LoadSceneMode.Single );
        }
    }
}