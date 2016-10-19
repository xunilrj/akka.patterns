using Akka.Actor;
using akkatest.Areas;
using Serilog;
using SomeBank.Accounts.AkkaNet.Actors;
using SomeBank.AkkaNet.Actors;

namespace SomeBank.Console
{
    class SomeBankService
    {
        ActorSystem ActorSystem;
        IActorRef Application;
        IActorRef Domain;

        internal bool Start()
        {
            var logger = new LoggerConfiguration()
                .WriteTo.ColoredConsole()
                .MinimumLevel.Information()
                .CreateLogger();
            Serilog.Log.Logger = logger;

            ActorSystem = ActorSystem.Create("BankSystem", @"akka
{
    loglevel=INFO,
    loggers=[""Akka.Logger.Serilog.SerilogLogger, Akka.Logger.Serilog""]

    actor {
        serializers {
            //wire = ""Akka.Serialization.WireSerializer, Akka.Serialization.Wire""
            //fsharprecord = ""SomeBank.AkkaNet.Serialization.FSharpRecordSerializer, SomeBank.AkkaNet""
        }
        serialization-bindings {
            //""System.Object"" = wire
            //""SomeBank.Accounts.Domain.Account.T, SomeBank.Accounts"" = fsharprecord
        }
     persistence {
            journal {
                plugin = ""akka.persistence.journal.inmem""
                inmem {
                    class=""Akka.Persistence.Journal.MemoryJournal, Akka.Persistence""
                    plugin-dispatcher = ""akka.actor.default-dispatcher""
                }
            }
            snapshot-store {
                plugin = ""akka.persistence.snapshot - store.local""
                local {
                    class = ""Akka.Persistence.Snapshot.LocalSnapshotStore, Akka.Persistence""
                    plugin-dispatcher = ""akka.persistence.dispatchers.default-plugin-dispatcher""
                    stream-dispatcher = ""akka.persistence.dispatchers.default-stream-dispatcher""
                    dir = ""snapshots""
                }
            }
        }
    }
}");
            //var greeter = System.ActorOf<ProductActor>("Product01");
            //greeter.Tell(new ChangeProductPriceCommand(19.99));

            Domain = ActorSystem.ActorOf(DomainActor.Props(), "domains");

            Application = ActorSystem.ActorOf(SupervisorActor.Props(), "Application");
            ActorSystem.ActorOf(SupervisorActor.Props(), "Input");
            ActorSystem.ActorOf(SupervisorActor.Props(), "Output");
            ActorSystem.ActorOf(SupervisorActor.Props(), "Repositories");

            System.Console.WriteLine($"Domain @ {Domain.Path}");

            // Simulates user interactions
            {
                var command = new CreateAccountCommand("AC001", 100);
                Application.Tell(command);
            }


            System.Threading.Thread.Sleep(1000);

            {
                var command = new TransferBetweenAccountsCommand("AC001", "AC002", 50);
                Application.Tell(command);
            }

            System.Threading.Thread.Sleep(4000);

            {
                var command = new CreateAccountCommand("AC002", 100);
                Application.Tell(command);
            }


            return true;
        }

        internal bool Stop()
        {
            ActorSystem.Terminate().Wait();
            return false;
        }
    }
}
