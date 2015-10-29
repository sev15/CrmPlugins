using NUnit.Framework;

namespace SEV.Crm.Services.Tests
{
    [TestFixture]
    public class CrmDeleteCacheTests
    {
        private const string KEY = "test key";

        private ICrmCache m_cache;

        [SetUp]
        public void Init()
        {
            m_cache = new CrmDeleteCache();
        }

        [Test]
        public void Add_ShouldPutAnObjectInCache()
        {
            Assert.That(m_cache.Contains(KEY), Is.False);

            m_cache.Add(KEY, new object());

            Assert.That(m_cache.Contains(KEY), Is.True);
        }

        [Test]
        public void Remove_ShouldRemoveAnObjectFromCache()
        {
            m_cache.Add(KEY, new object());

            m_cache.Remove(KEY);

            Assert.That(m_cache.Contains(KEY), Is.False);
        }

        [Test]
        public void Get_ShouldReturnAnObjectFromCacheByKey()
        {
            var obj = new object();
            m_cache.Add(KEY, obj);

            var value = m_cache.Get(KEY);

            Assert.That(value, Is.SameAs(obj));
        }

        [Test]
        public void Add_ShouldReplaceExistingObjectInCache_WhenAnotherObjectIsAddedForTheSamekey()
        {
            var obj = new object();
            m_cache.Add(KEY, obj);

            m_cache.Add(KEY, new object());

            Assert.That(m_cache.Contains(KEY), Is.True);
            Assert.That(m_cache.Get(KEY), Is.Not.SameAs(obj));
        }

        [Test]
        public void Contains_ShouldReturnTrue_WhenObjectIsAddedInCacheForTheGivenKey()
        {
            m_cache.Add(KEY, new object());

            bool result = m_cache.Contains(KEY);

            Assert.That(result, Is.True);
        }

        [Test]
        public void Contains_ShouldReturnFalse_WhenObjectIsNotAddedInCacheForTheGivenKey()
        {
            const string anotherKey = "key2";
            Assert.That(anotherKey, Is.Not.EqualTo(KEY));
            m_cache.Add(anotherKey, new object());

            bool result = m_cache.Contains(KEY);

            Assert.That(result, Is.False);
        }
    }
}