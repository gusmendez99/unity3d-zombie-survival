/********************************************
 * Copyright(c): 2016 Victor Klepikov       *
 *                                          *
 * Site: 	     http://u3d.as/5Fb		    *
 * Support: 	 http://bit.ly/vk-Support   *
 ********************************************/


using UnityEngine.Events;


namespace AdvancedShooterKit.Events
{
    public delegate void ActionHandler();
    public delegate void AxisHandler( float value );
    //
    [System.Serializable] public class ASKEvent : UnityEvent { }
}