using System;
using System.Collections.Generic;

[Serializable]
public class VoiceCommandDefinition
{
    public string Command;
    public string ActionClassName;
    public List<string> Parameters;

    /// <summary>
    /// Frases equivalentes que el usuario puede pronunciar en lugar del comando principal.
    /// </summary>
    public List<string> Aliases;
}

[Serializable]
public class VoiceCommandDefinitionList
{
    public List<VoiceCommandDefinition> definitions;
}
