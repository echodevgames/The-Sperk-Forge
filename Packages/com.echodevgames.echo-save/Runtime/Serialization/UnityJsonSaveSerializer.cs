
using System;
using UnityEngine;

namespace EchoDevGames.EchoSave
{
    /// <summary>
    /// Default Chronicle JSON serializer.
    ///
    /// Uses Unity JsonUtility for plain serializable DTOs. JsonUtility does not
    /// provide arbitrary dictionary, interface, polymorphic object-graph, or
    /// durable Unity-object-reference serialization. Projects requiring other
    /// shapes should provide another ISaveSerializer.
    /// </summary>
    public sealed class UnityJsonSaveSerializer :
        ISaveSerializer
    {
        public const string StableId =
            "echodevgames.unity-json";

        private static readonly
            SaveSerializerId SerializerId =
                new SaveSerializerId(
                    StableId);

        public SaveSerializerId Id =>
            SerializerId;

        public SaveSerializerResult Serialize<T>(
            T value,
            out string serialized)
        {
            serialized =
                string.Empty;

            if ((object)value == null)
            {
                return new SaveSerializerResult(
                    SaveSerializerStatus.InvalidRequest,
                    EchoSaveDiagnosticCodes
                        .SerializerInvalidRequest,
                    "A value is required for Chronicle serialization.");
            }

            if (value is ISavePackageDocument document)
            {
                SaveSerializerResult documentValidation =
                    SavePackageDocumentValidator
                        .ValidateCurrent(
                            document);

                if (!documentValidation.Succeeded)
                {
                    return documentValidation;
                }
            }

            try
            {
                serialized =
                    JsonUtility.ToJson(
                        value,
                        false);

                if (string.IsNullOrWhiteSpace(
                        serialized))
                {
                    serialized =
                        string.Empty;

                    return new SaveSerializerResult(
                        SaveSerializerStatus.Failed,
                        EchoSaveDiagnosticCodes
                            .SerializerFailure,
                        "Unity JsonUtility returned no serialized Chronicle data.");
                }

                return SaveSerializerResult.Success(
                    "The Chronicle DTO was serialized successfully.");
            }
            catch (Exception exception)
                when (IsExpectedSerializationException(
                    exception))
            {
                serialized =
                    string.Empty;

                return Failure(
                    "Chronicle serialization failed.",
                    exception);
            }
        }

        public SaveSerializerResult Deserialize<T>(
            string serialized,
            out T value)
        {
            value =
                default;

            if (string.IsNullOrWhiteSpace(
                    serialized))
            {
                return new SaveSerializerResult(
                    SaveSerializerStatus.InvalidRequest,
                    EchoSaveDiagnosticCodes
                        .SerializerInvalidRequest,
                    "Serialized Chronicle data is required.");
            }

            string trimmed =
                serialized.Trim();

            if (trimmed.Length < 2 ||
                trimmed[0] != '{' ||
                trimmed[trimmed.Length - 1] != '}')
            {
                return new SaveSerializerResult(
                    SaveSerializerStatus.MalformedData,
                    EchoSaveDiagnosticCodes
                        .SerializerMalformedData,
                    "Serialized Chronicle JSON must be one JSON object.");
            }

            try
            {
                value =
                    JsonUtility.FromJson<T>(
                        serialized);

                if ((object)value == null)
                {
                    value =
                        default;

                    return new SaveSerializerResult(
                        SaveSerializerStatus.MalformedData,
                        EchoSaveDiagnosticCodes
                            .SerializerMalformedData,
                        "Unity JsonUtility did not produce a Chronicle DTO.");
                }

                if (value is ISavePackageDocument document)
                {
                    SaveSerializerResult documentValidation =
                        SavePackageDocumentValidator
                            .ValidateCurrent(
                                document);

                    if (!documentValidation.Succeeded)
                    {
                        value =
                            default;
                        return documentValidation;
                    }
                }

                return SaveSerializerResult.Success(
                    "The Chronicle DTO was deserialized successfully.");
            }
            catch (Exception exception)
                when (IsExpectedSerializationException(
                    exception))
            {
                value =
                    default;

                return new SaveSerializerResult(
                    SaveSerializerStatus.MalformedData,
                    EchoSaveDiagnosticCodes
                        .SerializerMalformedData,
                    $"Chronicle deserialization failed. {exception.GetType().Name}: {exception.Message}");
            }
        }

        private static SaveSerializerResult Failure(
            string message,
            Exception exception) =>
            new SaveSerializerResult(
                SaveSerializerStatus.Failed,
                EchoSaveDiagnosticCodes
                    .SerializerFailure,
                $"{message} {exception.GetType().Name}: {exception.Message}");

        private static bool
            IsExpectedSerializationException(
                Exception exception) =>
            exception is ArgumentException ||
            exception is InvalidOperationException ||
            exception is NotSupportedException;
    }
}
