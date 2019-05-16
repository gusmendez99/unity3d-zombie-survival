/********************************************
 * Copyright(c): 2016 Victor Klepikov       *
 *                                          *
 * Site: 	     http://u3d.as/5Fb		    *
 * Support: 	 http://bit.ly/vk-Support   *
 ********************************************/


using UnityEngine;
using UnityEngine.UI;


namespace AdvancedShooterKit
{
    public class DropdownElement : MonoBehaviour
    {
        public enum EEnumType { DamageIndication = 0, DifficultyLevel = 1 }
        [SerializeField]
        private EEnumType enumType = EEnumType.DamageIndication;

        [SerializeField]
        private Button dropdownButton = null;
        [SerializeField]
        private RectTransform dropdownPanel = null;

        private Text m_Text = null;


        // Start
        void Start()
        {
            string[] names = null;
            string currentName = null;
            
            if( enumType == EEnumType.DamageIndication )
            {
                names = System.Enum.GetNames( typeof( EDamageIndication ) );
                currentName = GameSettings.DamageIndication.ToString();
            }
            else
            {
                names = System.Enum.GetNames( typeof( EDifficultyLevel ) );
                currentName = GameSettings.DifficultyLevel.ToString();
            }

            m_Text = this.GetComponentInChildren<Text>();
            m_Text.text = currentName;

            for( int i = 0; i < names.Length; i++ )
            {
                int mIndex = i;
                string mName = names[ mIndex ];                

                Button tmpBtn = Instantiate( dropdownButton );
                tmpBtn.GetComponentInChildren<Text>().text = mName;
                tmpBtn.transform.SetParent( dropdownPanel );                
                tmpBtn.onClick.AddListener( () => { OnButtonClick( mIndex, mName ); } );
            }
        }


        // OnButton Click
        private void OnButtonClick( int eIndex, string eName )
        {
            if( enumType == EEnumType.DamageIndication )            
                GameSettings.DamageIndication = ( EDamageIndication )eIndex;            
            else            
                GameSettings.DifficultyLevel = ( EDifficultyLevel )eIndex;            

            m_Text.text = eName;
            SwitchActive();
        }

        // Switch Active
        public void SwitchActive()
        {
            GameObject panelGo = dropdownPanel.gameObject;
            panelGo.SetActive( !panelGo.activeSelf );
        }
    }
}