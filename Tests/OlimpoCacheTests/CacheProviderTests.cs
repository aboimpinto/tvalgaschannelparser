using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OlimpoCache;

namespace OlimpoCacheTests
{
    [TestClass]
    public class CacheProviderTests
    {
        [TestMethod]
        public async Task SetCacheValue_SetKeyAndValue_KeyAndValueAreStored()
        {
            // Arrange
            var storedKey = "key";
            var storedValue = "value";

            // Act
            var testee = new MemoryCacheProvider();
            await testee.SetCacheValue(storedKey, storedValue);

            // Assert
            var returnedValue = await testee.GetCacheValueAsync(storedKey);
            returnedValue
                .Should()
                .Be(storedValue);
        }
    }
}
