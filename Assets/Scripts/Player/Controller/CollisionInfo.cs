using System;

[Serializable]
public class CollisionInfo {
    public bool IsGrounded;
    public bool WasGrounded;
    public bool JustGrounded => !WasGrounded && IsGrounded;

    internal void Reset() {
        WasGrounded = IsGrounded;
        IsGrounded = false;
    }
}