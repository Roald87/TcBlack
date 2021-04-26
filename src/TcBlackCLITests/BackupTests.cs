using System;
using System.IO;
using TcBlackCLI;
using Xunit;

[assembly: CLSCompliant(false)]
namespace TcBlackTests
{
    public class BackupTests
    {
        readonly string testDataPath = Path.GetFullPath("../../BackupTestData/");

        [Fact]
        public void InitializeObjectCreatesBackupFile()
        {
            string filename = Path.Combine(
                testDataPath, "InitializeObjectCreatesBackupFile.txt"
            );
            Backup backup = new Backup(filename);
            // Shouldn't raise a DirectoryNotFoundException
            File.ReadAllText(filename + ".bak");
            // Clean up
            backup.Delete();
        }

        [Fact]
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Usage",
            "CA1806: Do not ignore method results.",
            Justification = 
                "Only check that no execption is raised if the file already exists."
        )]
        public void BackupFileAlreadyExistsShouldOverwriteBackupFile()
        {
            string filename = Path.Combine(
                testDataPath, "BackupFileAlreadyExists.txt"
            );
            // Shouldn't raise an exception that the file already exists
            new Backup(filename);
        }

        [Fact]
        public void RestoreBackupFile()
        {
            string filename = Path.Combine(testDataPath, "RestoreFileTest.txt");
            string originalText = "Original text.";
            File.AppendAllText(filename, originalText);
            Assert.Equal(originalText, File.ReadAllText(filename));

            Backup backup = new Backup(filename);
            string newText = "New text.";
            File.WriteAllText(filename, newText);
            Assert.Equal(newText, File.ReadAllText(filename));

            backup.Restore();
            Assert.Equal(originalText, File.ReadAllText(filename));

            File.Delete(filename);
            File.Delete($"{filename}.bak");
        }
    }
}
