#include "common.h"
#include "common-whisper.h"
#include "whisper.h"
#include <vector>
#include <string>
#include <fstream>
#include <sstream>
#include <thread>
#include <cstdlib>

// Exportar funciones en C
extern "C" {

    __declspec(dllexport) char* transcribe_file(const char* model_path, const char* wav_path) {
        // Load model
        struct whisper_context_params cparams = whisper_context_default_params();
        cparams.use_gpu = false;

        struct whisper_context* ctx = whisper_init_from_file_with_params(model_path, cparams);
        if (!ctx) {
            fprintf(stderr, "Failed to load model\n");
            return nullptr;
        }

        // Read audio
        std::vector<float> pcmf32;
        std::vector<std::vector<float>> pcmf32s;

        if (!read_audio_data(wav_path, pcmf32, pcmf32s, false)) {
            fprintf(stderr, "Failed to read audio\n");
            whisper_free(ctx);
            return nullptr;
        }

        // Set parameters
        whisper_full_params wparams = whisper_full_default_params(WHISPER_SAMPLING_GREEDY);
        wparams.print_realtime = false;
        wparams.print_progress = false;
        wparams.print_timestamps = false;
        wparams.print_special = false;
        wparams.translate = false;
        wparams.language = "en";
        wparams.n_threads = std::thread::hardware_concurrency();
        wparams.no_timestamps = true;

        if (whisper_full(ctx, wparams, pcmf32.data(), pcmf32.size()) != 0) {
            fprintf(stderr, "Failed to run whisper\n");
            whisper_free(ctx);
            return nullptr;
        }

        // Get result
        std::stringstream ss;
        const int n_segments = whisper_full_n_segments(ctx);
        for (int i = 0; i < n_segments; ++i) {
            const char* text = whisper_full_get_segment_text(ctx, i);
            ss << text;
        }

        whisper_free(ctx);

        // Allocate memory to return string
        std::string result = ss.str();
        char* out = (char*)malloc(result.size() + 1);
        std::copy(result.begin(), result.end(), out);
        out[result.size()] = '\0';

        return out;
    }

    // Función para liberar memoria desde fuera (por ejemplo en C# o Unity)
    __declspec(dllexport) void free_transcription(char* ptr) {
        free(ptr);
    }
}
