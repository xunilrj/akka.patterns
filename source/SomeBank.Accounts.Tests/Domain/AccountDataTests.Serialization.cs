//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using Chessie.ErrorHandling.CSharp;
//using System;
//using static SomeBank.Accounts.Domain;
//using System.IO;
//using Microsoft.FSharp.Collections;
//using Wire.SerializerFactories;
//using System.Collections.Concurrent;
//using Wire;
//using Wire.ValueSerializers;

//namespace SomeBank.Domains.Accounts.Tests.Data
//{
//    public partial class AccountDataTests
//    {
//        [TestMethod]
//        public void A()
//        {
//            var options = new SerializerOptions(                
//                serializerFactories: new[]
//            {
//                new FSharpMap()
//            });
//            var s = new Wire.Serializer(options);

//            var mem = new MemoryStream();

//            var r1 = new RecordWithString("somestring");
//            s.Serialize(r1, mem);

//            mem = new MemoryStream();
//            var r2 = SomeBank.Accounts.Domain.createRecordWithMap;
//            s.Serialize(r2, mem);
//        }

//        public class FSharpMap : ValueSerializerFactory
//        {
//            public override ValueSerializer BuildSerializer(Serializer serializer, Type type, ConcurrentDictionary<Type, ValueSerializer> typeMapping)
//            {
//                throw new NotImplementedException();
//            }

//            public override bool CanDeserialize(Serializer serializer, Type type)
//            {
//                throw new NotImplementedException();
//            }

//            public override bool CanSerialize(Serializer serializer, Type type)
//            {
//                Console.WriteLine(type);
//                return false;
//            }
//        }
//    }
//}
