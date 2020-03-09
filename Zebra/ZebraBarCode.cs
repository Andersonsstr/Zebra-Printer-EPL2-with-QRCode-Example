using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.Text;
using System.Windows.Forms;

namespace Zebra
{
    public partial class Zebra : Form
    {
        public Zebra()
        {
            InitializeComponent();
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            imagem();
        }

        public void ZebraPrint()
        {
            try
            {

                string sku = txtSKU.Text;

                while (sku.Length < 12)
                {
                    sku = "0" + sku;
                }

                var sb = new StringBuilder();
                PrintDialog pd = new PrintDialog();
                pd.PrinterSettings = new System.Drawing.Printing.PrinterSettings();
                sb.AppendLine();
                sb.AppendLine("N"); // Limpar buffer de imagem

                /* O comando Q é a distância do comprimento em pontos... Para impressoras de 203dpi são 8 pontos por milímetros...
                10cm de comprimento = 800
                1cm de comprimento = 80
                */

                sb.AppendLine("q812"); //Definir largura da etiqueta. 
                sb.AppendLine("Q508"); //Definir comprimento da etiqueta 
                /*A letra A é passada sempre para inicializar um comando seguido de parâmetros, já é passado junto com a posição inicial horizontal (eixo x):
                Sintaxe: Ap1, p2, p3, p4, p5, p6, p7, "Dado", sendo representado pelos seguintes parâmetros:
                A PosiçãoHorizontalInicial, PosiçãoVerticalInicial, Rotação (Valor 0 é normal), Seletor de Fonte,  Multiplicador Horizontal (Expande o texto horizontalmente), Multiplicador Vertical (Expande o texto na vertical), Imagem Reversa (N = Valor padrão),  dado (passado como \VALOR\)*/
                sb.AppendLine(string.Format(CultureInfo.InvariantCulture,
                    "A200,80,0,5,1,1,N,\"{0}\"", "MARCA"));
                sb.AppendLine(string.Format(CultureInfo.InvariantCulture,
                    "A250,200,0,4,1,1,N,\"{0}\"", "DESCRICAO"));
                /*Letra B é passado para gerar um código de barra, deve ser informado já junto com a posição inicial horizontal (eixo x):
                B PosiçãoHorizontalInicial PosiçãoVerticalInicial, Rotação, Seletor de Código de Barras(Code128 Mode A), BarCode Width, Wide Bar Corde Width, Barcode Height, Print Human Readable (B=Yes), Data)*/
                sb.AppendLine(string.Format(CultureInfo.InvariantCulture,
                    "B250,400,0,1A,2,3,50,B,\"{0}\"", sku));
                sb.AppendLine("P1,1");//Comando de Impressão, Qty;
                if (DialogResult.OK == pd.ShowDialog(this))
                {
                    RawPrinterHelper.SendStringToPrinter(pd.PrinterSettings.PrinterName, sb.ToString());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro de Impressão: " + ex.Message);
            }
        }

        public void ZDesigner()
        {
            try
            {
                string cdBarra = txtSKU.Text;

                while (cdBarra.Length < 12)
                {
                    cdBarra = "0" + cdBarra;
                }

                var sb = new StringBuilder();
                PrintDialog pd = new PrintDialog();
                pd.PrinterSettings = new System.Drawing.Printing.PrinterSettings();
                sb.AppendLine("I8,A,001");
                sb.AppendLine();
                sb.AppendLine();
                sb.AppendLine("Q508,024");
                sb.AppendLine("q831");
                sb.AppendLine("rN");
                sb.AppendLine("S4");
                sb.AppendLine("D7");
                /*Z define a orientação de impressão, o T informa que ira imprimir a partir do topo de buffer de imagem e é o valor padrão*/
                sb.AppendLine("ZT");
                /* JF Este comando permite que o recurso Backup Top Of Form apresente a última etiqueta de uma operação 
                   de impressão em lote.Mediante solicitação para iniciar a impressão do próximo formulário(ou lote), a última 
                   etiqueta faz backup do Início do formulário antes de imprimir a próxima etiqueta.*/
                sb.AppendLine("JF");
                sb.AppendLine("OD");
                sb.AppendLine("R9,0");
                sb.AppendLine("f100");
                sb.AppendLine("N");
                /*A PosiçãoHorizontalInicial, PosiçãoVerticalInicial, Rotação (Valor 0 é normal), Seletor de Fonte, 
                 Multiplicador Horizontal(Expande o texto horizontalmente), Multiplicador Vertical(Expande o texto na vertical), 
                 Imagem Reversa(N = Valor padrão),  dado(passado como \VALOR\) */
                sb.AppendLine(string.Format(CultureInfo.InvariantCulture, "A200,50,0,5,1,1,R,\"{0}\"", "MARCA"));
                sb.AppendLine(string.Format(CultureInfo.InvariantCulture, "B200,189,0,E30,4,8,130,B,\"{0}\"", cdBarra));
                sb.AppendLine("LO60,170,692,8");
                sb.AppendLine("X203,384,8,609,478");
                sb.AppendLine("P1,1");
                if (DialogResult.OK == pd.ShowDialog(this))
                {
                    RawPrinterHelper.SendStringToPrinter(pd.PrinterSettings.PrinterName, sb.ToString());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro de Impressão: " + ex.Message);
            }
        }

        private void btnQrCode_Click(object sender, EventArgs e)
        {

            QRCoder.QRCodeGenerator qrGenerator = new QRCoder.QRCodeGenerator();
            var qrData = qrGenerator.CreateQrCode(txtSKU.Text, QRCoder.QRCodeGenerator.ECCLevel.H);
            var qrCode = new QRCoder.QRCode(qrData);
            var image = qrCode.GetGraphic(150);
            pictureBox1.Image = image;

            image = RedimensionarImagem(image, 200, 200);
            image = CropBitmap(image, 20, 20, 160, 160);

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
            
            //RawPrinterHelper.SendStringToPrinter("ZDesigner TLP 2844", sb.ToString());
        }


        public Bitmap CropBitmap(Bitmap bitmap, int cropX, int cropY, int cropWidth, int cropHeight)
        {
            Rectangle rect = new Rectangle(cropX, cropY, cropWidth, cropHeight);
            Bitmap cropped = bitmap.Clone(rect, bitmap.PixelFormat);
            return cropped;
        }

        public static Bitmap RedimensionarImagem(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }
            return destImage;
        }


        public void imagem()
        {
            try
            {
                string sku = txtSKU.Text;

                while (sku.Length < 12)
                {
                    sku = "0" + sku;
                }

                var sb = new StringBuilder();
                PrintDialog pd = new PrintDialog();
                pd.PrinterSettings = new System.Drawing.Printing.PrinterSettings();
                sb.AppendLine("I8,A,001");
                sb.AppendLine();
                sb.AppendLine();
                sb.AppendLine("Q508,024");
                sb.AppendLine("q831");
                sb.AppendLine("rN");
                sb.AppendLine("S4");
                sb.AppendLine("D7");
                sb.AppendLine("ZT");
                sb.AppendLine("JF");
                sb.AppendLine("OD");
                sb.AppendLine("R9,0");
                sb.AppendLine("f100");
                sb.AppendLine("N");
                /*Letra B é passado para gerar um código de barra, deve ser informado já junto com a posição inicial horizontal (eixo x):
                B PosiçãoHorizontalInicial PosiçãoVerticalInicial, Rotação, Seletor de Código de Barras(Code128 Mode A), BarCode Width, Wide Bar Corde Width, Barcode Height, Print Human Readable (B=Yes), Data)*/
                sb.AppendLine("GW205,20,8,63,ñÀãÀ ñÀãÀ ñÀãÀ ãÿøÇÿñãÿøÇÿñãÿøÇÿñð àqÇ qð àqÇ qð àqÇ qãøãÇ qãøãÇ qãøãÇ qñÇÇ qñÇÇ qñÇÇ qðÿÇÿñðÿÇÿñðÿÇÿñ qÿÿÀ  qÿÿÀ  qÿÿÀ þüqÿÿÿþüqÿÿÿþüqÿÿÿþ àqÀàþ àqÀàþ àqÀàð8ÿøàð8ÿøàð8ÿøàŽ àqÀÿŽ àqÀÿŽ àqÀÿ ~8ãÿÿ ~8ãÿÿ ~8ãÿÿãñÿÇqãñÿÇqãñÿÇqÿÿÿüÿÿÿÿÿÿüÿÿÿÿÿÿüÿÿÿ  qÀ   qÀ   qÀ ÿÇãÇÿñÿÇãÇÿñÿÇãÇÿñÇ qÇ qÇ qÇ qÇ qÇ qÇÿÇ qÇÿÇ qÇÿÇ qÇ Ç qÇ Ç qÇ Ç qÿÇàqÇÿñÿÇàqÇÿñÿÇàqÇÿñ  ÿÿÀ   ÿÿÀ   ÿÿÀ ");
                //sb.AppendLine("b250,200,A,\"0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ\"");
                sb.AppendLine("A250,200,0,4,1,1,N,\"0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ\"");

                sb.AppendLine("P1,1");//Comando de Impressão, Qty;
                if (DialogResult.OK == pd.ShowDialog(this))
                {
                    RawPrinterHelper.SendStringToPrinter(pd.PrinterSettings.PrinterName, sb.ToString());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro de Impressão: " + ex.Message);
            }
        }

        private void btnSelectPrinter_Click(object sender, EventArgs e)
        {
            SelectPrinter selectPrinter = new SelectPrinter();
            selectPrinter.Show();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
