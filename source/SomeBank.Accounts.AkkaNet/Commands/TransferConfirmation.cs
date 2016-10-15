using System;

namespace SomeBank.Accounts.AkkaNet.Actors
{
    class TransferConfirmation
    {
        public readonly Guid Id;

        public TransferConfirmation(Guid id)
        {
            Id = id;
        }
    }
}
