using Akka.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;

namespace SomeBank.AkkaNet.Serialization
{
    public class FSharpRecordSerializer : Serializer
    {
        public FSharpRecordSerializer(ExtendedActorSystem system) : base(system)
        {
        }

        public override bool IncludeManifest
        {
            get
            {
                return false;
            }
        }

        public override int Identifier
        {
            get
            {
                return 130525;
            }
        }

        public override object FromBinary(byte[] bytes, Type type)
        {
            return new object();
        }

        public override byte[] ToBinary(object obj)
        {
            return new byte[1];
        }
    }
}
