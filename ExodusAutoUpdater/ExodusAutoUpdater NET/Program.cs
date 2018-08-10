using System;
using System.IO;
using Newtonsoft.Json;
using System.Net;
using System.Threading.Tasks;
using ExodusAutoUpdater;

namespace ExodusAutoUpdater_NET
{
    class Program
    {
        static void Main(string[] args)
        {
            string latestReleasedVersion = GetLatestRelease();
            string latestInstalledVersion = GetLatestInstallation(LoadJson());
            Console.WriteLine("Zuletzt installierte Version: " + latestInstalledVersion);
            Console.WriteLine("Zuletzt veröffentlichte Version: " + latestReleasedVersion);
            if (latestReleasedVersion == latestInstalledVersion)
            {
                Console.WriteLine("Sie verwenden die aktuelle Version!");
            }
            else
            {
                Console.WriteLine("Es gibt eine neuere Version!");
                string exPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "/ExodusUpdate";


                if (Directory.Exists(exPath))
                {
                    Download_Data(exPath, latestReleasedVersion);
                }
                else
                {
                    try
                    {
                        Directory.CreateDirectory(exPath);
                    } 
                    catch (Exception)
                    {

                        throw;
                    }
                    Download_Data(exPath, latestReleasedVersion);
                }
                Task taskA = Task.Factory.StartNew(() => Open_Data(exPath));

                while (taskA.Status != TaskStatus.RanToCompletion)
                {

                }

                Delete_Data(exPath);

            }
            Console.ReadLine();
        }

        public static string GetLatestInstallation(dynamic array)
        {
            string convertedJson = Convert.ToString(array);
            string latestRelease = convertedJson.Substring(convertedJson.LastIndexOf("exodusVersionUpdated"));
            latestRelease = latestRelease.Truncate(31);
            latestRelease = latestRelease.Remove(0, 24);
            string latestReleaseLocal = latestRelease.TrimEnd('"');
            return latestReleaseLocal;
        }

        public static string GetLatestRelease()
        {
            string webString = new System.Net.WebClient().DownloadString("https://www.exodus.io/releases/");
            string latestRelease = webString.Substring(webString.LastIndexOf("Latest Release"));
            string latestReleaseOnline = latestRelease.Remove(0, 15);
            latestReleaseOnline = latestReleaseOnline.Truncate(6);
            return latestReleaseOnline;

        }

        public static dynamic LoadJson()
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            path = path + "\\Exodus\\exodus.conf.json";
            string json = "";
            using (StreamReader r = new StreamReader(path))
            {
                json = r.ReadToEnd();

            }

            dynamic array = JsonConvert.DeserializeObject(json);
            return array;

        }


        public static void Download_Data(string exPath, string version)
        {
            using (var client = new WebClient())
            {
                try
                {

                    string exePath = "https://exodusbin.azureedge.net/releases/exodus-windows-x64-" + version + ".exe";
                    client.DownloadFile(new Uri(exePath, UriKind.Absolute), exPath + @"/ExodusInstaller.exe");


                }
                catch (Exception)
                {

                    throw;
                }


            }
        }

        public static void Open_Data(string exPath)
        {
            try
            {
                System.Diagnostics.Process.Start(exPath + "/ExodusInstaller.exe");

            }
            catch (Exception)
            {

                throw;
            }
        }

        public static void Delete_Data(string exPath)
        {
            Directory.Delete(exPath, true);
        }
    }


}
