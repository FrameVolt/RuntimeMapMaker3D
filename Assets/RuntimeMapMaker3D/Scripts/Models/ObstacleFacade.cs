using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace RMM3D
{
    [DisallowMultipleComponent]
    public class ObstacleFacade : MonoBehaviour
    {
        [Inject]
        public void Construct(SlotsHolder slotsHolder, GroundGrid groundGrid, ObstacleModel obstacleModel, Vector3Int slotID, Vector3 euler, Vector3 scale, Color color)
        {
            this.slotsHolder = slotsHolder;
            this.groundGrid = groundGrid;
            this.slotID = slotID;
            this.euler = euler;
            this.obstacleModel = obstacleModel;
            this.color = color;
            this.transform.localEulerAngles = euler;
            this.transform.localScale = scale;
            obstacleRenderer = GetComponentsInChildren<Renderer>();
            SetColor(color);
        }

        private SlotsHolder slotsHolder;
        private GroundGrid groundGrid;

        public Vector3Int slotID { get; private set; }
        public Vector3 euler { get; private set; }
        public Color color { get; private set; }
        private ObstacleModel obstacleModel;
        private Renderer[] obstacleRenderer;


        public void SetSlotID(Vector3Int newSlotID)
        {
            slotID = newSlotID;
            transform.position = slotsHolder.GetSlotPos(newSlotID, groundGrid);

            
        }

        public void SetColor(Color color)
        {
            for (int i = 0; i < obstacleRenderer.Length; i++)
            {
                obstacleRenderer[i].material.color = color;
            }
            slotsHolder.SetSoltColor(slotID, color);
        }



        public class Factory : PlaceholderFactory<Vector3Int, ObstacleModel, Vector3, Vector3, Color, ObstacleFacade>
        {
            internal void DeSpawn(ObstacleFacade obstacleFacade)
            {
                GameObject.Destroy(obstacleFacade.gameObject);
            }
        }
    }

    /// <summary>
    /// Obstacle real factory, override create function, load from assetbundle
    /// </summary>
    public class ObstacleFactory : IFactory<Vector3Int, ObstacleModel, Vector3, Vector3, Color, ObstacleFacade>
    {
        DiContainer _container;
        private Transform parent;

        private AssetBundleSystem assetBundleSystem;

        public ObstacleFactory(DiContainer container, AssetBundleSystem assetBundleSystem)
        {
            _container = container;
            parent = new GameObject("ObstacleFactory").transform;
            this.assetBundleSystem = assetBundleSystem;
        }

        public ObstacleFacade Create(Vector3Int slotID, ObstacleModel obstacleModel, Vector3 euler, Vector3 scale, Color color)
        {

            var prefab = assetBundleSystem.assetBundle.LoadAsset<GameObject>(obstacleModel.assetName);
            ObstacleFacade result = _container.InstantiatePrefabForComponent<ObstacleFacade>(prefab, new List<object> { slotID, obstacleModel, euler, scale, color });
            result.transform.SetParent(parent);
            return result;
        }


        public void DeSpawn(ObstacleFacade obstacleFacade)
        {
            GameObject.Destroy(obstacleFacade.gameObject);
        }
    }
}