using System;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Streams;
using Akka.Streams.Dsl;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Xunit;

namespace SeungYongShim.Streams.Tests
{
    public class UnitTest1
    {
        [Fact]
        public async Task Test1()
        {
            var host = Host.CreateDefaultBuilder()
                           .UseAkka("test", "")
                           .UseAkkaWithXUnit2()
                           .Build();

            await host.StartAsync();

            var sys = host.Services.GetRequiredService<ActorSystem>();


            var sinkUnderTest = Flow.Create<int>()
                                    .Select(x => x * 2)
                                    .ToMaterialized(Sink.Aggregate<int, int>(0, (sum, i) => sum + i), Keep.Right);

            var result = await Source.From(Enumerable.Range(1, 4)).RunWith(sinkUnderTest, sys.Materializer());

            result.Should().Be(20);
        }
    }
}
