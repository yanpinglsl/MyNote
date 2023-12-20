using Demo;
using DemoTest.Data;
using Xunit;

namespace DemoTest
{
    public class CalculatorTests
    {

        [Theory]
        [InlineData(1,2,3)]
        [InlineData(2,2,4)]
        [InlineData(3,3,6)]
        public void ShouldAddEquals(int operand1,int operand2,int expected)
        {
            //Arrange
            var sut = new Calculator(); //sut-system under test
            //Act
            var result = sut.Add(operand1, operand2);
            //Assert
            Assert.Equal(expected, result);
        }

        /// <summary>
        /// 数据共享-MemberData
        /// </summary>
        /// <param name="operand1"></param>
        /// <param name="operand2"></param>
        /// <param name="expected"></param>
        [Theory]
        [MemberData(nameof(CalculatorTestData.TestData),MemberType =typeof(CalculatorTestData))]
        public void ShouldAddEquals2(int operand1, int operand2, int expected)
        {
            //Arrange
            var sut = new Calculator(); //sut-system under test
            //Act
            var result = sut.Add(operand1, operand2);
            //Assert
            Assert.Equal(expected, result);
        }

        /// <summary>
        /// 数据共享-MemberData-外部数据
        /// </summary>
        /// <param name="operand1"></param>
        /// <param name="operand2"></param>
        /// <param name="expected"></param>
        [Theory]
        [MemberData(nameof(CalculatorCsvData.TestData), MemberType = typeof(CalculatorCsvData))]
        public void ShouldAddEquals3(int operand1, int operand2, int expected)
        {
            //Arrange
            var sut = new Calculator(); //sut-system under test
            //Act
            var result = sut.Add(operand1, operand2);
            //Assert
            Assert.Equal(expected, result);
        }

        /// <summary>
        /// 数据共享-自定义特性继承自DataAttribute
        /// </summary>
        /// <param name="operand1"></param>
        /// <param name="operand2"></param>
        /// <param name="expected"></param>
        [Theory]
        [CalculatorDataAttribute]
        public void ShouldAddEquals4(int operand1, int operand2, int expected)
        {
            //Arrange
            var sut = new Calculator(); //sut-system under test
            //Act
            var result = sut.Add(operand1, operand2);
            //Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void ShouldAddEquals5()
        {
            //Arrange
            var sut = new Calculator(); //sut-system under test
            //Act
            var result = sut.Add(3, 2);
            //Assert
            Assert.Equal(5, result);

        }

        [Fact]
        public void ShouldAddEquals6()
        {
            //Arrange
            var sut = new Calculator(); //sut-system under test
            //Act
            var result = sut.Add(3, 3);
            //Assert
            Assert.Equal(6, result);

        }

        [Fact]
        public void ShouldAddEquals7()
        {
            //Arrange
            var sut = new Calculator(); //sut-system under test
            //Act
            var result = sut.Add(3, 4);
            //Assert
            Assert.Equal(7, result);

        }
    }
}
