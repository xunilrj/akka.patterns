using System;

namespace SomeBank.Accounts.AkkaNet.Actors
{
    public class TransferBetweenAccountsCommand
    {
        public Guid Id { get; internal set; }

        public string Source { get; private set; }
        public string Destination { get; private set; }
        public double Value { get; private set; }
        
        public TransferBetweenAccountsCommand(string source, string destination, double value)
        {
            Id = Guid.NewGuid();

            Source = source;
            Destination = destination;
            Value = value;
        }
    }
}
