using System;
using System.Collections.Generic;
using UnityEngine;
using AudioDetection.Interfaces;
namespace AudioDetection
{
    public class VoiceLoader : MonoBehaviour
    { // Clase que lee el JSON con los comandos y los registra al manager y a la engine, asociando los eventos
        public TextAsset commandFile;
        private VoiceInputEngine inputEngine;

        void Start()
        {
            var data = JsonUtility.FromJson<VoiceCommandDefinitionList>(commandFile.text);

            List<string> keywords = new();
            foreach (var def in data.definitions)
            {
                var type = Type.GetType(def.ActionClassName);
                if (type != null && typeof(IVoiceAction).IsAssignableFrom(type))
                {
                    var action = (IVoiceAction)Activator.CreateInstance(type);
                    VoiceCommandManager.Instance.RegisterCommand(def.Command, action);
                    keywords.Add(def.Command);
                }
                else
                {
                    Debug.LogWarning($"No se pudo crear instancia de {def.ActionClassName}");
                }
            }

            inputEngine = GetComponent<VoiceInputEngine>();
            inputEngine.Initialize(keywords.ToArray());
            inputEngine.OnCommandRecognized += VoiceCommandManager.Instance.HandleCommand;
        }
    }



}


