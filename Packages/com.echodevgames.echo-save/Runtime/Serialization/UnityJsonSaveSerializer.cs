
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
    ///
    /// Runtime Type overloads accept Type authority only from trusted running
    /// code. Chronicle never persists CLR type names in save documents.
    /// </summary>
    public sealed class UnityJsonSaveSerializer :
        IRuntimeTypeSaveSerializer
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
            out string serialized) =>
            Serialize(
                value,
                typeof(T),
                out serialized);

        public SaveSerializerResult Deserialize<T>(
            string serialized,
            out T value)
        {
            value =
                default;

            SaveSerializerResult result =
                Deserialize(
                    serialized,
                    typeof(T),
                    out object restored);

            if (!result.Succeeded)
            {
                return result;
            }

            if (!(restored is T typed))
            {
                return new SaveSerializerResult(
                    SaveSerializerStatus.Failed,
                    EchoSaveDiagnosticCodes
                        .SerializerFailure,
                    "Unity JSON restored a Chronicle value that does not match the requested runtime type.");
            }

            value =
                typed;

            return result;
        }

        public SaveSerializerResult Serialize(
            object value,
            Type valueType,
            out string serialized)
        {
            serialized =
                string.Empty;

            SaveSerializerResult typeValidation =
                ValidateRuntimeTypeAndValue(
                    value,
                    valueType);

            if (!typeValidation.Succeeded)
            {
                return typeValidation;
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

        public SaveSerializerResult Deserialize(
            string serialized,
            Type valueType,
            out object value)
        {
            value =
                null;

            SaveSerializerResult typeValidation =
                ValidateRuntimeType(
                    valueType);

            if (!typeValidation.Succeeded)
            {
                return typeValidation;
            }

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
                trimmed[
                    trimmed.Length - 1] != '}')
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
                    JsonUtility.FromJson(
                        serialized,
                        valueType);

                if (value == null ||
                    !valueType.IsInstanceOfType(
                        value))
                {
                    value =
                        null;

                    return new SaveSerializerResult(
                        SaveSerializerStatus.MalformedData,
                        EchoSaveDiagnosticCodes
                            .SerializerMalformedData,
                        "Unity JsonUtility did not produce the requested Chronicle DTO type.");
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
                            null;

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
                    null;

                return new SaveSerializerResult(
                    SaveSerializerStatus.MalformedData,
                    EchoSaveDiagnosticCodes
                        .SerializerMalformedData,
                    $"Chronicle deserialization failed. {exception.GetType().Name}: {exception.Message}");
            }
        }

        private static SaveSerializerResult
            ValidateRuntimeTypeAndValue(
                object value,
                Type valueType)
        {
            SaveSerializerResult typeValidation =
                ValidateRuntimeType(
                    valueType);

            if (!typeValidation.Succeeded)
            {
                return typeValidation;
            }

            if (value == null)
            {
                return new SaveSerializerResult(
                    SaveSerializerStatus.InvalidRequest,
                    EchoSaveDiagnosticCodes
                        .SerializerInvalidRequest,
                    "A value is required for Chronicle serialization.");
            }

            if (!valueType.IsInstanceOfType(
                    value))
            {
                return new SaveSerializerResult(
                    SaveSerializerStatus.InvalidRequest,
                    EchoSaveDiagnosticCodes
                        .SerializerInvalidRequest,
                    "The Chronicle value does not match the trusted runtime DTO type.");
            }

            return SaveSerializerResult.Success(
                "The Chronicle runtime DTO type is valid.");
        }

        private static SaveSerializerResult
            ValidateRuntimeType(
                Type valueType)
        {
            if (valueType == null ||
                valueType == typeof(void) ||
                valueType.IsPointer ||
                valueType.IsByRef ||
                valueType.ContainsGenericParameters)
            {
                return new SaveSerializerResult(
                    SaveSerializerStatus.InvalidRequest,
                    EchoSaveDiagnosticCodes
                        .SerializerInvalidRequest,
                    "A concrete trusted runtime DTO type is required.");
            }

            return SaveSerializerResult.Success(
                "The Chronicle runtime DTO type is valid.");
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
