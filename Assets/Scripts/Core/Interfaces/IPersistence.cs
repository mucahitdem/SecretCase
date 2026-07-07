namespace Game.Core.Interfaces
{
    public interface IPersistence
    {
        bool Exists(string path);
        void WriteAllText(string path, string contents);
        string ReadAllText(string path);
    }
}
