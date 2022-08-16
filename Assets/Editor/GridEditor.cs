using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GridManager))]
public class BoardEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUILayout.Space(8);

        if (GUILayout.Button("Generate Grid"))
        {
            GridManager generator = (GridManager)target;
            generator.Generate();
        }

        GUILayout.Space(8);

        if (GUILayout.Button("Update Links"))
        {
            GridManager generator = (GridManager)target;
            generator.Link();
        }
    }
}
