using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    // These tests ensure that our testing framework is working correctly. This
    // means that we always expect the tests to pass. If they fail, there is a
    // problem with the testing framework.
    public class SanityTest
    {
        // A simple test which will always pass.
        [Test]
        public void BasicTest()
        {
            bool isActive = false;
            Assert.AreEqual(false, isActive);
        }
    }
}
