
using System;

namespace EchoDevGames.EchoSave.Samples.ChronicleLaboratory
{
    [Serializable]
    public sealed class ChronicleSaveLaboratoryState
    {
        public int sperkLevel = 7;
        public int galacticRupees = 420;
        public int anvilTemperature = 9001;
        public bool hasForbiddenKey = true;
        public int realityDamagePercent = 3;

        public ChronicleSaveLaboratoryState Clone() =>
            new ChronicleSaveLaboratoryState
            {
                sperkLevel = sperkLevel,
                galacticRupees = galacticRupees,
                anvilTemperature = anvilTemperature,
                hasForbiddenKey = hasForbiddenKey,
                realityDamagePercent = realityDamagePercent
            };

        public void CopyFrom(
            ChronicleSaveLaboratoryState source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(
                    nameof(source));
            }

            sperkLevel = source.sperkLevel;
            galacticRupees = source.galacticRupees;
            anvilTemperature = source.anvilTemperature;
            hasForbiddenKey = source.hasForbiddenKey;
            realityDamagePercent = source.realityDamagePercent;
        }

        public bool ValueEquals(
            ChronicleSaveLaboratoryState other) =>
            other != null &&
            sperkLevel == other.sperkLevel &&
            galacticRupees == other.galacticRupees &&
            anvilTemperature == other.anvilTemperature &&
            hasForbiddenKey == other.hasForbiddenKey &&
            realityDamagePercent == other.realityDamagePercent;

        public static ChronicleSaveLaboratoryState
            CreateKnownBaseline() =>
            new ChronicleSaveLaboratoryState
            {
                sperkLevel = 7,
                galacticRupees = 420,
                anvilTemperature = 9001,
                hasForbiddenKey = true,
                realityDamagePercent = 3
            };
    }
}
