#if USE_GRID_SYSTEM && USE_TILEMAP
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditor;

namespace VirtualPhenix.GridEngine
{
    public class VP_GridPreviewManager : VP_SingletonMonobehaviour<VP_GridPreviewManager>
    {
        public static new VP_GridPreviewManager Instance
        {
            get
            {
                if (m_instance == null) 
                { 
                    m_instance = (VP_GridPreviewManager)FindObjectOfType(typeof(VP_GridPreviewManager)); 
                }

                return (VP_GridPreviewManager)m_instance;
            }
        }

#if UNITY_EDITOR
        [SerializeField] protected Grid m_grid;
        [SerializeField] protected List<PreviewObject> m_previewObjects = new List<PreviewObject>();
        // Holders
        protected Transform m_previewObjectsHolder;
        public Transform PreviewObjectsHolder
        {
            get
            {
                if (m_previewObjectsHolder == null) 
                { 
                    GetHolders(); 
                }
                return m_previewObjectsHolder;
            }
            protected set { m_previewObjectsHolder = value; }
        }

        protected override void Reset()
        {
            base.Reset();
            m_grid = FindObjectOfType<Grid>();
        }

        protected override void Initialize()
        {
            base.Initialize();

            // Update holders
            GetHolders();
        }

        protected virtual void GetHolders()
        {
            if (m_previewObjectsHolder == null)
            {
                m_previewObjectsHolder = transform.Find("PreviewGameObjects");
                if (m_previewObjectsHolder == null)
                {
                    m_previewObjectsHolder = new GameObject("PreviewGameObjects").transform;
                    m_previewObjectsHolder.SetParent(transform);
                    m_previewObjectsHolder.localPosition = Vector3.zero;
                }
            }
        }

        public virtual void InstantiatePreviewTileAtPosition(VP_GridTile tilePrefab, Vector2Int position, Vector3 offsetPosition, Quaternion rotation)
        {
    
            // Parameters
            var gridCellCenter = m_grid != null ? m_grid.GetCellCenterWorld(position.ToVector3IntXY0()) : VP_GridManager.Instance.Grid.GetCellCenterWorld(position.ToVector3IntXY0());
            var worldPosition = gridCellCenter + offsetPosition;
            VP_GridTile instantiatedTile = null;

            if (PrefabUtility.IsPartOfPrefabAsset(tilePrefab.gameObject))
            {
                instantiatedTile = (VP_GridTile)PrefabUtility.InstantiatePrefab(tilePrefab);
            }
            else
            {
                instantiatedTile = (VP_GridTile)Instantiate(tilePrefab, worldPosition, rotation, PreviewObjectsHolder) as VP_GridTile;
            }
            instantiatedTile.transform.parent = PreviewObjectsHolder;
            instantiatedTile.transform.position = worldPosition;
  
            instantiatedTile.transform.rotation = rotation;
            // Preview tile 
            var previewTile = new PreviewObject(instantiatedTile.gameObject, position);
            m_previewObjects.Add(previewTile);
            // Make the renderers attached to it transparent
            MakeVisualizerTransparent(instantiatedTile.transform);
            // Destroy the grid component
            //DestroyImmediate(instantiatedTile);
            var components = instantiatedTile.GetComponents<MonoBehaviour>();
            foreach (MonoBehaviour comp in components)
            {
                comp.enabled = false;
            }
        }

        public virtual void InstantiatePreviewGridObjectAtPosition(VP_GridObject objectPrefab, Vector2Int position, Vector3 offsetPosition, Orientations orientation)
        {
            var gm = VP_GridManager.Instance;

            // Check if there is a tile there
            var tileAtPosition = gm.GetGridTileAtPosition(position);
            if (!tileAtPosition)
                return;


            // Parameters
            var gridCellCenter = gm.Grid.GetCellCenterWorld(position.ToVector3IntXY0());
            var worldPosition = gridCellCenter + offsetPosition;
            VP_GridObject instantiatedObject = null;
            if (PrefabUtility.IsPartOfPrefabAsset(objectPrefab.gameObject))
            {
                instantiatedObject = (VP_GridObject)PrefabUtility.InstantiatePrefab(objectPrefab);
            }
            else
            {
                instantiatedObject = (VP_GridObject)Instantiate(objectPrefab, worldPosition, Quaternion.identity, PreviewObjectsHolder) as VP_GridObject;
            }
            var cellPosition = tileAtPosition.GridObjectsPivotPosition;
            instantiatedObject.transform.parent = PreviewObjectsHolder;
            instantiatedObject.transform.position = cellPosition;
            instantiatedObject.transform.rotation = gm.OrientationToRotation(position, orientation);
            // Preview tile 
            var previewObject = new PreviewObject(instantiatedObject.gameObject, position);
            m_previewObjects.Add(previewObject);
            // Make the renderers attached to it transparent
            MakeVisualizerTransparent(instantiatedObject.transform);

            var components = instantiatedObject.GetComponents<MonoBehaviour>();
            foreach (MonoBehaviour comp in components)
            {
                comp.enabled = false;
            }
        }

        public virtual void ClearPreviewObjectAtPosition(Vector2Int position)
        {
            var previewTileAtPosition = m_previewObjects.OrderBy(t => t.m_GridPosition == position).FirstOrDefault();
            if (previewTileAtPosition.m_GameObject != null && previewTileAtPosition.m_GridPosition == position)
            {
                m_previewObjects.Remove(previewTileAtPosition);
                DestroyImmediate(previewTileAtPosition.m_GameObject);
            }
            else if (previewTileAtPosition.m_GameObject == null)
            {
                m_previewObjects.Remove(previewTileAtPosition);
            }
        }

        [ContextMenu("Debug: Remove all preview objects")]
        public virtual void ClearAllPreviewTiles()
        {
            foreach (PreviewObject previewObject in m_previewObjects.ToList())
            {
                m_previewObjects.Remove(previewObject);
                if (previewObject.m_GameObject != null)
                {
                    DestroyImmediate(previewObject.m_GameObject);
                }
            }
        }

        protected static Transform MakeVisualizerTransparent(Transform transform)
        {
            // Attempt to get reference to GameObject Renderer
            Renderer meshRenderer = transform.gameObject.GetComponent<Renderer>();

            // If a Renderer was found
            if (meshRenderer != null)
            {
                // Define temporary Material used to create transparent copy of GameObject Material
                Material tempMat = new Material(Shader.Find("Standard"));

                Material[] tempMats = new Material[meshRenderer.sharedMaterials.Length];

                // Loop through each material in GameObject
                for (int i = 0; i < meshRenderer.sharedMaterials.Length; i++)
                {
                    // Get material from GameObject
                    tempMat = new Material(meshRenderer.sharedMaterials[i]);

                    // Change Shader to "Standard"
                    tempMat.shader = Shader.Find("Standard");

                    // Make Material transparent
                    tempMat.SetFloat("_Mode", 2);
                    tempMat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                    tempMat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    tempMat.SetInt("_ZWrite", 0);
                    tempMat.DisableKeyword("_ALPHATEST_ON");
                    tempMat.EnableKeyword("_ALPHABLEND_ON");
                    tempMat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    tempMat.renderQueue = 3000;

                    Color32 tempColor = tempMat.color;
                    tempColor.a = (byte)((int)tempColor.a * 0.5);
                    tempMat.color = tempColor;

                    // Replace GameObject Material with transparent one
                    tempMats[i] = tempMat;
                }

                meshRenderer.sharedMaterials = tempMats;
            }

            // Recursively run this method for each child transform
            foreach (Transform child in transform)
            {
                MakeVisualizerTransparent(child);
            }

            return transform;
        }
#endif
        [System.Serializable]
        public struct PreviewObject
        {
            public GameObject m_GameObject;
            public Vector2Int m_GridPosition;

            public PreviewObject(GameObject gameObject, Vector2Int gridPosition)
            {
                m_GameObject = gameObject;
                m_GridPosition = gridPosition;
            }
        }
    }

}
#endif