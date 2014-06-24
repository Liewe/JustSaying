using JustEat.Testing;
using JustSaying.Messaging.MessageSerialisation;
using JustSaying.Models;
using NUnit.Framework;

namespace JustSaying.Messaging.UnitTests.Serialisation.SerialisationRegister
{
    public class WhenAddingAGenericSerialiser : BehaviourTest<MessageSerialisationRegister>
    {
        private readonly ServiceStackSerialiser<Message> _serialiser = new ServiceStackSerialiser<Message>();

        protected override void Given() { }

        protected override void When()
        {
            SystemUnderTest.AddSerialiser<Message>(_serialiser);
        }

        [Then]
        public void MappingsCanBeRetreivedByStringType()
        {
            Assert.NotNull(SystemUnderTest.GetSerialiser(typeof(Message).Name));
        }

        [Test]
        public void MappingsCanBeRetreivedStronglyTyped()
        {
            Assert.NotNull(SystemUnderTest.GetSerialiser(typeof(Message)));
        }
    }
}