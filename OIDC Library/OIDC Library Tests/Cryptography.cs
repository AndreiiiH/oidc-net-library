using AndreiiiH.OIDC.Cryptography;
using NUnit.Framework;

namespace AndreiiiH.OIDC.Tests
{
    [TestFixture(Category = "Cryptography")]
    public class Cryptography
    {
        private const string _PASSWORD = "testPassword123";

        private const string _RS256_INPUT = "testInput";
        private string _rs256Output;

        [TestCase(Category = "RS256", TestName = "Random String Generation")]
        [TestOf(typeof(RS256))]
        public void GenerateRandomString()
        {
            string output = RS256.GenerateRandomString(64);
            Assert.IsNotEmpty(output);
            Assert.AreEqual(64, output.Length);
        }
        
        [TestCase(Category = "RS256", TestName = "RS256 Encryption and Decryption")]
        [TestOf(typeof(RS256))]
        public void EncryptDecrypt()
        {
            _rs256Output = RS256.Encrypt(_RS256_INPUT, _PASSWORD);
            Assert.IsNotEmpty(_rs256Output);

            string decrypted = RS256.Decrypt(_rs256Output, _PASSWORD).Trim('\0');
            Assert.IsNotEmpty(decrypted);
            Assert.AreEqual(_RS256_INPUT, decrypted);
        }
    }
}