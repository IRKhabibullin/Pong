/// <summary>
/// Interface for entities in game. Entity should be created for each interactable object via <see cref="EntitiesManager"/>.
/// Object controlling components must update their state according to values in corresponding entity instance.
/// </summary>
public interface IEntity
{
    // todo currently lets update state of controllers each frame. Probably would be better to make it via actions subscriptions
}
