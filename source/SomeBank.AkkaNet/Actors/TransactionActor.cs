using Akka.Actor;

namespace SomeBank.AkkaNet.Actors
{
    public class TransactionActor : UntypedActor
    {
        public static Props Props()
        {
            return Akka.Actor.Props.Create<TransactionActor>();
        }

        protected override void PostStop()
        {
            System.Console.WriteLine("Transaction Finished");

            base.PostStop();
        }

        protected override void OnReceive(object message)
        {
            //Find the Bounded Context Manager
            Context.ActorSelection("/user/domains/accounts").Tell(message);

            //Wait to this transaction to be completed than kill it
            Self.Tell(PoisonPill.Instance);
        }
    }
}
