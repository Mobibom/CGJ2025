using UnityEditor;
using UnityEngine;
using Newtonsoft.Json.Linq;
using RoomData;
using System.Collections.Generic;

[CustomEditor(typeof(RoomJsonReader))]
public class RoomJsonReaderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        RoomJsonReader reader = (RoomJsonReader)target;

        if (GUILayout.Button("🔄 从 JSON 自动读取房间名"))
        {
            if (reader.jsonFile == null)
            {
                Debug.LogError("❌ 未指定 JSON 文件");
                return;
            }

            string jsonText = reader.jsonFile.text;
            JObject root = JObject.Parse(jsonText);
            var skins = root["skins"]?["default"];

            if (skins == null)
            {
                Debug.LogError("❌ 无法读取 skins.default");
                return;
            }

            var newBlocks = new List<RoomBlock>();

            foreach (var room in skins.Children<JProperty>())
            {
                string roomName = room.Name;
                newBlocks.Add(new RoomBlock
                {
                    name = roomName,
                    sprite = null,
                    orderInLayer = 1
                });
            }

            Undo.RecordObject(reader, "自动填充房间块");
            reader.blocks = newBlocks;
            EditorUtility.SetDirty(reader);

            Debug.Log($"✅ 成功读取 {newBlocks.Count} 个房间");
        }

        if (GUILayout.Button("生成房子"))
        {
            reader.GenerateRooms();
        }
    }
}