using System.Collections;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace Tests.EditMode
{
    public class Tests
    {
        // A Test behaves as an ordinary method
        [Test]
        public void Tests1SimplePasses()
        {
            // Use the Assert class to test conditions
        
            Assert.True(5 % 1 == 5);
        }

        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        [UnityTest]
        public IEnumerator Tests1WithEnumeratorPasses()
        {
            // Use the Assert class to test conditions.
            // Use yield to skip a frame.
            yield return null;
        }
    }
}
