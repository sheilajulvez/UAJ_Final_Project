using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class WhisperInterop
{
    [DllImport("SpeechToText.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr transcribe_file(string modelPath, string wavPath);

    [DllImport("SpeechToText.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern void free_transcription(IntPtr ptr);


    public static string Transcribe(string modelPath, string wavPath)
    {
        IntPtr ptr = transcribe_file(modelPath, wavPath);
        if (ptr == IntPtr.Zero)
            return "Transcription failed or returned null.";

        string result = Marshal.PtrToStringAnsi(ptr);

        free_transcription(ptr);

        return result;
    }
}
