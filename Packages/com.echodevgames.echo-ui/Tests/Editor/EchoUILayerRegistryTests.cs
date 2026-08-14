using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace EchoDevGames.EchoUI.Tests.Editor
{
    public sealed class EchoUILayerRegistryTests
    {
        private GameObject root;

        [SetUp]
        public void SetUp()
        {
            root =
                new GameObject(
                    "Layer_Test_Root");
        }

        [TearDown]
        public void TearDown()
        {
            if (root != null)
            {
                Object.DestroyImmediate(root);
            }
        }

        [Test]
        public void LayerRegistryAcceptsProjectDefinedVariableCountAndOrder()
        {
            UILayerHost late =
                CreateLayer(
                    "hud-special",
                    80);

            UILayerHost early =
                CreateLayer(
                    "weird-programmer-layer",
                    -10);

            UILayerHost middle =
                CreateLayer(
                    "menus",
                    12);

            bool created =
                UILayerRegistry.TryCreate(
                    new[]
                    {
                        late,
                        early,
                        middle
                    },
                    out UILayerRegistry registry,
                    out string error);

            Assert.That(
                created,
                Is.True,
                error);

            Assert.That(
                registry.Count,
                Is.EqualTo(3));

            Assert.That(
                registry.OrderedHosts[0].LayerId.Value,
                Is.EqualTo("weird-programmer-layer"));

            Assert.That(
                registry.OrderedHosts[1].LayerId.Value,
                Is.EqualTo("menus"));

            Assert.That(
                registry.OrderedHosts[2].LayerId.Value,
                Is.EqualTo("hud-special"));
        }

        [Test]
        public void RuntimeDoesNotRequireRecommendedStarterLayerNames()
        {
            UILayerHost only =
                CreateLayer(
                    "banana-rack",
                    3);

            bool created =
                UILayerRegistry.TryCreate(
                    new[]
                    {
                        only
                    },
                    out UILayerRegistry registry,
                    out string error);

            Assert.That(
                created,
                Is.True,
                error);

            Assert.That(
                registry.TryGetHost(
                    "banana-rack",
                    out UILayerHost resolved),
                Is.True);

            Assert.That(
                resolved,
                Is.SameAs(only));
        }

        [Test]
        public void ResolvedLayerTopologyDoesNotRewriteOrFollowLaterAuthoredMutation()
        {
            UILayerHost host =
                CreateLayer(
                    "stable-layer",
                    5);

            Assert.That(
                UILayerRegistry.TryCreate(
                    new[]
                    {
                        host
                    },
                    out UILayerRegistry registry,
                    out string error),
                Is.True,
                error);

            SerializedObject serialized =
                new SerializedObject(host);

            SerializedProperty definition =
                serialized.FindProperty(
                    "definition");

            definition.FindPropertyRelative(
                    "layerId")
                .stringValue = "changed-after-resolution";

            definition.FindPropertyRelative(
                    "order")
                .intValue = 999;

            serialized.ApplyModifiedPropertiesWithoutUndo();

            Assert.That(
                registry.TryGetHost(
                    "stable-layer",
                    out UILayerHost resolved),
                Is.True);

            Assert.That(
                resolved,
                Is.SameAs(host));

            Assert.That(
                host.LayerId.Value,
                Is.EqualTo("stable-layer"));

            Assert.That(
                host.Order,
                Is.EqualTo(5));

            Assert.That(
                registry.TryGetHost(
                    "changed-after-resolution",
                    out _),
                Is.False);
        }

        [Test]
        public void DuplicateLayerIdsAreRejected()
        {
            UILayerHost first =
                CreateLayer(
                    "screens",
                    1);

            UILayerHost second =
                CreateLayer(
                    "screens",
                    2);

            bool created =
                UILayerRegistry.TryCreate(
                    new[]
                    {
                        first,
                        second
                    },
                    out _,
                    out string error);

            Assert.That(
                created,
                Is.False);

            StringAssert.Contains(
                "Duplicate",
                error);
        }

        [Test]
        public void DuplicateLayerOrderIsRejected()
        {
            UILayerHost first =
                CreateLayer(
                    "screens-a",
                    4);

            UILayerHost second =
                CreateLayer(
                    "screens-b",
                    4);

            bool created =
                UILayerRegistry.TryCreate(
                    new[]
                    {
                        first,
                        second
                    },
                    out _,
                    out string error);

            Assert.That(
                created,
                Is.False);

            StringAssert.Contains(
                "order",
                error);
        }

        private UILayerHost CreateLayer(
            string id,
            int order)
        {
            GameObject child =
                new GameObject(
                    "Layer_" + id);

            child.transform.SetParent(
                root.transform,
                false);

            UILayerHost host =
                child.AddComponent<UILayerHost>();

            SerializedObject serialized =
                new SerializedObject(host);

            SerializedProperty definition =
                serialized.FindProperty(
                    "definition");

            definition.FindPropertyRelative(
                    "layerId")
                .stringValue = id;

            definition.FindPropertyRelative(
                    "displayLabel")
                .stringValue = id;

            definition.FindPropertyRelative(
                    "order")
                .intValue = order;

            serialized.ApplyModifiedPropertiesWithoutUndo();

            return host;
        }
    }
}
