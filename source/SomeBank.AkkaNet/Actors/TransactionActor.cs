using Akka.Actor;
using Akka.Cluster.Sharding;

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

            var bc = ClusterSharding.Get(Context.System)
                .ShardRegion("AccountsBoundedContextActor");
            bc.Tell(message);
            
            //Context.ActorSelection("/user/domains/accounts").Tell(message);

            //Wait to this transaction to be completed than kill it
            Self.Tell(PoisonPill.Instance);
        }
    }
}
