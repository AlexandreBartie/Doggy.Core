using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Dooggy
{
    public class xDiretorio
    {

        public string path;

        public bool Criar() => Criar(path);
        public bool Criar(string prmPath)
        {

            path = prmPath;

            try
            {
                // Se o diretório não existir...
                if (!Directory.Exists(prmPath))
                {

                    // Criamos o diretório ...
                    Directory.CreateDirectory(prmPath);

                }

                return (true);
            }
            catch
            { }

            return (false);

        }

    }
}
