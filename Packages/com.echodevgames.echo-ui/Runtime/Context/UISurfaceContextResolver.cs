using System.Collections.Generic;

namespace EchoDevGames.EchoUI
{
    /// <summary>
    /// Resolves active designer-authored rules independently per response dimension.
    /// Earlier applicable rules win only the dimensions they explicitly supply.
    /// </summary>
    public static class UISurfaceContextResolver
    {
        public static UISurfaceContextResponse Resolve(
            IReadOnlyList<UISurfaceContextRule> orderedRules,
            UIContextState contextState,
            UISurfaceRuntimeOverride runtimeOverride)
        {
            UISurfaceVisibilityIntent visibility =
                UISurfaceVisibilityIntent.NoChange;
            UISurfaceInteractionIntent interaction =
                UISurfaceInteractionIntent.NoChange;
            UISurfaceSelectionIntent selection =
                UISurfaceSelectionIntent.NoChange;

            if (orderedRules != null &&
                contextState != null)
            {
                for (int index = 0;
                     index < orderedRules.Count;
                     index++)
                {
                    UISurfaceContextRule rule =
                        orderedRules[index];
                    if (rule == null ||
                        !rule.IsValid ||
                        !contextState.IsActive(
                            rule.ContextId))
                    {
                        continue;
                    }

                    UISurfaceContextResponse response =
                        rule.Response;

                    if (visibility == UISurfaceVisibilityIntent.NoChange &&
                        response.Visibility != UISurfaceVisibilityIntent.NoChange)
                    {
                        visibility =
                            response.Visibility;
                    }

                    if (interaction == UISurfaceInteractionIntent.NoChange &&
                        response.Interaction != UISurfaceInteractionIntent.NoChange)
                    {
                        interaction =
                            response.Interaction;
                    }

                    if (selection == UISurfaceSelectionIntent.NoChange &&
                        response.Selection != UISurfaceSelectionIntent.NoChange)
                    {
                        selection =
                            response.Selection;
                    }

                    if (visibility != UISurfaceVisibilityIntent.NoChange &&
                        interaction != UISurfaceInteractionIntent.NoChange &&
                        selection != UISurfaceSelectionIntent.NoChange)
                    {
                        break;
                    }
                }
            }

            if (runtimeOverride.HasVisibilityOverride)
            {
                visibility =
                    runtimeOverride.Visibility;
            }
            if (runtimeOverride.HasInteractionOverride)
            {
                interaction =
                    runtimeOverride.Interaction;
            }
            if (runtimeOverride.HasSelectionOverride)
            {
                selection =
                    runtimeOverride.Selection;
            }

            return new UISurfaceContextResponse(
                visibility,
                interaction,
                selection);
        }
    }
}
