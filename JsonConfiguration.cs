using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace StreamLabsSceneSwitcher
{
    public class JsonConfiguration<T> where T : JsonConfiguration<T>, new()
    {
        private static readonly object _instantiationLockObject = new object();

        private static bool _isInstanceSet = false;
        private static T _instance;
        public static T Instance
        {
            get
            {
                // lock here due to possible instantiation, as if unlocked, a thread could set _isInstanceSet to true
                // before it's done with instantiation, potentially causing another thread to get a null instance
                lock (_instantiationLockObject)
                {
                    if (_isInstanceSet == false)
                    {
                        Load();
                    }

                    return _instance;
                }
            }
        }

        public static void Load()
        {
            _isInstanceSet = true;

            string appsettingsFilename = "appsettings.json";

            string environment = System.Environment.GetEnvironmentVariable("Environment");

            if (String.IsNullOrWhiteSpace(environment) == false)
                appsettingsFilename = $"appsettings.{environment.Trim()}.json";

            appsettingsFilename = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, appsettingsFilename);

            if (File.Exists(appsettingsFilename) == false)
                throw new FileNotFoundException("Couldn't find: " + appsettingsFilename + " in application directory: " + Directory.GetCurrentDirectory() + ". Did you set the the Copy To Output Directory = Copy If Newer in the file properties in Visual Studio?");

            IConfigurationBuilder builder = new ConfigurationBuilder();
            //.SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            //.AddJsonFile(appsettingsFilename, optional: true, reloadOnChange: true);

            builder.AddJsonFile(appsettingsFilename, false, true);

            IConfigurationRoot configRoot = builder.Build();

            _instance = new T();
            configRoot.GetSection("configuration").Bind(_instance);
        }


        


        
    }
}
