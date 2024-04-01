#region License
// Copyright (c) 2007 James Newton-King
//
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
#endregion

using System;
using BenchmarkDotNet.Running;
using System.Reflection;
using Newtonsoft.Json.Tests.TestObjects;
using System.Collections.Generic;
using Newtonsoft.Json.Tests;
using BenchmarkDotNet.Attributes;
using System.IO;
using Newtonsoft.Json.Serialization;

namespace Newtonsoft.Json.TestConsole
{
    public class TestContractResolver : DefaultContractResolver
    {

        public override JsonContract ResolveContract(Type type)
        {
            return CreateContract(type); // Always create a contract without using a cache
            //return base.ResolveContract(type);
        }
    }

    [MemoryDiagnoser, ShortRunJob]
    public class SerializeBenchmarks
    {
        private static readonly IList<RootObject> LargeCollection;

        static SerializeBenchmarks()
        {
            string json = System.IO.File.ReadAllText(TestFixtureBase.ResolvePath("small.json"));

            LargeCollection = JsonConvert.DeserializeObject<IList<RootObject>>(json);
        }

        [Benchmark]
        public void SerializeLargeJsonFile()
        {
            using (StreamWriter file = System.IO.File.CreateText(TestFixtureBase.ResolvePath("largewrite.json")))
            {
                var settings = new JsonSerializerSettings 
                {
                   ContractResolver = new TestContractResolver()
                };
                var serializer = JsonSerializer.CreateDefault(settings);


                //JsonSerializer serializer = new JsonSerializer();
                serializer.Formatting = Formatting.Indented;
                serializer.Serialize(file, LargeCollection);
            }
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            var attribute = (AssemblyFileVersionAttribute)typeof(JsonConvert).GetTypeInfo().Assembly.GetCustomAttribute(typeof(AssemblyFileVersionAttribute));
            Console.WriteLine("Json.NET Version: " + attribute.Version);

            new BenchmarkSwitcher(new [] { typeof(SerializeBenchmarks) }).Run(args);//, new DebugInProcessConfig());
        }
    }
}