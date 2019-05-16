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
    [CustomEditor( typeof( Character ) )]
    [CanEditMultipleObjects]
    public class CharacterEditor : HealthEditor
    {
        protected SerializedProperty
            damageToPainProp, percentToPainProp, surfaceIndexProp,
            deathSoundProp, deathLayerProp;

        
        private ReorderableList painSoundsList = null;

        private string[] surfaces = new string[ 0 ];
        private bool damageHandlersFound = false;


        // OnEnable
        protected override void OnEnable()
        {
            base.OnEnable();            
            //
            surfaceIndexProp = serializedObject.FindProperty( "surfaceIndex" );
            damageToPainProp = serializedObject.FindProperty( "damageToPain" );
            percentToPainProp = serializedObject.FindProperty( "percentToPain" );
            deathSoundProp = serializedObject.FindProperty( "deathSound" );
            deathLayerProp = serializedObject.FindProperty( "deathLayer" );

            painSoundsList = new ReorderableList( serializedObject, serializedObject.FindProperty( "painSounds" ), true, true, true, true );

            eventsList.Add( serializedObject.FindProperty( "OnPain" ) );

            //
            damageHandlersFound = ( ( target as Component ).GetComponentInChildren<DamagePoint>() != null );
            surfaces = SurfaceDetector.GetNames;
        }


        // Show Parameters
        /*protected override void ShowParameters()
        {
            
        }*/


        // Show MainParams
        protected override void ShowMainParams()
        {
            GUI.enabled = !damageHandlersFound;
            int surfaceIndex = surfaceIndexProp.intValue;
            surfaceIndex = EditorGUILayout.Popup( "Hit Surface", surfaceIndex, surfaces );
            surfaceIndexProp.intValue = surfaceIndex;
            GUI.enabled = true;

            base.ShowMainParams();            
            
            GUILayout.Space( 5f );
            EditorGUILayout.PropertyField( damageToPainProp );
            EditorGUILayout.PropertyField( percentToPainProp );
            EditorHelper.ShowSFXListAndPlayButton( serializedObject, painSoundsList, "Pain Sounds" );

            GUILayout.Space( 5f );            
            EditorHelper.ShowSFXPropertyAndPlayButton( serializedObject, deathSoundProp );
            int layerValue = deathLayerProp.intValue;
            layerValue = EditorGUILayout.LayerField( "Death Layer", layerValue );
            deathLayerProp.intValue = layerValue;

            GUILayout.Space( 5f );
            EditorHelper.ShowMinMaxSlider( destroyBodyDelayProp, spawnObjectsDelayProp, 0f, 15f, "Drop objects delay" );

            GUILayout.Space( 5f );
            EditorHelper.ShowSimpleReorderableList( deathObjectList, "Dropped objects after death" );

            GUILayout.Space( 5f );
            EditorGUILayout.PropertyField( onlyOneDropProp );
            EditorHelper.ShowSimpleReorderableList( deathDropsList, "Dropped pickups after death" );
        }

        // Show Events
        protected override void ShowEvents()
        {
            foreach( var evnt in eventsList )
                EditorGUILayout.PropertyField( evnt, false, null );            
        }

        //
        protected override void ShowDeathDrops()
        {
            // null ha-ha
        }
    }
}