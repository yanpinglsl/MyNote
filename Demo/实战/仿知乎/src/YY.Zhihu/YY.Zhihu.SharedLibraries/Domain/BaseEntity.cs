using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YY.Zhihu.SharedLibraries.Domain
{
    public abstract class BaseEntity<TId> : IEntity<TId>
    {
        public TId Id { get; set; } = default!;

        private readonly List<BaseEvent> _domianEvets = new List<BaseEvent>();
        [NotMapped]
        public IReadOnlyCollection<BaseEvent> DomainEvents => _domianEvets.AsReadOnly();
        public void AddDomainEvent(BaseEvent baseEvent)
        {
            _domianEvets.Add(baseEvent);
        }
        public void DelDomainEvent(BaseEvent baseEvent)
        {
            _domianEvets.Remove(baseEvent);
        }
        public void ClearDomainEvent()
        {
            _domianEvets.Clear();
        }
    }
}
