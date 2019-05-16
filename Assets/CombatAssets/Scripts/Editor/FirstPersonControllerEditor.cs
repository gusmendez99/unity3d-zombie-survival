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
    [CustomEditor( typeof( FirstPersonController ) )]
    //[CanEditMultipleObjects]
    public class FirstPersonControllerEditor : Editor
    {
        private SerializedProperty
            canWalkProp, walkSpeedProp, backwardsSpeedProp, sidewaysSpeedProp, inAirSpeedProp,
            canRunProp, runSpeedProp,
            canCrouchProp, crouchSpeedProp, crouchHeightProp,
            canJumpProp, jumpForceProp,
            canClimbProp, climbingSpeedProp,
            useHeadBobProp, posForceProp, tiltForceProp,
            gravityMultiplierProp, fallingDistanceToDamageProp, fallingDamageMultiplierProp,
            stepIntervalProp,
            lookSmoothProp, maxLookAngleYProp, cameraOffsetProp;   

        
        private GUIContent guiContent = new GUIContent( string.Empty );


        // OnEnable
        void OnEnable()
        {
            canWalkProp = serializedObject.FindProperty( "canWalk" );
            walkSpeedProp = serializedObject.FindProperty( "walkSpeed" );
            backwardsSpeedProp = serializedObject.FindProperty( "backwardsSpeed" );
            sidewaysSpeedProp = serializedObject.FindProperty( "sidewaysSpeed" );
            inAirSpeedProp = serializedObject.FindProperty( "inAirSpeed" );

            canRunProp = serializedObject.FindProperty( "canRun" );
            runSpeedProp = serializedObject.FindProperty( "runSpeed" );

            canCrouchProp = serializedObject.FindProperty( "canCrouch" );
            crouchSpeedProp = serializedObject.FindProperty( "crouchSpeed" );
            crouchHeightProp = serializedObject.FindProperty( "crouchHeight" );

            canJumpProp = serializedObject.FindProperty( "canJump" );
            jumpForceProp = serializedObject.FindProperty( "jumpForce" );

            canClimbProp = serializedObject.FindProperty( "canClimb" );
            climbingSpeedProp = serializedObject.FindProperty( "climbingSpeed" );

            useHeadBobProp = serializedObject.FindProperty( "useHeadBob" );
            posForceProp = serializedObject.FindProperty( "posForce" );
            tiltForceProp = serializedObject.FindProperty( "tiltForce" );

            gravityMultiplierProp = serializedObject.FindProperty( "gravityMultiplier" );
            fallingDistanceToDamageProp = serializedObject.FindProperty( "fallingDistanceToDamage" );
            fallingDamageMultiplierProp = serializedObject.FindProperty( "fallingDamageMultiplier" );

            stepIntervalProp = serializedObject.FindProperty( "stepInterval" );

            lookSmoothProp = serializedObject.FindProperty( "lookSmooth" );
            maxLookAngleYProp = serializedObject.FindProperty( "maxLookAngleY" );
            cameraOffsetProp = serializedObject.FindProperty( "cameraOffset" );
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

            EditorHelper.ShowBoolField( ref canWalkProp, "Can Walk" );
            EditorHelper.ShowSubSlider( ref walkSpeedProp, 1f, 7f, "Normal Speed", space );
            EditorHelper.ShowSubSlider( ref backwardsSpeedProp, 0f, 1f, "Backward Speed", space );
            EditorHelper.ShowSubSlider( ref sidewaysSpeedProp, 0f, 1f, "Sideways Speed", space );
            EditorHelper.ShowSubSlider( ref inAirSpeedProp, 0f, 1f, "InAir Speed", space );
            GUI.enabled = true;

            EditorHelper.ShowBoolField( ref canRunProp, "Can Run" );
            EditorHelper.ShowSubSlider( ref runSpeedProp, 5f, 15f, "Move Speed", space );
            GUI.enabled = true;

            EditorHelper.ShowBoolField( ref canCrouchProp, "Can Crouch" );
            EditorHelper.ShowSubSlider( ref crouchSpeedProp, 0f, 1f, "Move Speed", space );
            EditorHelper.ShowSubSlider( ref crouchHeightProp, 1f, 1.75f, "Capsule Height", space );
            GUI.enabled = true;

            EditorHelper.ShowBoolField( ref canJumpProp, "Can Jump" );
            EditorHelper.ShowSubSlider( ref jumpForceProp, 1f, 10f, "Force", space );
            GUI.enabled = true;

            EditorHelper.ShowBoolField( ref canClimbProp, "Can Climb" );
            EditorHelper.ShowSubSlider( ref climbingSpeedProp, 0f, 1f, "Move Speed", space );
            GUI.enabled = true;

            GUILayout.Space( 5f );
            EditorHelper.ShowBoolField( ref useHeadBobProp, "Use Head Bob" );
            EditorHelper.ShowSubSlider( ref posForceProp, 0f, 1f, "Pos Force", space );
            EditorHelper.ShowSubSlider( ref tiltForceProp, 0f, 1f, "Tilt Force", space );
            GUI.enabled = true;

            GUILayout.Space( 5f );
            guiContent.text = "Gravity Multiplier";
            EditorGUILayout.Slider( gravityMultiplierProp, 1f, 5f, guiContent );
            guiContent.text = "Falling Distance toDamage";
            EditorGUILayout.Slider( fallingDistanceToDamageProp, 1f, 5f, guiContent );
            guiContent.text = "Falling Damage Multiplier";
            EditorGUILayout.Slider( fallingDamageMultiplierProp, 1f, 10f, guiContent );            

            GUILayout.Space( 5f );
            guiContent.text = "Step Interval";
            EditorGUILayout.Slider( stepIntervalProp, .1f, 1.5f, guiContent );

            GUILayout.Space( 5f );
            guiContent.text = "Look Smooth";
            EditorGUILayout.Slider( lookSmoothProp, .01f, 1f, guiContent );
            guiContent.text = "Max LookAngle Y";
            EditorGUILayout.Slider( maxLookAngleYProp, 25f, 90f, guiContent );
            guiContent.text = "Camera Offset";
            EditorGUILayout.PropertyField( cameraOffsetProp, guiContent );
        }
    }
}