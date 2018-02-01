using AndreiiiH.OIDC.Core;
using NUnit.Framework;

namespace AndreiiiH.OIDC.Tests
{
    [TestFixture(Category = "Core")]
    public class Core
    {
        private const string _STATE_INPUT = "testState";
        private string _stateOutput;
        
        [TestCase(Category = "Nonce", TestName = "Nonce Generation")]
        [TestOf(typeof(Nonce))]
        public void NonceGeneration()
        {
            string nonce = Nonce.Generate();
            Assert.IsNotEmpty(nonce);
        }

        [TestCase(Category = "State", TestName = "State Encryption, Decryption and Validation")]
        [TestOf(typeof(State))]
        public void StateEncryptionDecryption()
        {
            _stateOutput = State.Encrypt(_STATE_INPUT);
            Assert.IsNotEmpty(_stateOutput);
            
            string validatedState = State.Validate(State.Decrypt(_stateOutput));
            Assert.IsNotEmpty(validatedState);
            Assert.AreEqual(_STATE_INPUT, validatedState);
        }
    }
}