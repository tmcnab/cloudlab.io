namespace Jurassic.Cloudlab
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using DropNet;
    using DropNet.Exceptions;
    using Jurassic;
    using Jurassic.Library;
    using Jurassic.Numerics;

    [Serializable]
    public class FileSystemModule : ObjectInstance
    {
        public FileSystemModule(ScriptEngine engine, DropNetClient storageInterface) : base(engine)
        {
            this.PopulateFunctions();
            this.StorageInterface = storageInterface;
            this.CurrentWorkingDirectory = "/";
        }

        #region Members

        public DropNetClient StorageInterface { get; private set; }

        private string CurrentWorkingDirectory { get; set; }

        #endregion

        #region User-Visible Functions

        #region Attributes (Documented, Requires V&V)

        // Documented
        [JSFunction(Name="size")]
        public int Size(string path)
        {
            path = this.PathGuard(path);
            try
            {
                var fileMD = this.StorageInterface.GetMetaData(path);
                if (fileMD.Is_Dir) {
                    throw new JavaScriptException(this.Engine, "Error", "Specified path '" + path + "' is not a file");
                }

                if (fileMD.Bytes > int.MaxValue)
                {
                    throw new JavaScriptException(this.Engine, "RangeError", "File at path '" + path + "' is too large to represent");
                }
                else
                {
                    return (int)fileMD.Bytes;
                }
            }
            catch
            {                
                throw new JavaScriptException(this.Engine, "Error", "Specified file at path '" + path + "' was not found");
            }
        }

        // Documented
        [JSFunction(Name = "lastModified")]
        public DateInstance LastModified(string path)
        {
            path = this.PathGuard(path);
            try
            {
                var fileMD = this.StorageInterface.GetMetaData(path);
                if (fileMD.Is_Dir)
                {
                    throw new JavaScriptException(this.Engine, "FileError", "Specified path '" + path + "' is not a file");
                }

                return this.Engine.Date.Construct((fileMD.ModifiedDate.ToUniversalTime() - new DateTime(1970,1,1)).TotalMilliseconds);
            }
            catch
            {
                throw new JavaScriptException(this.Engine, "FileError", "Specified file at path '" + path + "' was not found");
            }
        }

        #endregion

        #region Directories (Documented, Requires V&V)

        // Documented
        [JSFunction(Name="list")]
        public ArrayInstance List([DefaultParameterValue("")] string path = "")
        {
            path = this.PathGuard(path);
            try
            {
                var filenames = new List<string>();

                var md = this.StorageInterface.GetMetaData(path);
                if (!md.Is_Dir) throw new Exception();

                foreach (var item in md.Contents)
                {
                    filenames.Add(item.Name);
                }

                return this.Engine.Array.Construct(filenames.ToArray());
            }
            catch
            {
                throw new JavaScriptException(this.Engine, "FileError", "Directory could not be found or is not a directory");
            }
        }

        // Documented
        [JSFunction(Name="makeDirectory")]
        public void MakeDirectory(string path)
        {
            path = this.PathGuard(path);
            try
            {
                this.StorageInterface.CreateFolder(path);
            }
            catch
            {
                throw new JavaScriptException(this.Engine, "FileError", "directory could not be created");
            }
        }

        // Documented
        [JSFunction(Name="removeDirectory")]
        public void RemoveDirectory(string path)
        {
            path = this.PathGuard(path);
            try
            {
                var fsEntry = this.StorageInterface.GetMetaData(path);
                if (fsEntry.Is_Dir)
                {
                    if (fsEntry.Contents.Count != 0)
                    {
                        throw new JavaScriptException(this.Engine, "FileError", "specified directory must be empty before removal");
                    }
                    else
                    {
                        try
                        {
                            this.StorageInterface.Delete(path);
                        }
                        catch
                        {
                            throw new JavaScriptException(this.Engine, "FileError", "directory could not be deleted");
                        }
                    }
                }
                else
                {
                    throw new JavaScriptException(this.Engine, "FileError", "specified path is not a directory");
                }
            }
            catch
            {
                throw new JavaScriptException(this.Engine, "FileError", "specified file or directory does not exist");
            }
        }

        #endregion

        #region Files

        // Documented
        [JSFunction(Name = "copy")]
        public void Copy(string source, string target)
        {
            if (source.IsNullOrWhiteSpace() || target.IsNullOrWhiteSpace())
            {
                throw new JavaScriptException(this.Engine, "FileError", "input paths must be valid");
            }

            try
            {
                this.StorageInterface.Copy(this.PathGuard(source), this.PathGuard(target));
            }
            catch
            {
                throw new JavaScriptException(this.Engine, "FileError", "there was an error copying the file");
            }
        }

        // Documented
        [JSFunction(Name = "read")]
        public object Read(string path, [DefaultParameterValue("string")] string mode)
        {
            path = this.PathGuard(path);
            try  
            {
                var file = this.StorageInterface.GetMetaData(path);
                if (mode == "binary")
                {
                    return new ByteStringInstance(this.Engine.Object.InstancePrototype, this.StorageInterface.GetFile(path));
                }
                else if (mode == "string")
                {
                    return UTF8Encoding.UTF8.GetString(this.StorageInterface.GetFile(path));
                }
                else
                {
                    throw new JavaScriptException(this.Engine, "RangeError", "mode must be 'string' or 'binary'");
                }
            }
            catch (DropboxException ex) {
                System.Diagnostics.Debug.WriteLine(ex.Response.Content);
                throw new JavaScriptException(this.Engine, "Error", ex.Response.Content); 
            }
        }

        // Documented
        [JSFunction(Name = "remove")]
        public void Remove(string path)
        {
            path = this.PathGuard(path);
            try
            {
                if (!this.StorageInterface.GetMetaData(path).Is_Dir)
                {
                    try   { this.StorageInterface.Delete(path); }
                    catch { throw new JavaScriptException(this.Engine, "FileError", "dropbox could not delete the file"); }
                }
                else
                {
                    throw new JavaScriptException(this.Engine, "FileError", "Specified path '" + path + "' is not a file");
                }
            }
            catch
            {
                throw new JavaScriptException(this.Engine, "FileError", "No such file at path '" + path + "' exists");
            }
        }

        // Not Tested
        [JSFunction(Name = "rename")]
        public void Rename(string path, string name)
        {
            if (!this.StorageInterface.GetMetaData(this.PathGuard(path)).Is_Dir)
            {
                this.StorageInterface.Move(this.PathGuard(path), this.DirectoryFromPath(path) + this.FilenameFromPath(name));
            }
        }

        // Stubbed
        [JSFunction(Name = "touch")]
        public void Touch(string path, [DefaultParameterValue(null)] DateInstance date = null)
        {
            throw new NotImplementedException();
        }

        // Stubbed
        [JSFunction(Name = "write")]
        public bool Write(string path, object content)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Paths

        [JSFunction(Name = "changeWorkingDirectory")]
        public void ChangeWorkingDirectory(string path)
        {
            path = this.PathGuard(path);
            try
            {
                if (this.StorageInterface.GetMetaData(path).Is_Dir)
                {
                    this.CurrentWorkingDirectory = path;
                }
                else
                {
                    throw new JavaScriptException(this.Engine, "FileError", "cannot change directory to a non-directory path");
                }
            }
            catch
            {
                throw new JavaScriptException(this.Engine, "FileError", "supplied path does not exist");
            }
        }

        [JSFunction(Name = "workingDirectory")]
        public string WorkingDirectory()
        {
            return this.CurrentWorkingDirectory;
        }

        #endregion

        #region Tests (Documented, Requires V&V)

        // Documented
        [JSFunction(Name = "exists")]
        public bool Exists(string path)
        {
            try
            {
                this.StorageInterface.GetMetaData(this.PathGuard(path));
                return true;
            }
            catch
            {
                return false;
            }
        }

        // Documented
        [JSFunction(Name = "isDirectory")]
        public bool IsDirectory(string path)
        {
            try
            {
                return this.StorageInterface.GetMetaData(this.PathGuard(path)).Is_Dir;
            }
            catch
            {
                return false;
            }
        }

        // Documented
        [JSFunction(Name = "isFile")]
        public bool IsFile(string path)
        {
            try
            {
                return !this.StorageInterface.GetMetaData(this.PathGuard(path)).Is_Dir;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #endregion

        #region Utilities

        public string PathGuard(string path)
        {
            if (path.StartsWith("/"))
                return path;
            else
                return this.CurrentWorkingDirectory + path;
        }

        public string FilenameFromPath(string path)
        {
            path = PathGuard(path);
            var components = path.Split('/');
            return components[components.Length - 1];
        }

        public string DirectoryFromPath(string path)
        {
            path = PathGuard(path);
            var components = path.Split('/');
            string dir = string.Empty;
            for (int i = 0; i < components.Length - 2; i++)
            {
                dir += '/' + components[i];
            }
            return dir;
        }

        #endregion
    }
}