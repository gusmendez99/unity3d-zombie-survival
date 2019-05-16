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
    public sealed class ASKSettingsWindow : EditorWindow
    {
        internal static string mainDirectory { get; private set; }
        internal static float width { get; private set; }
        
        private static readonly string[] tabs = { "Surfaces", "Input", "Game" };
        private static int currentTab = 0;

        private static bool needRepaint = false;


        // On ScriptsRecompiled
        [UnityEditor.Callbacks.DidReloadScripts]
        static void OnScriptsRecompiled()
        {
            needRepaint = true;
        }

        // OnInspectorUpdate
        void OnInspectorUpdate()
        {
            if( needRepaint )
            {
                needRepaint = false;
                Init();
            }
        }

        // OnDestroy
        void OnDestroy()
        {
            currentTab = 0;

            SurfaceDetectorTab.FullReset();
            InputSettingsTab.FullReset();
            GameSettingsTab.FullReset();

            AssetDatabase.DeleteAsset( mainDirectory + "/tmp" );
        }

        // Init
        //[MenuItem( "Window/ASK Settings" )]
        [MenuItem( "Window/Victor's Assets/Advanced Shooter Kit Settings" )]     
        public static void Init()
        {
            ASKSettingsWindow window = GetWindow<ASKSettingsWindow>( "ASK Settings" );
            window.minSize = new Vector2( 725f, 535f );
            window.Focus();
            mainDirectory = GetResourcesPath( MonoScript.FromScriptableObject( window ) );
            //
            SetupIt();
        }

        // SetupIt
        private static void SetupIt()
        {
            SurfaceDetectorTab.SetupTab();
            InputSettingsTab.SetupTab();
            GameSettingsTab.SetupTab();
        }

        // OnGUI
        void OnGUI()
        {
            width = position.width;

            GUILayout.Space( 7f );
            currentTab = GUILayout.Toolbar( currentTab, tabs, GUILayout.ExpandWidth( true ), GUILayout.Height( 35f ) );
            GUILayout.Space( 8f );

            EditorGUILayout.BeginHorizontal();

            switch( currentTab )
            {
                case 0: // Surfaces
                    SurfaceDetectorTab.OnWindowGUI();
                    break;
                case 1: // Input
                    InputSettingsTab.OnWindowGUI();
                    break;
                case 2: // Game
                    GameSettingsTab.OnWindowGUI();
                    break;
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            const float height = 30f;
            bool load = GUILayout.Button( "Reload Settings", GUILayout.Height( height ) );
            bool save = GUILayout.Button( "Save Settings", GUILayout.Height( height ) );
            EditorGUILayout.EndHorizontal();

            if( save )
            {
                SurfaceDetectorTab.SaveSettings();
                InputSettingsTab.SaveSettings();
                GameSettingsTab.SaveSettings();
            }

            if( load && EditorUtility.DisplayDialog( "ASK Settings Warning!", "Warning: All changes will be reset! Сontinue?", "Yes", "No" ) )
            {                  
                SurfaceDetectorTab.ReloadSettings();
                InputSettingsTab.ReloadSettings();
                GameSettingsTab.ReloadSettings();
            }
        }

        
        // Get Names
        internal static string[] GetNames( SerializedProperty array, int size )
        {
            string[] tmpNames = new string[ size ];

            for( int i = 0; i < size; i++ )
                tmpNames[ i ] = array.GetArrayElementAtIndex( i ).FindPropertyRelative( "name" ).stringValue;

            return tmpNames;
        }

        
        // Not Begin
        internal static bool NotBegin( int index )
        {
            return ( index - 1 >= 0 );
        }
        // Not End
        internal static bool NotEnd( int index, int size )
        {
            return ( index + 1 < size );
        }

                
        // Get ResourcesPath
        private static string GetResourcesPath( MonoScript monoScript )
        {
            string assetPath = AssetDatabase.GetAssetPath( monoScript );
            const string startFolder = "Assets";
            const string endFolder = "/Scripts";
            const string resFolder = "Resources";

            if( assetPath.Contains( startFolder ) && assetPath.Contains( endFolder ) )
            {
                int startIndex = assetPath.IndexOf( startFolder, 0 ) + startFolder.Length;
                int endIndex = assetPath.IndexOf( endFolder, startIndex );

                string between = assetPath.Substring( startIndex, endIndex - startIndex );
                string projectFolder = startFolder + between;
                string resPath = projectFolder + "/" + resFolder;

                //
                bool refresh = false;

                if( !AssetDatabase.IsValidFolder( resPath ) )
                {
                    AssetDatabase.CreateFolder( projectFolder, resFolder );
                    refresh = true;
                }

                if( !AssetDatabase.IsValidFolder( resPath + "/tmp" ) )
                {
                    AssetDatabase.CreateFolder( resPath, "tmp" );
                    refresh = true;
                }

                if( refresh )
                {
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }

                return resPath;
            }

            return string.Empty;
        }        
    }
}