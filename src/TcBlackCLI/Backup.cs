using System.IO;

namespace TcBlackCLI
{
    public class Backup
    {
        private string _filename;
        private string backUpFilename;

        /// <summary>
        /// Creates a backup .bak file of the original file.
        /// </summary>
        /// <param name="filename">Filename to back-up.</param>
        public Backup(string filename)
        {
            _filename = filename;
            backUpFilename = $"{filename}.bak";
            File.Copy(_filename, backUpFilename, true);
        }

        /// <summary>
        /// Overwrite the original file with the .bak file.
        /// </summary>
        public Backup Restore()
        {
            File.Copy(backUpFilename, _filename, true);

            return this;
        }

        /// <summary>
        /// Deletes the .bak file.
        /// </summary>
        public void Delete()
        {
            File.Delete(backUpFilename);
        }
    }
}
