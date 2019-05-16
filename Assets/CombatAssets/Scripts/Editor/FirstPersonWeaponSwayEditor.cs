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
    [CustomEditor( typeof( FirstPersonWeaponSway ) )]
    public class FirstPersonWeaponSwayEditor : Editor
    {
        // Sway
        private SerializedProperty 
            useSwayProp, swaySmoothingProp, borderSizeProp, fpMoveSpeedProp, runOffsetProp;

        // Animation
        private SerializedProperty 
            fireClipProp, fireAnimSpeedProp,
            reloadClipProp, reloadAnimSpeedProp,
            dropoutSmoothingProp, dropoutRotationProp;       

        // Ironsighting 
        private SerializedProperty 
            useIronsightingProp,
            ironsightSmoothingProp, ironsightDispersionProp,
            addMoveProp, addLookProp, addRunAndInAirProp,
            crouchedProp, zoomFOVProp, ironsightMoveSpeedProp, zoomPosProp;


        // Crosshair
        private SerializedProperty crosshairView = null;

        // Pivot
        private SerializedProperty pivotPositionProp = null;
        

        private GUIContent guiContent = new GUIContent( string.Empty );


        // OnEnable
        void OnEnable()
        {
            useSwayProp = serializedObject.FindProperty( "useSway" );
            swaySmoothingProp = serializedObject.FindProperty( "swaySmoothing" );
            borderSizeProp = serializedObject.FindProperty( "borderSize" );
            fpMoveSpeedProp = serializedObject.FindProperty( "fpMoveSpeed" );
            runOffsetProp = serializedObject.FindProperty( "runOffset" );

            fireClipProp = serializedObject.FindProperty( "fireClip" );
            fireAnimSpeedProp = serializedObject.FindProperty( "fireAnimSpeed" );
            reloadClipProp = serializedObject.FindProperty( "reloadClip" );
            reloadAnimSpeedProp = serializedObject.FindProperty( "reloadAnimSpeed" );
            dropoutSmoothingProp = serializedObject.FindProperty( "dropoutSmoothing" );
            dropoutRotationProp = serializedObject.FindProperty( "dropoutRotation" );            

            useIronsightingProp = serializedObject.FindProperty( "useIronsighting" );
            ironsightSmoothingProp = serializedObject.FindProperty( "ironsightSmoothing" );
            ironsightDispersionProp = serializedObject.FindProperty( "ironsightDispersion" );
            addMoveProp = serializedObject.FindProperty( "addMove" );
            addLookProp = serializedObject.FindProperty( "addLook" );
            addRunAndInAirProp = serializedObject.FindProperty( "addRunAndInAir" );
            crouchedProp = serializedObject.FindProperty( "crouched" );
            zoomFOVProp = serializedObject.FindProperty( "zoomFOV" );
            ironsightMoveSpeedProp = serializedObject.FindProperty( "ironsightMoveSpeed" );
            zoomPosProp = serializedObject.FindProperty( "zoomPos" );

            crosshairView = serializedObject.FindProperty( "crosshairView" );
            pivotPositionProp = serializedObject.FindProperty( "pivotPosition" );
        }


        // OnInspectorGUI
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            ShowParameters();
            serializedObject.ApplyModifiedProperties();
        }

        // ShowParameters
        private void ShowParameters()
        {
            const float space = 15f;

            GUILayout.Space( 5f );
            guiContent.text = "Use Sway";
            EditorGUILayout.PropertyField( useSwayProp, guiContent );

            GUI.enabled = useSwayProp.boolValue;

            EditorHelper.ShowSubSlider( ref swaySmoothingProp, .1f, 5f, "Smoothing", space );

            EditorHelper.ShowSubSlider( ref borderSizeProp, 1f, 35f, "Border Size", space );

            EditorHelper.ShowSubSlider( ref fpMoveSpeedProp, 0f, 1f, "Move Speed", space );

            EditorHelper.ShowPropertyField( ref runOffsetProp, "Run Offset", space );

            GUI.enabled = true;

            GUILayout.Space( 5f );
            guiContent.text = "Fire Clip";
            EditorGUILayout.PropertyField( fireClipProp, guiContent );

            GUI.enabled = ( fireClipProp.objectReferenceValue != null );

            EditorHelper.ShowSubSlider( ref fireAnimSpeedProp, 1f, 3f, "Play Speed", space );

            GUI.enabled = true;

            guiContent.text = "Reload Clip";
            EditorGUILayout.PropertyField( reloadClipProp, guiContent );
            GUI.enabled = ( reloadClipProp.objectReferenceValue != null );

            EditorHelper.ShowSubSlider( ref reloadAnimSpeedProp, 1f, 3f, "Play Speed", space );

            GUI.enabled = true;

            GUILayout.Space( 5f );
            guiContent.text = "Dropout Rotation";
            EditorGUILayout.PropertyField( dropoutRotationProp, guiContent );
            guiContent.text = "Dropout Smoothing";
            EditorGUILayout.Slider( dropoutSmoothingProp, 1f, 10f, guiContent );            

            GUILayout.Space( 5f );
            guiContent.text = "Use Ironsighting";
            EditorGUILayout.PropertyField( useIronsightingProp, guiContent );

            GUI.enabled = useIronsightingProp.boolValue;

            EditorHelper.ShowSubSlider( ref ironsightSmoothingProp, 1f, 10f, "Smoothing", space );

            EditorHelper.ShowSubSlider( ref ironsightDispersionProp, .1f, 1f, "Dispersion", space );

            EditorHelper.ShowSubSlider( ref addMoveProp, 0f, 2f, "+ Move", space * 2f );

            EditorHelper.ShowSubSlider( ref addLookProp, 0f, 2f, "+ Look", space * 2f );

            EditorHelper.ShowSubSlider( ref addRunAndInAirProp, 0f, 3f, "+ Run & InAir", space * 2f );

            EditorHelper.ShowSubSlider( ref crouchedProp, .1f, 1f, "х Crouched", space * 2f );

            GUILayout.Space( 2f );
            EditorHelper.ShowSubSlider( ref zoomFOVProp, 10f, 50f, "Cameras FOV", space );

            EditorHelper.ShowSubSlider( ref ironsightMoveSpeedProp, .1f, 1f, "Move Speed", space );

            EditorHelper.ShowPropertyField( ref zoomPosProp, "Parent Position", space );

            GUI.enabled = true;

            GUILayout.Space( 5f );
            guiContent.text = "Crosshair View";
            EditorGUILayout.PropertyField( crosshairView, guiContent );

            GUILayout.Space( 5f );
            guiContent.text = "Pivot Position";
            EditorGUILayout.PropertyField( pivotPositionProp, guiContent );
        }
    }
}