using UnityEngine;

namespace EchoDevGames.EchoSave
{
    /// <summary>
    /// Optional project-owned fixed-slot authoring metadata.
    ///
    /// Templates never become physical path authority and are not required by
    /// runtime capacity enforcement.
    /// </summary>
    [CreateAssetMenu(
        fileName = "SaveSlotTemplate",
        menuName = "EchoDevGames/The Chronicle/Save Slot Template")]
    public sealed class SaveSlotTemplate : ScriptableObject
    {
        [SerializeField]
        private string templateId = "slot";

        [SerializeField]
        private string displayLabel = "Slot";

        [SerializeField]
        private int displayOrder;

        public string TemplateId =>
            templateId ?? string.Empty;

        public string DisplayLabel =>
            displayLabel ?? string.Empty;

        public int DisplayOrder =>
            displayOrder;

        internal void SetDefinitionForTesting(
            string id,
            string label,
            int order)
        {
            templateId = id;
            displayLabel = label;
            displayOrder = order;
        }
    }
}
