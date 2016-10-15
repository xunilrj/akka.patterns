using Akka.Actor;

namespace SomeBank.AkkaUtils.Actors
{
    public class SupervisorActor : UntypedActor
    {
        public static Props Props()
        {
            return Akka.Actor.Props.Create<SupervisorActor>();
        }

        protected override void OnReceive(object message)
        {
            var transaction = Context.ActorOf(TransactionActor.Props());
            transaction.Tell(message);
        }
    }
}
