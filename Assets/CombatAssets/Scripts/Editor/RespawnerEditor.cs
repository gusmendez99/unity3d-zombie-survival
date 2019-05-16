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
    [CustomEditor( typeof( Respawner ) )]
    [CanEditMultipleObjects]
    public class RespawnerEditor : Editor
    {
        private SerializedProperty
            spawnModeProp,
            minRespawnTimeProp, maxRespawnTimeProp,
            spawnSFXProp,
            smoothScaleProp, scaleSpeedProp,
            RespawnStartedProp, RespawnEndedProp, 
            progress, seconds;

        // OnEnable
        void OnEnable()
        {
            spawnModeProp = serializedObject.FindProperty( "spawnMode" );
            minRespawnTimeProp = serializedObject.FindProperty( "minRespawnTime" );
            maxRespawnTimeProp = serializedObject.FindProperty( "maxRespawnTime" );
            spawnSFXProp = serializedObject.FindProperty( "spawnSFX" );
            smoothScaleProp = serializedObject.FindProperty( "smoothScale" );
            scaleSpeedProp = serializedObject.FindProperty( "scaleSpeed" );
            RespawnStartedProp = serializedObject.FindProperty( "RespawnStarted" );
            RespawnEndedProp = serializedObject.FindProperty( "RespawnEnded" );

            progress = serializedObject.FindProperty( "progress" );
            seconds = serializedObject.FindProperty( "seconds" );
        }


        // OnInspectorGUI
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            ShowParameters();
            serializedObject.ApplyModifiedProperties();
        }


        // Show Parameters
        private void ShowParameters()
        {
            GUILayout.Space( 5f );
            EditorGUILayout.PropertyField( spawnModeProp );
            EditorHelper.ShowMinMaxSlider( minRespawnTimeProp, maxRespawnTimeProp, 1f, 120f, "Respawn Range" );

            GUILayout.Space( 5f );
            EditorHelper.ShowSFXPropertyAndPlayButton( serializedObject, spawnSFXProp );

            EditorHelper.ShowBoolField( ref smoothScaleProp, "Smooth Scale" );
            EditorHelper.ShowPropertyField( ref scaleSpeedProp, "Scale Speed", 22f );
            GUI.enabled = true;

            GUILayout.Space( 10f );
            EditorHelper.ShowProgressBar( progress.floatValue, string.Format( "Time : {0}", System.TimeSpan.FromSeconds( seconds.intValue ) ) ); 
            GUILayout.Space( 5f );

            EditorGUILayout.PropertyField( RespawnStartedProp, false, null );
            EditorGUILayout.PropertyField( RespawnEndedProp, false, null );            
        }
    };
}