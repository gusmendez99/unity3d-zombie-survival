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
    [CustomEditor( typeof( AmmoBackpack ) )]
    public class AmmoBackpackEditor : Editor
    {
        private ReorderableList ammoList = null;


        // OnEnable
        void OnEnable()
        {
            ammoList = new ReorderableList( serializedObject, serializedObject.FindProperty( "ammunition" ), true, true, true, true );
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

            ammoList.drawHeaderCallback = ( Rect rect ) =>
            {
                EditorGUI.LabelField( rect, "Player Ammunition" );
            };

            float height = EditorGUIUtility.singleLineHeight;
            if( ammoList.count > 0 )
                ammoList.elementHeight = height * 3f;
            else
                ammoList.elementHeight = height;


            ammoList.drawElementCallback = ( Rect rect, int index, bool isActive, bool isFocused ) =>
            {
                SerializedProperty listElement = ammoList.serializedProperty.GetArrayElementAtIndex( index );
                SerializedProperty currentAmmoProp = listElement.FindPropertyRelative( "currentAmmo" );
                SerializedProperty maxAmmoProp = listElement.FindPropertyRelative( "maxAmmo" );
                SerializedProperty hudIconProp = listElement.FindPropertyRelative( "hudIcon" );

                const float space = 5f;
                float width = EditorGUIUtility.currentViewWidth;                
                float startX = rect.x;

                rect.y += 2f;

                rect.x = startX / 2f;
                rect.width = width / 1.13f;
                rect.height = height * 2.8f;
                EditorGUI.HelpBox( rect, string.Empty, MessageType.None );

                // current + max

                rect.x = startX;
                rect.y += 4f;
                rect.height = height;

                rect.width = width / 3.75f;
                EditorGUI.LabelField( rect, "Current Ammo" );

                rect.x += rect.width + space;
                rect.width = width / 7f;
                EditorGUI.PropertyField( rect, currentAmmoProp, GUIContent.none );

                rect.x += rect.width + space * 2f;
                rect.width = width / 5f;
                EditorGUI.LabelField( rect, "Max Ammo" );

                rect.x += rect.width + space;
                rect.width = width / 8f;
                EditorGUI.PropertyField( rect, maxAmmoProp, GUIContent.none );

                // hud icon

                rect.x = startX;
                rect.y += height + space;
                rect.width = width / 6f;
                EditorGUI.LabelField( rect, "Hud Icon" );

                rect.x += rect.width + space;
                rect.width = width / 2.45f;
                EditorGUI.PropertyField( rect, hudIconProp, GUIContent.none );

                rect.x += rect.width + space * 3f;
                rect.width = width / 5f;
                EditorGUI.LabelField( rect, "Index: " + index, EditorStyles.boldLabel );
            };

            ammoList.DoLayoutList();
        }
    }
}