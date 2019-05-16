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
    [CustomEditor( typeof( HitEffect ) )]
    [CanEditMultipleObjects]
    public class HitEffectEditor : Editor
    {
        private SerializedProperty
            lifetimeProp, framesXProp, framesYProp,
            surfacesArray, surfacesElement, indexProp,
            hitTextureProp, hitSoundProp, hitParticleProp,
            mainFoProp, surfacesFoProp;


        //
        private bool surfacesSFo = false;
        private string[] surfaces = new string[ 0 ];
        private int selection = 0;
        private readonly string[] stateNames = { "Generic", "Special" };


        // OnEnable
        void OnEnable()
        {
            lifetimeProp = serializedObject.FindProperty( "lifetime" );
            framesXProp = serializedObject.FindProperty( "framesX" );
            framesYProp = serializedObject.FindProperty( "framesY" );
            //
            surfacesArray = serializedObject.FindProperty( "surfaces" );
            surfaces = SurfaceDetector.GetNames;
            //
            mainFoProp = serializedObject.FindProperty( "mainFo" );
            surfacesFoProp = serializedObject.FindProperty( "surfacesFo" );
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
            CallNamedMethod( ref mainFoProp, "    Main", "ShowMainParams" );
            CallNamedMethod( ref surfacesFoProp, "    Surfaces", "ShowSurfaces" );
        }


        // Call NamedMethod ( Refletions no-no )
        private void CallNamedMethod( ref SerializedProperty spFoldout, string dataName, string methodName )
        {
            GUILayout.BeginVertical( "Box", GUILayout.ExpandWidth( true ) );

            GUILayout.BeginHorizontal();
            GUILayout.Space( 15f );
            bool foldout = spFoldout.boolValue;
            foldout = EditorGUILayout.Foldout( foldout, dataName, StyleHelper.FoldOutStyle );
            spFoldout.boolValue = foldout;
            GUILayout.EndHorizontal();


            if( foldout )
            {
                GUILayout.Space( 5f );
                switch( methodName )
                {
                    case "ShowMainParams": ShowMainParams(); break;
                    case "ShowSurfaces": ShowSurfaces(); break;
                }
                GUILayout.Space( 5f );
            }
            else
            {
                GUILayout.Space( 2f );
            }

            GUILayout.EndVertical();
        }


        // Show MainParams
        private void ShowMainParams()
        {
            EditorGUILayout.Slider( lifetimeProp, 1f, 120f, EditorHelper.PropertyLabel( "Lifetime" ) );
            //
            EditorGUILayout.IntSlider( framesXProp, 1, 10, EditorHelper.PropertyLabel( "Frames X" ) );
            EditorGUILayout.IntSlider( framesYProp, 1, 10, EditorHelper.PropertyLabel( "Frames Y" ) );
        }

        // Show Surfaces
        private void ShowSurfaces()
        {
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
                    hitTextureProp = surfacesElement.FindPropertyRelative( "hitTexture" );
                    hitSoundProp = surfacesElement.FindPropertyRelative( "hitSound" );
                    hitParticleProp = surfacesElement.FindPropertyRelative( "hitParticle" );

                    GUILayout.Space( 5f );

                    int surfaceIndex = indexProp.intValue;
                    surfaceIndex = ( surfaceIndex < surfacesLength ) ? surfaceIndex : surfacesLength - 1;
                    surfaceIndex = EditorGUILayout.Popup( "Surface Type", surfaceIndex, surfaces );
                    surfacesElement.FindPropertyRelative( "name" ).stringValue = surfaces[ surfaceIndex ];
                    indexProp.intValue = surfaceIndex;
                    GUILayout.Space( 10f );
                    EditorGUILayout.PropertyField( hitTextureProp, EditorHelper.PropertyLabel( "Hit Texture" ) );
                    EditorHelper.ShowSFXPropertyAndPlayButton( serializedObject, hitSoundProp, "Hit SFX" );
                    EditorGUILayout.PropertyField( hitParticleProp, EditorHelper.PropertyLabel( "Hit Particle" ) );
                }

                // Actions
                if( add )
                {
                    surfacesArray.InsertArrayElementAtIndex( surfacesSize );
                    surfacesElement = surfacesArray.GetArrayElementAtIndex( surfacesSize );

                    surfacesSize = surfacesArray.arraySize;
                    selection = ( surfacesSize > 1 ) ? surfacesSize - 1 : 0;

                    surfacesElement.FindPropertyRelative( "index" ).intValue = surfacesSize - 1;
                    surfacesElement.FindPropertyRelative( "hitTexture" ).objectReferenceValue = null;
                    surfacesElement.FindPropertyRelative( "hitSound" ).objectReferenceValue = null;
                    surfacesElement.FindPropertyRelative( "hitParticle" ).objectReferenceValue = null;
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
                hitTextureProp = surfacesElement.FindPropertyRelative( "hitTexture" );
                hitSoundProp = surfacesElement.FindPropertyRelative( "hitSound" );
                hitParticleProp = surfacesElement.FindPropertyRelative( "hitParticle" );
                //            
                EditorGUILayout.PropertyField( hitTextureProp, EditorHelper.PropertyLabel( "Hit Texture" ) );
                EditorHelper.ShowSFXPropertyAndPlayButton( serializedObject, hitSoundProp, "Hit SFX" );
                EditorGUILayout.PropertyField( hitParticleProp, EditorHelper.PropertyLabel( "Hit Particle" ) );
            }
        }
        //
    }
}