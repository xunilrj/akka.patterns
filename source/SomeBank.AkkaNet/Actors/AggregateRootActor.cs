using Akka.Actor;
using Akka.Persistence;
using System;

namespace SomeBank.AkkaNet.Actors
{
    public abstract class AggregateRootActor<T> : ReceivePersistentActor
    {
        protected T Data;

        public AggregateRootActor()
        {
            Recover<EventArgs>(evt =>
            {
                //_state.Update(evt);
            });

            Recover<SnapshotOffer>(snapshot =>
            {
                Data = (T)snapshot.Snapshot;
            });
        }
    }
}
