/********************************************
 * Copyright(c): 2016 Victor Klepikov       *
 *                                          *
 * Site: 	     http://u3d.as/5Fb		    *
 * Support: 	 http://bit.ly/vk-Support   *
 ********************************************/


using UnityEngine;
using UnityEditor;
using UnityEditorInternal;


namespace AdvancedShooterKit.Inspector
{    
    [CanEditMultipleObjects]
    [CustomEditor( typeof( Firearms ) )]
    public class FirearmsEditor : Editor
    {
        private SerializedProperty
            infiniteAmmoProp,
            ammoIndexProp, maxAmmoProp, currentAmmoProp, projectileObjectProp,
            addDamageProp, addSpeedProp, rateOfFireProp, dispersionProp,
            shotDelayProp, shotSFXProp, emptySFXProp, reloadSFXProp,
            shellOuterProp, shelloutForceProp, projectileOuterProp,
            projectilesPerShotProp, hitMaskProp,
            minCameraShakeProp, maxCameraShakeProp, listElement;

        private ReorderableList firingModesList, projectilesList;
        private bool isPlayerWeapon = false;


        // OnEnable
        void OnEnable()
        {
            infiniteAmmoProp = serializedObject.FindProperty( "infiniteAmmo" );
            maxAmmoProp = serializedObject.FindProperty( "maxAmmo" );

            addDamageProp = serializedObject.FindProperty( "addDamage" );
            addSpeedProp = serializedObject.FindProperty( "addSpeed" );

            rateOfFireProp = serializedObject.FindProperty( "rateOfFire" );
            dispersionProp = serializedObject.FindProperty( "dispersion" );

            shotSFXProp = serializedObject.FindProperty( "shotSFX" );
            emptySFXProp = serializedObject.FindProperty( "emptySFX" );
            reloadSFXProp = serializedObject.FindProperty( "reloadSFX" );

            shotDelayProp = serializedObject.FindProperty( "shotDelay" );
            projectileOuterProp = serializedObject.FindProperty( "projectileOuter" );

            shellOuterProp = serializedObject.FindProperty( "shellOuter" );
            shelloutForceProp = serializedObject.FindProperty( "shelloutForce" );

            projectilesPerShotProp = serializedObject.FindProperty( "projectilesPerShot" );
            hitMaskProp = serializedObject.FindProperty( "hitMask" );

            //
            firingModesList = new ReorderableList( serializedObject, serializedObject.FindProperty( "firingModes" ), true, true, true, true );
            projectilesList = new ReorderableList( serializedObject, serializedObject.FindProperty( "projectiles" ), true, true, true, true );
            //

            minCameraShakeProp = serializedObject.FindProperty( "minCameraShake" );
            maxCameraShakeProp = serializedObject.FindProperty( "maxCameraShake" );

            isPlayerWeapon = ( ( target as Component ).GetComponentInParent<PlayerCharacter>() != null );
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
            GUILayout.Space( 5f );
            EditorGUILayout.PropertyField( hitMaskProp );

            if( isPlayerWeapon )
            {
                GUILayout.Space( 5f );
                EditorGUILayout.PropertyField( infiniteAmmoProp );

                GUI.enabled = !infiniteAmmoProp.boolValue;
                GUILayout.Space( 5f );
                EditorGUILayout.PropertyField( maxAmmoProp );
                GUI.enabled = true;
            }

            GUILayout.Space( 5f );
            EditorGUILayout.PropertyField( addDamageProp );
            EditorGUILayout.PropertyField( addSpeedProp );            

            EditorGUILayout.Slider( rateOfFireProp, 1f, 2500f );
            EditorGUILayout.PropertyField( dispersionProp );
                        
            GUILayout.Space( 5f );
            EditorHelper.ShowSFXPropertyAndPlayButton( serializedObject, shotSFXProp );

            if( isPlayerWeapon )
            {
                GUI.enabled = !infiniteAmmoProp.boolValue;
                EditorHelper.ShowSFXPropertyAndPlayButton( serializedObject, emptySFXProp );
                EditorHelper.ShowSFXPropertyAndPlayButton( serializedObject, reloadSFXProp );
                GUI.enabled = true;
            }
            
            GUILayout.Space( 5f );
            EditorGUILayout.PropertyField( projectileOuterProp );

            GUI.enabled = ( projectileOuterProp.objectReferenceValue != null );
            EditorHelper.ShowSubSlider( ref shotDelayProp, 0f, 2f, "Spawn Delay", 20f );
            EditorHelper.ShowIntSubSlider( ref projectilesPerShotProp, 1, 12, "Projectiles Per Shot", 20f );
            GUI.enabled = true;            

            GUILayout.Space( 5f );
            EditorGUILayout.PropertyField( shellOuterProp );

            GUI.enabled = ( shellOuterProp.objectReferenceValue != null );
            EditorHelper.ShowSubSlider( ref shelloutForceProp, 10f, 50f, "Shellout Force", 20f );
            GUI.enabled = true;

            if( isPlayerWeapon )
            {
                // camera shake
                GUILayout.Space( 5f );
                EditorHelper.ShowMinMaxSlider( minCameraShakeProp, maxCameraShakeProp, .01f, 2f, "Camera Shake Range" );

                // firing modes
                GUILayout.Space( 5f );
                EditorHelper.ShowSimpleReorderableList( firingModesList, "Firing Modes" );
            }

            GUILayout.Space( 5f );
            projectilesList.drawHeaderCallback = ( Rect rect ) =>
            {
                EditorGUI.LabelField( rect, "Used Projectiles" );
            };

            if( projectilesList.count > 0 )
                projectilesList.elementHeight = EditorGUIUtility.singleLineHeight * ( isPlayerWeapon ? 3f : 1.75f );
            else
                projectilesList.elementHeight = EditorGUIUtility.singleLineHeight;

            projectilesList.drawElementCallback = ( Rect rect, int index, bool isActive, bool isFocused ) =>
            {
                listElement = projectilesList.serializedProperty.GetArrayElementAtIndex( index );
                ammoIndexProp = listElement.FindPropertyRelative( "ammoIndex" );
                currentAmmoProp = listElement.FindPropertyRelative( "currentAmmo" );
                projectileObjectProp = listElement.FindPropertyRelative( "projectileObject" );

                const float space = 5f;
                float width = EditorGUIUtility.currentViewWidth;
                float height = EditorGUIUtility.singleLineHeight;
                float startX = rect.x;

                rect.y += 2f;

                rect.x = startX / 2f;
                rect.width = width / 1.13f;
                rect.height = height * ( isPlayerWeapon ? 2.8f : 1.5f );
                EditorGUI.HelpBox( rect, string.Empty, MessageType.None );

                if( isPlayerWeapon )
                {
                    // Index

                    GUI.enabled = !infiniteAmmoProp.boolValue;

                    rect.x = startX;
                    rect.y += 4f;
                    rect.height = height;

                    rect.width = width / 4.1f;
                    EditorGUI.LabelField( rect, "Ammo Index" ); //

                    rect.x += rect.width + space;
                    rect.width = width / 10f;
                    EditorGUI.PropertyField( rect, ammoIndexProp, GUIContent.none );
                    ammoIndexProp.intValue = Mathf.Clamp( ammoIndexProp.intValue, 0, AmmoBackpack.size - 1 );

                    // current Ammo

                    rect.x += rect.width + space * 6f;
                    rect.width = width / 3.75f;
                    EditorGUI.LabelField( rect, "Current Ammo" );

                    rect.x += rect.width + space;
                    rect.width = width / 10f;
                    EditorGUI.PropertyField( rect, currentAmmoProp, GUIContent.none );
                    currentAmmoProp.intValue = Mathf.Clamp( currentAmmoProp.intValue, 0, maxAmmoProp.intValue );

                    GUI.enabled = true;

                    rect.x = startX;
                    rect.y += height + space;
                }
                else
                {
                    rect.x = startX;
                    rect.y += 4f;
                    rect.width = width;
                    rect.height = height;
                }

                // Projectile                
                rect.width = width / 3.5f;
                EditorGUI.LabelField( rect, "Projectile Object" );

                rect.x += rect.width + space;
                rect.width = width / 2f;
                EditorGUI.PropertyField( rect, projectileObjectProp, GUIContent.none );
            };

            projectilesList.DoLayoutList();

        }
    };
}