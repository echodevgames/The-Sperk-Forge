using System;

namespace EchoDevGames.EchoUI
{
    public readonly struct UITransitionOperationId : IEquatable<UITransitionOperationId>
    {
        public UITransitionOperationId(long value)
        {
            Value = value;
        }

        public long Value { get; }
        public bool IsValid => Value > 0;

        public bool Equals(UITransitionOperationId other) => Value == other.Value;
        public override bool Equals(object obj) => obj is UITransitionOperationId other && Equals(other);
        public override int GetHashCode() => Value.GetHashCode();
        public override string ToString() => Value.ToString();
        public static bool operator ==(UITransitionOperationId left, UITransitionOperationId right) => left.Equals(right);
        public static bool operator !=(UITransitionOperationId left, UITransitionOperationId right) => !left.Equals(right);
    }
}
