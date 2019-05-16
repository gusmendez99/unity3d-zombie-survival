/********************************************
 * Copyright(c): 2016 Victor Klepikov       *
 *                                          *
 * Site: 	     http://u3d.as/5Fb		    *
 * Support: 	 http://bit.ly/vk-Support   *
 ********************************************/


using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

using UObject = UnityEngine.Object;


namespace AdvancedShooterKit.Utils
{
    public static class EventHandler
    {
        // Send
        public static T Send<T>( this T obj, string methodName, params object[] parameters ) where T : Component
        {
            Send( obj.gameObject, methodName, parameters );
            return obj;
        }
        // Send
        public static GameObject Send( this GameObject gameObject, string methodName, params object[] parameters )
        {
            MonoBehaviour[] components = gameObject.GetComponents<MonoBehaviour>();

            foreach( MonoBehaviour comp in components )
            {
                MethodInfo[] methodsInfo = comp.GetMethods();

                foreach( MethodInfo method in methodsInfo )
                    if( method.Name == methodName )
                    {
                        ParameterInfo[] parametersInfo = method.GetParameters();

                        if( parametersInfo.Length == parameters.Length )
                        {
                            method.Invoke( comp, parameters );
                            break;
                        }
                    }
            }

            return gameObject;
        }

        // GetMethods
        private static MethodInfo[] GetMethods( this MonoBehaviour obj )
        {
            return obj
                .GetType()
                .GetMethods( BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic );
        }
    };




    public static class ComponentExtensions
    {
        private sealed class CoroutineLoopData
        {
            public Action call = null;
            public IEnumerator theEnum = null;
            public MonoBehaviour copyObj = null;

            public CoroutineLoopData( Action call, IEnumerator theEnum, MonoBehaviour copyObj )
            {
                this.call = call;
                this.theEnum = theEnum;
                this.copyObj = copyObj;
            }

            public static CoroutineLoopData StartNew( Action call, IEnumerator theEnum, MonoBehaviour copyObj )
            {
                var newObj = new CoroutineLoopData( call, theEnum, copyObj );
                copyObj.StartCoroutine( theEnum );
                return newObj;
            }
        };

        private static List<CoroutineLoopData> corLoopList = new List<CoroutineLoopData>();

        

        // Destroy GameObject
        public static T DestroyGameObject<T>( this T obj, float delay ) where T : Component
        {
            UObject.Destroy( obj.gameObject, delay );
            return obj;
        }

        // Destroy GameObject
        public static T DestroyGameObject<T>( this T obj, float delay, Action OnDestroy ) where T : Component
        {
            UObject.Destroy( obj.gameObject, delay );
            obj.RunAction( OnDestroy, delay );
            return obj;
        }


        // Hide GameObject
        public static T HideGameObject<T>( this T obj, float delay ) where T : Component
        {
            return obj.RunAction( () => obj.gameObject.SetActive( false ), delay );
        }

        // Show GameObject
        public static T ShowGameObject<T>( this T obj, float delay ) where T : Component
        {
            return obj.RunAction( () => obj.gameObject.SetActive( true ), delay );
        }

        // Hide GameObject OnTime
        public static T HideGameObjectOnTime<T>( this T obj, float hideDelay, float showDelay ) where T : Component
        {
            return obj.HideGameObject( hideDelay )
                      .ShowGameObject( hideDelay + showDelay );
        }


        // Run Action
        public static T RunAction<T>( this T obj, Action action, float delay ) where T : Component
        {
            if( action == null )
                return obj;

            SpawnEmptyMonoObject( string.Format( "actionTimerOf_{0}-td{1}", obj.name, delay ) )
                .DestroyGameObject( delay + .1f )
                .StartCoroutine( RunTimer( action, delay ) );

            return obj;
        }
        // RunTimer
        private static IEnumerator RunTimer( Action action, float delay )
        {
            yield return new WaitForSeconds( delay );
            action.Invoke();
        }



        // RunAction <T, V>
        public static T RunAction<T, V>( this T obj, Action<V> action, V val, float delay ) where T : Component
        {
            if( action == null )
                return obj;

            SpawnEmptyMonoObject( string.Format( "actionTimerOf_{0}-td{1}", obj.name, delay ) )
                .DestroyGameObject( delay + .1f )
                .StartCoroutine( RunTimer( action, val, delay ) );

            return obj;
        }
        // RunTimer
        private static IEnumerator RunTimer<T>( Action<T> action, T val, float delay )
        {
            yield return new WaitForSeconds( delay );
            action.Invoke( val );
        }



        // RunAction AsLoop
        public static T RunActionAsLoop<T>( this T obj, Action call, float interval, int loops = 0 ) where T : Component
        {
            IEnumerator theEnum = RunLoopTimer( call, interval, loops );
            MonoBehaviour copyObj = SpawnEmptyMonoObject( string.Format( "loopTimerOf_{0}-td{1}", obj.name, corLoopList.Count ) );

            corLoopList.Add( CoroutineLoopData.StartNew( call, theEnum, copyObj ) );

            return obj;
        }
        // Run LoopTimer
        private static IEnumerator RunLoopTimer( Action action, float interval, int loops )
        {
            int loopsCount = 0;
            interval = Mathf.Max( 0, interval );

            while( true )
            {
                loopsCount++;
                action.Invoke();

                if( loops > 0 && loopsCount >= loops )
                {
                    StopLoopedAction( action );
                    yield break;
                }

                yield return new WaitForSeconds( interval );
            }
        }

        //Stop LoopedAction
        public static T StopLoopedAction<T>( this T obj, Action call ) where T : Component
        {
            StopLoopedAction( call );
            return obj;
        }
        // Stop LoopedAction
        private static void StopLoopedAction( Action call )
        {
            for( int i = 0; i < corLoopList.Count; i++ )
                if( corLoopList[ i ].call == call )
                {
                    UObject.Destroy( corLoopList[ i ].copyObj.gameObject );
                    corLoopList.RemoveAt( i );
                    break;
                }
        }

        

        // StartCoroutine OnCopy
        public static T StartCoroutineOnCopy<T>( this T obj, IEnumerator routine ) where T : Component
        {
            SpawnEmptyMonoObject( "coroutineOf-" + obj.name )
                .StartCoroutine( routine );

            return obj;
        }

        // SpawnEmpty MonoObject
        private static MonoBehaviour SpawnEmptyMonoObject( string name )
        {
            return new
                GameObject( string.Format( name ), typeof( EmptyMono ) )
                .MoveToCache()
                .GetComponent<MonoBehaviour>();
        }


        // Spawn Copy
        public static T SpawnCopy<T>( this T original, Vector3 position, Quaternion rotation ) where T : UObject
        {
            return UObject.Instantiate( original, position, rotation ) as T;
        }

        // SpawnCopy WithDelay
        public static void SpawnCopyWithDelay<T>( this T original, Vector3 position, Quaternion rotation, float delay ) where T : Component
        {
            original.RunAction( () => UObject.Instantiate( original, position, rotation ), delay );
        }
    };    
}