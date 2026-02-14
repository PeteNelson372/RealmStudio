namespace RealmStudioX
{
    public interface ICommand : IDisposable
    {
        void Execute();
        void Undo();
    }

}
