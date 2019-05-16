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
    public static class EditorHelper
    {
        private static GUIContent m_GuiContent = new GUIContent( string.Empty );


        // Show PropertyField
        public static void ShowPropertyField( ref SerializedProperty property, string label, float space )
        {
            if( space != 0f )
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space( space );
            }

            EditorGUILayout.PropertyField( property, PropertyLabel( label ) );

            if( space != 0f )
                GUILayout.EndHorizontal();
        }

        // Show FixedPropertyField
        public static void ShowFixedPropertyField( ref SerializedProperty property, string label, float space, float width )
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space( space );
            GUILayout.Label( label, GUILayout.Width( width ) );
            EditorGUILayout.PropertyField( property, GUIContent.none );
            GUILayout.EndHorizontal();
        }

        // Show BoolField
        public static void ShowBoolField( ref SerializedProperty property, string label )
        {
            GUILayout.Space( 5f );
            EditorGUILayout.PropertyField( property, PropertyLabel( label ) );
            GUI.enabled = property.boolValue;
        }

        // Show SubSlider
        public static void ShowSubSlider( ref SerializedProperty property, float minVal, float maxVal, string label, float space )
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space( space );
            EditorGUILayout.Slider( property, minVal, maxVal, PropertyLabel( label ) );
            GUILayout.EndHorizontal();
        }

        // Show int SubSlider
        public static void ShowIntSubSlider( ref SerializedProperty property, int minVal, int maxVal, string label, float space )
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space( space );
            EditorGUILayout.IntSlider( property, minVal, maxVal, PropertyLabel( label ) );
            GUILayout.EndHorizontal();
        }

        // Show SimpleReorderableList
        public static void ShowSimpleReorderableList( ReorderableList list, string label, float space = 0f )
        {
            GUILayout.Space( 5f );

            if( space != 0f )
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space( space );
                GUILayout.BeginVertical();
            }

            list.drawHeaderCallback = ( Rect rect ) =>
            {
                EditorGUI.LabelField( rect, label );
            };

            list.drawElementCallback = ( Rect rect, int index, bool isActive, bool isFocused ) =>
            {
                rect.y += 2f;
                rect.height = EditorGUIUtility.singleLineHeight;

                EditorGUI.PropertyField( rect, list.serializedProperty.GetArrayElementAtIndex( index ), GUIContent.none );
            };
            list.DoLayoutList();

            if( space != 0f )
            {
                GUILayout.EndVertical();
                GUILayout.EndHorizontal();
            }
        }


        // Show CameraShakeSlider
        public static void ShowMinMaxSlider( SerializedProperty minProperty, SerializedProperty maxProperty, float minLimit, float maxLimit, string label, bool intValues = false )
        {
            float minValue = intValues ? ( float )minProperty.intValue : minProperty.floatValue;
            float maxValue = intValues ? ( float )maxProperty.intValue : maxProperty.floatValue;
            GUILayout.BeginHorizontal();
            GUILayout.Label( PropertyLabel( label ) );
            GUILayout.Space( 15f );
            EditorGUILayout.LabelField( minValue.ToString( intValues ? "f0" : "f1" ), GUILayout.Width( 30f ) );
            EditorGUILayout.MinMaxSlider( ref minValue, ref maxValue, minLimit, maxLimit );
            EditorGUILayout.LabelField( maxValue.ToString( intValues ? "f0" : "f1" ), GUILayout.Width( 30f ) );
            GUILayout.EndHorizontal();

            if( intValues )
            {
                minProperty.intValue = Mathf.RoundToInt( minValue );
                maxProperty.intValue = Mathf.RoundToInt( maxValue );
            }
            else
            {
                minProperty.floatValue = minValue;
                maxProperty.floatValue = maxValue;
            }
        }


        // Show SFXProperty AndPlayButton
        public static void ShowSFXPropertyAndPlayButton( SerializedObject serializedObject, SerializedProperty property, string label = "" )
        {
            GUILayout.BeginHorizontal();

            if( label == "" )
                EditorGUILayout.PropertyField( property );
            else
                EditorGUILayout.PropertyField( property, PropertyLabel( label ) );

            GUI.enabled = ( property.objectReferenceValue != null );
            if( GUILayout.Button( "►", GUILayout.Width( 22f ), GUILayout.Height( 15f ) ) )
            {
                try
                {
                    AudioSource tmpAudio = ( serializedObject.targetObject as Component ).transform.root.GetComponentInChildren<AudioSource>();

                    if( tmpAudio == null )
                        tmpAudio = PlayerCamera.m_Transform.GetComponentInChildren<AudioSource>();

                    if( tmpAudio != null )
                        tmpAudio.PlayOneShot( property.objectReferenceValue as AudioClip );
                }
                catch
                {
                    Debug.LogError( "This prefab must be placed on scene." );
                }                
            }
            GUI.enabled = true;

            GUILayout.EndHorizontal();
        }

        // Show SFXList AndPlayButton
        public static void ShowSFXListAndPlayButton( SerializedObject serializedObject, ReorderableList list, string label, float space = 0f )
        {
            GUILayout.Space( 5f );

            if( space != 0f )
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space( space );
                GUILayout.BeginVertical();
            }

            list.drawHeaderCallback = ( Rect rect ) =>
            {
                EditorGUI.LabelField( rect, label );
            };

            list.drawElementCallback = ( Rect rect, int index, bool isActive, bool isFocused ) =>
            {
                rect.y += 2f;
                rect.height = EditorGUIUtility.singleLineHeight;
                rect.width = EditorGUIUtility.currentViewWidth - 100f;

                SerializedProperty property = list.serializedProperty.GetArrayElementAtIndex( index );                
                EditorGUI.PropertyField( rect, property, GUIContent.none );

                rect.x += rect.width + 10f;
                rect.width = 22f;

                GUI.enabled = ( property.objectReferenceValue != null );
                if( GUI.Button( rect, "►" ) )
                {
                    try
                    {
                        AudioSource tmpAudio = ( serializedObject.targetObject as Component ).transform.root.GetComponentInChildren<AudioSource>();

                        if( tmpAudio == null )
                            tmpAudio = PlayerCamera.m_Transform.GetComponentInChildren<AudioSource>();

                        if( tmpAudio != null )
                            tmpAudio.PlayOneShot( property.objectReferenceValue as AudioClip );
                    }
                    catch
                    {
                        Debug.LogError( "This prefab must be placed on scene." );
                    }
                }
                GUI.enabled = true;

            };
            list.DoLayoutList();

            if( space != 0f )
            {
                GUILayout.EndVertical();
                GUILayout.EndHorizontal();
            }
        }



        // Show Position&Rotation List
        public static void ShowPositionAndRotationList( ReorderableList list, string label )
        {
            GUILayout.Space( 5f );

            list.drawHeaderCallback = ( Rect rect ) =>
            {
                EditorGUI.LabelField( rect, label );
            };

            list.elementHeight = EditorGUIUtility.singleLineHeight * 3f;

            list.drawElementCallback = ( Rect rect, int index, bool isActive, bool isFocused ) =>
            {
                SerializedProperty element = list.serializedProperty.GetArrayElementAtIndex( index );
                SerializedProperty positionProp = element.FindPropertyRelative( "position" );
                SerializedProperty rotationProp = element.FindPropertyRelative( "rotation" );

                const float space = 3f;
                float width = EditorGUIUtility.currentViewWidth;
                float height = EditorGUIUtility.singleLineHeight;
                float startX = rect.x;

                rect.y += 2f;

                rect.x = startX / 2f;
                rect.width = width / 1.13f;
                rect.height = height * 2.8f;
                EditorGUI.HelpBox( rect, string.Empty, MessageType.None );

                // Position
                rect.x = startX;
                rect.y += 5f;
                rect.height = height;
                rect.width = width / 7f;
                EditorGUI.LabelField( rect, "Position" );

                rect.x += rect.width + space;
                rect.width = width / 1.5f;
                EditorGUI.PropertyField( rect, positionProp, GUIContent.none );


                // Rotation
                rect.x = startX;
                rect.y += height + space;
                rect.width = width / 7f;
                EditorGUI.LabelField( rect, "Rotation" );

                rect.x += rect.width + space;
                rect.width = width / 1.5f;
                EditorGUI.PropertyField( rect, rotationProp, GUIContent.none );
            };
            list.DoLayoutList();
        }


        // Show ProgressBar
        public static void ShowProgressBar( float percent, string label )
        {
            Rect rect = GUILayoutUtility.GetRect( 18f, 18f, "TextField" );
            EditorGUI.ProgressBar( rect, percent, label );
            EditorGUILayout.Space();
        }

        
        //Property Label
        public static GUIContent PropertyLabel( string text )
        {
            m_GuiContent.text = text;
            return m_GuiContent;
        }
    }
}