using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.Text;
using System.Windows.Forms;

namespace Zebra
{
    public partial class ZebraQRCode : Form
    {
        public ZebraQRCode()
        {
            InitializeComponent();
        }      
        
        private void btnQrCode_Click(object sender, EventArgs e)
        {

            if(txtSKU.TextLength > 0)
            {
                QRCoder.QRCodeGenerator qrGenerator = new QRCoder.QRCodeGenerator();
                var qrData = qrGenerator.CreateQrCode(txtSKU.Text, QRCoder.QRCodeGenerator.ECCLevel.H);
                var qrCode = new QRCoder.QRCode(qrData);
                var image = qrCode.GetGraphic(150);
                pictureBox1.Image = image;

                //Redimenciona a imagem para 200x200px
                image = ImageHelper.RedimensionarImagem(image, 200, 200);
                //Corta as bordas brancas da imagem, tamanho final fica em 160x160px
                image = ImageHelper.CropBitmap(image, 20, 20, 160, 160);

                var sb = new StringBuilder();
                PrintDialog pd = new PrintDialog();
                pd.PrinterSettings = new System.Drawing.Printing.PrinterSettings();
                sb.AppendLine("I8,A,001");
                sb.AppendLine("Q160,024");
                sb.AppendLine("q831");
                sb.AppendLine("rN");
                sb.AppendLine("S4");
                sb.AppendLine("D7");
                sb.AppendLine("ZT");
                sb.AppendLine("JF");
                sb.AppendLine("OD");
                sb.AppendLine("R283,0");
                sb.AppendLine("f100");
                sb.AppendLine("N");

                //Converte bitmap para padrão .pcx que é o único formato de imagem suportado pela impressora
                sb.AppendLine(RawPrinterHelper.SendImageToPrinter(image, 30, 0));

                sb.AppendLine("P1");//Comando de Impressão, Qty;        

                //Verifica qual é a impressora padrão Selecionada
                FileHelper arquivo = new FileHelper();

                string impressora = arquivo.Ler();      
                
                if(impressora != "Empty")
                {
                    if (!RawPrinterHelper.SendStringToPrinter(impressora, sb.ToString()))
                        MessageBox.Show("Erro na impressão, favor verificar se a impressora esta configurada corretamente.");
                }
                else
                {
                    MessageBox.Show("Impressora de etiquetas padrão não foi definida, favor realizar a configuração!");
                }
            }
            else
            {
                MessageBox.Show("Digite um código para gerar um QR Code.");
            }
        }
      
        private void btnSelectPrinter_Click(object sender, EventArgs e)
        {
            SelectPrinter selectPrinter = new SelectPrinter();
            selectPrinter.Show();
        }
    }
}
