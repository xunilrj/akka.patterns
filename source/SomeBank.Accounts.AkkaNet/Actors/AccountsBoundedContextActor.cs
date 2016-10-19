using Akka.Actor;
using Akka.Cluster.Sharding;
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

                var accountActorProps = AccountActor.Props(tmsg.Name);
                
                var aggregateRoot = Context.ActorOf(accountActorProps, tmsg.Name);
                aggregateRoot.Forward(message);
            }
            else if (message is TransferBetweenAccountsCommand)
            {
                var tmsg = message as TransferBetweenAccountsCommand;
                Context.ActorSelection($"/user/domains/accounts/{tmsg.Source}").Tell(message);
            }
        }
        
    }

    public sealed class Envelope
    {
        public int ShardId;
        public int EntityId;
        public object Message;
    }

    public sealed class MessageExtractor : IMessageExtractor
    {
        public string EntityId(object message)
        {
            return (message as Envelope)?.EntityId.ToString();
        }

        public string ShardId(object message)
        {
            return (message as Envelope)?.ShardId.ToString();
        }

        public object EntityMessage(object message)
        {
            return (message as Envelope)?.Message;
        }
    }
}
