using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Globalization;

public class VoiceCommandEditorWindow : EditorWindow
{
    private List<VoiceCommandDefinition> commands = new();
    private Dictionary<string, string> newParams = new(); // nuevo parámetro auxiliar por comando
    private string newCommand = "";
    private Vector2 scroll;

    [MenuItem("Tools/Voice Command Editor")]
    public static void ShowWindow()
    {
        var window = GetWindow<VoiceCommandEditorWindow>("Voice Command Editor");
        window.LoadCommandsFromJSON();
    }

    private void OnGUI()
    {
        GUILayout.Label("Añadir nuevo comando de voz", EditorStyles.boldLabel);
        newCommand = EditorGUILayout.TextField("Comando:", newCommand);

        if (GUILayout.Button("Añadir Comando"))
        {
            string trimmedLower = newCommand.Trim().ToLower();
            if (!string.IsNullOrWhiteSpace(newCommand) && !commands.Exists(c => c.Command == trimmedLower))
            {
                commands.Add(new VoiceCommandDefinition
                {
                    Command = trimmedLower,
                    ActionClassName = ToClassName(trimmedLower),
                    Parameters = new List<string>()
                });
                newParams[trimmedLower] = "";
                newCommand = "";
            }
        }

        GUILayout.Space(10);
        scroll = EditorGUILayout.BeginScrollView(scroll);

        int commandToRemove = -1;

        for (int i = 0; i < commands.Count; i++)
        {
            var cmd = commands[i];
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField($"Comando: {cmd.Command} → {ToClassName(cmd.Command)}", EditorStyles.boldLabel);

            // Para eliminar sin alterar índice, iteramos al revés
            for (int j = cmd.Parameters.Count - 1; j >= 0; j--)
            {
                EditorGUILayout.BeginHorizontal();
                cmd.Parameters[j] = EditorGUILayout.TextField($"Parámetro {j + 1}:", cmd.Parameters[j]);
                if (GUILayout.Button("X", GUILayout.Width(20)))
                {
                    cmd.Parameters.RemoveAt(j);
                }
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.BeginHorizontal();

            if (!newParams.ContainsKey(cmd.Command))
                newParams[cmd.Command] = "";

            newParams[cmd.Command] = EditorGUILayout.TextField("Nuevo parámetro:", newParams[cmd.Command]);

            if (GUILayout.Button("+", GUILayout.Width(30)))
            {
                string np = newParams[cmd.Command]?.Trim();
                if (!string.IsNullOrWhiteSpace(np))
                {
                    cmd.Parameters.Add(np);
                    newParams[cmd.Command] = "";
                }
            }
            EditorGUILayout.EndHorizontal();

            if (GUILayout.Button("Eliminar comando"))
                commandToRemove = i;

            EditorGUILayout.EndVertical();
        }

        if (commandToRemove >= 0)
        {
            var cmdToRemove = commands[commandToRemove];
            commands.RemoveAt(commandToRemove);
            if (newParams.ContainsKey(cmdToRemove.Command))
                newParams.Remove(cmdToRemove.Command);
        }

        EditorGUILayout.EndScrollView();

        GUILayout.Space(10);
        if (GUILayout.Button("Guardar y generar scripts"))
        {
            GenerateJSON();
            GenerateActionScripts();
            AssetDatabase.Refresh();
            Debug.Log("Comandos generados correctamente.");
        }
    }

    private string ToClassName(string command)
    {
        string sanitized = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(command.ToLower().Replace(" ", ""));
        return sanitized + "Action";
    }

    private void GenerateJSON()
    {
        var list = new VoiceCommandDefinitionList
        {
            definitions = new List<VoiceCommandDefinition>()
        };

        foreach (var cmd in commands)
        {
            list.definitions.Add(new VoiceCommandDefinition
            {
                Command = cmd.Command,
                ActionClassName = ToClassName(cmd.Command),
                Parameters = new List<string>(cmd.Parameters)
            });
        }

        string json = JsonUtility.ToJson(list, true);
        string dir = "Assets/VoiceCommandToolkit/VoiceCommands";
        string path = Path.Combine(dir, "commands.json");

        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);

        File.WriteAllText(path, json);
    }

    private void GenerateActionScripts()
    {
        string folderPath = "Assets/VoiceCommandToolkit/Scripts/Actions";
        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);

        string jsonPath = Path.Combine("Assets/VoiceCommandToolkit/VoiceCommands", "commands.json");
        VoiceCommandDefinitionList oldList = null;
        if (File.Exists(jsonPath))
        {
            string oldJson = File.ReadAllText(jsonPath);
            oldList = JsonUtility.FromJson<VoiceCommandDefinitionList>(oldJson);
        }

        foreach (var cmd in commands)
        {
            string className = ToClassName(cmd.Command);
            string filePath = Path.Combine(folderPath, className + ".cs");

            bool shouldGenerate = true;

            if (oldList != null)
            {
                var oldCmd = oldList.definitions.Find(d => d.Command == cmd.Command);
                if (oldCmd != null)
                {
                    bool parametersEqual = false;
                    if ((oldCmd.Parameters == null && (cmd.Parameters == null || cmd.Parameters.Count == 0)) ||
                        (oldCmd.Parameters != null && cmd.Parameters != null && oldCmd.Parameters.Count == cmd.Parameters.Count))
                    {
                        parametersEqual = true;
                        for (int i = 0; i < oldCmd.Parameters.Count; i++)
                        {
                            if (oldCmd.Parameters[i] != cmd.Parameters[i])
                            {
                                parametersEqual = false;
                                break;
                            }
                        }
                    }

                    if (parametersEqual)
                    {
                        shouldGenerate = !File.Exists(filePath);
                    }
                }
            }

            if (shouldGenerate)
            {
                string paramListComment = string.Join(", ", cmd.Parameters);
                string script =
$@"using UnityEngine;
using AudioDetection.Interfaces;

public class {className} : IVoiceAction {{
    public void Execute(object[] parameters) {{
        // TODO: Implementar lógica para '{cmd.Command}'
        // Parámetros esperados: {paramListComment}
    }}
}}";

                File.WriteAllText(filePath, script);
            }
        }
    }

    private void LoadCommandsFromJSON()
    {
        commands.Clear();
        newParams.Clear();

        string path = "Assets/VoiceCommandToolkit/VoiceCommands/commands.json";
        if (!File.Exists(path)) return;

        string json = File.ReadAllText(path);
        var list = JsonUtility.FromJson<VoiceCommandDefinitionList>(json);

        if (list?.definitions != null)
        {
            foreach (var def in list.definitions)
            {
                commands.Add(new VoiceCommandDefinition
                {
                    Command = def.Command,
                    ActionClassName = def.ActionClassName,
                    Parameters = def.Parameters ?? new List<string>()
                });
                newParams[def.Command] = "";
            }
        }
    }
}
