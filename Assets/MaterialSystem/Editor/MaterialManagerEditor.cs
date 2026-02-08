using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(MaterialManager))]
public class MaterialManagerEditor : Editor
{
    private VisualElement root;
    private MaterialManager manager;

    public override VisualElement CreateInspectorGUI()
    {
        manager = (MaterialManager)target;

        root = new VisualElement();
        root.style.paddingLeft = 6;
        root.style.paddingRight = 6;
        root.style.paddingTop = 4;

        DrawHeader();
        DrawMaterialList();
        DrawFooter();

        return root;
    }

    private void DrawHeader()
    {
        var header = new Label("Material Manager");
        header.style.unityFontStyleAndWeight = FontStyle.Bold;
        header.style.fontSize = 14;
        header.style.marginBottom = 6;
        root.Add(header);
    }

    private void DrawMaterialList()
    {
        if (manager == null)
            return;

        foreach (var material in Resources.LoadAll<Material>(""))
        {
            DrawMaterialRow(material);
        }
    }

    private void DrawMaterialRow(Material material)
    {
        var row = new VisualElement();
        row.style.flexDirection = FlexDirection.Row;
        row.style.marginBottom = 2;

        var nameLabel = new Label(material.name);
        nameLabel.style.flexGrow = 1;
        nameLabel.style.unityTextAlign = TextAnchor.MiddleLeft;

        int currentCount = manager.GetMaterialCount(material);

        var countField = new IntegerField
        {
            value = currentCount
        };
        countField.style.width = 60;

        countField.RegisterValueChangedCallback(evt =>
        {
            Undo.RecordObject(manager, "Change Material Count");
            manager.SetMaterialCount(material, Mathf.Max(0, evt.newValue));
            EditorUtility.SetDirty(manager);
        });

        row.Add(nameLabel);
        row.Add(countField);

        root.Add(row);
    }

    private void DrawFooter()
    {
        var spacer = new VisualElement();
        spacer.style.height = 6;
        root.Add(spacer);

        var resetButton = new Button(() =>
        {
            Undo.RecordObject(manager, "Reset Materials");
            manager.RestoreToDefault();
            EditorUtility.SetDirty(manager);
            root.Clear();
            CreateInspectorGUI();
        })
        {
            text = "Reset To Default"
        };

        root.Add(resetButton);
    }
}
