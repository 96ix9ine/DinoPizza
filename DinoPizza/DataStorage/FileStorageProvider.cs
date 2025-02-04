using Newtonsoft.Json;

namespace DinoPizza.DataStorage
{
    public class FileStorageProvider
    {
        public static string FileStorageFolder = "FileStorage";
        private readonly IWebHostEnvironment _webHostEnvironment;

        public string GetPath()
        {
            string rootPath = _webHostEnvironment.WebRootPath;
            rootPath = System.IO.Path.Combine(rootPath, FileStorageFolder);

            return rootPath;
        }

        public FileStorageProvider(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        private string GetFilename<T>()
        {
            string rootPath = _webHostEnvironment.WebRootPath;
            rootPath = System.IO.Path.Combine(rootPath, FileStorageFolder);
            System.IO.Directory.CreateDirectory(rootPath);

            string filename = typeof(T).FullName + ".json";
            filename = System.IO.Path.Combine(rootPath, filename);

            return filename;
        }

        public IEnumerable<T> LoadList<T>(string filename)
            where T : class, new()
        {
            if (!File.Exists(filename))
            {
                return Enumerable.Empty<T>();
            }

            string json = File.ReadAllText(filename);
            var list = JsonConvert
                .DeserializeObject<List<T>>(json);

            return list;
        }

        public void SaveList<T>(IEnumerable<T> list)
            where T : class, new ()
        {
            string json = JsonConvert.SerializeObject(list, 
                new JsonSerializerSettings 
                { 
                    Formatting = Formatting.Indented 
                });

            string filename = GetFilename<T>();
            File.WriteAllText(filename, json);
        }
    }
}
