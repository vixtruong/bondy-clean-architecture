namespace Bondy.SharedKernel.Common
{
    public abstract class Entity
    {
        public long Id { get; protected set; }

        public DateTime CreatedAt { get; protected set; }

        public DateTime? UpdatedAt { get; protected set; }
    }
}
