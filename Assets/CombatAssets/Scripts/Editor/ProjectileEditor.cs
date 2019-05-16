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
    [CustomEditor( typeof( Projectile ) )]
    [CanEditMultipleObjects]
    public class ProjectileEditor : Editor
    {
        private SerializedProperty
            typeProp, subTypeProp,
            damageProp, speedProp, lifetimeProp,
            decalObjectProp, shellObjectProp, explosionObjectProp,
            soundProp, impactAfterHitProp, noiseProp, resetAnglesProp;        


        // OnEnable
        void OnEnable()
        {
            typeProp = serializedObject.FindProperty( "type" );
            subTypeProp = serializedObject.FindProperty( "subType" );

            damageProp = serializedObject.FindProperty( "damage" );
            speedProp = serializedObject.FindProperty( "speed" );
            lifetimeProp = serializedObject.FindProperty( "lifetime" );

            explosionObjectProp = serializedObject.FindProperty( "explosionObject" );
            decalObjectProp = serializedObject.FindProperty( "decalObject" );
            shellObjectProp = serializedObject.FindProperty( "shellObject" );

            soundProp = serializedObject.FindProperty( "sound" );
            impactAfterHitProp = serializedObject.FindProperty( "impactAfterHit" );
            noiseProp = serializedObject.FindProperty( "noise" );
            resetAnglesProp = serializedObject.FindProperty( "resetAngles" );            
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
            bool isExplosionObject = ( explosionObjectProp.objectReferenceValue != null );


            //int typeIndex = typeProp.enumValueIndex;      // Simple = 0, Ballistic = 1
            int subTypeIndex = subTypeProp.enumValueIndex;  // Bullet = 0, Arrow = 1, Throw = 2, Rocket = 3

            GUILayout.Space( 5f );
            if( subTypeIndex == 0 || subTypeIndex == 1 )
                EditorGUILayout.PropertyField( typeProp );
            EditorGUILayout.PropertyField( subTypeProp );

            GUILayout.Space( 5f );

            if( isExplosionObject == false )
                EditorGUILayout.PropertyField( damageProp );

            EditorGUILayout.PropertyField( speedProp );
            EditorGUILayout.PropertyField( lifetimeProp );
            GUILayout.Space( 5f );

            EditorGUILayout.PropertyField( explosionObjectProp );
            GUILayout.Space( 5f );

            if( ( subTypeIndex == 0 || subTypeIndex == 1 ) && isExplosionObject == false )
                EditorGUILayout.PropertyField( decalObjectProp );

            if( subTypeIndex == 0 )
                EditorGUILayout.PropertyField( shellObjectProp );

            if( subTypeIndex == 0 )
                GUILayout.Space( 5f );

            if( subTypeIndex == 2 || subTypeIndex == 3 )
                EditorHelper.ShowSFXPropertyAndPlayButton( serializedObject, soundProp, ( subTypeIndex == 2 ? "Pin SFX" : "Fly SFX" ) );      

            if( subTypeIndex == 2 )
                EditorGUILayout.PropertyField( impactAfterHitProp, EditorHelper.PropertyLabel( "Explode After Hit" ) );

            if( subTypeIndex == 3 )
            {
                EditorGUILayout.PropertyField( noiseProp );
                EditorGUILayout.PropertyField( resetAnglesProp );
            }
        }
        //
    }
}