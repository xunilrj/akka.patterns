using Akka.Actor;
using Akka.Cluster.Sharding;
using Akka.Cluster.Tools.Singleton;
using Akka.Configuration;
using Akka.Event;
using Serilog;
using Serilog.Debugging;
using System;
using System.IO;
using System.Text;
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

            sharding.Start(
                settings: shardingSettings,
                messageExtractor: new PersonCommandExtractor(),
                entityProps: Props.Create<Person>(),
                typeName: "person"
                );

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

    public class PersonCommandExtractor : HashCodeMessageExtractor
    {
        public PersonCommandExtractor() : base(30)
        {
        }

        public override object EntityMessage(object message)
        {
            return message as Person.ChangeName;
        }

        public override string EntityId(object message)
        {
            var command = message as Person.ChangeName;
            return $"{command?.Id:n}";
        }
    }

    public class Person : UntypedActor
    {
        public class ChangeName
        {
            public ChangeName(Guid id, string name)
            {
                Id = id;
                Name = name;
            }

            public Guid Id { get; private set; }

            public string Name { get; private set; }
        }

        public class Shoot
        {
            public static readonly Shoot Instance = new Shoot();
            private Shoot()
            {
            }
        }

        private string _name;
        private readonly ILoggingAdapter _log;
        private TimeSpan _passivateAfter;

        public Person()
        {
            _passivateAfter = TimeSpan.FromSeconds(3);
            _log = Context.GetLogger();
            Context.SetReceiveTimeout(_passivateAfter);
        }

        protected override void OnReceive(object message)
        {
            if (message is ChangeName)
            {
                DoNameChange(((ChangeName)message).Name);
            }
            if (message is ReceiveTimeout)
            {
                Context.Parent.Tell(new Passivate(Shoot.Instance));
            }
            if (message is Shoot)
            {
                _log.Warning($"Shooting myself. Nobody loves me. My name was {_name}.");
                Context.Stop(Self);
            }
        }

        private void DoNameChange(string name)
        {
            if (name.Equals(_name, StringComparison.InvariantCultureIgnoreCase))
            {
                _log.Warning($"Can't change name.");
            }
            else
            {
                _name = name;
                _log.Info($"Name changed to {name}.");
            }
        }
    }
}
