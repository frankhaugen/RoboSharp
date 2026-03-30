namespace RoboSharp.World;

public readonly record struct ActorCell(int ActorId)
{
    public static ActorCell Empty => new(0);
    public bool HasActor => ActorId != 0;
}
