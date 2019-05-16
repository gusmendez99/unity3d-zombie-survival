/********************************************
 * Copyright(c): 2016 Victor Klepikov       *
 *                                          *
 * Site: 	     http://u3d.as/5Fb		    *
 * Support: 	 http://bit.ly/vk-Support   *
 ********************************************/


/** Uncomment this 'define' for 'TouchControlsKit' integration. 
    OR Add to 'PlayerSettings->OtherSettings->Configuration->ScriptingDefineSymbols'.*/

//#define TOUCH_CONTROLS_KIT


#if TOUCH_CONTROLS_KIT
using TouchControlsKit;
using TCKAxisType = TouchControlsKit.EAxisType;
using TCKActionEvent = TouchControlsKit.EActionEvent;
#endif

using UnityEngine;
using UnityEngine.EventSystems;
using AdvancedShooterKit.Events;
using AdvancedShooterKit.Utils;


namespace AdvancedShooterKit
{
    public class ASKInputManager : MonoBehaviour
    {
        [System.Serializable]
        public sealed class UIPrefabs
        {
            public HudElements hudElements = null;
            public MenuElements menuElements = null;

#if TOUCH_CONTROLS_KIT
            public TCKInput touchUIElements = null;
#endif
        }

        [System.Serializable]
        public sealed class Axes
        {
            public string moveX = "Move Horizontal", moveY = "Move Vertical", lookX = "Look Horizontal", lookY = "Look Vertical";

#if TOUCH_CONTROLS_KIT
            public string moveJoystick = "Move Joystick", lookTouchpad = "Look Touchpad";
#endif
        }

        [System.Serializable]
        public sealed class Actions
        {
            public string
                fire = "Fire", zoom = "Zoom", run = "Run", jump = "Jump", crouch = "Crouch", use = "Use",
                reloadWeapon = "Reload Weapon",
                nextFiremode = "Next Firemode", nextAmmotype = "Next Ammotype", toSubweapon = "To Subweapon",
                dropWeapon = "Drop Weapon", prevWeapon = "Prev Weapon", nextWeapon = "Next Weapon",
                pause = "Pause", blockCursor = "Block Cursor", unblockCursor = "Unblock Cursor";
        }


        [SerializeField]
        private EUpdateType updateType = EUpdateType.Update;

#if TOUCH_CONTROLS_KIT
        public enum EInputType { Standalone = 0, TouchControlsKit = 1 }
        [SerializeField]
        private EInputType inputType = EInputType.Standalone;
#endif

        [SerializeField]
        private UIPrefabs m_UIPrefabs = null;

        [SerializeField]
        private Axes axes = null;

        [SerializeField]
        private Actions actions = null;


        internal static bool GameIsPaused { get { return gameIsPaused; } }
        private static bool cursorIsBlocked = true, gameIsPaused = false;


        // Spawn UIElements 
        private void SpawnUIElements()
        {
            SpawnSingleUIElement( m_UIPrefabs.hudElements ).AwakeHUD();
            SpawnSingleUIElement( m_UIPrefabs.menuElements ).AwakeMENU();

#if TOUCH_CONTROLS_KIT
            SpawnSingleUIElement<TCKInput>( m_UIPrefabs.touchUIElements );
#endif

            if( FindObjectOfType<EventSystem>() == null )            
                new GameObject( "EventSystem", typeof( EventSystem ) );            
        }

        // Spawn SingleUIElement
        private static T SpawnSingleUIElement<T>( T prefab ) where T : MonoBehaviour
        {
            T[] lostPrafabs = FindObjectsOfType<T>();
            int lostSize = lostPrafabs.Length;

            if( lostSize > 1 )
                for( int i = 1; i < lostSize; i++ )
                    Destroy( lostPrafabs[ i ].gameObject );

            T curretElement = ( lostSize > 0 ) ? lostPrafabs[ 0 ] : null;
            if( curretElement == null )
            {
                if( prefab != null )
                    curretElement = prefab.SpawnCopy( Vector3.zero, Quaternion.identity );
                else
                    Debug.LogError( "Error: UI Prefab is not setup." );
            }

            return curretElement;
        }

        
        // Awake
        void Awake()
        {
            gameIsPaused = false;
            SpawnUIElements();

            InputSettings.BindAction( actions.jump, EActionEvent.Down, FirstPersonController.Jump );
            InputSettings.BindAction( actions.crouch, EActionEvent.Down, FirstPersonController.Crouch );
            //
            InputSettings.BindAction( actions.use, EActionEvent.Down, PlayerCamera.UseItem );
            //
            InputSettings.BindAction( actions.reloadWeapon, EActionEvent.Down, WeaponsManager.ReloadWeapon );
            InputSettings.BindAction( actions.nextFiremode, EActionEvent.Down, WeaponsManager.SwitchFiremode );
            InputSettings.BindAction( actions.nextAmmotype, EActionEvent.Down, WeaponsManager.SwitchAmmotype );
            InputSettings.BindAction( actions.toSubweapon, EActionEvent.Down, WeaponsManager.SwitchToSubWeapon );
            InputSettings.BindAction( actions.dropWeapon, EActionEvent.Down, WeaponsManager.DropCurrentWeapon );
            InputSettings.BindAction( actions.prevWeapon, EActionEvent.Down, WeaponsManager.SelectPreviousWeapon );
            InputSettings.BindAction( actions.nextWeapon, EActionEvent.Down, WeaponsManager.SelectNextWeapon );
            //
            InputSettings.BindAction( actions.blockCursor, EActionEvent.Down, BlockCursor );
            InputSettings.BindAction( actions.unblockCursor, EActionEvent.Down, UnblockCursor );

            if( Time.timeScale != 1f )
                Time.timeScale = 1f;
        }

        // Start
        void Start()
        {
            GameSettings.UpdateMixerVolumes();
            MenuElements.SetActive( false );
        }

        // OnDisable
        void OnDisable()
        {
            moveHorizontal = moveVertical = 0f;
            lookHorizontal = lookVertical = 0f;
            zoomAction = false;            
        }


        // Update
        void Update()
        {
            if( updateType == EUpdateType.Update )
                InputsUpdate();
        }
        // Late Update
        void LateUpdate()
        {
            if( updateType == EUpdateType.LateUpdate )
                InputsUpdate();
        }
        // Fixed Update
        void FixedUpdate()
        {
            if( updateType == EUpdateType.FixedUpdate )
                InputsUpdate();
        }


        // Inputs Update
        private void InputsUpdate()
        {
            if( gameIsPaused )
                return;

#if TOUCH_CONTROLS_KIT
            if( inputType == EInputType.TouchControlsKit )
                TouchKitInput();
            else
#endif
                StandaloneInput();
        }

        // Standalone Input
        private void StandaloneInput()
        {
            if( InputSettings.GetAction( actions.pause, EActionEvent.Down ) )
                Pause();
            
            InputSettings.RunActions();
            InputSettings.RunActionAxis();
            InputSettings.RunAxis();            


            // Cursor lock
            if( cursorIsBlocked && Time.timeSinceLevelLoad > .1f )
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }


            moveHorizontal = InputSettings.GetAxis( axes.moveX );
            moveVertical = InputSettings.GetAxis( axes.moveY );
            lookHorizontal = InputSettings.GetAxis( axes.lookX ) * GameSettings.GetLookSensitivityByInvert_X;
            lookVertical = InputSettings.GetAxis( axes.lookY ) * GameSettings.GetLookSensitivityByInvert_Y;

            runAction = InputSettings.GetAction( actions.run, EActionEvent.Press );

            zoomAction = InputSettings.GetAction( actions.zoom, EActionEvent.Press );
            zoomActionDown = InputSettings.GetAction( actions.zoom, EActionEvent.Down );
            zoomActionUp = InputSettings.GetAction( actions.zoom, EActionEvent.Up );

            bool fireAction = InputSettings.GetAction( actions.fire, EActionEvent.Press );
            // Fire and Reset Weapon
            if( fireAction && !FirstPersonController.isRunning )
                WeaponsManager.WeaponFire();
            else
                WeaponsManager.WeaponReset();

            
            // Select Weapon ByIndex
            if( Input.GetKeyDown( KeyCode.Alpha1 ) )
                WeaponsManager.SelectWeaponByIndex( 0 );
            else if( Input.GetKeyDown( KeyCode.Alpha2 ) )
                WeaponsManager.SelectWeaponByIndex( 1 );
            else if( Input.GetKeyDown( KeyCode.Alpha3 ) )
                WeaponsManager.SelectWeaponByIndex( 2 );
            else if( Input.GetKeyDown( KeyCode.Alpha4 ) )
                WeaponsManager.SelectWeaponByIndex( 3 );
            else if( Input.GetKeyDown( KeyCode.Alpha5 ) )
                WeaponsManager.SelectWeaponByIndex( 4 );
            else if( Input.GetKeyDown( KeyCode.Alpha6 ) )
                WeaponsManager.SelectWeaponByIndex( 5 );
            else if( Input.GetKeyDown( KeyCode.Alpha7 ) )
                WeaponsManager.SelectWeaponByIndex( 6 );
            else if( Input.GetKeyDown( KeyCode.Alpha8 ) )
                WeaponsManager.SelectWeaponByIndex( 7 );
            else if( Input.GetKeyDown( KeyCode.Alpha9 ) )
                WeaponsManager.SelectWeaponByIndex( 8 );
        }

#if TOUCH_CONTROLS_KIT
        // TouchKit Input
        private void TouchKitInput()
        {
            if( TCKInput.CheckController( actions.pause ) && TCKInput.GetAction( actions.pause, TCKActionEvent.Down ) )
                Pause();

            runAction = ( TCKInput.CheckController( actions.run ) && TCKInput.GetAction( actions.run, TCKActionEvent.Press ) );

            if( TCKInput.CheckController( actions.jump ) && TCKInput.GetAction( actions.jump, TCKActionEvent.Down ) )
                FirstPersonController.Jump();
            if( TCKInput.CheckController( actions.crouch ) && TCKInput.GetAction( actions.crouch, TCKActionEvent.Down ) )
                FirstPersonController.Crouch();
            
            if( TCKInput.CheckController( actions.use ) && TCKInput.GetAction( actions.use, TCKActionEvent.Down ) )
                PlayerCamera.UseItem();    
                    
            if( TCKInput.CheckController( actions.reloadWeapon ) && TCKInput.GetAction( actions.reloadWeapon, TCKActionEvent.Down ) )
                WeaponsManager.ReloadWeapon();
            if( TCKInput.CheckController( actions.nextFiremode ) && TCKInput.GetAction( actions.nextFiremode, TCKActionEvent.Down ) )
                WeaponsManager.SwitchFiremode();
            if( TCKInput.CheckController( actions.nextAmmotype ) && TCKInput.GetAction( actions.nextAmmotype, TCKActionEvent.Down ) )
                WeaponsManager.SwitchAmmotype();
            if( TCKInput.CheckController( actions.toSubweapon ) && TCKInput.GetAction( actions.toSubweapon, TCKActionEvent.Down ) )
                WeaponsManager.SwitchToSubWeapon();
            if( TCKInput.CheckController( actions.dropWeapon ) && TCKInput.GetAction( actions.dropWeapon, TCKActionEvent.Down ) )
                WeaponsManager.DropCurrentWeapon();
            if( TCKInput.CheckController( actions.prevWeapon ) && TCKInput.GetAction( actions.prevWeapon, TCKActionEvent.Down ) )
                WeaponsManager.SelectPreviousWeapon();
            if( TCKInput.CheckController( actions.nextWeapon ) && TCKInput.GetAction( actions.nextWeapon, TCKActionEvent.Down ) )
                WeaponsManager.SelectNextWeapon();


            if( TCKInput.CheckController( axes.moveJoystick ) )
            {
                moveHorizontal = Mathf.Clamp( TCKInput.GetAxis( axes.moveJoystick, TCKAxisType.Horizontal ), -1f, 1f );
                moveVertical = runAction ? 1f : Mathf.Clamp( TCKInput.GetAxis( axes.moveJoystick, TCKAxisType.Vertical ), -1f, 1f );
            }

            if( TCKInput.CheckController( axes.lookTouchpad ) )
            {
                lookHorizontal = TCKInput.GetAxis( axes.lookTouchpad, TCKAxisType.Horizontal ) * GameSettings.GetLookSensitivityByInvert_X;
                lookVertical = TCKInput.GetAxis( axes.lookTouchpad, TCKAxisType.Vertical ) * GameSettings.GetLookSensitivityByInvert_Y;
            }

            if( TCKInput.CheckController( actions.zoom ) )
            {
                zoomAction = TCKInput.GetAction( actions.zoom, TCKActionEvent.Press );
                zoomActionDown = TCKInput.GetAction( actions.zoom, TCKActionEvent.Down );
                zoomActionUp = TCKInput.GetAction( actions.zoom, TCKActionEvent.Up );
            }

            if( TCKInput.CheckController( actions.fire ) )
            {
                bool fireAction = TCKInput.GetAction( actions.fire, TCKActionEvent.Press );
                // Fire and Reset Weapon
                if( fireAction && !FirstPersonController.isRunning )
                    WeaponsManager.WeaponFire();
                else
                    WeaponsManager.WeaponReset();
            }
        }
#endif

        // Bind Action
        public static void BindAction( string m_Name, EActionEvent m_Event, ActionHandler m_Handler )
        {
            InputSettings.BindAction( m_Name, m_Event, m_Handler );
        }
        // Unbind Action
        public static void UnbindAction( string m_Name, EActionEvent m_Event, ActionHandler m_Handler )
        {
            InputSettings.UnbindAction( m_Name, m_Event, m_Handler );
        }

        // Bind ActionAxis
        public static void BindActionAxis( string m_Name, EAxisState m_State, ActionHandler m_Handler )
        {
            InputSettings.BindActionAxis( m_Name, m_State, m_Handler );
        }
        // Unbind ActionAxis
        public static void UnbindActionAxis( string m_Name, EAxisState m_State, ActionHandler m_Handler )
        {
            InputSettings.UnbindActionAxis( m_Name, m_State, m_Handler );
        }

        // Bind Axis
        public static void BindAxis( string m_Name, AxisHandler m_Handler )
        {
            InputSettings.BindAxis( m_Name, m_Handler );
        }
        // Unbind Axis
        public static void UnbindAxis( string m_Name, AxisHandler m_Handler )
        {
            InputSettings.UnbindAxis( m_Name, m_Handler );
        }        
        

        // Get Action
        public static bool GetAction( string m_Name, EActionEvent m_Event )
        {
            return InputSettings.GetAction( m_Name, m_Event );
        }

        // Get ActionAxis
        public static bool GetActionAxis( string m_Name, EAxisState m_State )
        {
            return InputSettings.GetActionAxis( m_Name, m_State );
        }

        // Get Axis
        public static float GetAxis( string m_Name )
        {
            return InputSettings.GetAxis( m_Name );
        }


        // Block Cursor
        public static void BlockCursor()
        {
            cursorIsBlocked = true;
        }
        // Unblock Cursor
        public static void UnblockCursor()
        {
            cursorIsBlocked = false;

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        // Pause
        public static void Pause()
        {
            if( !PlayerCharacter.Instance.isAlive )
                return;

            gameIsPaused = !gameIsPaused;
            Time.timeScale = gameIsPaused ? 0f : 1f;
            MenuElements.SetActive( gameIsPaused );
            //
            if( GameSettings.ShowHud )
                HudElements.SetActive( !gameIsPaused );

#if TOUCH_CONTROLS_KIT
            TCKInput.SetActive( !gameIsPaused );
#endif
        }

        // PlayerDie
        internal static void PlayerDie()
        {
            MenuElements.SetActive( true );

#if TOUCH_CONTROLS_KIT
            TCKInput.SetActive( false );
#endif
        }


        // move Horizontal 
        internal static float moveHorizontal { get; private set; }
        // move Vertical 
        internal static float moveVertical { get; private set; }

        // look Horizontal 
        internal static float lookHorizontal { get; private set; }
        // look Vertical 
        internal static float lookVertical { get; private set; }

        // run Action 
        internal static bool runAction { get; private set; }

        // zoom Action 
        internal static bool zoomAction { get; private set; }
        // zoom ActionDown
        internal static bool zoomActionDown { get; private set; }
        // zoom ActionUp
        internal static bool zoomActionUp { get; private set; }
    }
}