﻿using System.Collections.Generic;

namespace DemoTest
{
    public  class CalculatorTestData
    {
        private static readonly List<object[]> Data = new List<object[]>
        {
            new object[]{ 1,2,3},
            new object[]{ 1,3,4},
            new object[]{ 2,4,6},
            new object[]{ 0,1,1},
        };

        public static IEnumerable<object[]> TestData => Data;
    }
}
