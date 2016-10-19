using Akka.Actor;
using Akka.Cluster.Sharding;
using akkatest.Areas;
using Serilog;
using SomeBank.Accounts.AkkaNet.Actors;
using SomeBank.AkkaNet.Actors;
using System;
using System.Threading;

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

    remote {
        helios.tcp {
            port = 8081
            hostname = localhost
        }
    }

    cluster {
        seed-nodes = [""akka.tcp://BankSystem@localhost:8081""]
        auto-down-unreachable-after = 5s
        sharding {
            least-shard-allocation-strategy.rebalance-threshold = 3
        }
    }

    actor {
        provider = ""Akka.Cluster.ClusterActorRefProvider, Akka.Cluster""
        serializers {
                wire = ""Akka.Serialization.WireSerializer, Akka.Serialization.Wire""
        }
        serialization-bindings {
                ""System.Object"" = wire
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

            var sharding = ClusterSharding.Get(ActorSystem);
            var settings = ClusterShardingSettings.Create(ActorSystem);

            sharding.Start(
                typeName: "AccountsBoundedContextActor",
                entityProps: Props.Create<AccountsBoundedContextActor>(),
                settings: settings,
                idExtractor: x => Tuple.Create(x.ToString(), x), //FAKE
                shardResolver: x => x.ToString()); //FAKE
            
            Domain = ActorSystem.ActorOf(DomainActor.Props(), "domains");

            Application = ActorSystem.ActorOf(SupervisorActor.Props(), "Application");
            ActorSystem.ActorOf(SupervisorActor.Props(), "Input");
            ActorSystem.ActorOf(SupervisorActor.Props(), "Output");
            ActorSystem.ActorOf(SupervisorActor.Props(), "Repositories");

            System.Console.WriteLine($"Domain @ {Domain.Path}");

            Thread.Sleep(2000);

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
