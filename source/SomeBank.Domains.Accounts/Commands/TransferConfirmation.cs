using System;

namespace SomeBank.Domains.Accounts.Commands
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
