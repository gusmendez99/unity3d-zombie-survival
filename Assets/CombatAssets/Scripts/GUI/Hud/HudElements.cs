/********************************************
 * Copyright(c): 2016 Victor Klepikov       *
 *                                          *
 * Site: 	     http://u3d.as/5Fb		    *
 * Support: 	 http://bit.ly/vk-Support   *
 ********************************************/


using UnityEngine;
using UnityEngine.UI;
using System.Collections;


namespace AdvancedShooterKit
{
    public class HudElements : MonoBehaviour
    {
        [System.Serializable]
        public class HealthBar
        {
            public RectTransform healthPanel = null;
            public Text healthText = null;
            
            public Image painScreen, painPointer;

            [Range( 25f, 250f )]
            public float pointerDistance = 175f;

            [Range( .1f, 4f )]
            public float painClearDelay = 1.75f;

            public Color32 painColor = Color.white;

            internal Vector3 damageTargetPosition { get; private set; }

            // SetDamage TargetPosition
            // Use Vector3.zero for hide painPointer and show only painScreen
            internal static void SetDamageTargetPosition( Vector3 targetPosition )
            {
                m_Instance.damageTargetPosition = targetPosition;
            }
            
            private Vector2 sizeDelta = Vector2.zero;
            private float starterX = 0f;

            private static HealthBar m_Instance = null;


            // Awake
            internal void Awake()
            {
                m_Instance = this;
                sizeDelta = healthPanel.sizeDelta;
                starterX = sizeDelta.x;
                painScreen.color = Color.clear;
                painPointer.color = Color.clear;
            }

            // Update Bar
            internal static void UpdateBar( int currentHealth, int maxHealth )
            {
                m_Instance.sizeDelta.x = ( currentHealth * m_Instance.starterX ) / maxHealth;
                m_Instance.healthText.text = currentHealth.ToString();
                m_Instance.healthPanel.sizeDelta = m_Instance.sizeDelta;
            }

            // Set Active
            internal static void SetActive( bool value )
            {
                m_Instance.healthPanel.gameObject.SetActive( value );
                m_Instance.healthText.gameObject.SetActive( value );
            }
        }
        
        [System.Serializable]
        public class WeaponInformer
        {
            public Text allAmmoText, currentAmmoText;
            public Image ammoIcon, shootingModeIcon;
            public Sprite SMAutomatic, SMSingle, SMDouble, SMTriple;

            private static WeaponInformer m_Instance = null;

            // Awake
            internal void Awake()
            {
                m_Instance = this;
            }


            // Update AmmoIcon
            internal static void UpdateAmmoIcon( Sprite icon )
            {
                m_Instance.ammoIcon.sprite = icon;
            }

            // Update CurrentAmmoInfo
            internal static void UpdateCurrentAmmoInfo( int value )
            {
                m_Instance.currentAmmoText.text = value.ToString();
            }

            // Update AllAmmoInfo
            internal static void UpdateAllAmmoInfo( int value )
            {
                m_Instance.allAmmoText.text = value.ToString();
            }

            // Update ShootingModeIcon
            internal static void UpdateShootingModeIcon( EFiringMode value )
            {
                Sprite newIcon = null;

                switch( value )
                {
                    case EFiringMode.Automatic:
                        newIcon = m_Instance.SMAutomatic;
                        break;

                    case EFiringMode.Single:
                        newIcon = m_Instance.SMSingle;
                        break;

                    case EFiringMode.Double:
                        newIcon = m_Instance.SMDouble;
                        break;

                    case EFiringMode.Triple:
                        newIcon = m_Instance.SMTriple;
                        break;

                    default:
                        newIcon = m_Instance.SMAutomatic;
                        break;
                }

                m_Instance.shootingModeIcon.sprite = newIcon;
            }

            // Set Active
            internal static void SetActive( bool value )
            {
                m_Instance.ammoIcon.gameObject.SetActive( value );
                m_Instance.currentAmmoText.gameObject.SetActive( value );
                m_Instance.allAmmoText.gameObject.SetActive( value );
                m_Instance.shootingModeIcon.gameObject.SetActive( value );
            }
        }
        
        [System.Serializable]
        public class Crosshair
        {
            public Color32 normalColor = Color.white;
            public Color32 damagerColor = Color.yellow;
            public Color32 onDamageColor = Color.red;

            internal static ECrosshairColor currentColorType { get; private set; }

            public Sprite pointIcon, cancelIcon, handIcon, swapIcon, ammoIcon, healthIcon;
            public RectTransform pointRT, upRT, downRT, leftRT, rightRT; // t is Transform/RectTransform
                                                                         //UP is y or Vector3.forward * value //DOWN is -y or Vector3.back * value //LEFT is -x or Vector3.left * value //RIGHT is x or Vector3.right * value            

            public Image damageIndicator = null;
            private Image pointImg, upImg, downImg, leftImg, rightImg;

            private static Crosshair m_Instance = null;


            // Awake
            internal void Awake()
            {
                m_Instance = this;                

                pointImg = pointRT.GetComponent<Image>();
                pointImg.sprite = pointIcon;

                upImg = upRT.GetComponent<Image>();
                downImg = downRT.GetComponent<Image>();
                leftImg = leftRT.GetComponent<Image>();
                rightImg = rightRT.GetComponent<Image>();

                damageIndicator.color = Color.clear;
                SetColor( ECrosshairColor.Normal );
            }


            internal static void SetColor( ECrosshairColor colorType )
            {
                Color32 newColor = Color.clear;

                switch( colorType )
                {
                    case ECrosshairColor.Normal:
                        newColor = m_Instance.normalColor;
                        break;

                    case ECrosshairColor.Damager:
                        newColor = m_Instance.damagerColor;
                        break;

                    default:
                        newColor = m_Instance.normalColor;
                        break;
                }

                currentColorType = colorType;
                m_Instance.pointImg.color = m_Instance.upImg.color = m_Instance.downImg.color = m_Instance.leftImg.color = m_Instance.rightImg.color = newColor;
            }


            // Set PointSprite
            internal static void SetPointSprite( ECrosshairMode mode )
            {
                if( mode == ECrosshairMode.Point )
                {
                    m_Instance.pointRT.sizeDelta = Vector2.one * 2f;
                    WeaponsManager.UpdateHud( true );
                }
                else if( mode == ECrosshairMode.None )
                {
                    m_Instance.pointRT.sizeDelta = Vector2.zero;
                    WeaponsManager.UpdateHud( true );
                }
                else
                {
                    m_Instance.pointRT.sizeDelta = Vector2.one * 64f;
                    SetActive( ECrosshairView.OnlyPoint );
                }

                switch( mode )
                {
                    case ECrosshairMode.None:
                    case ECrosshairMode.Point:
                        m_Instance.pointImg.sprite = m_Instance.pointIcon; 
                        break;
                    case ECrosshairMode.Cancel:
                        m_Instance.pointImg.sprite = m_Instance.cancelIcon; 
                        break;
                    case ECrosshairMode.Hand:
                        m_Instance.pointImg.sprite = m_Instance.handIcon;
                        break;
                    case ECrosshairMode.Swap:
                        m_Instance.pointImg.sprite = m_Instance.swapIcon;
                        break;
                    case ECrosshairMode.Ammo:
                        m_Instance.pointImg.sprite = m_Instance.ammoIcon; 
                        break;
                    case ECrosshairMode.Health:
                        m_Instance.pointImg.sprite = m_Instance.healthIcon;
                        break;                  

                    default: 
                        break;
                }
            }

            // Update Position
            internal static void UpdatePosition( float value )
            {
                if( !isActive )
                    return;

                value /= 2f;
                m_Instance.upRT.anchoredPosition3D = Vector3.up * value;
                m_Instance.downRT.anchoredPosition3D = Vector3.down * value;
                m_Instance.leftRT.anchoredPosition3D = Vector3.left * value;
                m_Instance.rightRT.anchoredPosition3D = Vector3.right * value;
            }

            // Set Active
            internal static void SetActive( ECrosshairView view )
            {
                bool pointActive = false, upActive = false, downActive = false, leftActive = false, rightActive = false;

                switch( view )
                {
                    case ECrosshairView.None:
                        break;

                    case ECrosshairView.OnlyPoint:
                        pointActive = true;                        
                        break;

                    case ECrosshairView.OnlyCross:
                        upActive = downActive = leftActive = rightActive = true;
                        break;

                    case ECrosshairView.All:
                        pointActive = upActive = downActive = leftActive = rightActive = true;
                        break;
                }

                m_Instance.pointRT.gameObject.SetActive( pointActive );
                m_Instance.upRT.gameObject.SetActive( upActive );
                m_Instance.downRT.gameObject.SetActive( downActive );
                m_Instance.leftRT.gameObject.SetActive( leftActive );
                m_Instance.rightRT.gameObject.SetActive( rightActive );
            }
        }
                
        [SerializeField]
        private HealthBar healthBar = new HealthBar();
        [SerializeField]
        private Crosshair crosshair = new Crosshair();
        [SerializeField]
        private WeaponInformer weaponInformer = new WeaponInformer();

        [SerializeField]
        private Transform rotationElements = null;

        private static HudElements m_Instance = null;
        private static GameObject cameraObj = null;

        private Vector3 nativePosition, nativeRotation;


        // Awake
        internal void AwakeHUD()
        {
            m_Instance = this;
            cameraObj = this.GetComponentInChildren<Camera>().gameObject;
            //
            healthBar.Awake();
            crosshair.Awake();
            weaponInformer.Awake();
            //
            nativePosition = rotationElements.localPosition;
            nativeRotation = rotationElements.localEulerAngles;
            //
            SetActive( GameSettings.ShowHud );
        }

        // Update
        void Update()
        {
            float smooth = Time.smoothDeltaTime * 15f;
            Vector3 targetValues = Vector3.zero;
            //
            targetValues.x = nativeRotation.x - CameraHeadBob.xTilt * 1.25f;
            targetValues.y = nativeRotation.y + CameraHeadBob.yTilt * 3f;
            targetValues.z = nativeRotation.z;
            rotationElements.localRotation = Quaternion.Slerp( rotationElements.localRotation, Quaternion.Euler( targetValues ), smooth );
            //
            targetValues.x = nativePosition.x - CameraHeadBob.xPos * 100f;
            targetValues.y = nativePosition.y - CameraHeadBob.yPos * 125f;
            targetValues.z = nativePosition.z;
            rotationElements.localPosition = Vector3.Lerp( rotationElements.localPosition, targetValues, smooth );
        }
        

        // SetActive
        public static void SetActive( bool value )
        {
            isActive = value;

            if( cameraObj != null )
                cameraObj.SetActive( value );            
        }
        // IsActive
        public static bool isActive { get; private set; }


        // Show PainScreen
        public static void ShowPainScreen()
        {
            if( !PlayerCharacter.Instance.isAlive )
                return;

            m_Instance.StopCoroutine( "ClearPainScreen" );
            m_Instance.healthBar.painScreen.color = Color.clear;
            m_Instance.healthBar.painPointer.color = Color.clear;
            m_Instance.StartCoroutine( "ClearPainScreen" );
        }

        // Clear PainScreen
        private IEnumerator ClearPainScreen()
        {
            Color tmpColor = healthBar.painColor;
            healthBar.painScreen.color = tmpColor;

            if( healthBar.damageTargetPosition != Vector3.zero )
            {
                healthBar.painPointer.color = tmpColor;

                Transform playerTransform = PlayerCharacter.Instance.m_Transform;

                float dx = playerTransform.position.x - healthBar.damageTargetPosition.x;
                float dz = playerTransform.position.z - healthBar.damageTargetPosition.z;

                float deltay = Mathf.Atan2( dx, dz ) * Mathf.Rad2Deg - 270f - playerTransform.eulerAngles.y;

                float pX = healthBar.pointerDistance * Mathf.Cos( deltay * Mathf.Deg2Rad );
                float pY = healthBar.pointerDistance * Mathf.Sin( deltay * Mathf.Deg2Rad );

                Vector2 newPointerPosition = new Vector2( pX, -pY );
                float angle = Mathf.Atan2( newPointerPosition.y, newPointerPosition.x ) * Mathf.Rad2Deg - 90f;

                RectTransform pointerRect = healthBar.painPointer.rectTransform;
                pointerRect.anchoredPosition = newPointerPosition;
                pointerRect.rotation = Quaternion.AngleAxis( angle, Vector3.forward );
            }

            float elapsed = 0f;
            while( elapsed < healthBar.painClearDelay )
            {
                elapsed += Time.deltaTime;
                yield return null;
            }

            while( tmpColor.a > 0f )
            {
                tmpColor.a -= Time.deltaTime;
                healthBar.painScreen.color = tmpColor;
                healthBar.painPointer.color = tmpColor;
                yield return null;
            }
        }
        

        // Show DamegeIndicator
        public static void ShowDamegeIndicator()
        {
            if( !PlayerCharacter.Instance.isAlive )
                return;

            m_Instance.StopCoroutine( "ClearDamageIndicator" );
            m_Instance.crosshair.damageIndicator.color = Color.clear;
            m_Instance.StartCoroutine( "ClearDamageIndicator" );
        }

        // Clear DamageIndicator
        private IEnumerator ClearDamageIndicator()
        {
            Color tmpColor = crosshair.onDamageColor;
            crosshair.damageIndicator.color = tmpColor;

            while( tmpColor.a > 0f )
            {
                tmpColor.a -= Time.deltaTime;
                crosshair.damageIndicator.color = tmpColor;
                yield return null;
            }
        }


        // Player Die
        internal static void PlayerDie()
        {
            m_Instance.StopCoroutine( "ClearPainScreen" );
            m_Instance.StopCoroutine( "ClearDamageIndicator" );
            m_Instance.healthBar.painScreen.color = m_Instance.healthBar.painColor;
            Crosshair.SetActive( ECrosshairView.None );
            Crosshair.SetColor( ECrosshairColor.Normal );
            WeaponInformer.SetActive( false );
            HealthBar.SetActive( false );
        }
    }
}