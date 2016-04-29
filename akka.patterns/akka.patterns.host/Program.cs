using Akka.Actor;
using Akka.Cluster.Sharding;
using Akka.Cluster.Tools.PublishSubscribe;
using Akka.Cluster.Tools.Singleton;
using Akka.Configuration;
using Akka.Event;
using Akka.Persistence;
using Serilog;
using Serilog.Debugging;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Topshelf;

namespace akka.patterns.host
{
    class Program
    {
        static void Main(string[] args)
        {
            var logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.ColoredConsole()
                    .CreateLogger();
            Log.Logger = logger;

            var writer = new StreamWriter(File.Open(@"serilog.txt", FileMode.Append, FileAccess.Write, FileShare.ReadWrite));
            SelfLog.Out = writer;

            HostFactory.Run(x =>
            {
                x.UseSerilog(logger);
                x.Service<AkkaHost>(s =>
                {
                    s.ConstructUsing(name => new AkkaHost(logger));
                    s.WhenStarted(tc => tc.Start());
                    s.WhenStopped(tc => tc.Stop());
                });
                x.RunAsLocalSystem();
                x.StartAutomatically();

                x.SetDescription("Akka Patterns Host");
                x.SetDisplayName("AkkaPatternsHost");
                x.SetServiceName("AkkaPatternsHost");
            });
        }
    }

    public class AkkaHost
    {
        private ILogger Logger;

        public AkkaHost(ILogger logger)
        {
            Logger = logger;
        }

        public bool Start()
        {
            Logger.Verbose("Starting...");
            Logger.Verbose("Started");

            var akkaConfig = GetAkkaConfig("MySystem", true, 8080);

            var fullConfig = ConfigurationFactory.Load()
                .WithFallback(akkaConfig)
                .WithFallback(ClusterSingletonManager.DefaultConfig());

            var system = ActorSystem.Create("MySystem", fullConfig);
            var sharding = ClusterSharding.Get(system);
            var shardingSettings = sharding.Settings;

            var rend1 = sharding.Start(
                settings: shardingSettings,
                messageExtractor: new EntityExtractor(),
                entityProps: Props.Create<RendezvousActor>(),
                typeName: "RendezvousActor"
                );

            var pubsub = DistributedPubSub.Get(system);
            var mediator = pubsub.Mediator;
            mediator.Tell(new Subscribe("domainevents", rend1));

            //mediator.Tell(new Publish("topic-name", new MyMessage()));

            //system.EventStream.Subscribe(rend1, typeof(EventArgs1));
            //system.EventStream.Subscribe(rend1, typeof(EventArgs2));

            Task.Factory.StartNew(async () => { await Task.Delay(5000); mediator.Tell(new Publish("domainevents", new EventArgs1())); });
            Task.Factory.StartNew(async () => { await Task.Delay(10000); mediator.Tell(new Publish("domainevents", new EventArgs2())); });

            return true;
        }

        private Config GetAkkaConfig(string systemName, bool seed, int port)
        {
            var sb = new StringBuilder();

            sb.AppendLine($"akka.remote.helios.tcp.port = {(seed ? port : 0)}");
            sb.AppendLine($"akka.cluster.seed-nodes = [\"akka.tcp://{systemName}@127.0.0.1:{port}\"]");

            Logger.Verbose("Akka Config");
            Logger.Verbose(sb.ToString());

            return ConfigurationFactory.ParseString(sb.ToString());
        }

        public bool Stop()
        {
            Logger.Verbose("Stopping...");
            Logger.Verbose("Stopped");
            return true;
        }
    }

    public class EventArgs1 : EventArgs, IEntity
    {
        public Guid Oid { get; set; }
    }

    public class EventArgs2 : EventArgs, IEntity
    {
        public Guid Oid { get; set; }
    }

    public class EntityExtractor : HashCodeMessageExtractor
    {
        public EntityExtractor() : base(30)
        {
        }

        public override object EntityMessage(object message)
        {
            return message;
        }

        public override string EntityId(object message)
        {
            return message.GetHashCode().ToString();
        }
    }

    public interface IEntity
    {
        Guid Oid { get; }
    }

    public class RendezvousActor : PersistentActor, IEntity
    {
        public RendezvousActor()
        {
            Oid = Guid.NewGuid();
        }

        public Guid Oid { get; set; }

        public override string PersistenceId
        {
            get
            {
                return Oid.ToString();
            }
        }

        protected override bool ReceiveCommand(object message)
        {
            return true;
        }

        protected override bool ReceiveRecover(object message)
        {
            return true;
        }
    }
}
