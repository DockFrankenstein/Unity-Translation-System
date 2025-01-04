using System.IO;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace Translations.Editor
{
    public class BuildPostprocess : IPostprocessBuildWithReport
    {
        public int callbackOrder => 0;

        public void OnPostprocessBuild(BuildReport report)
        {
            var serializer = TranslationSettings.Instance.serialization;

            //Move editor translations to build
            var transPath = $"{Path.GetDirectoryName(report.summary.outputPath)}/{serializer.path}";

            if (Directory.Exists(transPath))
                Directory.Delete(transPath, true);

            CopyTrans(serializer.TranslationRootPath, transPath);
        }

        static void CopyTrans(string sourceDir, string destinationDir)
        {
            // Get information about the source directory
            var dir = new DirectoryInfo(sourceDir);

            // Check if the source directory exists
            if (!dir.Exists)
                return;

            // Cache directories before we start copying
            DirectoryInfo[] dirs = dir.GetDirectories();

            // Create the destination directory
            Directory.CreateDirectory(destinationDir);

            // Get the files in the source directory and copy to the destination directory
            foreach (FileInfo file in dir.GetFiles())
            {
                if (file.Extension == ".meta")
                    continue;

                string targetFilePath = Path.Combine(destinationDir, file.Name);
                file.CopyTo(targetFilePath);
            }

            foreach (DirectoryInfo subDir in dirs)
            {
                string newDestinationDir = Path.Combine(destinationDir, subDir.Name);
                CopyTrans(subDir.FullName, newDestinationDir);
            }
        }

    }
}