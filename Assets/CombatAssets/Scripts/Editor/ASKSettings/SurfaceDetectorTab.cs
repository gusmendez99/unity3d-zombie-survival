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
    public static class SurfaceDetectorTab
    {
        private static string MAIN_DATABASE_PATH {get { return ASKSettingsWindow.mainDirectory + "/SurfaceDetector.asset"; } }
        private static string TMP_DATABASE_PATH { get { return ASKSettingsWindow.mainDirectory + "/tmp/SurfaceDetectorTMP.asset"; } }
        
        //
        private static SerializedObject tmpSerializedObject = null;
        private static SerializedProperty
            nameProp,
            surfacesArray, surfacesElement,
            materialsArray, materialsElement,
            texturesArray, texturesElement;

        private static int selection, currentTab;
        private static Vector2 leftScroll, rightScroll;

        private static readonly string[] tabs = { "Meshes Materials", "Terrain Textures" };

        // Load CurrentAssetFile
        private static SurfaceDetector LoadAssetFile( string path )
        {
            SurfaceDetector currentFile = AssetDatabase.LoadAssetAtPath( path, typeof( SurfaceDetector ) ) as SurfaceDetector;

            if( currentFile == null )
            {
                currentFile = ScriptableObject.CreateInstance<SurfaceDetector>();
                AssetDatabase.CreateAsset( currentFile, path );
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

            return currentFile;
        }

        // Save CopyAssetFile
        private static void SaveCopyAssetFile( string copyFrom, string copyTo )
        {
            if( copyFrom == MAIN_DATABASE_PATH )
                LoadAssetFile( copyFrom );            
            
            AssetDatabase.DeleteAsset( copyTo );
            AssetDatabase.CopyAsset( copyFrom, copyTo );
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        // Setup Tab
        internal static void SetupTab()
        {          
            if( tmpSerializedObject == null )            
                SaveCopyAssetFile( MAIN_DATABASE_PATH, TMP_DATABASE_PATH );

            tmpSerializedObject = new SerializedObject( LoadAssetFile( TMP_DATABASE_PATH ) );
            //
            surfacesArray = tmpSerializedObject.FindProperty( "surfaces" );          
        }

        // Reload Settings
        internal static void ReloadSettings()
        {
            SaveCopyAssetFile( MAIN_DATABASE_PATH, TMP_DATABASE_PATH );
            FullReset();
            SetupTab();
        }

        // Save Settings
        internal static void SaveSettings()
        {
            SaveCopyAssetFile( TMP_DATABASE_PATH, MAIN_DATABASE_PATH );
            
            /*
            SurfaceDetector mainFile = LoadAssetFile( MAIN_DATABASE_PATH );
            SurfaceDetector tmpFile = LoadAssetFile( TMP_DATABASE_PATH );

            mainFile.genericSurface = tmpFile.genericSurface;
            mainFile.surfaces = tmpFile.surfaces;
            mainFile.decal = tmpFile.decal;

            EditorUtility.SetDirty( mainFile );
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            */
        }


        // OnWindowGUI
        internal static void OnWindowGUI()
        {
            // BEGIN
            tmpSerializedObject.Update();
            // BEGIN

            ShowLeftSide();
            ShowRightSide();
            
            // END
            tmpSerializedObject.ApplyModifiedProperties();
            // END
        }

        // Show LeftSide
        private static void ShowLeftSide()
        {
            int surfacesSize = surfacesArray.arraySize;

            leftScroll = EditorGUILayout.BeginScrollView( leftScroll, "Box", GUILayout.Width( 200f ), GUILayout.ExpandHeight( true ) );

            GUILayout.Space( 5f );
            EditorGUILayout.BeginHorizontal( GUILayout.ExpandWidth( true ) );
            GUILayout.Space( 25f );
            bool add = GUILayout.Button( "Add Surface", GUILayout.Height( 35f ) );
            GUI.enabled = true;
            GUILayout.Space( 25f );
            EditorGUILayout.EndHorizontal();
            GUILayout.Space( 5f );

            EditorGUILayout.BeginVertical( "Box", GUILayout.ExpandWidth( true ), GUILayout.ExpandHeight( true ) );
            if( surfacesSize > 0 )
                selection = GUILayout.SelectionGrid( selection, ASKSettingsWindow.GetNames( surfacesArray, surfacesSize ), 1 );
            EditorGUILayout.EndVertical();

            EditorGUILayout.EndScrollView();

            EditorGUILayout.BeginVertical( GUILayout.Width( 25f ) );
            GUILayout.Space( 5f );
            GUI.enabled = ( surfacesSize > 0 );
            bool delete = GUILayout.Button( "X" );
            GUI.enabled = true;
            GUI.enabled = ASKSettingsWindow.NotBegin( selection );
            GUILayout.Space( 15f );
            bool moveUp = GUILayout.Button( "▲", GUILayout.Height( 30f ) );
            GUI.enabled = true;
            GUI.enabled = ASKSettingsWindow.NotEnd( selection, surfacesSize );
            bool moveDown = GUILayout.Button( "▼", GUILayout.Height( 30f ) );
            GUI.enabled = true;
            EditorGUILayout.EndVertical();

                                   
            if( add )
            {
                surfacesArray.InsertArrayElementAtIndex( surfacesSize );
                surfacesElement = surfacesArray.GetArrayElementAtIndex( surfacesSize );

                surfacesSize = surfacesArray.arraySize;
                selection = ( surfacesSize > 1 ) ? surfacesSize - 1 : 0;

                surfacesElement.FindPropertyRelative( "name" ).stringValue = "New Surface " + surfacesSize;
                surfacesElement.FindPropertyRelative( "materials" ).ClearArray();
                surfacesElement.FindPropertyRelative( "textures" ).ClearArray();
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

        // Show RightSide
        private static void ShowRightSide()
        {
            int surfacesSize = surfacesArray.arraySize;            

            GUILayout.BeginVertical( "Box", GUILayout.ExpandWidth( true ), GUILayout.ExpandHeight( true ) );

            if( surfacesSize > 0 )
            {
                surfacesElement = surfacesArray.GetArrayElementAtIndex( selection );
                nameProp = surfacesElement.FindPropertyRelative( "name" );

                const float space = 10f;

                GUILayout.Space( 5f );

                GUILayout.BeginHorizontal();
                EditorHelper.ShowFixedPropertyField( ref nameProp, "Surface Name", space, 90f );
                GUILayout.Label( "Index: " + selection, EditorStyles.label, GUILayout.Width( 65f ) );
                GUILayout.EndHorizontal();

                GUILayout.Space( 7f );
                currentTab = GUILayout.Toolbar( currentTab, tabs, GUILayout.ExpandWidth( true ), GUILayout.Height( 25f ) );
                GUILayout.Space( 15f );

                switch( currentTab )
                {
                    case 0: //Materials
                        ShowTab( ref materialsArray, ref materialsElement, space );
                        break;
                    case 1: //Terrain Textures
                        ShowTab( ref texturesArray, ref texturesElement, space );
                        break;
                }

            }
            GUILayout.EndVertical();
        }


        // Show Tab
        private static void ShowTab( ref SerializedProperty array, ref SerializedProperty element, float space )
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space( 100f );
            bool add = GUILayout.Button( "Add " + ( ( currentTab == 0 ) ? "Mesh Material" : "Terrain Texture" ), GUILayout.Height( 20f ) );
            GUILayout.Space( 100f );
            GUILayout.EndHorizontal();

            array = surfacesElement.FindPropertyRelative( currentTab == 0 ? "materials" : "textures" );
            int materialsSize = array.arraySize;

            if( add )
            {
                array.InsertArrayElementAtIndex( materialsSize );
                element = array.GetArrayElementAtIndex( materialsSize );
                element.objectReferenceValue = null;
                materialsSize = array.arraySize;
            }

            GUILayout.Space( 5f );
            rightScroll = EditorGUILayout.BeginScrollView( rightScroll, "Box", GUILayout.ExpandWidth( true ), GUILayout.ExpandHeight( true ) );
            GUILayout.Space( 5f );
            byte cells = 0;
            for( int i = 0; i < materialsSize; i++ )
            {
                element = array.GetArrayElementAtIndex( i );

                if( cells == 0 )
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Space( space );
                }

                GUILayout.BeginHorizontal( "Box", GUILayout.MaxWidth( 100f ) );
                GUILayout.BeginVertical();

                GUILayout.Label( AssetPreview.GetAssetPreview( element.objectReferenceValue ), GUILayout.Width( 95f ), GUILayout.Height( 95f ) );

                GUILayout.Space( -95f );

                EditorGUILayout.PropertyField( element, GUIContent.none, GUILayout.Width( 95f ), GUILayout.Height( 18f ) );

                GUILayout.BeginHorizontal();
                bool mUp = ASKSettingsWindow.NotBegin( i ) ? GUILayout.Button( "◄", GUILayout.Width( 22f ), GUILayout.Height( 20f ) ) : false;
                GUILayout.Space( ASKSettingsWindow.NotBegin( i ) ? 47f : 76f );
                bool mDown = ASKSettingsWindow.NotEnd( i, materialsSize ) ? GUILayout.Button( "►", GUILayout.Width( 22f ), GUILayout.Height( 20f ) ) : false;
                GUILayout.EndHorizontal();

                GUILayout.Space( materialsSize > 1 ? 28f : 48f );

                GUILayout.BeginHorizontal();
                GUILayout.Space( 77f );
                bool mDel = GUILayout.Button( "X", GUILayout.Width( 18f ), GUILayout.Height( 16f ) );
                GUILayout.EndHorizontal();

                GUILayout.Space( 4f );

                GUILayout.EndVertical();
                GUILayout.EndHorizontal();


                if( mUp )
                {
                    array.MoveArrayElement( i - 1, i );
                    return;
                }
                if( mDown )
                {
                    array.MoveArrayElement( i + 1, i );
                    return;
                }

                if( mDel )
                {
                    element.objectReferenceValue = null;
                    array.DeleteArrayElementAtIndex( i );
                    return;
                }

                cells++;
                int maxCells = Mathf.RoundToInt( ASKSettingsWindow.width / 170f );

                if( cells == maxCells || i == materialsSize - 1 )
                {
                    GUILayout.Space( 5f );
                    GUILayout.EndHorizontal();
                    cells = 0;
                }
            }
            GUILayout.Space( 5f );
            EditorGUILayout.EndScrollView();
        }


        // FullReset
        internal static void FullReset()
        {
            tmpSerializedObject = null;
            nameProp = null;
            surfacesArray = surfacesElement = null;
            materialsArray = materialsElement = null;
            texturesArray = texturesElement = null;

            leftScroll = rightScroll = Vector2.zero;
            selection = 0;
        }
    }
}