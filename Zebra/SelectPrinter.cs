using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Zebra
{
    public partial class SelectPrinter : Form
    {
        public SelectPrinter()
        {
            InitializeComponent();

            foreach (string printer in System.Drawing.Printing.PrinterSettings.InstalledPrinters)
            {               
                cbImpressoras.Items.Add(printer);
                if (cbImpressoras.Items.Count > 0)
                    cbImpressoras.Select(0, 0);

                FileHelper arquivo = new FileHelper();
                string impressora = arquivo.Ler();

                if(impressora != "Empty")
                {
                    for (int i = 0; i<cbImpressoras.Items.Count; i++)
                    {
                        if(impressora == cbImpressoras.Items[i].ToString())
                        {
                            cbImpressoras.SelectedItem = impressora;
                        }
                    }
                }
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            FileHelper arquivo = new FileHelper();
            arquivo.Escrever<string>(cbImpressoras.SelectedItem.ToString());
            this.Dispose();
        }
    }
}
