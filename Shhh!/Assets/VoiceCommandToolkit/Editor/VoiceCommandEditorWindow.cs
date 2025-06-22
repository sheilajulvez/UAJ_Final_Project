using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class VoiceCommandEditorWindow : EditorWindow {
    private List<string> commands = new();
    private string newCommand = "";

    [MenuItem("Tools/Voice Command Editor")]
    public static void ShowWindow() {
        GetWindow<VoiceCommandEditorWindow>("Voice Command Editor");
    }

    private void OnGUI() {
        GUILayout.Label("Añadir nuevo comando de voz", EditorStyles.boldLabel);

        GUI.SetNextControlName("CommandInputField");
        newCommand = EditorGUILayout.TextField("Comando:", newCommand);

        if (GUILayout.Button("Añadir")) {
            if (!string.IsNullOrWhiteSpace(newCommand) && !commands.Contains(newCommand)) {
                commands.Add(newCommand.Trim().ToLower());

                newCommand = "";
                GUI.FocusControl(null);
                EditorApplication.delayCall += () => {
                    EditorGUI.FocusTextInControl("CommandInputField");
                };
            }
        }


        GUILayout.Space(10);
        GUILayout.Label("Comandos actuales:");

        int indexToRemove = -1;

        for (int i = 0; i < commands.Count; i++) {
            GUILayout.BeginHorizontal();
            GUILayout.Label($"- {commands[i]} → {ToClassName(commands[i])}");
            if (GUILayout.Button("Eliminar", GUILayout.Width(70))) {
                indexToRemove = i; // Guardamos para eliminar después
            }
            GUILayout.EndHorizontal();
        }

        if (indexToRemove >= 0) {
            commands.RemoveAt(indexToRemove);
        }

        GUILayout.Space(20);
        EditorGUILayout.TextArea(
            "Si la clase NombreComandoAction no está en esta lista y en el JSON creado, no se detectará.",
            new GUIStyle(EditorStyles.label) {
                wordWrap = true
            }
        );

        GUILayout.Space(10);
        if (GUILayout.Button("Generar JSON y scripts")) {
            GenerateJSON();
            GenerateActionScripts();
            AssetDatabase.Refresh();
            Debug.Log("Comandos generados correctamente.");
        }
    }

    private string ToClassName(string command) {
        string sanitized = System.Globalization.CultureInfo.CurrentCulture.TextInfo
            .ToTitleCase(command.ToLower().Replace(" ", ""));
        return sanitized + "Action";
    }

    private void GenerateJSON() {
        var list = new VoiceCommandDefinitionList {
            definitions = new List<VoiceCommandDefinition>()
        };
        foreach (var cmd in commands) {
            list.definitions.Add(new VoiceCommandDefinition {
                Command = cmd,
                ActionClassName = ToClassName(cmd)
            });
        }

        string json = JsonUtility.ToJson(list, true);
        string path = "Assets/VoiceCommandToolkit/VoiceCommands/commands.json";
        File.WriteAllText(path, json);
    }

    private void GenerateActionScripts() {
        string folderPath = "Assets/VoiceCommandToolkit/Scripts/Actions";
        if (!Directory.Exists(folderPath)) {
            Directory.CreateDirectory(folderPath);
        }

        foreach (var cmd in commands) {
            string className = ToClassName(cmd);
            string filePath = Path.Combine(folderPath, className + ".cs");
            if (File.Exists(filePath)) continue;

            string template =
$@"using System;
using System.Collections.Generic;
using UnityEngine;

public class {className} : IVoiceAction {{
    public void Execute() {{
        // TODO: Implementar lógica para '{cmd}'
    }}
}}";
            File.WriteAllText(filePath, template);
        }
    }
}
