namespace Identity.Domain.ValueObjects;

public sealed record Scope(string Value)
{
    public override string ToString() => Value;
}

