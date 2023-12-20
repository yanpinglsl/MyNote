using Demo2;
using System;
using System.Collections.Generic;
using Xunit;
using Xunit.Abstractions;

namespace Demo2Test
{
    /// <summary>
    /// PatientShould：针对Patient的测试，注意命名规范
    /// </summary>
    [Collection("Lone Time Task Collection")]
    public class PatientShould:IClassFixture<LongTimeFixture>,IDisposable
    {
        private readonly ITestOutputHelper _output;
        private readonly Patient _patient;
        private readonly LongTimeTask _task;
        public PatientShould(ITestOutputHelper output,LongTimeFixture fixture)
        {
            this._output = output;
            _patient = new Patient();
            //_task = new LongTimeTask();
            _task = fixture.Task;
        }

        [Fact]
        [Trait("Category","New")]
        public void BeNewWhenCreated()
        {
            _output.WriteLine("第一个测试");
            // Arrange
            //var patient = new Patient();
            // Act
            var result = _patient.IsNew;
            // Assert
            Assert.True(result);
            //Assert.False(result);
        }

        [Fact]
        public void HaveCorrectFullName()
        {
            //var patient = new Patient();
            _patient.FirstName = "Nick";
            _patient.LastName = "Carter";
            var fullName = _patient.FullName;
            Assert.Equal("Nick Carter", fullName);
            Assert.StartsWith("Nick", fullName);
            Assert.EndsWith("Carter", fullName);
            Assert.Contains("Carter", fullName);
            Assert.Contains("Car", fullName);
            Assert.NotEqual("CAR", fullName);
            Assert.Matches(@"^[A-Z][a-z]*\s[A-Z][a-z]*", fullName);
        }

        [Fact]
        [Trait("Category", "New")]
        public void HaveDefaultBloodSugarWhenCreated()
        {
            //var p = new Patient();
            var bloodSugar = _patient.BloodSugar;
            Assert.Equal(4.9f, bloodSugar,5);
            Assert.InRange(bloodSugar, 3.9, 6.1);
        }

        [Fact]
        public void HaveNoNameWhenCreated()
        {
            //var p = new Patient();
            Assert.Null(_patient.FirstName);
            Assert.NotNull(_patient);
        }

        //[Fact]
        [Fact(Skip ="不需要跑这个测试")]
        public void HaveHadAColdBefore()
        {
            //var p = new Patient();

            var diseases = new List<string>
            {
                "感冒",
                "发烧",
                "水痘",
                "腹泻"
            };

            _patient.History.Add("发烧");
            _patient.History.Add("感冒");
            _patient.History.Add("水痘");
            _patient.History.Add("腹泻");

            Assert.Contains("感冒",_patient.History);
            Assert.DoesNotContain("心脏病", _patient.History);

            //判断p.History至少有一个元素：水
            Assert.Contains(_patient.History, x => x.StartsWith("水"));

            Assert.All(_patient.History, x => Assert.True(x.Length >= 2));

            //判断集合是否相等,是比较集合元素的值，而不是比较引用
            Assert.Equal(diseases, _patient.History);

        }

        /// <summary>
        /// 测试Object
        /// </summary>
        [Fact]
        public void BeAPerson()
        {
            var p = new Patient();
            var p2 = new Patient();
            Assert.IsNotType<Person>(p); //验证对象不是是某个类型
            Assert.IsType<Patient>(p);//验证对象是某个类型

            Assert.IsAssignableFrom<Person>(p);//验证某个对象是指定类型或指定类型的子类

            //判断是否为同一个实例
            Assert.NotSame(p, p2);
            //Assert.Same(p, p2);

        }

        /// <summary>
        /// 判断是否发生异常
        /// </summary>
        [Fact]
        public void ThrowException() //注意不能使用ctrl+R,T快捷键
        {
            var p = new Patient();
            var ex = Assert.Throws<InvalidOperationException>(()=> { p.NotAllowed(); });
            Assert.Equal("not able to create", ex.Message);
        }

        /// <summary>
        /// 判断是否触发事件
        /// </summary>
        [Fact]
        public void RaizeSleepEvent()
        {
            var p = new Patient();
            Assert.Raises<EventArgs>(
                handler=>p.PatientSlept+=handler,
                handler=>p.PatientSlept -= handler,
                () => p.Sleep());
        }

        /// <summary>
        /// 测试属性改变事件是否触发
        /// </summary>
        [Fact]
        public void RaisePropertyChangedEvent()
        {
            var p = new Patient();
            Assert.PropertyChanged(p, nameof(p.HeartBeatRate),
                () => p.IncreaseHeartBeatRate());
        }

        public void Dispose()
        {
            _output.WriteLine("清理了资源");
        }
    }
}
