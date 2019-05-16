/********************************************
 * Copyright(c): 2016 Victor Klepikov       *
 *                                          *
 * Site: 	     http://u3d.as/5Fb		    *
 * Support: 	 http://bit.ly/vk-Support   *
 ********************************************/


using UnityEngine;
using AdvancedShooterKit.Events;


namespace AdvancedShooterKit
{
    public class InputSettings : ScriptableObject
    {
        [System.Serializable]
        public sealed class ActionData
        {
            [System.Serializable]
            public sealed class ActionAxis
            {
                public string axisName = string.Empty;

                [SerializeField]
                private EAxisSource axisSource = EAxisSource.InputManager;

                [SerializeField]
                [Range( 0f, 1f )]
                private float threshold = 0f;
                [SerializeField]
                private EAxisStateClamp axisStateClamp = EAxisStateClamp.NotClamped;

                private EAxisState axisState = EAxisState.NONE;
                private bool touchDown = false;

                private int pressedFrame = -1; //( pressedFrame == Time.frameCount - 1 )
                private int releasedFrame = -1; //( releasedFrame == Time.frameCount - 1 )


                public EAxisState getAxisState
                {
                    get
                    {
                        float value = ( axisSource == EAxisSource.InputManager ) ? GetAxis( axisName ) : Input.GetAxis( axisName );

                        bool positiveOrNotClamped = ( axisStateClamp == EAxisStateClamp.OnlyPositive || axisStateClamp == EAxisStateClamp.NotClamped );
                        bool negativeOrNotClamped = ( axisStateClamp == EAxisStateClamp.OnlyNegative || axisStateClamp == EAxisStateClamp.NotClamped );
                        int frameCount = Time.frameCount - 1;

                        if( positiveOrNotClamped && value > threshold )
                        {
                            if( pressedFrame == frameCount )
                            {
                                touchDown = true;
                            }
                            //
                            if( touchDown )
                            {
                                axisState = EAxisState.PositivePress;
                            }
                            else
                            {
                                pressedFrame = Time.frameCount;                                
                                axisState = EAxisState.PositiveDown;
                            }                            
                        }
                        else if( negativeOrNotClamped && value < -threshold )
                        {
                            if( pressedFrame == frameCount )
                            {
                                touchDown = true;
                            }
                            //
                            if( touchDown )
                            {
                                axisState = EAxisState.NegativePress;
                            }
                            else
                            {
                                pressedFrame = Time.frameCount;
                                axisState = EAxisState.NegativeDown;
                            }
                        }
                        else if( value <= threshold || value >= -threshold )
                        {
                            bool positiveDownOrPressed = ( axisState == EAxisState.PositiveDown || axisState == EAxisState.PositivePress );
                            bool negativeDownOrPressed = ( axisState == EAxisState.NegativeDown || axisState == EAxisState.NegativePress );

                            if( touchDown && ( positiveDownOrPressed || negativeDownOrPressed ) )
                            {
                               if( positiveOrNotClamped && positiveDownOrPressed )
                                    axisState = EAxisState.PositiveUp;
                                else if( negativeOrNotClamped && negativeDownOrPressed )
                                    axisState = EAxisState.NegativeUp;                                                               

                                releasedFrame = Time.frameCount;
                            }
                            else
                            {                              
                                if( releasedFrame == frameCount ) 
                                    touchDown = false;

                                if( !touchDown )
                                    axisState = EAxisState.NONE;
                            }
                        }

                        return axisState;
                    }
                }
                //                
            }


            public string name = string.Empty;
            public EActionType type = EActionType.KeyCode;

            public KeyCode[] keys = null;            
            public ActionAxis[] actionAxes = null;


            // for delegates
            internal struct ActionEvents
            {
                internal bool useDown, usePress, useUp;
                internal ActionHandler downHandler, pressHandler, upHandler;
            }
            internal ActionEvents actionEvents, axisPositiveEvents, axisNegativeEvents;
        }        

        [System.Serializable]
        public sealed class AxisData
        {
            public string name = string.Empty;
            public EAxisType type = EAxisType.Unity;

            public string[] unityAxes = null;

            [System.Serializable]
            public struct CustomKeys
            {
                public KeyCode negativeKey, positiveKey;
            }
            public CustomKeys[] customKeys = null;

            public bool normalize = false;

            // for delegates
            [System.NonSerialized]
            public bool useIt = false;
            [System.NonSerialized]
            public AxisHandler axisHandler;
        }

        [SerializeField]
        private ActionData[] actionDatabase = null;
        [SerializeField]
        private AxisData[] axesDatabase = null;

        public static ActionData[] ActionDB { get { return m_Instance.actionDatabase; } }
        public static AxisData[] AxesDB { get { return m_Instance.axesDatabase; } }


        private static InputSettings instance = null;
        private static InputSettings m_Instance
        {
            get
            {
                if( instance == null )
                    instance = Resources.Load<InputSettings>( "InputSettings" );

                return instance;
            }
        }
        //



        // Bind Action
        internal static void BindAction( string m_Name, EActionEvent m_Event, ActionHandler m_Handler )
        {
            foreach( ActionData aData in ActionDB )            
                if( aData.name == m_Name )
                {
                    switch( m_Event )
                    {
                        case EActionEvent.Down:
                            aData.actionEvents.useDown = true;
                            if( aData.actionEvents.downHandler != m_Handler )
                                aData.actionEvents.downHandler += m_Handler;
                            break;
                        case EActionEvent.Press:
                            aData.actionEvents.usePress = true;
                            if( aData.actionEvents.pressHandler != m_Handler )
                                aData.actionEvents.pressHandler += m_Handler;
                            break;
                        case EActionEvent.Up:
                            aData.actionEvents.useUp = true;
                            if( aData.actionEvents.upHandler != m_Handler )
                                aData.actionEvents.upHandler += m_Handler;
                            break;
                    }
                    return;
                }
            
            Debug.LogError( "Action " + m_Name + " Not Found." );
        }
        // Unbind Action
        internal static void UnbindAction( string m_Name, EActionEvent m_Event, ActionHandler m_Handler )
        {
            foreach( ActionData aData in ActionDB )
            {
                if( aData.name == m_Name )
                {
                    switch( m_Event )
                    {
                        case EActionEvent.Down:
                            if( aData.actionEvents.downHandler == m_Handler )
                            {
                                aData.actionEvents.downHandler -= m_Handler;
                                aData.actionEvents.useDown = ( aData.actionEvents.downHandler != null );
                            }
                            break;
                        case EActionEvent.Press:
                            if( aData.actionEvents.pressHandler == m_Handler )
                            {
                                aData.actionEvents.pressHandler -= m_Handler;
                                aData.actionEvents.usePress = ( aData.actionEvents.pressHandler != null );
                            }
                            break;
                        case EActionEvent.Up:
                            if( aData.actionEvents.upHandler == m_Handler )
                            {
                                aData.actionEvents.upHandler -= m_Handler;
                                aData.actionEvents.useUp = ( aData.actionEvents.upHandler != null );
                            }
                            break;
                    }
                    return;
                }
            }
            Debug.LogError( "Action " + m_Name + " Not Found." );
        }

        // Bind ActionAxis
        internal static void BindActionAxis( string m_Name, EAxisState m_State, ActionHandler m_Handler )
        {
            foreach( ActionData aData in ActionDB )
                if( aData.name == m_Name )
                {
                    switch( m_State )
                    {
                        case EAxisState.PositiveDown:
                            aData.axisPositiveEvents.useDown = true;
                            if( aData.axisPositiveEvents.downHandler != m_Handler )
                                aData.axisPositiveEvents.downHandler += m_Handler;
                            break;
                        case EAxisState.PositivePress:
                            aData.axisPositiveEvents.usePress = true;
                            if( aData.axisPositiveEvents.pressHandler != m_Handler )
                                aData.axisPositiveEvents.pressHandler += m_Handler;
                            break;
                        case EAxisState.PositiveUp:
                            aData.axisPositiveEvents.useUp = true;
                            if( aData.axisPositiveEvents.upHandler != m_Handler )
                                aData.axisPositiveEvents.upHandler += m_Handler;
                            break;

                        case EAxisState.NegativeDown:
                            aData.axisNegativeEvents.useDown = true;
                            if( aData.axisNegativeEvents.downHandler != m_Handler )
                                aData.axisNegativeEvents.downHandler += m_Handler;
                            break;
                        case EAxisState.NegativePress:
                            aData.axisNegativeEvents.usePress = true;
                            if( aData.axisNegativeEvents.pressHandler != m_Handler )
                                aData.axisNegativeEvents.pressHandler += m_Handler;
                            break;
                        case EAxisState.NegativeUp:
                            aData.axisNegativeEvents.useUp = true;
                            if( aData.axisNegativeEvents.upHandler != m_Handler )
                                aData.axisNegativeEvents.upHandler += m_Handler;
                            break;
                    }
                    return;
                }
            //
            Debug.LogError( "Action " + m_Name + " Not Found." );
        }
        // Unbind ActionAxis
        internal static void UnbindActionAxis( string m_Name, EAxisState m_State, ActionHandler m_Handler )
        {
            foreach( ActionData aData in ActionDB )
                if( aData.name == m_Name )
                {
                    switch( m_State )
                    {
                        case EAxisState.PositiveDown:
                            if( aData.axisPositiveEvents.downHandler == m_Handler )
                            {
                                aData.axisPositiveEvents.downHandler -= m_Handler;
                                aData.axisPositiveEvents.useDown = ( aData.axisPositiveEvents.downHandler != null );
                            }
                            break;
                        case EAxisState.PositivePress:
                            if( aData.axisPositiveEvents.pressHandler == m_Handler )
                            {
                                aData.axisPositiveEvents.pressHandler -= m_Handler;
                                aData.axisPositiveEvents.usePress = ( aData.axisPositiveEvents.pressHandler != null );
                            }
                            break;
                        case EAxisState.PositiveUp:
                            if( aData.axisPositiveEvents.upHandler == m_Handler )
                            {
                                aData.axisPositiveEvents.upHandler -= m_Handler;
                                aData.axisPositiveEvents.useUp = ( aData.axisPositiveEvents.upHandler != null );
                            }
                            break;

                        case EAxisState.NegativeDown:
                            if( aData.axisNegativeEvents.downHandler == m_Handler )
                            {
                                aData.axisNegativeEvents.downHandler -= m_Handler;
                                aData.axisNegativeEvents.useDown = ( aData.axisNegativeEvents.downHandler != null );
                            }
                            break;
                        case EAxisState.NegativePress:
                            if( aData.axisNegativeEvents.pressHandler == m_Handler )
                            {
                                aData.axisNegativeEvents.pressHandler -= m_Handler;
                                aData.axisNegativeEvents.usePress = ( aData.axisNegativeEvents.pressHandler != null );
                            }
                            break;
                        case EAxisState.NegativeUp:
                            if( aData.axisNegativeEvents.upHandler == m_Handler )
                            {
                                aData.axisNegativeEvents.upHandler -= m_Handler;
                                aData.axisNegativeEvents.useUp = ( aData.axisNegativeEvents.upHandler != null );
                            }
                            break;
                    }
                    return;
                }
            //
            Debug.LogError( "Action " + m_Name + " Not Found." );
        }


        // Bind Axis
        internal static void BindAxis( string m_Name, AxisHandler m_Handler )
        {
            foreach( AxisData aData in AxesDB )
            {
                if( aData.name == m_Name )
                {
                    aData.useIt = true;
                    if( aData.axisHandler != m_Handler )
                        aData.axisHandler += m_Handler;
                    return;
                }
            }
            Debug.LogError( "Axis " + m_Name + " Not Found." );
        }
        // Unbind Axis
        internal static void UnbindAxis( string m_Name, AxisHandler m_Handler )
        {
            foreach( AxisData aData in AxesDB )
            {
                if( aData.name == m_Name )
                {
                    if( aData.axisHandler == m_Handler )
                    {
                        aData.axisHandler -= m_Handler;
                        aData.useIt = ( aData.axisHandler != null );
                    }
                    return;
                }
            }
            Debug.LogError( "Axis " + m_Name + " Not Found." );
        }


        // Run Actions
        internal static void RunActions()
        {
            foreach( ActionData aData in ActionDB )
            {
                if( aData.actionEvents.useDown )
                {
                    if( aData.type == EActionType.KeyCode || aData.type == EActionType.Mixed )
                    {
                        foreach( KeyCode key in aData.keys )
                            if( Input.GetKeyDown( key ) )
                            {
                                aData.actionEvents.downHandler.Invoke();
                                break;
                            }
                    }
                    if( aData.type == EActionType.Axis || aData.type == EActionType.Mixed )
                    {
                        foreach( ActionData.ActionAxis aAxis in aData.actionAxes )
                        {
                            EAxisState axisState = aAxis.getAxisState;
                            if( axisState == EAxisState.NegativeDown || axisState == EAxisState.PositiveDown )
                            {
                                aData.actionEvents.downHandler.Invoke();
                                break;
                            }
                        }
                    }
                }

                if( aData.actionEvents.usePress )
                {
                    if( aData.type == EActionType.KeyCode || aData.type == EActionType.Mixed )
                    {
                        foreach( KeyCode key in aData.keys )
                            if( Input.GetKeyDown( key ) )
                            {
                                aData.actionEvents.pressHandler.Invoke();
                                break;
                            }
                    }
                    if( aData.type == EActionType.Axis || aData.type == EActionType.Mixed )
                    {
                        foreach( ActionData.ActionAxis aAxis in aData.actionAxes )
                        {
                            EAxisState axisState = aAxis.getAxisState;
                            if( axisState == EAxisState.NegativeDown || axisState == EAxisState.PositiveDown )
                            {
                                aData.actionEvents.pressHandler.Invoke();
                                break;
                            }
                        }
                    }
                }

                if( aData.actionEvents.useUp )
                {
                    if( aData.type == EActionType.KeyCode || aData.type == EActionType.Mixed )
                    {
                        foreach( KeyCode key in aData.keys )
                            if( Input.GetKeyDown( key ) )
                            {
                                aData.actionEvents.upHandler.Invoke();
                                break;
                            }
                    }
                    if( aData.type == EActionType.Axis || aData.type == EActionType.Mixed )
                    {
                        foreach( ActionData.ActionAxis aAxis in aData.actionAxes )
                        {
                            EAxisState axisState = aAxis.getAxisState;
                            if( axisState == EAxisState.NegativeDown || axisState == EAxisState.PositiveDown )
                            {
                                aData.actionEvents.upHandler.Invoke();
                                break;
                            }
                        }
                    }
                }
                //
            }
        }

        // Run ActionAxis
        internal static void RunActionAxis()
        {
            foreach( ActionData aData in ActionDB )
            {
                if( aData.type == EActionType.KeyCode )
                    continue;

                foreach( ActionData.ActionAxis aAxis in aData.actionAxes )
                {
                    EAxisState axisState = aAxis.getAxisState;

                    if( aData.axisPositiveEvents.useDown && axisState == EAxisState.PositiveDown )
                    {
                        aData.axisPositiveEvents.downHandler.Invoke();
                        break;
                    }
                    if( aData.axisPositiveEvents.usePress && axisState == EAxisState.PositivePress )
                    {
                        aData.axisPositiveEvents.pressHandler.Invoke();
                        break;
                    }
                    if( aData.axisPositiveEvents.useUp && axisState == EAxisState.PositiveUp )
                    {
                        aData.axisPositiveEvents.upHandler.Invoke();
                        break;
                    }

                    if( aData.axisNegativeEvents.useDown && axisState == EAxisState.NegativeDown )
                    {
                        aData.axisNegativeEvents.downHandler.Invoke();
                        break;
                    }
                    if( aData.axisNegativeEvents.usePress && axisState == EAxisState.NegativePress )
                    {
                        aData.axisNegativeEvents.pressHandler.Invoke();
                        break;
                    }
                    if( aData.axisNegativeEvents.useUp && axisState == EAxisState.NegativeUp )
                    {
                        aData.axisNegativeEvents.upHandler.Invoke();
                        break;
                    }
                }
            }
        }

        // RunAxis
        internal static void RunAxis()
        {
            foreach( AxisData aData in AxesDB )
            {
                if( aData.useIt )
                    aData.axisHandler.Invoke( GetAxisValue( aData ) );
            }
        }



        // Get Action
        internal static bool GetAction( string m_Name, EActionEvent m_Event )
        {
            foreach( ActionData aData in ActionDB )
                if( aData.name == m_Name )
                {
                    switch( m_Event )
                    {
                        case EActionEvent.Down:
                            if( aData.type == EActionType.KeyCode || aData.type == EActionType.Mixed )
                            {
                                foreach( KeyCode key in aData.keys )
                                    if( Input.GetKeyDown( key ) )
                                        return true;
                            }
                            if( aData.type == EActionType.Axis || aData.type == EActionType.Mixed )
                            {
                                foreach( ActionData.ActionAxis aAxis in aData.actionAxes )
                                {
                                    EAxisState axisState = aAxis.getAxisState;
                                    if( axisState == EAxisState.NegativeDown || axisState == EAxisState.PositiveDown )
                                        return true;
                                }
                            }
                            break;

                        case EActionEvent.Press:
                            if( aData.type == EActionType.KeyCode || aData.type == EActionType.Mixed )
                            {
                                foreach( KeyCode key in aData.keys )
                                    if( Input.GetKey( key ) )
                                        return true;
                            }
                            if( aData.type == EActionType.Axis || aData.type == EActionType.Mixed )
                            {
                                foreach( ActionData.ActionAxis aAxis in aData.actionAxes )
                                {
                                    EAxisState axisState = aAxis.getAxisState;
                                    if( axisState == EAxisState.NegativePress || axisState == EAxisState.PositivePress )
                                        return true;
                                }
                            }
                            break;

                        case EActionEvent.Up:
                            if( aData.type == EActionType.KeyCode || aData.type == EActionType.Mixed )
                            {
                                foreach( KeyCode key in aData.keys )
                                    if( Input.GetKeyUp( key ) )
                                        return true;
                            }
                            if( aData.type == EActionType.Axis || aData.type == EActionType.Mixed )
                            {
                                foreach( ActionData.ActionAxis aAxis in aData.actionAxes )
                                {
                                    EAxisState axisState = aAxis.getAxisState;
                                    if( axisState == EAxisState.NegativeUp || axisState == EAxisState.PositiveUp )
                                        return true;
                                }
                            }
                            break;
                    }
                    return false;
                }

            Debug.LogError( "Action " + m_Name + " Not Found!" );
            return false;
        }
        
        // Get ActionAxis
        internal static bool GetActionAxis( string m_Name, EAxisState m_State )
        {
            foreach( ActionData aData in ActionDB )
                if( aData.name == m_Name )
                {
                    if( aData.type != EActionType.KeyCode )
                        foreach( ActionData.ActionAxis aAxis in aData.actionAxes )
                            if( aAxis.getAxisState == m_State )
                                return true;

                    return false;
                }

            Debug.LogError( "ActionAxis " + m_Name + " Not Found!" );
            return false;
        }

        
        // Get Axis
        internal static float GetAxis( string axisName )
        {
            foreach( AxisData aData in AxesDB )
            {
                if( aData.name == axisName )
                    return GetAxisValue( aData );
            }

            Debug.LogError( "Axis " + axisName + " Not Found." );
            return 0f;
        }


        // Get AxisValue
        private static float GetAxisValue( AxisData aData )
        {
            float axisValue = 0f;

            if( aData.type == EAxisType.Unity || aData.type == EAxisType.Mixed )
            {
                foreach( string aName in aData.unityAxes )
                    axisValue += Input.GetAxis( aName );
            }
            if( aData.type == EAxisType.Custom || aData.type == EAxisType.Mixed )
            {
                foreach( AxisData.CustomKeys aKeys in aData.customKeys )
                {
                    if( Input.GetKey( aKeys.negativeKey ) )
                        axisValue += -1f;
                    else if( Input.GetKey( aKeys.positiveKey ) )
                        axisValue += 1f;
                }
            }

            axisValue = aData.normalize ? Mathf.Clamp( axisValue, -1f, 1f ) : axisValue;
            return axisValue;
        }
    }
}