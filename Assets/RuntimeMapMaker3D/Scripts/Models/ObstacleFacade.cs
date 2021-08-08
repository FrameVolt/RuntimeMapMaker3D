using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using Zenject.SpaceFighter;

[DisallowMultipleComponent]
public class ObstacleFacade : MonoBehaviour
{
    [Inject]
    public void Construct(ObstacleModel obstacleModel, Vector3Int slotID)
    {
        this.slotID = slotID;
        this.obstacleModel = obstacleModel;
    }

    public Vector3Int slotID;
    private ObstacleModel obstacleModel;

    public class Factory : PlaceholderFactory<Vector3Int, ObstacleModel, ObstacleFacade>
    {
        internal void DeSpawn(ObstacleFacade obstacleFacade)
        {
            GameObject.Destroy(obstacleFacade.gameObject);
        }
    }
}

public class ObstacleFactory : IFactory<Vector3Int, ObstacleModel, ObstacleFacade>
{
    DiContainer _container;
    private Transform parent;
    public ObstacleFactory(DiContainer container)
    {
        _container = container;
        parent = new GameObject("ObstacleFactory").transform;
    }

    public ObstacleFacade Create(Vector3Int slotID, ObstacleModel obstacleModel)
    {
        ObstacleFacade result = _container.InstantiatePrefabForComponent<ObstacleFacade>(obstacleModel.prefab, new List<object>{ slotID, obstacleModel });
        result.transform.SetParent(parent);
        return result;
    }


    public void DeSpawn(ObstacleFacade obstacleFacade)
    {
        GameObject.Destroy(obstacleFacade.gameObject);
    }
}

