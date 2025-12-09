using System;
using System.Windows.Input;

public class RelayCommand : ICommand
{
    private readonly Action<object?> _execute;
    private readonly Func<object?, bool>? _canExecute;

    // 1. Dichiarazione di un evento standard. 
    //    Ora la classe "possiede" l'evento e può invocarlo.
    public event EventHandler? CanExecuteChanged;

    public RelayCommand(Action<object?> execute, Func<object?, bool>? canExecute = null)
    {
        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecute = canExecute;
    }

    public RelayCommand(Action execute, Func<bool>? canExecute = null)
        : this(_ => execute(), canExecute != null ? _ => canExecute() : null)
    {
    }

    public bool CanExecute(object? parameter)
    {
        return _canExecute?.Invoke(parameter) ?? true;
    }

    public void Execute(object? parameter)
    {
        _execute(parameter);
    }

    // 2. Metodo pubblico per sollevare l'evento.
    //    Il ViewModel chiamerà questo metodo.
    public void RaiseCanExecuteChanged()
    {
        // Questo invoca l'evento, notificando alla UI di rivalutare CanExecute.
        CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}