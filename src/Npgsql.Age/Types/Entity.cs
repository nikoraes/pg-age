namespace Npgsql.Age.Types
{
    /// <summary>
    /// Represents the base type for graph entities (vertices and edges).
    /// </summary>
    /// <typeparam name="T">The type of the entity's properties.</typeparam>
    /// <param name="Id">The unique identifier of the entity.</param>
    /// <param name="Label">The label of the entity.</param>
    /// <param name="Properties">The properties of the entity.</param>
    public abstract record Entity<T>(GraphId Id, string Label, T Properties);
}
