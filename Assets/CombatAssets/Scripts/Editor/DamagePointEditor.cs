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
    [CustomEditor( typeof( DamagePoint ) )]
    [CanEditMultipleObjects]
    public class DamagePointEditor : Editor
    {
        private SerializedProperty
            damageModifierProp,
            armorTypeProp, surfaceIndexProp;

        private string[] surfaces = new string[ 0 ];


        // OnEnable
        void OnEnable()
        {
            armorTypeProp = serializedObject.FindProperty( "armorType" );
            surfaceIndexProp = serializedObject.FindProperty( "surfaceIndex" );
            damageModifierProp = serializedObject.FindProperty( "damageModifier" );
            //
            surfaces = SurfaceDetector.GetNames;
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
            if( surfaces.Length != SurfaceDetector.GetCount )
                surfaces = SurfaceDetector.GetNames;

            GUILayout.Space( 5f );
            EditorGUILayout.PropertyField( damageModifierProp );

            GUILayout.Space( 5f );
            EditorGUILayout.PropertyField( armorTypeProp );

            int surfaceIndex = surfaceIndexProp.intValue;
            surfaceIndex = EditorGUILayout.Popup( "Hit Surface", surfaceIndex, surfaces );
            surfaceIndexProp.intValue = surfaceIndex;
        }
    };
}