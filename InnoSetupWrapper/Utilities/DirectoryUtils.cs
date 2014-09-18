using System.IO;
using System.Linq;
using System.Threading;

namespace SpiceLogic.InnoSetupWrapper.Utilities
{
    internal static class DirectoryUtils
    {
        /// <summary>
        /// Deletes this instance.
        /// </summary>
        public static void Delete(string directoryPath, CancellationToken cancellationToken)
        {
            deleteEngine(directoryPath, cancellationToken);
        }

        /// <summary>
        /// Deletes the engine.
        /// </summary>
        /// <param name="dirPath">The dir path.</param>
        /// <param name="cancellationToken"></param>
        private static void deleteEngine(string dirPath, CancellationToken cancellationToken)
        {
            //Get an object repesenting the directory path below
            DirectoryInfo di = new DirectoryInfo(dirPath);
            if (!di.Exists)
                return;
            //Traverse all of the child directors in the root; get to the lowest child
            //and delte all files, working our way back up to the top.  All files
            //must be deleted in the directory, before the directory itself can be deleted.
            foreach (DirectoryInfo diChild in di.GetDirectories())
            {
                cancellationToken.ThrowIfCancellationRequested();
                traverseDirectory(diChild, cancellationToken);
            }

            //Finally, clean all of the files directly in the root directory
            cleanAllFilesInDirectory(di, cancellationToken);
            di.Delete(true);
        }

        /// <SUMMARY>
        /// A method to traverse down through child directories until
        /// we have reached the lowest level and then clean (delete) all
        /// files before deleting the directory itself.
        /// </SUMMARY>
        /// <PARAM name="di"></PARAM>
        /// <REMARKS>All files must be deleted in a directory prior to deleting the
        /// directory itself to prevent the following exception:
        /// "The directory is not empty."
        /// </REMARKS>

        private static void traverseDirectory(DirectoryInfo di, CancellationToken cancellationToken)
        {
            //If the current directory has more child directories, then continure
            //to traverse down until we are at the lowest level.  At that point all of the
            //files will be deleted.
            foreach (DirectoryInfo diChild in di.GetDirectories())
            {
                cancellationToken.ThrowIfCancellationRequested();
                traverseDirectory(diChild, cancellationToken);
            }

            //Now that we have no more child directories to traverse, delete all of the files
            //in the current directory, and then delete the directory itself.
            cleanAllFilesInDirectory(di, cancellationToken);

            //The containing directory can only be deleted if the directory
            //is now completely empty and all files previously within
            //were deleted.
            if (!di.GetFiles().Any())
                di.Delete();
        }

        /// <summary>
        /// Cleans all files information directory.
        /// </summary>
        /// <param name="DirectoryToClean">The directory automatic clean.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        private static void cleanAllFilesInDirectory(DirectoryInfo DirectoryToClean, CancellationToken cancellationToken)
        {
            foreach (FileInfo fi in DirectoryToClean.GetFiles())
            {
                cancellationToken.ThrowIfCancellationRequested();

                //Read only files can not be deleted, so mark the attribute as 'IsReadOnly = False'
                fi.IsReadOnly = false;
                fi.Delete();

                //On a rare occasion, files being deleted might be slower than program execution, and upon returning
                //from this call, attempting to delete the directory will throw an exception stating it is not yet
                //empty, even though a fraction of a second later it actually is.  Therefore the 'Optional' code below
                //can stall the process just long enough to ensure the file is deleted before proceeding. The value
                //can be adjusted as needed from testing and running the process repeatedly.
                Thread.Sleep(50);
                //50 millisecond stall (0.05 Seconds)
            }
        }
    }
}
