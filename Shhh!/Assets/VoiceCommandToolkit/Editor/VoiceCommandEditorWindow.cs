using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Globalization;

public class VoiceCommandEditorWindow : EditorWindow
{
    private List<VoiceCommandDefinition> commands = new();
    private Dictionary<string, string> newParams = new();
    private Dictionary<string, string> newAliases = new();
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
        GUILayout.Label("Anadir nuevo comando de voz", EditorStyles.boldLabel);
        newCommand = EditorGUILayout.TextField("Comando:", newCommand);

        if (GUILayout.Button("Anadir Comando"))
        {
            string trimmedLower = newCommand.Trim().ToLower();

            if (!string.IsNullOrWhiteSpace(newCommand) && !commands.Exists(c => c.Command == trimmedLower))
            {
                commands.Add(new VoiceCommandDefinition
                {
                    Command = trimmedLower,
                    ActionClassName = ToClassName(trimmedLower),
                    Parameters = new List<string>(),
                    Aliases = new List<string>()
                });

                newParams[trimmedLower] = "";
                newAliases[trimmedLower] = "";
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
            cmd.Parameters ??= new List<string>();
            cmd.Aliases ??= new List<string>();

            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField($"Comando: {cmd.Command} -> {ToClassName(cmd.Command)}", EditorStyles.boldLabel);

            EditorGUILayout.LabelField("Parametros:", EditorStyles.miniBoldLabel);
            for (int j = cmd.Parameters.Count - 1; j >= 0; j--)
            {
                EditorGUILayout.BeginHorizontal();
                cmd.Parameters[j] = EditorGUILayout.TextField($"  {j + 1}:", cmd.Parameters[j]);
                if (GUILayout.Button("X", GUILayout.Width(20)))
                {
                    cmd.Parameters.RemoveAt(j);
                }
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.BeginHorizontal();
            if (!newParams.ContainsKey(cmd.Command))
            {
                newParams[cmd.Command] = "";
            }

            newParams[cmd.Command] = EditorGUILayout.TextField("  Nuevo parametro:", newParams[cmd.Command]);
            if (GUILayout.Button("+", GUILayout.Width(30)))
            {
                string newParameter = newParams[cmd.Command]?.Trim();
                if (!string.IsNullOrWhiteSpace(newParameter) && !cmd.Parameters.Contains(newParameter))
                {
                    cmd.Parameters.Add(newParameter);
                    newParams[cmd.Command] = "";
                    GUI.FocusControl(null);
                    Repaint();
                }
            }
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(4);
            EditorGUILayout.LabelField("Aliases (sinonimos aceptados):", EditorStyles.miniBoldLabel);

            if (cmd.Aliases.Count == 0)
            {
                EditorGUILayout.LabelField("  Sin aliases definidos.", EditorStyles.miniLabel);
            }
            else
            {
                for (int k = cmd.Aliases.Count - 1; k >= 0; k--)
                {
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(16);
                    EditorGUILayout.LabelField($"- {cmd.Aliases[k]}", GUILayout.ExpandWidth(true));
                    if (GUILayout.Button("X", GUILayout.Width(20)))
                    {
                        cmd.Aliases.RemoveAt(k);
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }

            EditorGUILayout.BeginHorizontal();
            if (!newAliases.ContainsKey(cmd.Command))
            {
                newAliases[cmd.Command] = "";
            }

            newAliases[cmd.Command] = EditorGUILayout.TextField("  Nuevo alias:", newAliases[cmd.Command]);
            if (GUILayout.Button("+", GUILayout.Width(30)))
            {
                string newAlias = newAliases[cmd.Command]?.Trim().ToLower();
                if (!string.IsNullOrWhiteSpace(newAlias) && !cmd.Aliases.Contains(newAlias))
                {
                    cmd.Aliases.Add(newAlias);
                    newAliases[cmd.Command] = "";
                }
            }
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(4);

            if (GUILayout.Button("Eliminar comando"))
            {
                commandToRemove = i;
            }

            EditorGUILayout.EndVertical();
            GUILayout.Space(4);
        }

        if (commandToRemove >= 0)
        {
            var cmdToRemove = commands[commandToRemove];
            commands.RemoveAt(commandToRemove);
            newParams.Remove(cmdToRemove.Command);
            newAliases.Remove(cmdToRemove.Command);
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
                Parameters = new List<string>(cmd.Parameters ?? new List<string>()),
                Aliases = new List<string>(cmd.Aliases ?? new List<string>())
            });
        }

        string json = JsonUtility.ToJson(list, true);
        string dir = "Assets/VoiceCommandToolkit/VoiceCommands";
        string path = Path.Combine(dir, "commands.json");

        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }

        File.WriteAllText(path, json);
    }

    private void GenerateActionScripts()
    {
        string folderPath = "Assets/VoiceCommandToolkit/Scripts/Actions";
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

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
                        (oldCmd.Parameters != null && cmd.Parameters != null &&
                         oldCmd.Parameters.Count == cmd.Parameters.Count))
                    {
                        parametersEqual = true;
                        for (int i = 0; i < (oldCmd.Parameters?.Count ?? 0); i++)
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
                string paramListComment = string.Join(", ", cmd.Parameters ?? new List<string>());
                string aliasListComment = cmd.Aliases != null && cmd.Aliases.Count > 0
                    ? string.Join(", ", cmd.Aliases)
                    : "ninguno";

                string script =
$@"using UnityEngine;
using AudioDetection.Interfaces;

public class {className} : IVoiceAction {{
    public void Execute(object[] parameters) {{
        // TODO: Implementar logica para '{cmd.Command}'
        // Parametros esperados: {paramListComment}
        // Aliases registrados: {aliasListComment}
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
        newAliases.Clear();

        string path = "Assets/VoiceCommandToolkit/VoiceCommands/commands.json";
        if (!File.Exists(path))
        {
            return;
        }

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
                    Aliases = def.Aliases ?? new List<string>()
                });
                newParams[def.Command] = "";
                newAliases[def.Command] = "";
            }
        }
    }
}
