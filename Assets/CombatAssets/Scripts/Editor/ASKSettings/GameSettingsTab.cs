/********************************************
 * Copyright(c): 2016 Victor Klepikov       *
 *                                          *
 * Site: 	     http://u3d.as/5Fb		    *
 * Support: 	 http://bit.ly/vk-Support   *
 ********************************************/


using UnityEngine;
using UnityEditor;


namespace AdvancedShooterKit.Inspector
{
    public static class GameSettingsTab
    {
        private static string MAIN_DATABASE_PATH { get { return ASKSettingsWindow.mainDirectory + "/GameSettings.asset"; } }
        private static string TMP_DATABASE_PATH { get { return ASKSettingsWindow.mainDirectory + "/tmp/GameSettingsTMP.asset"; } }
        
        //
        private static SerializedObject tmpSerializedObject = null;
        private static SerializedProperty
            showHudProp, showCrosshairProp, damageIndicationProp,
            difficultyLevelProp,
            invertLookXProp, invertLookYProp,
            lookSensitivityProp,
            masterVolumeProp, sfxVolumeProp, musicVolumeProp, voiceVolumeProp,
            masterMixerProp, sfxOutputProp, musicOutputProp, voiceOutputProp;

        private static Vector2 scroll = Vector2.zero;


        // Load CurrentAssetFile
        private static GameSettings LoadAssetFile( string path )
        {
            GameSettings currentFile = AssetDatabase.LoadAssetAtPath( path, typeof( GameSettings ) ) as GameSettings;

            if( currentFile == null )
            {
                currentFile = ScriptableObject.CreateInstance<GameSettings>();
                AssetDatabase.CreateAsset( currentFile, path );
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

            return currentFile;
        }

        // Save CopyAssetFile
        private static void SaveCopyAssetFile( string copyFrom, string copyTo )
        {
            if( copyFrom == MAIN_DATABASE_PATH )
                LoadAssetFile( copyFrom );

            AssetDatabase.DeleteAsset( copyTo );
            AssetDatabase.CopyAsset( copyFrom, copyTo );
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        

        // Setup Tab
        internal static void SetupTab()
        {
            if( tmpSerializedObject == null )
                SaveCopyAssetFile( MAIN_DATABASE_PATH, TMP_DATABASE_PATH );            

            tmpSerializedObject = new SerializedObject( LoadAssetFile( TMP_DATABASE_PATH ) );

            //
            showHudProp = tmpSerializedObject.FindProperty( "showHud" );
            showCrosshairProp = tmpSerializedObject.FindProperty( "showCrosshair" );
            damageIndicationProp = tmpSerializedObject.FindProperty( "damageIndication" );
            difficultyLevelProp = tmpSerializedObject.FindProperty( "difficultyLevel" );
            invertLookXProp = tmpSerializedObject.FindProperty( "invertLookX" );
            invertLookYProp = tmpSerializedObject.FindProperty( "invertLookY" );
            lookSensitivityProp = tmpSerializedObject.FindProperty( "lookSensitivity" );
            masterVolumeProp = tmpSerializedObject.FindProperty( "masterVolume" );
            sfxVolumeProp = tmpSerializedObject.FindProperty( "sfxVolume" );
            musicVolumeProp = tmpSerializedObject.FindProperty( "musicVolume" );
            voiceVolumeProp = tmpSerializedObject.FindProperty( "voiceVolume" );
            masterMixerProp = tmpSerializedObject.FindProperty( "masterMixer" );
            sfxOutputProp = tmpSerializedObject.FindProperty( "sfxOutput" );
            musicOutputProp = tmpSerializedObject.FindProperty( "musicOutput" );
            voiceOutputProp = tmpSerializedObject.FindProperty( "voiceOutput" );
        }

        // Reload Settings
        internal static void ReloadSettings()
        {
            SaveCopyAssetFile( MAIN_DATABASE_PATH, TMP_DATABASE_PATH );
            FullReset();
            SetupTab();
        }

        // Save Settings
        internal static void SaveSettings()
        {
            SaveCopyAssetFile( TMP_DATABASE_PATH, MAIN_DATABASE_PATH );

            /*
            GameSettings mainFile = LoadAssetFile( MAIN_DATABASE_PATH );
            GameSettings tmpFile = LoadAssetFile( TMP_DATABASE_PATH );

            mainFile.genericSurface = tmpFile.genericSurface;
            mainFile.surfaces = tmpFile.surfaces;
            mainFile.decal = tmpFile.decal;

            EditorUtility.SetDirty( mainFile );
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            */
        }
        


        // OnWindowGUI
        internal static void OnWindowGUI()
        {
            // BEGIN
            tmpSerializedObject.Update();
            // BEGIN

            ShowSide();

            // END
            tmpSerializedObject.ApplyModifiedProperties();
            // END
        }

        // Show Side
        private static void ShowSide()
        {
            scroll = EditorGUILayout.BeginScrollView( scroll, "Box", GUILayout.ExpandWidth( true ), GUILayout.ExpandHeight( true ) );
            //

            GUILayout.Space( 5f );            
            //
            GUILayout.BeginVertical( "Box" );
            GUILayout.Space( -2f );
            GUILayout.BeginHorizontal();
            GUILayout.Space( ASKSettingsWindow.width / 2.25f );
            GUILayout.Label( "Gameplay", StyleHelper.LabelStyle );
            GUILayout.EndHorizontal();
            ShowGameplayTab();
            GUILayout.Space( 10f );
            GUILayout.EndVertical();
            //
            GUILayout.Space( 7f );
            //
            GUILayout.BeginVertical( "Box" );
            GUILayout.Space( -2f );
            GUILayout.BeginHorizontal();
            GUILayout.Space( ASKSettingsWindow.width / 2.25f );
            GUILayout.Label( "Audio", StyleHelper.LabelStyle );
            GUILayout.EndHorizontal();
            ShowSoundTab();
            GUILayout.Space( 10f );
            GUILayout.EndVertical();

            //
            EditorGUILayout.EndScrollView();
        }

        
        // Show GameplayTab
        private static void ShowGameplayTab()
        {
            const float centerSpace = 150f;
            const float width = 115f;

            GUILayout.BeginHorizontal();
            GUILayout.Space( centerSpace );
            GUILayout.Label( "Difficulty Level", GUILayout.Width( width ) );
            EditorGUILayout.PropertyField( difficultyLevelProp, GUIContent.none );
            GUILayout.Space( centerSpace );
            GUILayout.EndHorizontal();            

            GUILayout.Space( 5f );

            GUILayout.BeginHorizontal();
            GUILayout.Space( centerSpace );
            GUILayout.Label( "Invert Look X", GUILayout.Width( width ) );
            EditorGUILayout.PropertyField( invertLookXProp, GUIContent.none );
            GUILayout.Space( centerSpace );
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space( centerSpace );
            GUILayout.Label( "Invert Look Y", GUILayout.Width( width ) );
            EditorGUILayout.PropertyField( invertLookYProp, GUIContent.none );
            GUILayout.Space( centerSpace );
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space( centerSpace );
            GUILayout.Label( "Look Sensitivity", GUILayout.Width( width ) );
            EditorGUILayout.PropertyField( lookSensitivityProp, GUIContent.none );
            GUILayout.Space( centerSpace );
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space( centerSpace );
            GUILayout.Label( "Show HUD", GUILayout.Width( width ) );
            EditorGUILayout.PropertyField( showHudProp, GUIContent.none );
            GUILayout.Space( centerSpace );
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space( centerSpace );
            GUILayout.Label( "Show Crosshair", GUILayout.Width( width ) );
            EditorGUILayout.PropertyField( showCrosshairProp, GUIContent.none );
            GUILayout.Space( centerSpace );
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space( centerSpace );
            GUILayout.Label( "Damage Indication", GUILayout.Width( width ) );
            EditorGUILayout.PropertyField( damageIndicationProp, GUIContent.none );
            GUILayout.Space( centerSpace );
            GUILayout.EndHorizontal();
        }
        // Show SoundTab
        private static void ShowSoundTab()
        {
            const float centerSpace = 150f;

            GUILayout.BeginHorizontal();
            GUILayout.Space( centerSpace );
            GUILayout.Label( "Master Volume", GUILayout.Width( 115f ) );
            EditorGUILayout.PropertyField( masterVolumeProp, GUIContent.none );
            GUILayout.Space( centerSpace );
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space( centerSpace );
            GUILayout.Label( "Master Mixer", GUILayout.Width( 115f ) );
            EditorGUILayout.PropertyField( masterMixerProp, GUIContent.none );
            GUILayout.Space( centerSpace );
            GUILayout.EndHorizontal();
            //
            GUILayout.Space( 5f );
            //
            GUILayout.BeginHorizontal();
            GUILayout.Space( centerSpace );
            GUILayout.Label( "Music Volume", GUILayout.Width( 115f ) );
            EditorGUILayout.PropertyField( musicVolumeProp, GUIContent.none );
            GUILayout.Space( centerSpace );
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space( centerSpace );
            GUILayout.Label( "Music Output", GUILayout.Width( 115f ) );
            EditorGUILayout.PropertyField( musicOutputProp, GUIContent.none );
            GUILayout.Space( centerSpace );
            GUILayout.EndHorizontal();
            //            
            GUILayout.Space( 5f );
            //
            GUILayout.BeginHorizontal();
            GUILayout.Space( centerSpace );
            GUILayout.Label( "SFX Volume", GUILayout.Width( 115f ) );
            EditorGUILayout.PropertyField( sfxVolumeProp, GUIContent.none );
            GUILayout.Space( centerSpace );
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space( centerSpace );
            GUILayout.Label( "SFX Output", GUILayout.Width( 115f ) );
            EditorGUILayout.PropertyField( sfxOutputProp, GUIContent.none );
            GUILayout.Space( centerSpace );
            GUILayout.EndHorizontal();
            //
            GUILayout.Space( 5f );
            //
            GUILayout.BeginHorizontal();
            GUILayout.Space( centerSpace );
            GUILayout.Label( "Voice Volume", GUILayout.Width( 115f ) );
            EditorGUILayout.PropertyField( voiceVolumeProp, GUIContent.none );
            GUILayout.Space( centerSpace );
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space( centerSpace );
            GUILayout.Label( "Voice Output", GUILayout.Width( 115f ) );
            EditorGUILayout.PropertyField( voiceOutputProp, GUIContent.none );
            GUILayout.Space( centerSpace );
            GUILayout.EndHorizontal();
        }


        // FullReset
        internal static void FullReset()
        {
            tmpSerializedObject = null;

            showHudProp = showCrosshairProp = damageIndicationProp = null;
            difficultyLevelProp = null;
            invertLookXProp = invertLookYProp = lookSensitivityProp = null;
            masterVolumeProp = sfxVolumeProp = musicVolumeProp = voiceVolumeProp = null;
            masterMixerProp = sfxOutputProp = musicOutputProp = voiceOutputProp = null;

            scroll = Vector2.zero;
        }
    }
}