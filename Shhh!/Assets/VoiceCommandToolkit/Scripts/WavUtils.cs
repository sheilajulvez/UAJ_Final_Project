using System;
using System.IO;
using UnityEngine;

public static class WavUtils
{
    const int HEADER_SIZE = 44;

    public static void Save(string filepath, AudioClip clip)
    {
        if (!filepath.ToLower().EndsWith(".wav"))
            filepath += ".wav";

        var samples = new float[clip.samples * clip.channels];
        clip.GetData(samples, 0);

        using (var fileStream = CreateEmpty(filepath))
        {
            ConvertAndWrite(fileStream, samples, clip.channels);
            WriteHeader(fileStream, clip.channels, clip.frequency, samples.Length);
        }
    }

    static FileStream CreateEmpty(string filepath)
    {
        var fileStream = new FileStream(filepath, FileMode.Create);
        for (int i = 0; i < HEADER_SIZE; i++) fileStream.WriteByte(0);
        return fileStream;
    }

    static void ConvertAndWrite(FileStream fileStream, float[] samples, int channels)
    {
        Int16[] intData = new Int16[samples.Length];
        Byte[] bytesData = new Byte[samples.Length * 2];

        const float rescaleFactor = 32767; // float [-1,1] -> Int16

        for (int i = 0; i < samples.Length; i++)
        {
            intData[i] = (short)(samples[i] * rescaleFactor);
            byte[] byteArr = BitConverter.GetBytes(intData[i]);
            byteArr.CopyTo(bytesData, i * 2);
        }
        fileStream.Write(bytesData, 0, bytesData.Length);
    }

    static void WriteHeader(FileStream fileStream, int channels, int sampleRate, int samples)
    {
        fileStream.Seek(0, SeekOrigin.Begin);

        byte[] riff = System.Text.Encoding.UTF8.GetBytes("RIFF");
        fileStream.Write(riff, 0, 4);

        byte[] chunkSize = BitConverter.GetBytes(fileStream.Length - 8);
        fileStream.Write(chunkSize, 0, 4);

        byte[] wave = System.Text.Encoding.UTF8.GetBytes("WAVE");
        fileStream.Write(wave, 0, 4);

        byte[] fmt = System.Text.Encoding.UTF8.GetBytes("fmt ");
        fileStream.Write(fmt, 0, 4);

        byte[] subChunk1 = BitConverter.GetBytes(16);
        fileStream.Write(subChunk1, 0, 4);

        UInt16 audioFormat = 1;
        byte[] audioFormatBytes = BitConverter.GetBytes(audioFormat);
        fileStream.Write(audioFormatBytes, 0, 2);

        UInt16 numChannels = (ushort)channels;
        byte[] channelsBytes = BitConverter.GetBytes(numChannels);
        fileStream.Write(channelsBytes, 0, 2);

        byte[] sampleRateBytes = BitConverter.GetBytes(sampleRate);
        fileStream.Write(sampleRateBytes, 0, 4);

        byte[] byteRate = BitConverter.GetBytes(sampleRate * channels * 2);
        fileStream.Write(byteRate, 0, 4);

        UInt16 blockAlign = (ushort)(channels * 2);
        byte[] blockAlignBytes = BitConverter.GetBytes(blockAlign);
        fileStream.Write(blockAlignBytes, 0, 2);

        UInt16 bitsPerSample = 16;
        byte[] bitsPerSampleBytes = BitConverter.GetBytes(bitsPerSample);
        fileStream.Write(bitsPerSampleBytes, 0, 2);

        byte[] dataString = System.Text.Encoding.UTF8.GetBytes("data");
        fileStream.Write(dataString, 0, 4);

        byte[] subChunk2 = BitConverter.GetBytes(samples * 2);
        fileStream.Write(subChunk2, 0, 4);
    }
}
