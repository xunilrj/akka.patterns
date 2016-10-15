﻿using Akka.Actor;
using SomeBank.AkkaUtils.Actors;
using SomeBank.Domains.Accounts.Commands;
using Chessie.ErrorHandling.CSharp;
using static SomeBank.Accounts;

namespace SomeBank.Domains.Accounts.Actors
{
    class AccountActor : AggregateRootActor
    {
        public static Props Props()
        {
            return Akka.Actor.Props.Create<AccountActor>();
        }

        Account.T Data;

        public AccountActor()
        {
            Receive<CreateAccountCommand>(x => Handle(x));
            Receive<TransferBetweenAccountsCommand>(x => Handle(x));
            Receive<TransferConfirmation>(x => Handle(x));
        }

        private void Handle(CreateAccountCommand command)
        {
            var result = Account.Create(command.Name, command.InitialBalance);

            if(result.IsOk)
            {
                Data = result.SucceededWith();
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
    }
}
