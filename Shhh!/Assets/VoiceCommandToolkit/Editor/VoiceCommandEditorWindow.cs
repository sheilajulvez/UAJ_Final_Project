using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Globalization;

public class VoiceCommandEditorWindow : EditorWindow
{
    private List<VoiceCommandDefinition> commands = new();
    private Dictionary<string, string> newParams = new(); // nuevo parámetro auxiliar por comando
    private Dictionary<string, string> newContexts = new();
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
                    Parameters = new List<string>(),
                    Contexts = new List<string>()
                });

                newParams[trimmedLower] = "";
                newContexts[trimmedLower] = "";

                newCommand = "";

                GUI.FocusControl(null);
                Repaint();
            }
        }

        GUILayout.Space(10);
        scroll = EditorGUILayout.BeginScrollView(scroll);

        int commandToRemove = -1;

        for (int i = 0; i < commands.Count; i++)
        {
            var cmd = commands[i];

            if (cmd.Parameters == null)
                cmd.Parameters = new List<string>();

            if (cmd.Contexts == null)
                cmd.Contexts = new List<string>();

            EditorGUILayout.BeginVertical("box");

            EditorGUILayout.LabelField(
                $"Comando: {cmd.Command} → {ToClassName(cmd.Command)}",
                EditorStyles.boldLabel
            );

            
            // PARÁMETROS
            
            GUILayout.Space(5);
            EditorGUILayout.LabelField("Parámetros", EditorStyles.boldLabel);

            int parameterToRemove = -1;

            for (int j = 0; j < cmd.Parameters.Count; j++)
            {
                EditorGUILayout.BeginHorizontal();

                EditorGUI.BeginChangeCheck();

                string editedParameter = EditorGUILayout.TextField(
                    $"Parámetro {j + 1}:",
                    cmd.Parameters[j]
                );

                if (EditorGUI.EndChangeCheck())
                {
                    cmd.Parameters[j] = editedParameter.Trim();
                }

                if (GUILayout.Button("X", GUILayout.Width(20)))
                {
                    parameterToRemove = j;
                }

                EditorGUILayout.EndHorizontal();
            }

            if (parameterToRemove >= 0)
            {
                cmd.Parameters.RemoveAt(parameterToRemove);

                GUI.FocusControl(null);
                Repaint();
            }

            EditorGUILayout.BeginHorizontal();

            if (!newParams.ContainsKey(cmd.Command))
                newParams[cmd.Command] = "";

            newParams[cmd.Command] = EditorGUILayout.TextField(
                "Nuevo parámetro:",
                newParams[cmd.Command]
            );

            if (GUILayout.Button("+", GUILayout.Width(30)))
            {
                string newParameter = newParams[cmd.Command]?.Trim();

                if (!string.IsNullOrWhiteSpace(newParameter) &&
                    !cmd.Parameters.Contains(newParameter))
                {
                    cmd.Parameters.Add(newParameter);
                    newParams[cmd.Command] = "";

                    GUI.FocusControl(null);
                    Repaint();
                }
            }

            EditorGUILayout.EndHorizontal();

            
            // CONTEXTOS
            
            GUILayout.Space(8);
            EditorGUILayout.LabelField("Contextos válidos", EditorStyles.boldLabel);

            int contextToRemove = -1;

            for (int j = 0; j < cmd.Contexts.Count; j++)
            {
                EditorGUILayout.BeginHorizontal();

                EditorGUI.BeginChangeCheck();

                string editedContext = EditorGUILayout.TextField(
                    $"Contexto {j + 1}:",
                    cmd.Contexts[j]
                );

                if (EditorGUI.EndChangeCheck())
                {
                    cmd.Contexts[j] = editedContext.Trim().ToUpper();
                }

                if (GUILayout.Button("X", GUILayout.Width(20)))
                {
                    contextToRemove = j;
                }

                EditorGUILayout.EndHorizontal();
            }

            if (contextToRemove >= 0)
            {
                cmd.Contexts.RemoveAt(contextToRemove);

                GUI.FocusControl(null);
                Repaint();
            }

            EditorGUILayout.BeginHorizontal();

            if (!newContexts.ContainsKey(cmd.Command))
                newContexts[cmd.Command] = "";

            newContexts[cmd.Command] = EditorGUILayout.TextField(
                "Nuevo contexto:",
                newContexts[cmd.Command]
            );

            if (GUILayout.Button("+", GUILayout.Width(30)))
            {
                string newContext = newContexts[cmd.Command]?.Trim().ToUpper();

                if (!string.IsNullOrWhiteSpace(newContext) &&
                    !cmd.Contexts.Contains(newContext))
                {
                    cmd.Contexts.Add(newContext);
                    newContexts[cmd.Command] = "";

                    GUI.FocusControl(null);
                    Repaint();
                }
            }

            EditorGUILayout.EndHorizontal();

            GUILayout.Space(5);

            if (GUILayout.Button("Eliminar comando"))
            {
                commandToRemove = i;
            }

            EditorGUILayout.EndVertical();
        }

        if (commandToRemove >= 0)
        {
            var cmdToRemove = commands[commandToRemove];

            commands.RemoveAt(commandToRemove);

            if (newParams.ContainsKey(cmdToRemove.Command))
                newParams.Remove(cmdToRemove.Command);

            if (newContexts.ContainsKey(cmdToRemove.Command))
                newContexts.Remove(cmdToRemove.Command);

            GUI.FocusControl(null);
            Repaint();
        }

        EditorGUILayout.EndScrollView();

        GUILayout.Space(10);

        if (GUILayout.Button("Guardar y generar scripts"))
        {
            GenerateJSON();
            GenerateActionScripts();

            AssetDatabase.Refresh();

            GUI.FocusControl(null);
            Repaint();

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
                Parameters = new List<string>(cmd.Parameters),
                Contexts = new List<string>(cmd.Contexts)
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
                    Parameters = def.Parameters ?? new List<string>(),
                    Contexts = def.Contexts ?? new List<string>()
                });
                newParams[def.Command] = "";
                newContexts[def.Command] = "";
            }
        }
    }
}
