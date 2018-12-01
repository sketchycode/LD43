public struct MassChangeEvent {
    public float MassDelta;
    public float NewMass;
    public MassChangeSourceType MassChangeSource;
}

public enum MassChangeSourceType {
    Moving,
    Jumping
}