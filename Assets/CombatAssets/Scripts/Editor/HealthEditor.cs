/********************************************
 * Copyright(c): 2016 Victor Klepikov       *
 *                                          *
 * Site: 	     http://u3d.as/5Fb		    *
 * Support: 	 http://bit.ly/vk-Support   *
 ********************************************/


using UnityEngine;
using UnityEditor;
using m_ReorderableList = UnityEditorInternal.ReorderableList;
using m_List = System.Collections.Generic.List<UnityEditor.SerializedProperty>;


namespace AdvancedShooterKit.Inspector
{
    [CustomEditor( typeof( Health ) )]
    [CanEditMultipleObjects]
    public class HealthEditor : Editor
    {
        private SerializedProperty
            immortalProp, armorTypeProp,
            maxHealthProp, currentHealthProp,
            regenerationProp, amountProp, delayProp, intervalProp;

        protected SerializedProperty
            spawnObjectsDelayProp, destroyBodyDelayProp,
            onlyOneDropProp;

        protected SerializedProperty mainFoProp, eventsFoProp;

        protected m_ReorderableList deathDropsList, deathObjectList;

        protected m_List eventsList = new m_List();



        // OnEnable
        protected virtual void OnEnable()
        {
            immortalProp = serializedObject.FindProperty( "immortal" );
            armorTypeProp = serializedObject.FindProperty( "armorType" );
            maxHealthProp = serializedObject.FindProperty( "maxHealth" );
            currentHealthProp = serializedObject.FindProperty( "currentHealth" );

            regenerationProp = serializedObject.FindProperty( "regeneration" );
            amountProp = serializedObject.FindProperty( "regAmount" );
            delayProp = serializedObject.FindProperty( "regDelay" );
            intervalProp = serializedObject.FindProperty( "regInterval" );


            spawnObjectsDelayProp = serializedObject.FindProperty( "spawnObjectsDelay" );
            destroyBodyDelayProp = serializedObject.FindProperty( "destroyBodyDelay" );

            deathObjectList = new m_ReorderableList( serializedObject, serializedObject.FindProperty( "deathObjects" ), true, true, true, true );

            onlyOneDropProp = serializedObject.FindProperty( "dropOnlyOnePickup" );
            deathDropsList = new m_ReorderableList( serializedObject, serializedObject.FindProperty( "deathDrops" ), true, true, true, true );

            eventsList.Add( serializedObject.FindProperty( "OnDamage" ) );
            eventsList.Add( serializedObject.FindProperty( "OnDead" ) );

            mainFoProp = serializedObject.FindProperty( "mainFo" );
            eventsFoProp = serializedObject.FindProperty( "eventsFo" );
        }

        // OnInspectorGUI
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            ShowParameters();
            serializedObject.ApplyModifiedProperties();
        }

        // Show Parameters
        protected virtual void ShowParameters()
        {
            CallMethodByName( ref mainFoProp, "    Main", "ShowMainParams" );
            CallMethodByName( ref eventsFoProp, "    Events", "ShowEvents" );
        }

        // Call NamedMethod
        protected void CallMethodByName( ref SerializedProperty spFoldout, string dataName, string methodName )
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
                    case "ShowEvents": ShowEvents(); break;
                }
                GUILayout.Space( 5f );
            }
            else
            {
                GUILayout.Space( 2f );
            }

            GUILayout.EndVertical();
        }



        protected virtual void ShowMainParams()
        {
            GUILayout.Space( 5f );
            EditorGUILayout.PropertyField( immortalProp );
            GUILayout.Space( 5f );

            GUI.enabled = !immortalProp.boolValue;
            EditorGUILayout.PropertyField( armorTypeProp );
            EditorHelper.ShowMinMaxSlider( currentHealthProp, maxHealthProp, 1f, 200f, "Health Level", true );
            GUI.enabled = true;

            // Regeneration
            EditorGUILayout.PropertyField( regenerationProp );
            GUI.enabled = regenerationProp.boolValue;
            GUILayout.BeginHorizontal();
            GUILayout.Space( 15 );
            EditorGUILayout.IntSlider( amountProp, 1, 10 );
            GUILayout.EndHorizontal();
            EditorHelper.ShowSubSlider( ref delayProp, .1f, 5f, "Delay", 15f );
            EditorHelper.ShowSubSlider( ref intervalProp, .01f, 5f, "Interval", 15f );
            GUI.enabled = true;

            GUILayout.Space( 5f );
            float currentHealth = currentHealthProp.intValue;
            float maxHealth = maxHealthProp.intValue;
            float percent = currentHealth / maxHealth;
            EditorHelper.ShowProgressBar( percent, "Percent: " + Mathf.RoundToInt( percent * 100f ) );

            // Drops After Death            
            ShowDeathDrops();
        }

        protected virtual void ShowEvents()
        {
            foreach( var evnt in eventsList )
                EditorGUILayout.PropertyField( evnt, false, null );
        }


        // ShowDeathDrops
        protected virtual void ShowDeathDrops()
        {
            GUILayout.Space( 5f );
            EditorHelper.ShowMinMaxSlider( destroyBodyDelayProp, spawnObjectsDelayProp, 0f, 15f, "Spawn objects delay" );

            GUILayout.Space( 5f );            
            EditorHelper.ShowSimpleReorderableList( deathObjectList, "Dropped objects after destroy" );

            GUILayout.Space( 5f );
            EditorGUILayout.PropertyField( onlyOneDropProp );
            EditorHelper.ShowSimpleReorderableList( deathDropsList, "Dropped pickups after destroy" );
            GUILayout.Space( 5f );
        }
        
    }
}