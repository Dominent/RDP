
namespace GD.RDP.GUI.PacketTracer.ViewModels
{
    using System;
    using System.ComponentModel;
    using System.Linq.Expressions;

    public abstract class ObservableObject : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged<T>(Expression<Func<T>> func)
        {
            if (this.PropertyChanged == null)
            {
                return;
            }

            MemberExpression expression = func.Body as MemberExpression;

            var property = expression.Member.Name;

            PropertyChanged(this, new PropertyChangedEventArgs(property));
        }
    }
}
