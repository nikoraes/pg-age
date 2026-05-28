namespace Npgsql.Age.Types
{
    public abstract record Entity<T>(GraphId Id, string Label, T Properties);
}
