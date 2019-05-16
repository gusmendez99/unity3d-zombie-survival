/********************************************
 * Copyright(c): 2016 Victor Klepikov       *
 *                                          *
 * Site: 	     http://u3d.as/5Fb		    *
 * Support: 	 http://bit.ly/vk-Support   *
 ********************************************/


using UnityEngine;


namespace AdvancedShooterKit
{
    public sealed class SurfaceDetector : ScriptableObject
    {
        [System.Serializable]
        public sealed class SurfaceData
        {
            public string name = string.Empty;
            public Material[] materials = null;
            public Texture[] textures = null;
        }

        [SerializeField]
        private SurfaceData[] surfaces = null;


        // instance
        private static SurfaceDetector instance = null;
        // m_Instance
        private static SurfaceDetector m_Instance
        {
            get
            {
                if( instance == null )
                    instance = Resources.Load<SurfaceDetector>( "SurfaceDetector" );

                return instance;
            }
        }


        // Get SurfaceName ByHit
        public static string GetSurfaceNameByHit( RaycastHit hit )
        {
            int surfaceIndex = GetSurfaceIndexByHit( hit );
            return ( surfaceIndex > -1 ) ? m_Instance.surfaces[ surfaceIndex ].name : "Unknown Surface";
        }
        // Get SurfaceIndex ByHit
        public static int GetSurfaceIndexByHit( RaycastHit hit )
        {
            const int negativeIndex = -1;

            Collider hitCollider = hit.collider;
            if( hitCollider == null || hitCollider.isTrigger )
                return negativeIndex;

            // if TerrainTexture != null, return it.
            Texture hitTexture = hit.GetTerrainTexture();
            if( hitTexture != null )
            {
                for( int i = 0; i < GetCount; i++ )
                    foreach( Texture tex in m_Instance.surfaces[ i ].textures )
                        if( tex == hitTexture )
                            return i;
            }

            // Get Object Material if TerrainTexture == null
            Material hitMaterial = hit.GetMaterial();
            if( hitMaterial != null )
            {
                for( int i = 0; i < GetCount; i++ )
                    foreach( Material mat in m_Instance.surfaces[ i ].materials )
                        if( mat == hitMaterial )
                            return i;
            }

            return negativeIndex;
        }


        // Get MaterialName ByHit
        public static string GetMaterialNameByHit( RaycastHit hit )
        {
            Material theMaterial = hit.GetMaterial();
            return ( theMaterial != null ) ? theMaterial.name : "Unknown Material";
        }


        // Get TerrainTextureName ByHit
        public static string GetTerrainTextureNameByHit( RaycastHit hit )
        {
            Texture tmpTex = hit.GetTerrainTexture();
            return ( tmpTex != null ) ? tmpTex.name : "Unknown Texture";
        }

        // Get Count
        public static int GetCount { get { return m_Instance.surfaces.Length; } }
        // Get Names 
        public static string[] GetNames
        {
            get
            {
                string[] tmpNames = new string[ GetCount ];

                for( int i = 0; i < GetCount; i++ )
                    tmpNames[ i ] = m_Instance.surfaces[ i ].name;

                return tmpNames;
            }
        }
        //        
    };

    

    // Extensions for RaycastHit struct
    public static class RaycastHitExtensions
    {
        // Get Material ByHit
        public static Material GetMaterial( this RaycastHit hit )
        {
            Collider hitCollider = hit.collider;
            if( hitCollider == null || hitCollider.isTrigger )
                return null;

            Renderer renderer = hitCollider.GetComponent<Renderer>();
            if( renderer != null )
            {
                MeshCollider meshCollider = hitCollider.GetComponent<MeshCollider>();
                if( meshCollider != null && !meshCollider.convex )
                {
                    Mesh sharedMesh = meshCollider.sharedMesh;
                    int tIndex = hit.triangleIndex * 3;
                    int index1 = sharedMesh.triangles[ tIndex ];
                    int index2 = sharedMesh.triangles[ tIndex + 1 ];
                    int index3 = sharedMesh.triangles[ tIndex + 2 ];
                    int subMeshCount = sharedMesh.subMeshCount;

                    int[] triangles = null;

                    for( int i = 0; i < subMeshCount; i++ )
                    {
                        triangles = sharedMesh.GetTriangles( i );

                        for( int j = 0; j < triangles.Length; j += 3 )
                            if( triangles[ j ] == index1 && triangles[ j + 1 ] == index2 && triangles[ j + 2 ] == index3 )
                                return renderer.sharedMaterials[ i ];
                    }
                }
                else
                {
                    return renderer.sharedMaterial;
                }
            }

            return null;
        }

        // Get TerrainTexture ByHit
        public static Texture GetTerrainTexture( this RaycastHit hit )
        {
            Collider hitCollider = hit.collider;

            if( hitCollider == null || hitCollider.isTrigger )
                return null;

            Terrain terrain = hitCollider.GetComponent<Terrain>();
            if( terrain == null )
                return null;

            TerrainData terrainData = terrain.terrainData;
            Vector3 terrainPos = terrain.transform.position;
            int mapX = Mathf.RoundToInt( ( ( hit.point.x - terrainPos.x ) / terrainData.size.x ) * terrainData.alphamapWidth );
            int mapZ = Mathf.RoundToInt( ( ( hit.point.z - terrainPos.z ) / terrainData.size.z ) * terrainData.alphamapHeight );
            float[,,] splatmapData = terrainData.GetAlphamaps( mapX, mapZ, 1, 1 );

            for( int i = 0; i < terrainData.splatPrototypes.Length; i++ )
                if( splatmapData[ 0, 0, i ] > .5f )
                    return terrainData.splatPrototypes[ i ].texture;

            return null;
        }
    };
}