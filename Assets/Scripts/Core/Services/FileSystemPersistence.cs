using System.IO;
using Game.Core.Interfaces;

namespace Game.Core.Services
{
    public class FileSystemPersistence : IPersistence
    {
        public bool Exists(string path) => File.Exists(path);

        public void WriteAllText(string path, string contents) => File.WriteAllText(path, contents);

        public string ReadAllText(string path) => File.ReadAllText(path);
    }
}
