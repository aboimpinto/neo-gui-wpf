namespace Neo.Gui.ViewModels
{
    public interface IView
    {
        object Tag { get; set; }

        object DataContext { get; set; }
    }
}
