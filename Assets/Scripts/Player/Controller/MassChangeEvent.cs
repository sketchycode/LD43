
public struct MassChangeEvent {
    public float MassDelta;
    public float NewMass;
    public float NewMassNormalized;
    public MassChangeSourceType MassChangeSource;
}

public enum MassChangeSourceType {
    Moving,
    Jumping,
    BabyChunk
}