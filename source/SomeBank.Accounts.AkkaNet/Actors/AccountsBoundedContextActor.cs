using Akka.Actor;
using SomeBank.AkkaNet.Actors;

namespace SomeBank.Accounts.AkkaNet.Actors
{
    public class AccountsBoundedContextActor : BoundedContextActor
    {
        public static Props Props()
        {
            return Akka.Actor.Props.Create<AccountsBoundedContextActor>();
        }

        protected override void OnReceive(object message)
        {
            if (message is CreateAccountCommand)
            {
                var tmsg = message as CreateAccountCommand;

                var aggregateRoot = Context.ActorOf(AccountActor.Props(), tmsg.Name);
                aggregateRoot.Forward(message);
            }
            else if (message is TransferBetweenAccountsCommand)
            {
                var tmsg = message as TransferBetweenAccountsCommand;
                Context.ActorSelection($"/user/domains/accounts/{tmsg.Source}").Tell(message);
            }
        }
    }
}
