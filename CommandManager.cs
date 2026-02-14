/**************************************************************************************************************************
* Copyright 2024, Peter R. Nelson
*
* This file is part of the RealmStudio application. The RealmStudio application is intended
* for creating fantasy maps for gaming and world building.
*
* RealmStudio is free software: you can redistribute it and/or modify it under the terms
* of the GNU General Public License as published by the Free Software Foundation,
* either version 3 of the License, or (at your option) any later version.
*
* This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
* without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
* See the GNU General Public License for more details.
*
* You should have received a copy of the GNU General Public License along with this program.
* The text of the GNU General Public License (GPL) is found in the LICENSE.txt file.
* If the LICENSE.txt file is not present or the text of the GNU GPL is not present in the LICENSE.txt file,
* see https://www.gnu.org/licenses/.
*
* For questions about the RealmStudio application or about licensing, please email
* support@brookmonte.com
*
***************************************************************************************************************************/
#nullable enable

namespace RealmStudioX
{
    public sealed class CommandManager
    {
        public event Action? HistoryChanged;

        private readonly Stack<ICommand> _undo = new();
        private readonly Stack<ICommand> _redo = new();

        public void Execute(ICommand command)
        {
            command.Execute();
            _undo.Push(command);
            
            ClearRedo();

            HistoryChanged?.Invoke();
        }

        public void Undo()
        {
            if (_undo.TryPop(out var cmd))
            {
                cmd.Undo();
                _redo.Push(cmd);

                HistoryChanged?.Invoke();
            }
        }

        public void Redo()
        {
            if (_redo.TryPop(out var cmd))
            {
                cmd.Execute();
                _undo.Push(cmd);

                HistoryChanged?.Invoke();
            }
        }

        private void ClearRedo()
        {
            while (_redo.Count > 0)
            {
                _redo.Pop().Dispose();
            }
        }

        public void ClearAll()
        {
            while (_undo.Count > 0) _undo.Pop().Dispose();
            while (_redo.Count > 0) _redo.Pop().Dispose();
        }
    }
}
