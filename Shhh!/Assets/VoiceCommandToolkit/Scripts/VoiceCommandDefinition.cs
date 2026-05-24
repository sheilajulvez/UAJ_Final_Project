
using System;
using System.Collections.Generic;



[Serializable]
public class VoiceCommandDefinition
{
    public string Command;
    public string ActionClassName;
    public List<string> Parameters;
    public List<string> Contexts;
}

[Serializable]
public class VoiceCommandDefinitionList
{
    public List<VoiceCommandDefinition> definitions;
}
