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
    [CustomEditor( typeof( FootstepSFXManager ) )]
    public class FootstepSFXManagerEditor : Editor
    {
        private SerializedProperty
            surfacesArray, surfacesElement, indexProp,
            jumpingSFXProp, landingSFXProp, footstepSoundsArray;

        private ReorderableList fsGenericList;
        private ReorderableList[] footstepSoundsList = new ReorderableList[ 0 ];

        //
        private bool surfacesSFo = false;
        private string[] surfaces = new string[ 0 ];
        private int selection = 0;
        private readonly string[] stateNames = { "Generic", "Special" };


        // OnEnable
        void OnEnable()
        {
            footstepSoundsArray = serializedObject.FindProperty( "generic" ).FindPropertyRelative( "footstepSounds" );
            fsGenericList = new ReorderableList( serializedObject, footstepSoundsArray, true, true, true, true );

            surfacesArray = serializedObject.FindProperty( "surfaces" );
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
            GUILayout.BeginVertical( "Box", GUILayout.ExpandWidth( true ) );
            GUILayout.Space( 5f );

            // Surface index & Minimum decrement to show pain screen
            int surfacesLength = surfaces.Length;

            if( surfacesLength != SurfaceDetector.GetCount )
                surfaces = SurfaceDetector.GetNames;

            surfacesSFo = System.Convert.ToBoolean( GUILayout.Toolbar( System.Convert.ToInt32( surfacesSFo ), stateNames, GUILayout.Height( 20f ) ) );

            GUILayout.Space( 5f );

            if( surfacesSFo )
            {
                int surfacesSize = surfacesArray.arraySize;

                EditorGUILayout.BeginVertical( "box" );
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.BeginVertical( GUILayout.Width( 30f ) );
                GUILayout.Space( 4f );
                GUI.enabled = ASKSettingsWindow.NotBegin( selection );
                bool moveUp = GUILayout.Button( "▲", GUILayout.Height( 16f ) );
                GUI.enabled = true;
                GUI.enabled = ASKSettingsWindow.NotEnd( selection, surfacesSize );
                bool moveDown = GUILayout.Button( "▼", GUILayout.Height( 16f ) );
                GUI.enabled = true;
                EditorGUILayout.EndVertical();
                //
                GUI.enabled = surfacesSize < surfacesLength;
                bool add = GUILayout.Button( "Add Surface", GUILayout.Height( 35f ) );
                GUI.enabled = ( surfacesSize > 0 );
                bool delete = GUILayout.Button( "Remove it", GUILayout.Height( 35f ) );
                GUI.enabled = true;
                EditorGUILayout.EndHorizontal();

                if( surfacesSize > 0 )
                {
                    GUILayout.Space( 5f );
                    selection = GUILayout.SelectionGrid( selection, ASKSettingsWindow.GetNames( surfacesArray, surfacesSize ), 1 );
                }
                GUILayout.Space( 5f );
                EditorGUILayout.EndVertical();

                if( surfacesSize > 0 )
                {
                    surfacesElement = surfacesArray.GetArrayElementAtIndex( selection );
                    //
                    indexProp = surfacesElement.FindPropertyRelative( "index" );
                    jumpingSFXProp = surfacesElement.FindPropertyRelative( "jumpingSFX" );
                    landingSFXProp = surfacesElement.FindPropertyRelative( "landingSFX" );
                    footstepSoundsArray = surfacesElement.FindPropertyRelative( "footstepSounds" );

                    if( surfacesSize != footstepSoundsList.Length )
                        footstepSoundsList = new ReorderableList[ surfacesSize ];

                    if( footstepSoundsList[ selection ] == null )
                        footstepSoundsList[ selection ] = new ReorderableList( serializedObject, footstepSoundsArray, true, true, true, true );

                    GUILayout.Space( 5f );
                    int surfaceIndex = indexProp.intValue;
                    surfaceIndex = ( surfaceIndex < surfacesLength ) ? surfaceIndex : surfacesLength - 1;
                    surfaceIndex = EditorGUILayout.Popup( "Surface Type", surfaceIndex, surfaces );
                    surfacesElement.FindPropertyRelative( "name" ).stringValue = surfaces[ surfaceIndex ];
                    indexProp.intValue = surfaceIndex;
                    GUILayout.Space( 10f );

                    EditorHelper.ShowSFXPropertyAndPlayButton( serializedObject, jumpingSFXProp );
                    EditorHelper.ShowSFXPropertyAndPlayButton( serializedObject, landingSFXProp );
                    EditorHelper.ShowSFXListAndPlayButton( serializedObject, footstepSoundsList[ selection ], "Footstep Sounds" );
                }

                // Actions
                if( add )
                {
                    surfacesArray.InsertArrayElementAtIndex( surfacesSize );
                    surfacesElement = surfacesArray.GetArrayElementAtIndex( surfacesSize );

                    surfacesSize = surfacesArray.arraySize;
                    selection = ( surfacesSize > 1 ) ? surfacesSize - 1 : 0;

                    surfacesElement.FindPropertyRelative( "index" ).intValue = surfacesSize - 1;
                    surfacesElement.FindPropertyRelative( "jumpingSFX" ).objectReferenceValue = null;
                    surfacesElement.FindPropertyRelative( "landingSFX" ).objectReferenceValue = null;
                    surfacesElement.FindPropertyRelative( "footstepSounds" ).ClearArray();
                }

                if( moveUp )
                {
                    surfacesArray.MoveArrayElement( selection - 1, selection-- );
                    return;
                }
                if( moveDown )
                {
                    surfacesArray.MoveArrayElement( selection + 1, selection++ );
                    return;
                }
                if( delete )
                {
                    surfacesArray.DeleteArrayElementAtIndex( selection );
                    surfacesSize = surfacesArray.arraySize;
                    selection = ASKSettingsWindow.NotEnd( selection, surfacesSize ) ? selection : surfacesSize - 1;
                    return;
                }
            }
            else
            {
                surfacesElement = serializedObject.FindProperty( "generic" );
                jumpingSFXProp = surfacesElement.FindPropertyRelative( "jumpingSFX" );
                landingSFXProp = surfacesElement.FindPropertyRelative( "landingSFX" );
                footstepSoundsArray = surfacesElement.FindPropertyRelative( "footstepSounds" );
                //            
                EditorHelper.ShowSFXPropertyAndPlayButton( serializedObject, jumpingSFXProp );
                EditorHelper.ShowSFXPropertyAndPlayButton( serializedObject, landingSFXProp );
                EditorHelper.ShowSFXListAndPlayButton( serializedObject, fsGenericList, "Footstep Sounds" );
            }

            GUILayout.Space( 5f );
            GUILayout.EndVertical();
        }
    }
}