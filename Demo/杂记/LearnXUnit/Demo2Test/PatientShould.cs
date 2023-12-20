using Demo2;
using System;
using System.Collections.Generic;
using Xunit;
using Xunit.Abstractions;

namespace Demo2Test
{
    /// <summary>
    /// PatientShould�����Patient�Ĳ��ԣ�ע�������淶
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
            _output.WriteLine("��һ������");
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
        [Fact(Skip ="����Ҫ���������")]
        public void HaveHadAColdBefore()
        {
            //var p = new Patient();

            var diseases = new List<string>
            {
                "��ð",
                "����",
                "ˮ��",
                "��к"
            };

            _patient.History.Add("����");
            _patient.History.Add("��ð");
            _patient.History.Add("ˮ��");
            _patient.History.Add("��к");

            Assert.Contains("��ð",_patient.History);
            Assert.DoesNotContain("���ಡ", _patient.History);

            //�ж�p.History������һ��Ԫ�أ�ˮ
            Assert.Contains(_patient.History, x => x.StartsWith("ˮ"));

            Assert.All(_patient.History, x => Assert.True(x.Length >= 2));

            //�жϼ����Ƿ����,�ǱȽϼ���Ԫ�ص�ֵ�������ǱȽ�����
            Assert.Equal(diseases, _patient.History);

        }

        /// <summary>
        /// ����Object
        /// </summary>
        [Fact]
        public void BeAPerson()
        {
            var p = new Patient();
            var p2 = new Patient();
            Assert.IsNotType<Person>(p); //��֤��������ĳ������
            Assert.IsType<Patient>(p);//��֤������ĳ������

            Assert.IsAssignableFrom<Person>(p);//��֤ĳ��������ָ�����ͻ�ָ�����͵�����

            //�ж��Ƿ�Ϊͬһ��ʵ��
            Assert.NotSame(p, p2);
            //Assert.Same(p, p2);

        }

        /// <summary>
        /// �ж��Ƿ����쳣
        /// </summary>
        [Fact]
        public void ThrowException() //ע�ⲻ��ʹ��ctrl+R,T��ݼ�
        {
            var p = new Patient();
            var ex = Assert.Throws<InvalidOperationException>(()=> { p.NotAllowed(); });
            Assert.Equal("not able to create", ex.Message);
        }

        /// <summary>
        /// �ж��Ƿ񴥷��¼�
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
        /// �������Ըı��¼��Ƿ񴥷�
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
            _output.WriteLine("��������Դ");
        }
    }
}
