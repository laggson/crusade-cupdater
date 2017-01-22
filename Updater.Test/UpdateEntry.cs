namespace Updater.Test
{
    internal class UpdateEntry
    {
        string name;
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
            }
        }
        string version;
        public string Version
        {
            get { return version; }
            set
            {
                version = value;
            }
        }

    }
}
