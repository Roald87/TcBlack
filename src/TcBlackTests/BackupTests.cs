using Xunit;
using TcBlack;
using System.IO;

namespace TcBlackTests
{
    public class BackupTests
    {
        [Fact]
        public void InitializeObjectCreatesBackupFile()
        {
            string filename = "../../../BackupTestData/InitializeObjectCreatesBackupFile.txt";
            Backup backup = new Backup(filename);
            // Shouldn't raise a DirectoryNotFoundException
            File.ReadAllText(filename + ".bak");
            // Clean up
            backup.Delete();
        }

        [Fact]
        public void BackupFileAlreadyExistsShouldOverwriteBackupFile()
        {
            string filename = "../../../BackupTestData/BackupFileAlreadyExists.txt";
            // Shouldn't raise an exception that the file already exists
            Backup backup = new Backup(filename);
        }

        [Fact]
        public void RestoreBackupFile()
        {
            string filename = "../../../BackupTestData/RestoreFileTest.txt";
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
