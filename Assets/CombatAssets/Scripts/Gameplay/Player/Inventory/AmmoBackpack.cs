/********************************************
 * Copyright(c): 2016 Victor Klepikov       *
 *                                          *
 * Site: 	     http://u3d.as/5Fb		    *
 * Support: 	 http://bit.ly/vk-Support   *
 ********************************************/


using UnityEngine;


namespace AdvancedShooterKit
{
    public class AmmoBackpack : MonoBehaviour
    {
        [System.Serializable]
        public struct Ammunition
        {
            public int currentAmmo;
            public int maxAmmo;            
            public Sprite hudIcon;
        };

        public Ammunition[] ammunition = null;


        // instance
        private static AmmoBackpack instance = null;
        // m_Instance
        private static AmmoBackpack m_Instance
        {
            get
            {
                if( instance == null )
                    instance = FindObjectOfType<AmmoBackpack>();

                return instance;
            }
        }


        // Ammunition array length
        public static int size { get { return m_Instance.ammunition.Length; } }

        // IsFull
        public static bool IsFull( int index )
        {
            if( index >= 0 && index < size )
            {
                return m_Instance.ammunition[ index ].currentAmmo >= m_Instance.ammunition[ index ].maxAmmo;
            }
            else
            {
                Debug.LogError( "IsFull ERROR: Array index out of range." );
                return true;
            }
        }
        // IsEmpty
        public static bool IsEmpty( int index )
        {
            if( index >= 0 && index < size )
            {
                return m_Instance.ammunition[ index ].currentAmmo <= 0;
            }
            else
            {
                Debug.LogError( "IsEmpty ERROR: Array index out of range." );
                return false;
            }
        }

        // Add Ammo
        public static bool AddAmmo( int index, ref int addАmount )
        {
            if( ( index < 0 ) || ( index >= size ) )
            {
                Debug.LogError( "AddAmmo ERROR: Array index out of range." );
                return false;
            }

            int currentAmmo = GetCurrentAmmo( index );
            int maxAmmo = GetMaxAmmo( index );

            if( currentAmmo >= maxAmmo )
                return false;

            //
            int missingAmmo = Mathf.Max( 0, maxAmmo - currentAmmo );
            currentAmmo += Mathf.Min( Mathf.Max( 0, addАmount ), missingAmmo );
            addАmount = Mathf.Max( 0, addАmount -= missingAmmo );
            SetCurrentAmmo( index, currentAmmo );

            WeaponsManager.UpdateHud();
            return true;
        }

        // Get CurrentAmmo
        public static int GetCurrentAmmo( int index )
        {
            if( index >= 0 && index < size )
            {
                return m_Instance.ammunition[ index ].currentAmmo;
            }
            else
            {
                Debug.LogError( "GetCurrentAmmo ERROR: Array index out of range." );
                return 0;
            }            
        }
        // Set CurrentAmmo
        public static void SetCurrentAmmo( int index, int count )
        {
            if( index >= 0 && index < size )
            {
                count = Mathf.Min( GetMaxAmmo( index ), Mathf.Max( 0, count ) );
                m_Instance.ammunition[ index ].currentAmmo = count;
            }
            else
            {
                Debug.LogError( "SetCurrentAmmo ERROR: Array index out of range." );
            }            
        }

        // Get MaxAmmo
        public static int GetMaxAmmo( int index )
        {
            if( index >= 0 && index < size )
            {
                return m_Instance.ammunition[ index ].maxAmmo;
            }
            else
            {
                Debug.LogError( "GetMaxAmmo ERROR: Array index out of range." );
                return 0;
            }
        }
        // Set MaxAmmo
        public static void SetMaxAmmo( int index, int count )
        {
            if( index >= 0 && index < size )            
                m_Instance.ammunition[ index ].maxAmmo = Mathf.Max( 0, count );            
            else
                Debug.LogError( "SetMaxAmmo ERROR: Array index out of range." );            
        }

        // Get HudIcon
        public static Sprite GetHudIcon( int index )
        {
            if( index >= 0 && index < size )
            {
                return m_Instance.ammunition[ index ].hudIcon;
            }
            else
            {
                Debug.LogError( "GetHudIcon ERROR: Array index out of range." );
                return null;
            }
        }
        // Set HudIcon
        public static void SetHudIcon( int index, Sprite icon )
        {
            if( index >= 0 && index < size )
                m_Instance.ammunition[ index ].hudIcon = icon;
            else
                Debug.LogError( "SetHudIcon ERROR: Array index out of range." );
        }
    };
}