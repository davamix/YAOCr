# YAOCr

YAOCr is a RAG tool for windows desktop ([WinUI 3](https://github.com/microsoft/microsoft-ui-xaml)) that allows you to chat with an LLM model and work with your own documents (.txt, .json...).

### Tools
- LLM Backend: [Llama.cpp](https://github.com/ggml-org/llama.cpp)
- Vector DB: [Qdrant](https://qdrant.tech/documentation/)
- LLM model: [Gemma 3](https://huggingface.co/unsloth/gemma-3-4b-it-GGUF)
- Embedding model: [Embedding Gemma](https://huggingface.co/unsloth/embeddinggemma-300m-GGUF)

### Docker
- Replace `d:\\llama.cpp\\models` by the path where the models are saved

Llama.cpp
```
docker run --name llama_cpp -p 8010:8010 -v d:\\llama.cpp\\models:/models ghcr.io/ggml-org/llama.cpp:server -m /models/gemma-3-4b-it-UD-Q4_K_XL.gguf --host 0.0.0.0 --port 8010
```

Llama.cpp GPU version
```
docker run --name llama_cpp_gpu -p 8010:8010 -v d:\\llama.cpp\\models:/models --gpus all ghcr.io/ggml-org/llama.cpp:server-cuda -m /models/gemma-3-4b-it-UD-Q4_K_XL.gguf --host 0.0.0.0 --port 8010 --n-gpu-layers 99
```

Embeddings
```
docker run --name embeddings -p 8001:8001 -v d:\\llama.cpp\\models:/models ghcr.io/ggml-org/llama.cpp:server -m /models/embeddinggemma-300M-BF16.gguf -c 2048 -ub 2048 --host 0.0.0.0 --port 8001 --embeddings -ngl 99
```

Embeddings GPU version
```
docker run --name embeddings_gpu -p 8001:8001 -v d:\\llama.cpp\\models:/models --gpus all ghcr.io/ggml-org/llama.cpp:server-cuda -m /models/embeddinggemma-300M-BF16.gguf -c 2048 -ub 2048 --host 0.0.0.0 --port 8001 --embeddings -ngl 99
```

Qdrant (https://qdrant.tech/documentation/quickstart/)
```
docker run -p 6333:6333 -p 6334:6334 -v "$(pwd)/qdrant_storage:/qdrant/storage:z" qdrant/qdrant
```

### Documents support
Only support the following types:
- `text/plain`
- `application/json`
- `.csv`
- `.sql`

## Demo
[YAOCr_demo.webm](https://github.com/user-attachments/assets/66168b9d-8d9a-49cb-8da4-3c64840ad4f0)

## Chat window
<img width="1431" height="764" alt="yaocr_chat" src="https://github.com/user-attachments/assets/7f5ed80d-5524-4903-b9f3-ad89a0bb73a7" />


## Conversation tools (Rename, Delete, Export)
<img width="370" height="235" alt="yaocr_chat_tools" src="https://github.com/user-attachments/assets/b56e53fd-f865-43da-beaa-dcd09aaafa74" />

## Settings window
<img width="1080" height="720" alt="yaocr_settings" src="https://github.com/user-attachments/assets/707f203e-9bb4-4ac5-898c-d7e7f155a1a3" />

## Dark mode
<img width="1431" height="764" alt="yaocr_chat_dark" src="https://github.com/user-attachments/assets/88f91669-976d-4ca0-9960-617512d0fb47" />




