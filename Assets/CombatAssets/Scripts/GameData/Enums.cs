/********************************************
 * Copyright(c): 2016 Victor Klepikov       *
 *                                          *
 * Site: 	     http://u3d.as/5Fb		    *
 * Support: 	 http://bit.ly/vk-Support   *
 ********************************************/


namespace AdvancedShooterKit
{
    // Difficulty Levels for gaymplay setups.
    public enum EDifficultyLevel
    {
        Easy = 0,
        Normal = 1,
        Hard = 2,
        Delta = 3,
        Extreme = 4
    };

    // Using for sets Crosshair View in "FirstPersonWeaponSway.cs"
    public enum ECrosshairView
    {
        None = 0,
        OnlyPoint = 1,
        OnlyCross = 2,
        All = 3
    };

    // Using for sets Crosshair Mode
    public enum ECrosshairMode
    {
        None = 0, 
        Point = 1, 
        Cancel = 2, 
        Hand = 3, 
        Swap = 4, 
        Ammo = 5, 
        Health = 6 
    };

    // Using for sets Crosshair Color
    public enum ECrosshairColor
    {
        Normal = 0,
        Damager = 1
    };

    // Show Damege Indicator mode.
    public enum EDamageIndication
    {
        OnlyCharacters = 0,
        ForAll = 1,
        OFF = 2
    };

    // Using for sets weapon type in "WeaponsManager.cs"
    public enum EWeaponType 
    { 
        Standart = 0, 
        Keep = 1, 
        Thrown = 2 
    };

    // Using for sets firing modes in weapon
    public enum EFiringMode 
    { 
        Automatic = 0, 
        Single = 1, 
        Double = 2, 
        Triple = 3 
    };

    // Using for sets ironsighting modes of first person
    public enum EIronsightingMode
    {
        Press = 0,
        Click = 1,
        Mixed = 2
    };

    // Using for sets Armor hardnes in "Damager.cs"
    public enum EArmorType 
    { 
        None = 0, 
        Lite = 1, 
        Medium = 2, 
        Heavy = 3, 
        Ultra = 4 
    };

    // Unsing for read/write damege data in combat
    public enum EDamageType
    {
        Unknown = 0,
        Impact = 1,
        Melee = 2,
        Explosion = 3
    };

    // Projectile Type
    public enum EProjectileType
    {
        Simple = 0,
        Ballistic = 1
    };

    // Projectile SubType
    public enum EProjectileSubType
    {
        Bullet = 0,
        Arrow = 1,
        Throw = 2,
        Rocket = 3
    };
    
    // Using for sets Pickup Type
    public enum EPickupType
    {
        Health = 0,
        Melee = 1,
        Firearms = 2,
        Ammo = 3,
        Thrown = 4
    };

    // Spawn Mode
    public enum ESpawnMode
    {
        StartPosition = 0,
        SamePosition = 1
    };

    // Using for "ASKInputManager.cs"
    public enum EUpdateType 
    { 
        Update = 0, 
        LateUpdate = 2, 
        FixedUpdate = 3,
        OFF = 4
    };

    // Using for "ASKInputManager.cs"
    public enum EActionEvent 
    { 
        Down = 0, 
        Press = 1, 
        Up = 2 
    };

    // Using for "ASKInputManager.cs"
    public enum EAxisType
    {
        Unity = 0,
        Custom = 1,
        Mixed = 2
    };

    // Using for "ASKInputManager.cs"
    public enum EActionType
    {
        KeyCode = 0,
        Axis = 1,        
        Mixed = 2
    };

    // Using for "ASKInputManager.cs"
    public enum EAxisState
    {
        NONE = 0,
        PositiveDown = 1, PositivePress = 2, PositiveUp = 3,
        NegativeDown = 4, NegativePress = 5, NegativeUp = 6
    };

    // Using for "ASKInputManager.cs"
    public enum EAxisStateClamp
    {
        NotClamped = 0,
        OnlyPositive = 1,
        OnlyNegative = 2
    };

    // Using for "ASKInputManager.cs"
    public enum EAxisSource
    {
        InputManager = 0,
        NativeInput = 1
    };

    // Using for resetAngles in "Rocket as projectile sub type" for reset noice in fly
    public enum EResetModes 
    { 
        UpdateStart = 0, 
        UpdateEnd = 1 
    };
    
    // Setter VolumeType
    public enum EVolumeType
    {
        Master = 0,
        Music = 1,
        SFX = 2,
        Voice = 3
    };
}
