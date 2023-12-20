using Demo2;
using System;

namespace Demo2Test
{
    public class LongTimeFixture : IDisposable
    {
        public LongTimeTask Task { get; }
        public LongTimeFixture()
        {
            Task = new LongTimeTask();
        }
        public void Dispose()
        {
        }
    }
}
