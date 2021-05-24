using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GridGenerator))]
public class GridGeneratorEditor : Editor
{
    GridGenerator grid;
    Editor shapeEditor;
    Editor colourEditor;

    public override void OnInspectorGUI()
    {
        using (var check = new EditorGUI.ChangeCheckScope())
        {
            base.OnInspectorGUI();
            if (check.changed)
            {
                grid.OnValidate();
            }
        }

        if (GUILayout.Button("Generate Planet"))
        {
            grid.GenerateButton();
        }

        DrawSettingsEditor(grid.shapeSettings, grid.OnShapeSettingsUpdated, ref grid.shapeSettingsFoldout, ref shapeEditor);
        
        DrawSettingsEditor(grid.colourSettings, grid.OnColourSettingsUpdated, ref grid.colourSettingsFoldout, ref colourEditor);
    }

    void DrawSettingsEditor(Object settings, System.Action onSettingsUpdated, ref bool foldout, ref Editor editor)
    {
        if (settings != null)
        {
            foldout = EditorGUILayout.InspectorTitlebar(foldout, settings);
            using (var check = new EditorGUI.ChangeCheckScope())
            {
                if (foldout)
                {
                    CreateCachedEditor(settings, null, ref editor);
                    editor.OnInspectorGUI();

                    if (check.changed)
                    {
                        if (onSettingsUpdated != null)
                        {
                            onSettingsUpdated();
                        }
                    }
                }
            }
        }
    }

    private void OnEnable()
    {
        grid = (GridGenerator)target;
    }
}
