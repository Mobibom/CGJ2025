using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace RoomData
{
    [System.Serializable]
    public class RoomBlock
    {
        public string name;        // Room 名字，比如 Room1_1
        public Sprite sprite;      // 用户拖入的图
        public int orderInLayer;   // 层级排序
        public Enum_SceneState gameState = Enum_SceneState.DemoRoom;
        
    }
}