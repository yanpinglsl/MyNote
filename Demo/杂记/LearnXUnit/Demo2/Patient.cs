using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Demo2
{
    public class Patient : Person, INotifyPropertyChanged
    {
        public Patient()
        {
            IsNew = true;
            BloodSugar = 4.900003f;
            History = new List<string>();
            //NotAllowed();
        }

        public string FullName => $"{FirstName} {LastName}";
        public int HeartBeatRate { get; set; }

        public bool IsNew { get; set; }

        public float BloodSugar { get; set; }
        public List<string> History { get; set; }

        /// <summary>
        /// 事件
        /// </summary>
        public event EventHandler<EventArgs> PatientSlept;


        public void OnPatientSleep()
        {
            PatientSlept?.Invoke(this, EventArgs.Empty);
        }

        public void Sleep()
        {
            OnPatientSleep();
        }

        public void NotAllowed()
        {
            throw new InvalidOperationException("not able to create");
        }

        public void IncreaseHeartBeatRate()
        {
            HeartBeatRate = CalculateHeartBeatRate() + 2;
            OnPropertyChanged(nameof(HeartBeatRate));
        }


        private int CalculateHeartBeatRate()
        {
            var random = new Random();
            return random.Next(1, 100);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            //var handler = PropertyChanged;
            //if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        }
    }
}
