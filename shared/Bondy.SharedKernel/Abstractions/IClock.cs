namespace Bondy.SharedKernel.Abstractions
{
    interface IClock
    {
        DateTime UtcNow { get; }
    }
}
