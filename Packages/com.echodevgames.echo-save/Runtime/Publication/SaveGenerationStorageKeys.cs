
namespace EchoDevGames.EchoSave
{
    internal readonly struct SaveGenerationStorageKeys
    {
        private SaveGenerationStorageKeys(
            SaveStorageKey candidateDirectory,
            SaveStorageKey candidatePayload,
            SaveStorageKey candidateManifest,
            SaveStorageKey generationDirectory,
            SaveStorageKey generationPayload,
            SaveStorageKey generationManifest,
            SaveStorageKey head)
        {
            CandidateDirectory =
                candidateDirectory;
            CandidatePayload =
                candidatePayload;
            CandidateManifest =
                candidateManifest;
            GenerationDirectory =
                generationDirectory;
            GenerationPayload =
                generationPayload;
            GenerationManifest =
                generationManifest;
            Head =
                head;
        }

        internal SaveStorageKey CandidateDirectory
        {
            get;
        }

        internal SaveStorageKey CandidatePayload
        {
            get;
        }

        internal SaveStorageKey CandidateManifest
        {
            get;
        }

        internal SaveStorageKey GenerationDirectory
        {
            get;
        }

        internal SaveStorageKey GenerationPayload
        {
            get;
        }

        internal SaveStorageKey GenerationManifest
        {
            get;
        }

        internal SaveStorageKey Head
        {
            get;
        }

        internal static SaveStorageResult TryCreate(
            SaveSlotId slotId,
            SaveGenerationId generationId,
            out SaveGenerationStorageKeys keys)
        {
            keys = default;

            if (!SaveSlotId.TryParse(
                    slotId.Value,
                    out SaveSlotId validatedSlot) ||
                !SaveGenerationId.TryParse(
                    generationId.Value,
                    out SaveGenerationId validatedGeneration))
            {
                return new SaveStorageResult(
                    SaveStorageStatus.InvalidPath,
                    EchoSaveDiagnosticCodes
                        .StorageInvalidPath,
                    "Chronicle generation publication requires valid technical slot and generation IDs.");
            }

            string slotRoot =
                "slots/" +
                validatedSlot.Value;

            string candidateRoot =
                slotRoot +
                "/incomplete/" +
                validatedGeneration.Value;

            string generationRoot =
                slotRoot +
                "/generations/" +
                validatedGeneration.Value;

            SaveStorageResult result =
                SaveStorageKey.TryCreate(
                    candidateRoot,
                    out SaveStorageKey candidateDirectory);

            if (!result.Succeeded)
            {
                return result;
            }

            result =
                SaveStorageKey.TryCreate(
                    candidateRoot +
                    "/payload.json",
                    out SaveStorageKey candidatePayload);

            if (!result.Succeeded)
            {
                return result;
            }

            result =
                SaveStorageKey.TryCreate(
                    candidateRoot +
                    "/manifest.json",
                    out SaveStorageKey candidateManifest);

            if (!result.Succeeded)
            {
                return result;
            }

            result =
                SaveStorageKey.TryCreate(
                    generationRoot,
                    out SaveStorageKey generationDirectory);

            if (!result.Succeeded)
            {
                return result;
            }

            result =
                SaveStorageKey.TryCreate(
                    generationRoot +
                    "/payload.json",
                    out SaveStorageKey generationPayload);

            if (!result.Succeeded)
            {
                return result;
            }

            result =
                SaveStorageKey.TryCreate(
                    generationRoot +
                    "/manifest.json",
                    out SaveStorageKey generationManifest);

            if (!result.Succeeded)
            {
                return result;
            }

            result =
                SaveStorageKey.TryCreate(
                    slotRoot +
                    "/head.json",
                    out SaveStorageKey head);

            if (!result.Succeeded)
            {
                return result;
            }

            keys =
                new SaveGenerationStorageKeys(
                    candidateDirectory,
                    candidatePayload,
                    candidateManifest,
                    generationDirectory,
                    generationPayload,
                    generationManifest,
                    head);

            return SaveStorageResult.Success(
                "Chronicle generation publication storage keys are valid.");
        }
    }
}
