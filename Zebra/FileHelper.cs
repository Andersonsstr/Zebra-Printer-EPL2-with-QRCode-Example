using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Zebra
{

    //Esta classe é usada somente para guardar o nome da impressora Zebra padrão que for selecionado pelo o usuário.

    public class FileHelper
    {
        static string fileName = "zebraSelector";
        static string directory = System.AppDomain.CurrentDomain.BaseDirectory.ToString();
        string path = directory + fileName;

        public FileHelper()
        {
        }

        public void Escrever<T>(T conteudo)
        {
            ExcluirArquivo();
            CriaArquivo();

            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(this.path, true))
            {
                sw.WriteLine(Convert.ToBase64String(Encoding.ASCII.GetBytes(conteudo.ToString())));
                sw.Close();
                Ler();
            }
        }

        public string Ler()
        {
            string conteudo = string.Empty;
            if (File.Exists(this.path))
            {
                using (System.IO.StreamReader sr = new System.IO.StreamReader(this.path))
                {
                    conteudo = sr.ReadToEnd();
                    sr.Close();

                    conteudo = Encoding.ASCII.GetString(Convert.FromBase64String(conteudo));
                    Console.Write(conteudo);
                }
            }
            else
            {
                conteudo = "Empty";
            }
            return conteudo;
        }

        public void CriaArquivo()
        {
            if (!File.Exists(this.path))
            {
                using (File.Create(this.path));
            }
        }

        public void ExcluirArquivo()
        {
            if (File.Exists(this.path))
            {
                 File.Delete(this.path);
            }
        }
    }
}
