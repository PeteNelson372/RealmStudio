namespace RealmStudio
{
    public static class CommandManager
    {
        private static readonly Stack<IMapOperation> UndoStack = new(100);
        private static readonly Stack<IMapOperation> RedoStack = new(100);

        public static void AddCommand(IMapOperation operation)
        {
            UndoStack.Push(operation);
            RedoStack.Clear();
        }

        public static void Undo()
        {

            if (UndoStack.TryPop(out IMapOperation? operation))
            {
                if (operation != null)
                {
                    RedoStack.Push(operation);
                    operation.UndoOperation();
                }

            }
        }

        public static void Redo()
        {
            if (RedoStack.TryPop(out IMapOperation? operation))
            {
                if (operation != null)
                {
                    UndoStack.Push(operation);
                    operation.DoOperation();
                }
            }
        }
    }
}
