namespace RealmStudioX
{
    internal class TabManager
    {
        private readonly TabControl _tabControl;
        private readonly Dictionary<string, TabPage> _allTabs = new();

        public TabManager(TabControl tabControl)
        {
            _tabControl = tabControl;

            foreach (TabPage tab in tabControl.TabPages)
            {
                _allTabs[tab.Name] = tab;
            }
        }

        public void SetVisible(string tabName, bool visible)
        {
            if (!_allTabs.TryGetValue(tabName, out var tab))
                return;

            bool isVisible = _tabControl.TabPages.Contains(tab);

            if (visible && !isVisible)
            {
                _tabControl.TabPages.Add(tab);
            }
            else if (!visible && isVisible)
            {
                _tabControl.TabPages.Remove(tab);
            }
        }
    }
}
