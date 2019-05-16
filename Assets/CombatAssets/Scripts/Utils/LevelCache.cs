/********************************************
 * Copyright(c): 2016 Victor Klepikov       *
 *                                          *
 * Site: 	     http://u3d.as/5Fb		    *
 * Support: 	 http://bit.ly/vk-Support   *
 ********************************************/


using UnityEngine;


namespace AdvancedShooterKit.Utils
{
    public static class LevelCache
    {
        private static Transform cache = null;
        private static Transform Cache
        {
            get
            {
                if( cache == null )
                    cache = new GameObject( "ASK_LevelCache" ).transform;

                return cache;
            }
        }


        // Move ToCache
        public static T MoveToCache<T>( this T obj ) where T : Component
        {
            obj.transform.SetParent( Cache );
            return obj;
        }

        // Move ToCache
        public static GameObject MoveToCache( this GameObject obj )
        {
            obj.transform.SetParent( Cache );
            return obj;
        }
    };
}