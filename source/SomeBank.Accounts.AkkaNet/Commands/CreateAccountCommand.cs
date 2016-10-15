namespace SomeBank.Accounts.AkkaNet.Actors
{
    public class CreateAccountCommand
    {
        public string Name { get; set; }
        public double InitialBalance { get; private set; }

        public CreateAccountCommand(string name, double initialBalance)
        {
            Name = name;
            InitialBalance = initialBalance;
        }
    }
}
