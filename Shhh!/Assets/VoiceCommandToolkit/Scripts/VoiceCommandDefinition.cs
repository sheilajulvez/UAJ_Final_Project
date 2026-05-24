
using System;
using System.Collections.Generic;



[Serializable]
public class VoiceCommandDefinition
{
    public string Command;
    public string ActionClassName;
    public List<string> Parameters;
}

[Serializable]
public class VoiceCommandDefinitionList
{
    public List<VoiceCommandDefinition> definitions;
}
