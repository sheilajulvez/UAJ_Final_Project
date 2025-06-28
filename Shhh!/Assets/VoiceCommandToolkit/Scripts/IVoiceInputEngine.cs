using System;

namespace AudioDetection.Interfaces
{
    public interface IVoiceInputEngine
    {
        /// <summary>
        /// Inicializa el motor con las palabras clave a reconocer.
        /// </summary>
        /// <param name="commands">Array de comandos o palabras clave a detectar.</param>
        void Initialize(string[] commands);

        /// <summary>
        /// Evento que se dispara cuando se reconoce un comando con sus parámetros.
        /// </summary>
        event Action<string, object[]> OnCommandRecognized;
    }
}
