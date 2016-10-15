using Akka.Actor;
using SomeBank.Domains.Accounts.Commands;

namespace SomeBank.Domains.Accounts.Actors
{
    class TransferActor : ReceiveActor
    {
        public static Props Props()
        {
            return Akka.Actor.Props.Create<TransferActor>();
        }

        public TransferActor()
        {
            Receive<TransferBetweenAccountsCommand>(x => Handle(x));
            Receive<TransferConfirmation>(x => Handle(x));
        }

        private void Handle(TransferBetweenAccountsCommand command)
        {
            Context.Log(x => x.Info("{Actor} - Sending the transfer to the destination account", Self.Path));

            //BecomeStacked(() => WaitConfirmation());

            Context.ActorSelection($"/user/domains/accounts/{command.Destination}").Tell(command);

            Context.System.Scheduler.ScheduleTellOnce(500, Self, command, Self);
        }

        private void WaitConfirmation()
        {
            Context.Log(x => x.Info("{Actor} - Waiting confirmation...", Self.Path));

            Receive<TransferConfirmation>(x => Handle(x));
        }

        private void Handle(TransferConfirmation command)
        {
            Context.Log(x => x.Info("{Actor} - Confirmation Recieved!!", Self.Path));

            Context.ActorSelection("..").Tell(command);
            Self.Tell(PoisonPill.Instance);
        }
    }
}
