using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using Zenject;

namespace RMM3D
{
    public class SlotsHolder
    {
        public SlotsHolder(GroundGrid groundGrid, ObstacleFacade.Factory obstacleFactory)
        {

            this.slotMap = new SoltMap();
            this.slotMap.Solts = new Solt[groundGrid.xAmount, groundGrid.yAmount, groundGrid.zAmount];

            int halfXAmount = groundGrid.xAmount / 2;
            int halfZAmount = groundGrid.zAmount / 2;
            float offset = groundGrid.size / 2;

            for (int i = 0; i < groundGrid.xAmount; i++)
            {
                for (int j = 0; j < groundGrid.yAmount; j++)
                {
                    for (int k = 0; k < groundGrid.zAmount; k++)
                    {
                        var slot = new Solt();
                        slot.position = new Vector3(i - halfXAmount + offset, j, k - halfZAmount + offset);
                        slot.rotation = Vector3.zero;
                        this.slotMap.Solts[i, j, k] = slot;
                    }
                }
            }

            this.obstacleFactory = obstacleFactory;
        }

        public SoltMap slotMap { get; private set; }
        private readonly ObstacleFacade.Factory obstacleFactory;


        public void SetSoltMap(Solt[,,] slots)
        {
            this.slotMap.Solts = slots;

            for (int i = 0; i < slots.GetLength(0); i++)
            {
                for (int j = 0; j < slots.GetLength(1); j++)
                {
                    for (int k = 0; k < slots.GetLength(2); k++)
                    {
                        if (slots[i, j, k].obstacleData != null)
                        {
                            var obstacle = obstacleFactory.Create(new Vector3Int(i, j, k), slots[i, j, k].obstacleData);
                            obstacle.transform.position = slots[i, j, k].position;
                            obstacle.transform.eulerAngles = slots[i, j, k].rotation;
                            slots[i, j, k].item = obstacle.gameObject;
                        }

                    }
                }
            }
        }

        public void ResetSoltMap()
        {
            var slots = slotMap.Solts;
            for (int i = 0; i < slots.GetLength(0); i++)
            {
                for (int j = 0; j < slots.GetLength(1); j++)
                {
                    for (int k = 0; k < slots.GetLength(2); k++)
                    {
                        if (slots[i, j, k].item != null)
                        {
                            slotMap.ReleaseSlotItem(new Vector3Int(i, j, k), obstacleFactory);
                        }
                    }
                }
            }
        }

        public IEnumerator YieldReplaceSlotMap(Solt[,,] slots)
        {
            var oldSolts = slotMap.Solts;
            for (int i = 0; i < oldSolts.GetLength(0); i++)
            {
                for (int j = 0; j < oldSolts.GetLength(1); j++)
                {
                    for (int k = 0; k < oldSolts.GetLength(2); k++)
                    {
                        if (oldSolts[i, j, k].item != null)
                        {
                            slotMap.ReleaseSlotItem(new Vector3Int(i, j, k), obstacleFactory);
                        }
                    }
                }
            }
            yield return null;

            for (int i = 0; i < slots.GetLength(0); i++)
            {
                for (int j = 0; j < slots.GetLength(1); j++)
                {
                    for (int k = 0; k < slots.GetLength(2); k++)
                    {
                        if (slots[i, j, k].obstacleData != null)
                        {
                            var obstacle = obstacleFactory.Create(new Vector3Int(i, j, k), slots[i, j, k].obstacleData);
                            obstacle.transform.position = slots[i, j, k].position;
                            obstacle.transform.eulerAngles = slots[i, j, k].rotation;
                            slots[i, j, k].item = obstacle.gameObject;
                        }

                    }
                }
            }
            this.slotMap.Solts = slots;
        }



        public void ReplaceSlotMap(Solt[,,] newSolts)
        {
            var oldSlots = this.slotMap.Solts;

            for (int i = 0; i < newSolts.GetLength(0); i++)
            {
                for (int j = 0; j < newSolts.GetLength(1); j++)
                {
                    for (int k = 0; k < newSolts.GetLength(2); k++)
                    {
                        if (newSolts[i, j, k].obstacleData != oldSlots[i, j, k].obstacleData)
                        {
                            if (oldSlots[i, j, k].obstacleData != null)
                            {
                                slotMap.ReleaseSlotItem(new Vector3Int(i, j, k), obstacleFactory);
                            }
                            if (newSolts[i, j, k].obstacleData != null)
                            {
                                var obstacle = obstacleFactory.Create(new Vector3Int(i, j, k), newSolts[i, j, k].obstacleData);
                                obstacle.transform.position = newSolts[i, j, k].position;
                                obstacle.transform.eulerAngles = newSolts[i, j, k].rotation;
                                newSolts[i, j, k].item = obstacle.gameObject;
                            }

                        }
                    }
                }
            }

            this.slotMap.Solts = newSolts;
        }

    }






    [System.Serializable]
    public class SoltMap
    {
        public Solt[,,] Solts;

        public static Vector3 GetSlotPos(Vector3Int ID, GroundGrid groundGrid)
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
            return Solts[slotID.x, slotID.y, slotID.z].item;
        }

        public ObstacleModel TryGetObstacleModel(Vector3Int slotID)
        {
            return Solts[slotID.x, slotID.y, slotID.z].obstacleData;
        }


        public void SetSlotItem(Vector3Int slotID, GameObject item, ObstacleModel obstacleData)
        {
            Solts[slotID.x, slotID.y, slotID.z].rotation = item.transform.eulerAngles;
            Solts[slotID.x, slotID.y, slotID.z].item = item;
            Solts[slotID.x, slotID.y, slotID.z].obstacleData = obstacleData;
        }

        /// <summary>
        /// 只移除，未释放Slot内的对象
        /// </summary>
        public void RemoveSlotItem(Vector3Int slotID)
        {
            Solts[slotID.x, slotID.y, slotID.z].rotation = Vector3.zero;
            Solts[slotID.x, slotID.y, slotID.z].item = null;
            Solts[slotID.x, slotID.y, slotID.z].obstacleData = null;
        }

        /// <summary>
        /// 移除，并释放Slot内的对象
        /// </summary>
        public void ReleaseSlotItem(Vector3Int slotID, ObstacleFacade.Factory obstacleFactory)
        {

            var slotItem = Solts[slotID.x, slotID.y, slotID.z].item;

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

            Solts[slotID.x, slotID.y, slotID.z].rotation = Vector3.zero;
            Solts[slotID.x, slotID.y, slotID.z].item = null;
            Solts[slotID.x, slotID.y, slotID.z].obstacleData = null;
        }



        public Vector3Int TranPos2SlotID(Vector3 transPos, GroundGrid groundGrid)
        {
            var halfXAmount = groundGrid.xAmount / 2;
            var yAmount = groundGrid.yAmount;
            var halfZAmount = groundGrid.zAmount / 2;


            Vector3Int result = new Vector3Int();
            result.x = Mathf.FloorToInt(transPos.x + halfXAmount / groundGrid.size);
            result.y = Mathf.FloorToInt(transPos.y / groundGrid.size);
            result.z = Mathf.FloorToInt(transPos.z + halfZAmount / groundGrid.size);

            Debug.Log(transPos.z + " " + halfZAmount / groundGrid.size);


            return result;
        }


        public int AmountItem()
        {
            int index = 0;
            for (int i = 0; i < Solts.GetLength(0); i++)
            {
                for (int j = 0; j < Solts.GetLength(1); j++)
                {
                    for (int k = 0; k < Solts.GetLength(2); k++)
                    {
                        if (Solts[i, j, k].item != null)
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
            for (int i = 0; i < Solts.GetLength(0); i++)
            {
                for (int j = 0; j < Solts.GetLength(1); j++)
                {
                    for (int k = 0; k < Solts.GetLength(2); k++)
                    {
                        if (Solts[i, j, k].obstacleData != null)
                        {
                            index++;
                        }
                    }
                }
            }
            return index;
        }
        public static Solt[,,] Copy(Solt[,,] value)
        {
            int a = value.GetLength(0);
            int b = value.GetLength(1);
            int c = value.GetLength(2);


            Solt[,,] newSolts = new Solt[a, b, c];


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
    public struct Solt
    {
        public Vector3 position;
        public Vector3 rotation;
        [System.NonSerialized] public GameObject item;
        public bool isRoot;
        public ObstacleModel obstacleData;



        public static Solt Copy(Solt value)
        {
            Solt result = new Solt();
            result.position = value.position;
            result.rotation = value.rotation;
            result.isRoot = value.isRoot;
            result.obstacleData = value.obstacleData;
            return result;
        }
    }

    [System.Serializable]
    public struct Vector3S
    {
        public float x;
        public float y;
        public float z;

        public Vector3S(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Vector3S))
            {
                return false;
            }

            var s = (Vector3S)obj;
            return x == s.x &&
                   y == s.y &&
                   z == s.z;
        }

        public override int GetHashCode()
        {
            var hashCode = 373119288;
            hashCode = hashCode * -1521134295 + x.GetHashCode();
            hashCode = hashCode * -1521134295 + y.GetHashCode();
            hashCode = hashCode * -1521134295 + z.GetHashCode();
            return hashCode;
        }

        public Vector3 ToVector3()
        {
            return new Vector3(x, y, z);
        }

        public static bool operator ==(Vector3S a, Vector3S b)
        {
            return a.x == b.x && a.y == b.y && a.z == b.z;
        }

        public static bool operator !=(Vector3S a, Vector3S b)
        {
            return a.x != b.x && a.y != b.y && a.z != b.z;
        }

        public static implicit operator Vector3(Vector3S x)
        {
            return new Vector3(x.x, x.y, x.z);
        }

        public static implicit operator Vector3S(Vector3 x)
        {
            return new Vector3S(x.x, x.y, x.z);
        }
    }
}