using Prism.Commands;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CommandBindingDemo
{
    public class CommandViewModel : ObservableObject//, ReactiveObject
    {
        //public RelayCommand OpenCommand { get; set; } = new RelayCommand(() =>
        //{
        //    MessageBox.Show("Hello,World!");
        //});
        //WPF之Command
        public RelayCommand<object> OpenCommandParam { get; set; } = new RelayCommand<object>((param) =>
        {
            MessageBox.Show($"Hello,World!{param.ToString()}");
        });
        public RelayCommand<TextBox> MouseDownCommand { get; set; } = new RelayCommand<TextBox>((param) =>
        {
            param.Text += DateTime.Now + " 您单击了TextBox" + "\r";
        });
        //MvvmLight之RelayCommand
        public GalaSoft.MvvmLight.Command.RelayCommand<string> MvvmLightCommand { get; } = new GalaSoft.MvvmLight.Command.RelayCommand<string>((message) =>
            {
                MessageBox.Show(message);
            });
        //Prism之DelegateCommand
        //无参命令
        public DelegateCommand DelegateCommand { get; } = new DelegateCommand(() =>
        {
            MessageBox.Show("无参的DelegateCommand");
        });
        //有参命令
        public DelegateCommand<string> ParamCommand { get; }= new DelegateCommand<string>((message) =>
        {
            MessageBox.Show($"有参的DelegateCommand{message}");
         });
        //合并命令
        public CompositeCommand CompositeCommand { get; } = new CompositeCommand();

        //ReactiveUI之ReactiveCommand
       public ICommand GeneralCommand { get; }
        public ICommand ParamterCommand { get; }
        public ICommand TaskCommand { get; }
        public ICommand CombineCommand { get; }
        public ReactiveCommand<Unit,DateTime> ObservableCommand { get; }

        public CommandViewModel()
        {
            CompositeCommand.RegisterCommand(DelegateCommand);
            CompositeCommand.RegisterCommand(ParamCommand);

            GeneralCommand = ReactiveCommand.Create(General);
            ParamterCommand = ReactiveCommand.Create<object,bool>(GetParam);
            TaskCommand = ReactiveCommand.Create(GetTask);

            var childCommands = new List<ReactiveCommand<Unit, Unit>>();
            childCommands.Add(ReactiveCommand.Create<Unit, Unit>((param) =>
            {
                MessageBox.Show("childCommand1");
                return Unit.Default;
            }));
            childCommands.Add(ReactiveCommand.Create<Unit, Unit>((param) =>
            {
                MessageBox.Show("childCommand2");
                return Unit.Default;
            }));
            childCommands.Add(ReactiveCommand.Create<Unit, Unit>((param) =>
            {
                MessageBox.Show("childCommand3");
                return Unit.Default;
            }));
            CombineCommand = ReactiveCommand.CreateCombined(childCommands);
            ObservableCommand = ReactiveCommand.CreateFromObservable<Unit, DateTime>(DoObservableCommand);
            ObservableCommand.Subscribe(v => ShowObservableResult(v));
        }
        private void ShowObservableResult(DateTime v)
        {
            MessageBox.Show($"时间：{v}");
        }
        private IObservable<DateTime> DoObservableCommand(Unit arg)
        {
            //todo 业务代码

            var result = DateTime.Now;

            return Observable.Return(result).Delay(TimeSpan.FromSeconds(1));
        }
        public void General()
        {
            MessageBox.Show("ReactiveCommand！");
        }
        public async Task GetTask()
        {
            await Task.Delay(1000);
        }
        public bool GetParam(object arg)
        {
            MessageBox.Show(arg.ToString());
            return true;
        }

    }
}
