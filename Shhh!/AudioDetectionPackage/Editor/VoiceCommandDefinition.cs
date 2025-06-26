using System;
using System.Collections.Generic;


// Definicion para estructurar el JSON y que el VoiceLoader sepa leerlo
[Serializable]
public class VoiceCommandDefinition {
    public string Command;
    public string ActionClassName;
}

[Serializable]
public class VoiceCommandDefinitionList {
    public List<VoiceCommandDefinition> definitions;
}
