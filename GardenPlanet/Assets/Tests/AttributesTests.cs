using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace GardenPlanet
{
    [TestFixture]
    public class AttributesTest
    {
        [Test]
        public void TestEmptyConstructor()
        {
            var attrs = new Attributes();
            Assert.That(attrs.Count, Is.EqualTo(0));
        }

        [Test]
        public void TestConstructorFromExistingAttributues()
        {
            var copyAttrs = new Attributes();
            copyAttrs.Set("fTest", 5.3f);
            copyAttrs.Set("iTest", 10);
            copyAttrs.Set("bTest", true);
            copyAttrs.Set("sTest", "peekachu");

            var attrs = new Attributes(copyAttrs);
            Assert.That(attrs.Count, Is.EqualTo(copyAttrs.Count));
            Assert.That(attrs.Get<float>("fTest"), Is.EqualTo(copyAttrs.Get<float>("fTest")));
            Assert.That(attrs.Get<int>("iTest"), Is.EqualTo(copyAttrs.Get<int>("iTest")));
            Assert.That(attrs.Get<bool>("bTest"), Is.EqualTo(copyAttrs.Get<bool>("bTest")));
            Assert.That(attrs.Get<string>("sTest"), Is.EqualTo(copyAttrs.Get<string>("sTest")));
        }

        [Test]
        public void TestConstructorFromExistingHashtable()
        {
            var copyHash = new Hashtable
            {
                {"fTest", 10.7f},
                {"iTest", 87},
                {"bTest", false},
                {"sTest", "meow"}
            };
            var attrs = new Attributes(copyHash);
            Assert.That(attrs.Count, Is.EqualTo(copyHash.Count));
            Assert.That(attrs.Get<float>("fTest"), Is.EqualTo(copyHash["fTest"]));
            Assert.That(attrs.Get<int>("iTest"), Is.EqualTo(copyHash["iTest"]));
            Assert.That(attrs.Get<bool>("bTest"), Is.EqualTo(copyHash["bTest"]));
            Assert.That(attrs.Get<string>("sTest"), Is.EqualTo(copyHash["sTest"]));
        }

        [Test]
        public void TestCollectionInitialisation()
        {
            var attrs = new Attributes
            {
                {"sTest", "poops"},
                {"fTest", 87.7f},
                {"bTest", true},
                {"iTest", 12345}
            };
            Assert.That(attrs.Count, Is.EqualTo(4));
            Assert.That(attrs.Get<string>("sTest"), Is.EqualTo("poops"));
            Assert.That(attrs.Get<float>("fTest"), Is.EqualTo(87.7f));
            Assert.That(attrs.Get<bool>("bTest"), Is.EqualTo(true));
            Assert.That(attrs.Get<int>("iTest"), Is.EqualTo(12345));
        }

        [Test]
        public void TestEmptyEquality()
        {
            var attrs1 = new Attributes();
            var attrs2 = new Attributes();
            Assert.That(attrs1, Is.EqualTo(attrs2));
        }

        [Test]
        public void TestEquality()
        {
            var attrs1 = new Attributes()
            {
                {"foo", "bar"},
                {"baz", "boop"}
            };
            var attrs2 = new Attributes()
            {
                {"foo", "bar"},
                {"baz", "boop"}
            };
            Assert.That(attrs1, Is.EqualTo(attrs2));
        }

        [Test]
        public void TestNegativeEquality()
        {
            var attrs1 = new Attributes()
            {
                {"goodpokemon", "trubbish"},
                {"badpokemon", "tangela"}
            };
            var attrs2 = new Attributes()
            {
                {"amazingpokemon", "scizor"},
                {"ridiculouspokemon", "binacle"}
            };
            Assert.That(attrs1 != attrs2);
        }

        [Test]
        public void TestEqualityDifferentOrderInitialisation()
        {
            var attrs1 = new Attributes
            {
                {"foo", "bar"},
                {"baz", "boop"},
                {"wow", "cool"},
                {"many", "tests"}
            };
            var attrs2 = new Attributes
            {
                {"many", "tests"},
                {"wow", "cool"},
                {"baz", "boop"},
                {"foo", "bar"}
            };
            Assert.That(attrs1 == attrs2);
        }

        [Test]
        public void TestThatAttributesCanBeConcatenated()
        {
            var attrs1 = new Attributes
            {
                {"big dog", "Roman"},
                {"not paranoia", "Usooooos penetentary"}
            };
            var attrs2 = new Attributes
            {
                {"rainmaker", "Okada"},
                {"villian", "Marty"}
            };
            Assert.That(attrs1 + attrs2, Is.EqualTo(
                new Attributes
                {
                    {"big dog", "Roman"},
                    {"not paranoia", "Usooooos penetentary"},
                    {"rainmaker", "Okada"},
                    {"villian", "Marty"}
                }
            ));
        }

        [Test]
        public void TestThatEmptyAttributesCanBeConcatenated()
        {
            var attrs1 = new Attributes();
            var attrs2 = new Attributes();
            Assert.That(attrs1 + attrs2, Is.EqualTo(new Attributes()));
        }

        [Test]
        public void TestThatEmptyAndNonEmptyAttributesCanBeConcatenated()
        {
            var attrs1 = new Attributes
            {
                {"boots and cats", "and boots and cats and boots"}
            };
            var attrs2 = new Attributes();
            Assert.That(attrs1 + attrs2, Is.EqualTo(
                new Attributes
                {
                    {"boots and cats", "and boots and cats and boots"}
                }
            ));
        }

        [Test]
        public void TestThatConcatenatedAttributesAreOverriden()
        {
            var attrs1 = new Attributes
            {
                {"a value", "I'm not going to be there"},
                {"another value", "Goodbye by brother"}
            };
            var attrs2 = new Attributes
            {
                {"a value", "I have defeated you"},
                {"different value", "So, we meet again"}
            };
            Assert.That(attrs1 + attrs2, Is.EqualTo(
                new Attributes
                {
                    {"another value", "Goodbye by brother"},
                    {"a value", "I have defeated you"},
                    {"different value", "So, we meet again"}
                }
            ));
        }

        [Test]
        public void TestIteration()
        {
            var attrsReference = new Hashtable
            {
                {"sTest", "hello"},
                {"fTest", 123.7f},
                {"bTest", false},
                {"iTest", 7654321}
            };
            var attrs = new Attributes(attrsReference);
            using(var iterator = attrs.GetEnumerator())
            {
                var index = 0;
                while(iterator.MoveNext())
                {
                    if(attrsReference.ContainsKey(iterator.Current.Key) &&
                       attrsReference[iterator.Current.Key] == iterator.Current.Value)
                    {
                        attrsReference.Remove(iterator.Current.Key);
                    }
                    index++;
                }
                Assert.That(index, Is.EqualTo(4));
                Assert.That(attrsReference.Count, Is.EqualTo(0));
            }
        }

        [Test]
        public void TestNonExistantKey()
        {
            var attrs = new Attributes();
            Assert.That(()=>attrs.Get<float>("nope"), Throws.TypeOf<KeyNotFoundException>());
        }

        [Test]
        public void TestFloats()
        {
            var attrs = new Attributes()
            {
                {"floating", 10.5f}
            };
            Assert.That(attrs.Get<float>("floating"), Is.EqualTo(10.5f));
            Assert.That(attrs.Get<int>("floating"), Is.EqualTo(10));
            Assert.That(()=>attrs.Get<string>("floating"), Throws.TypeOf<InvalidCastException>());
            Assert.That(()=>attrs.Get<bool>("floating"), Throws.TypeOf<InvalidCastException>());
        }

        [Test]
        public void TestIntegers()
        {
            var attrs = new Attributes()
            {
                {"inting", 55}
            };
            Assert.That(attrs.Get<int>("inting"), Is.EqualTo(55));
            Assert.That(attrs.Get<float>("inting"), Is.EqualTo(55.0f));
            Assert.That(()=>attrs.Get<string>("inting"), Throws.TypeOf<InvalidCastException>());
            Assert.That(()=>attrs.Get<bool>("inting"), Throws.TypeOf<InvalidCastException>());
        }

        [Test]
        public void TestStrings()
        {
            var attrs = new Attributes()
            {
                {"stringy", "hello!"}
            };
            Assert.That(attrs.Get<string>("stringy"), Is.EqualTo("hello!"));
            Assert.That(()=>attrs.Get<int>("stringy"), Throws.TypeOf<InvalidCastException>());
            Assert.That(()=>attrs.Get<float>("stringy"), Throws.TypeOf<InvalidCastException>());
            Assert.That(()=>attrs.Get<bool>("stringy"), Throws.TypeOf<InvalidCastException>());
        }

        [Test]
        public void TestBools()
        {
            var attrs = new Attributes()
            {
                {"booling", true}
            };
            Assert.That(attrs.Get<bool>("booling"), Is.EqualTo(true));
            Assert.That(()=>attrs.Get<string>("booling"), Throws.TypeOf<InvalidCastException>());
            Assert.That(()=>attrs.Get<float>("booling"), Throws.TypeOf<InvalidCastException>());
            Assert.That(()=>attrs.Get<int>("booling"), Throws.TypeOf<InvalidCastException>());
        }

    }
}