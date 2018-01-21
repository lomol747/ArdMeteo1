using System.Windows.Input;

namespace ArdMeteo
{
    /// <summary>
    /// Интерфейс для обработки кнопок
    /// </summary>
    public interface IDelegateCommand : ICommand
    {
        void RaiseCanExecuteChanged();
    }
}
//НЕ ИСПОЛЬЗУЕТСЯ