using UnityEngine;
using UnityEditor; // 注意：需要导入 UnityEditor 命名空间

// 告诉 Unity 这个编辑器是为 SceneLightController 类服务的
[CustomEditor(typeof(SceneLightController))]
public class SceneLightControllerEditor : Editor
{
    // 这是在 Inspector 绘制时调用的核心方法
    public override void OnInspectorGUI()
    {
        // 绘制默认的 Inspector 界面（显示所有 [SerializeField] 或 public 变量）
        // 这一行非常重要，否则你的其他变量将不会显示
        DrawDefaultInspector();

        // 获取当前正在编辑的 SceneLightController 实例
        SceneLightController myScript = (SceneLightController)target;

        // --- 添加按钮 ---
        
        if (GUILayout.Button("Save Light to ConfigA"))
        {
            // 当按钮被点击时，调用 SceneLightController 实例上的方法
            // 使用 myScript.duration 来获取你在 Inspector 中设置的时长
            myScript.SaveDataToConfigA();
        }
        
        if (GUILayout.Button("Save Light to ConfigB"))
        {
            // 当按钮被点击时，调用 SceneLightController 实例上的方法
            // 使用 myScript.duration 来获取你在 Inspector 中设置的时长
            myScript.SaveDataToConfigB();
        }
        
        if (GUILayout.Button("Save Light to ConfigC"))
        {
            // 当按钮被点击时，调用 SceneLightController 实例上的方法
            // 使用 myScript.duration 来获取你在 Inspector 中设置的时长
            myScript.SaveDataToConfigC();
        }

        // 按钮：Lerp A to B
        // GUILayout.Button("按钮文本") 创建一个按钮
        if (GUILayout.Button("Lerp A to B"))
        {
            // 当按钮被点击时，调用 SceneLightController 实例上的方法
            // 使用 myScript.duration 来获取你在 Inspector 中设置的时长
            myScript.LerpAToB(myScript.duration);
        }

        // 按钮：Lerp B to C
        if (GUILayout.Button("Lerp B to C"))
        {
            myScript.LerpBToC(myScript.duration);
        }

        // 按钮：Lerp C to A
        if (GUILayout.Button("Lerp C to A"))
        {
            myScript.LerpCToA(myScript.duration);
        }

        // 确保修改被保存到场景中（在编辑器运行时可能不是必须的，但对于编辑器工具是好习惯）
        if (GUI.changed)
        {
            EditorUtility.SetDirty(myScript);
        }
    }
}