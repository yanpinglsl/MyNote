using Xunit;

namespace Demo2Test
{
    [CollectionDefinition("Lone Time Task Collection")]
    public class TaskCollection:ICollectionFixture<LongTimeFixture>
    {
    }
}
