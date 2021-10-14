// Copyright (c) LouYaoMing. All Right Reserved.
// Licensed under MIT License.

using System;
using UnityEngine;
using UnityEngine.Assertions;

namespace RMM3D
{
    public class SlotsHolder
    {
        public SlotsHolder(GroundGrid groundGrid, ObstacleFacade.Factory obstacleFactory)
        {
            this.groundGrid = groundGrid;
            this.obstacleFactory = obstacleFactory;
            InitSlots();
        }
        private readonly GroundGrid groundGrid;
        private readonly ObstacleFacade.Factory obstacleFactory;
        public Slot[,,] Slots;

        private void InitSlots()
        {
            this.Slots = new Slot[groundGrid.xAmount, groundGrid.yAmount, groundGrid.zAmount];

            int halfXAmount = groundGrid.xAmount / 2;
            int halfZAmount = groundGrid.zAmount / 2;
            float offset = groundGrid.size / 2;

            for (int i = 0; i < groundGrid.xAmount; i++)
            {
                for (int j = 0; j < groundGrid.yAmount; j++)
                {
                    for (int k = 0; k < groundGrid.zAmount; k++)
                    {
                        var slot = new Slot();
                        slot.position = new Vector3(i - halfXAmount + offset, j, k - halfZAmount + offset);
                        slot.rotation = Vector3.zero;
                        slot.scale = Vector3.one;
                        slot.color = Vector4.one;
                        this.Slots[i, j, k] = slot;
                    }
                }
            }
        }


        public void SetSlotMap(Slot[,,] slots)
        {
            this.Slots = slots;

            for (int i = 0; i < slots.GetLength(0); i++)
            {
                for (int j = 0; j < slots.GetLength(1); j++)
                {
                    for (int k = 0; k < slots.GetLength(2); k++)
                    {
                        if (slots[i, j, k].obstacleData != null)
                        {
                            var obstacle = obstacleFactory.Create(new Vector3Int(i, j, k), slots[i, j, k].obstacleData, slots[i,j,k].rotation, slots[i, j, k].scale, slots[i,j,k].color);
                            obstacle.transform.position = slots[i, j, k].position;
                            slots[i, j, k].item = obstacle.gameObject;
                        }

                    }
                }
            }
        }

        public void ResetSlotMap()
        {
            var slots = Slots;
            for (int i = 0; i < slots.GetLength(0); i++)
            {
                for (int j = 0; j < slots.GetLength(1); j++)
                {
                    for (int k = 0; k < slots.GetLength(2); k++)
                    {
                        if (slots[i, j, k].item != null)
                        {
                            ReleaseSlotItem(new Vector3Int(i, j, k), obstacleFactory);
                        }
                    }
                }
            }
        }


        public Vector3 GetSlotPos(Vector3Int ID, GroundGrid groundGrid)
        {
            var halfXAmount = groundGrid.xAmount / 2;
            var yAmount = groundGrid.yAmount;
            var halfZAmount = groundGrid.zAmount / 2;
            var offset = groundGrid.size / 2;

            var result = new Vector3(ID.x * groundGrid.size - halfXAmount + offset, ID.y, ID.z * groundGrid.size - halfZAmount + offset);

            return result;
        }

        public GameObject TryGetItem(Vector3Int slotID)
        {
            GameObject result = null;
            try
            {
                var slot = Slots[slotID.x, slotID.y, slotID.z];
                result = slot.item;

            }
            catch(IndexOutOfRangeException ex)
            {
                Debug.LogWarning(ex);
            }

            return result;
        }

        public ObstacleModel TryGetObstacleModel(Vector3Int slotID)
        {
            ObstacleModel result = null;

            try
            {
                var slot = Slots[slotID.x, slotID.y, slotID.z];
                result = slot.obstacleData;
            }
            catch(IndexOutOfRangeException ex)
            {
                Debug.LogWarning(ex);
            }

            return result;
        }


        public void SetRoatation(Vector3Int slotID, Vector3 eular)
        {
            Slots[slotID.x, slotID.y, slotID.z].rotation = eular;
        }
        public void SetScale(Vector3Int slotID, Vector3 scale)
        {
            Slots[slotID.x, slotID.y, slotID.z].scale = scale;
        }
        public void SetSoltColor(Vector3Int slotID, Color color)
        {
            Slots[slotID.x, slotID.y, slotID.z].color = color;
        }
        public void SetSlotItem(Vector3Int slotID, ObstacleFacade obstacle, ObstacleModel obstacleData)
        {
            Slots[slotID.x, slotID.y, slotID.z].rotation = obstacle.transform.eulerAngles;
            Slots[slotID.x, slotID.y, slotID.z].item = obstacle.gameObject;
            Slots[slotID.x, slotID.y, slotID.z].obstacleData = obstacleData;
            Slots[slotID.x, slotID.y, slotID.z].color = obstacle.color;
        }

        /// <summary>
        /// 只移除，未释放Slot内的对象
        /// </summary>
        public void RemoveSlotItem(Vector3Int slotID)
        {
            Slots[slotID.x, slotID.y, slotID.z].rotation = Vector3.zero;
            Slots[slotID.x, slotID.y, slotID.z].item = null;
            Slots[slotID.x, slotID.y, slotID.z].obstacleData = null;
            Slots[slotID.x, slotID.y, slotID.z].color = Color.white;
        }

        /// <summary>
        /// 移除，并释放Slot内的对象
        /// </summary>
        public void ReleaseSlotItem(Vector3Int slotID, ObstacleFacade.Factory obstacleFactory)
        {

            var slotItem = Slots[slotID.x, slotID.y, slotID.z].item;

            Assert.IsNotNull(slotItem, "试图移除已经为空的Slot");
            var obstacleFacade = slotItem.GetComponent<ObstacleFacade>();
            if (slotItem.activeSelf != false)
            {
                obstacleFactory.DeSpawn(obstacleFacade);
            }
            else
            {
                Debug.LogWarning("试图移除已经disactive的对象" + slotItem.transform.position);
            }

            Slots[slotID.x, slotID.y, slotID.z].rotation = Vector3.zero;
            Slots[slotID.x, slotID.y, slotID.z].item = null;
            Slots[slotID.x, slotID.y, slotID.z].obstacleData = null;
            Slots[slotID.x, slotID.y, slotID.z].color = Color.white;
        }

        public Vector3Int TranPos2SlotID(Vector3 transPos, GroundGrid groundGrid)
        {
            var halfXAmount = groundGrid.xAmount / 2;
            var yAmount = groundGrid.yAmount;
            var halfZAmount = groundGrid.zAmount / 2;


            Vector3Int result = new Vector3Int();
            result.x = Mathf.FloorToInt(transPos.x + halfXAmount / groundGrid.size);
            result.y = Mathf.RoundToInt(transPos.y / groundGrid.size);
            result.z = Mathf.FloorToInt(transPos.z + halfZAmount / groundGrid.size);

            Debug.Log(transPos.z + " " + halfZAmount / groundGrid.size);


            return result;
        }


        public int AmountItem()
        {
            int index = 0;
            for (int i = 0; i < Slots.GetLength(0); i++)
            {
                for (int j = 0; j < Slots.GetLength(1); j++)
                {
                    for (int k = 0; k < Slots.GetLength(2); k++)
                    {
                        if (Slots[i, j, k].item != null)
                        {
                            index++;
                        }
                    }
                }
            }
            return index;
        }
        public int AmountObstacleModel()
        {
            int index = 0;
            for (int i = 0; i < Slots.GetLength(0); i++)
            {
                for (int j = 0; j < Slots.GetLength(1); j++)
                {
                    for (int k = 0; k < Slots.GetLength(2); k++)
                    {
                        if (Slots[i, j, k].obstacleData != null)
                        {
                            index++;
                        }
                    }
                }
            }
            return index;
        }
        public Slot[,,] Copy(Slot[,,] value)
        {
            int a = value.GetLength(0);
            int b = value.GetLength(1);
            int c = value.GetLength(2);

            Slot[,,] newSolts = new Slot[a, b, c];

            for (int i = 0; i < a; i++)
            {
                for (int j = 0; j < b; j++)
                {
                    for (int k = 0; k < c; k++)
                    {
                        newSolts[i, j, k] = value[i, j, k];
                    }
                }
            }
            return newSolts;
        }
    }

    [System.Serializable]
    public struct Slot
    {
        public Vector3 position;
        public Vector3 rotation;
        public Vector3 scale;
        public Vector4 color;
        [System.NonSerialized] public GameObject item;
        public bool isRoot;
        public ObstacleModel obstacleData;

        public static Slot Copy(Slot value)
        {
            Slot result = new Slot();
            result.position = value.position;
            result.rotation = value.rotation;
            result.isRoot = value.isRoot;
            result.obstacleData = value.obstacleData;
            return result;
        }
    }
}