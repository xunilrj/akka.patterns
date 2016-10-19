using System;
using Akka.Actor;
using Chessie.ErrorHandling.CSharp;
using SomeBank.AkkaNet.Actors;
using static SomeBank.Accounts.Domain;

namespace SomeBank.Accounts.AkkaNet.Actors
{
    class AccountActor : AggregateRootActor<Account.T>
    {
        string Id;
        public override string PersistenceId => Id;

        public static Props Props(string accountId)
        {
            return Akka.Actor.Props.Create<AccountActor>(accountId);
        }
        
        public AccountActor(string accountId)
        {
            Id = accountId;

            Command<CreateAccountCommand>(x => Handle(x));
            Command<TransferBetweenAccountsCommand>(x => Handle(x));
            Command<TransferConfirmation>(x => Handle(x));
        }

        private void Handle(CreateAccountCommand command)
        {
            var result = Account.Create(command.Name, command.InitialBalance);

            if(result.IsOk)
            {
                Data = result.SucceededWith();

                SaveSnapshot(Data);
            }
            else
            {
                //TODO Samething as error 400
                //Sender.Tell()
            }

            Context.Log(x => x.Info("{Actor} - Account created {Id}", Self.Path, command.Name));
        }

        private void Handle(TransferBetweenAccountsCommand command)
        {
            if (IsTransferFromThisAccount(command))
            {
                StartTransferToDestination(command);
            }
            else if (IsTransferToThisAccount(command))
            {
                StartTranferFromSource(command);
            }
        }

        private void Handle(TransferConfirmation command)
        {
            var result = Account.AcceptTransferTo(command.Id, Data);

            if(result.IsOk)
            {
                Data = result.SucceededWith();
            }
            else
            {
                //TODO Samething as error 400
                //Sender.Tell()
            }

            Context.Log(x => x.Info("{Actor} - Transfer Accepted.", Self.Path));
        }

        private void StartTranferFromSource(TransferBetweenAccountsCommand command)
        {
            var result = Account.StartTransferFrom(command.Id, command.Source, command.Value, Data);

            if (result.IsOk)
            {
                Data = result.SucceededWith();
            }
            else
            {
                //TODO Samething as error 400
                //Sender.Tell()
            }

            //TODO point to persist

            Context.Log(x => x.Info("{Actor} - Transfer accepted", Self.Path));

            Sender.Tell(new TransferConfirmation(command.Id));
        }

        private void StartTransferToDestination(TransferBetweenAccountsCommand command)
        {
            var result = Account.StartTransferTo(command.Id, command.Destination, command.Value, Data);

            if (result.IsOk)
            {
                Data = result.SucceededWith();
            }
            else
            {
                //TODO Samething as error 400
                //Sender.Tell()
            }

            //TODO point to persist

            Context.Log(x => x.Info("{Actor} - Transfer started", Self.Path));

            var transferActor = Context.ActorOf(TransferActor.Props(), command.Id.ToString());
            transferActor.Tell(command);
        }

        private bool IsTransferToThisAccount(TransferBetweenAccountsCommand command)
        {
            return Data.Name == command.Destination;
        }

        private bool IsTransferFromThisAccount(TransferBetweenAccountsCommand command)
        {
            return Data.Name == command.Source;
        }


    }
}
