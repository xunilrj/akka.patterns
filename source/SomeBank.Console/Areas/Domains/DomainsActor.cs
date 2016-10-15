using Akka.Actor;
using SomeBank.Domains.Accounts.Actors;

namespace akkatest.Areas
{
    class DomainActor : UntypedActor
    {
        IActorRef AccountsContext;

        public static Props Props()
        {
            return Akka.Actor.Props.Create<DomainActor>();
        }

        protected override void PreStart()
        {
            AccountsContext = Context.ActorOf(AccountsBoundedContextActor.Props(), "accounts");
            System.Console.WriteLine($"Accounts @ {AccountsContext.Path}");

            base.PreStart();
        }

        protected override void OnReceive(object message)
        {
            AccountsContext.Forward(message);
        }
    }
}
