using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeveragePairer
{
    public abstract class FileTasks
    {
        public abstract void CreateIO(string x);
        public abstract bool ExistsIO(string x);
    }

    public class Files : FileTasks
    {
        public override void CreateIO(string file)
        {
            if(!ExistsIO(file))
            {
                File.Create(file);
            }
        }

        public override bool ExistsIO(string file)
        {
            return File.Exists(file);
        }
    }

    public class Directories : FileTasks
    {
        public override void CreateIO(string directory)
        {
            if (!ExistsIO(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }

        public override bool ExistsIO(string directory)
        {
            return Directory.Exists(directory);
        }
    }
}
