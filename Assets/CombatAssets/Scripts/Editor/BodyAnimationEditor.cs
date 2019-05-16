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
    [CustomEditor( typeof( BodyAnimation ) )]
    public class BodyAnimationEditor : Editor
    {
        // Animation Clips
        private SerializedProperty
            idleProp, walkProp, runProp,
            strafeLeftProp, strafeRightProp,
            crouchProp, crouchWalkProp,
            crouchStrafeLeftProp, crouchStrafeRightProp,
            fallingProp;


        // Animation Clips
        private SerializedProperty
            walkSpeedProp,
            backwardsSpeedProp,
            runSpeedProp,
            strafeSpeedProp,
            crouchWalkSpeedProp,
            crouchBackwardsSpeedProp,
            crouchStrafeSpeedProp,
            halfStrafeAngleProp,
            hSAngleSmoothProp;


        private GUIContent guiContent = new GUIContent( string.Empty );


        // OnEnable
        void OnEnable()
        {
            idleProp = serializedObject.FindProperty( "idle" );
            walkProp = serializedObject.FindProperty( "walk" );
            runProp = serializedObject.FindProperty( "run" );
            strafeLeftProp = serializedObject.FindProperty( "strafeLeft" );
            strafeRightProp = serializedObject.FindProperty( "strafeRight" );
            crouchProp = serializedObject.FindProperty( "crouch" );
            crouchWalkProp = serializedObject.FindProperty( "crouchWalk" );
            crouchStrafeLeftProp = serializedObject.FindProperty( "crouchStrafeLeft" );
            crouchStrafeRightProp = serializedObject.FindProperty( "crouchStrafeRight" );
            fallingProp = serializedObject.FindProperty( "falling" );

            walkSpeedProp = serializedObject.FindProperty( "walkSpeed" );
            backwardsSpeedProp = serializedObject.FindProperty( "backwardsSpeed" );
            runSpeedProp = serializedObject.FindProperty( "runSpeed" );
            strafeSpeedProp = serializedObject.FindProperty( "strafeSpeed" );
            crouchWalkSpeedProp = serializedObject.FindProperty( "crouchWalkSpeed" );
            crouchBackwardsSpeedProp = serializedObject.FindProperty( "crouchBackwardsSpeed" );
            crouchStrafeSpeedProp = serializedObject.FindProperty( "crouchStrafeSpeed" );
            halfStrafeAngleProp = serializedObject.FindProperty( "halfStrafeAngle" );
            hSAngleSmoothProp = serializedObject.FindProperty( "hSAngleSmooth" );
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
            guiContent.text = "Idle";
            EditorGUILayout.PropertyField( idleProp, guiContent );

            GUILayout.Space( 5f );

            guiContent.text = "Walk";
            EditorGUILayout.PropertyField( walkProp, guiContent );
            EditorHelper.ShowSubSlider( ref walkSpeedProp, 0f, 2f, "Forward Speed", space );
            EditorHelper.ShowSubSlider( ref backwardsSpeedProp, 0f, 2f, "Backward Speed", space );

            GUILayout.Space( 5f );

            guiContent.text = "Run";
            EditorGUILayout.PropertyField( runProp, guiContent );
            EditorHelper.ShowSubSlider( ref runSpeedProp, 0f, 2f, "Run Speed", space );

            GUILayout.Space( 5f );

            guiContent.text = "Strafe Left";
            EditorGUILayout.PropertyField( strafeLeftProp, guiContent );
            guiContent.text = "Strafe Right";
            EditorGUILayout.PropertyField( strafeRightProp, guiContent );
            EditorHelper.ShowSubSlider( ref strafeSpeedProp, 0f, 2f, "Strafe Speed", space );

            GUILayout.Space( 5f );

            guiContent.text = "Crouch";
            EditorGUILayout.PropertyField( crouchProp, guiContent );

            GUILayout.Space( 5f );

            guiContent.text = "Crouch Walk";
            EditorGUILayout.PropertyField( crouchWalkProp, guiContent );
            EditorHelper.ShowSubSlider( ref crouchWalkSpeedProp, 0f, 2f, "Forward Speed", space );
            EditorHelper.ShowSubSlider( ref crouchBackwardsSpeedProp, 0f, 2f, "Backward Speed", space );

            GUILayout.Space( 5f );

            guiContent.text = "Crouch Strafe Left";
            EditorGUILayout.PropertyField( crouchStrafeLeftProp, guiContent );
            guiContent.text = "Crouch Strafe Right";
            EditorGUILayout.PropertyField( crouchStrafeRightProp, guiContent );
            EditorHelper.ShowSubSlider( ref crouchStrafeSpeedProp, 0f, 2f, "Crouch Strafe Speed", space );

            GUILayout.Space( 5f );

            guiContent.text = "Falling";
            EditorGUILayout.PropertyField( fallingProp, guiContent );

            GUILayout.Space( 5f );

            guiContent.text = "Half-Strafe Angle";
            EditorGUILayout.Slider( halfStrafeAngleProp, 15f, 45f, guiContent );
            EditorHelper.ShowSubSlider( ref hSAngleSmoothProp, .1f, 15f, "HS-Angle Smooth", space );
        }
    }
}