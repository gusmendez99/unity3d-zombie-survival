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
    [CustomEditor( typeof( Pickup ) ), CanEditMultipleObjects]
    public class PickupEditor : Editor
    {
        private SerializedProperty
            dropChanceProp,
            pickupTypeProp, pickupDistanceProp, pickupSoundProp,
            amountProp,ammoIndexProp, weaponIndexProp,
            PickupedProp;


        // OnEnable
        void OnEnable()
        {
            dropChanceProp = serializedObject.FindProperty( "dropChance" );
            pickupTypeProp = serializedObject.FindProperty( "pickupType" );
            pickupDistanceProp = serializedObject.FindProperty( "pickupDistance" );
            pickupSoundProp = serializedObject.FindProperty( "pickupSound" );
            amountProp = serializedObject.FindProperty( "amount" );
            ammoIndexProp = serializedObject.FindProperty( "ammoIndex" );
            weaponIndexProp = serializedObject.FindProperty( "weaponIndex" );
            PickupedProp = serializedObject.FindProperty( "Pickuped" );
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
            EditorGUILayout.PropertyField( dropChanceProp );

            GUILayout.Space( 5f );
            EditorGUILayout.PropertyField( pickupTypeProp );
            EditorGUILayout.PropertyField( pickupDistanceProp );
            EditorHelper.ShowSFXPropertyAndPlayButton( serializedObject, pickupSoundProp, "Pickup SFX" );

            GUILayout.Space( 5f );

            int maxAmmoIndex = AmmoBackpack.size - 1;
            int maxWeaponIndex = WeaponsManager.size - 1;

            //Health = 0, Melee = 1, Firearms = 2, Ammo = 3, Thrown = 4
            switch( pickupTypeProp.enumValueIndex )
            {
                case 0:
                    EditorGUILayout.IntSlider( amountProp, 1, 100, EditorHelper.PropertyLabel( "Health Amount" ) );
                    break;

                case 1:
                    EditorGUILayout.IntSlider( weaponIndexProp, 0, maxWeaponIndex );
                    break;

                case 2:
                    EditorGUILayout.IntSlider( amountProp, 1, 100, EditorHelper.PropertyLabel( "Ammo Amount" ) );
                    EditorGUILayout.IntSlider( ammoIndexProp, 0, maxAmmoIndex );
                    EditorGUILayout.IntSlider( weaponIndexProp, 0, maxWeaponIndex );
                    break;

                case 3:
                    EditorGUILayout.IntSlider( amountProp, 1, 100, EditorHelper.PropertyLabel( "Ammo Amount" ) );
                    EditorGUILayout.IntSlider( ammoIndexProp, 0, maxAmmoIndex );
                    break;

                case 4:
                    EditorGUILayout.IntSlider( amountProp, 1, 10, EditorHelper.PropertyLabel( "Ammo Amount" ) );
                    EditorGUILayout.IntSlider( ammoIndexProp, 0, maxAmmoIndex );
                    EditorGUILayout.IntSlider( weaponIndexProp, 0, maxWeaponIndex );
                    break;
            }

            GUILayout.Space( 5f );
            EditorGUILayout.PropertyField( PickupedProp, false, null );
        }
        //
    }
}