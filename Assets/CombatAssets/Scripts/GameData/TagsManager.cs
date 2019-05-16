/********************************************
 * Copyright(c): 2016 Victor Klepikov       *
 *                                          *
 * Site: 	     http://u3d.as/5Fb		    *
 * Support: 	 http://bit.ly/vk-Support   *
 ********************************************/


namespace AdvancedShooterKit
{
    public static class TagsManager
    {
        public const string MainCamera = "MainCamera";
        //
        public const string Player = "Player";
        public const string NPC = "ASK/NonPlayerCharacter";
        //
        public const string Pickup = "ASK/Pickup";


        // IsMainCamera
        public static bool IsMainCamera( string tag )
        {
            return tag == MainCamera;
        }

        // IsPlayer
        public static bool IsPlayer( string tag )
        {
            return tag == Player;
        }
        // IsNPC
        public static bool IsNPC( string tag )
        {
            return tag == NPC;
        }

        // IsPickup
        public static bool IsPickup( string tag )
        {
            return tag == Pickup;
        }
    }
}