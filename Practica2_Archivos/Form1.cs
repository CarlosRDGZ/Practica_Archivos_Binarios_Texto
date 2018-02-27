using System;
using System.IO;
using System.Windows.Forms;

namespace Practica2_Archivos
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            openFD.Filter = "Bitmap|*.bmp";
            saveFD.Filter = "XML|*.xml";
        }

        private void BtnSearchImg_Click(object sender, EventArgs e)
        {
            openFD.ShowDialog();
            if (openFD.FileName != "")
                txtImgInfo.Text = ImgInfo(openFD.FileName);
            else
                txtImgInfo.Text = "No image selected";
        }

        // ReturnedString: {
        //   DATA_SIZE,
        //   WIDTH,
        //   HEIGHT,
        //   BITS_PER_PIXEL
        // }
        private string ImgInfo(string file)
        {
            FileStream stream   = new FileStream(file, FileMode.Open);
            BinaryReader reader = new BinaryReader(stream);

            // Size in bytes of the file.
            // Bitmap header size = 50
            long fileSize = new FileInfo(file).Length;
            if (fileSize < 50)
                return "Not a Bitmap";

            // SIGNATURE
            string signature = new string(reader.ReadChars(2));
            // BMP_SIZE
            int bmpSize = reader.ReadInt32();

            // Unused data
            stream.Seek(8, SeekOrigin.Current);

            // INFO_SIZE
            int infoHeaderSize = reader.ReadInt32();

            if (signature != "BM" || bmpSize != fileSize || infoHeaderSize != 40)
                return "Not a Bitmap";

            // WIDTH, HEIGHT
            int width = reader.ReadInt32();
            int height = reader.ReadInt32();

            // Unused data
            stream.Seek(2, SeekOrigin.Current);

            // BITS_PER_PIXEL
            short bitsPerPixel = reader.ReadInt16();

            // Unused data
            stream.Seek(2, SeekOrigin.Current);

            // DATA_SIZE
            int imgDataSize = reader.ReadInt32();

            reader.Close();
            stream.Close();

            return "Tamaño real en bytes: " + imgDataSize + "\r\n"
                 + "Ancho: " + width + "\r\n"
                 + "Alto: " + height + "\r\n"
                 + "Número de bits por pixel: " + bitsPerPixel;
        }

        //                            BMP FILES HEADER
        // offset:  0, size: 2, desc: signature, must be 4D42 hex ("BM") | SIGNATURE (control)
        // --------------------------------------------------------------------------------
        // offset:  2, size: 4, desc: size of BMP file in bytes (unreliable) | BMP_SIZE (control)
        // --------------------------------------------------------------------------------
        // UNUSED DATA
        // offset:  6, size: 2, desc: reserved, must be zero
        // offset:  8, size: 2, desc: reserved, must be zero
        // offset: 10, size: 4, desc: offset to start of image data in bytes
        // --------------------------------------------------------------------------------
        // offset: 14, size: 4, desc: size of BITMAPINFOHEADER structure, must be 40 | INFO_SIZE (control)
        // --------------------------------------------------------------------------------
        // offset: 18, size: 4, desc: image width in pixels  | WIDTH
        // offset: 22, size: 4, desc: image height in pixels | HEIGHT
        // --------------------------------------------------------------------------------
        // offset: 26, size: 2, desc: number of planes in the image, must be 1 | UNUSED DATA
        // --------------------------------------------------------------------------------
        // offset: 28, size: 2, desc: number of bits per pixel (1, 4, 8, or 24) | BITS_PER_PIXEL
        // --------------------------------------------------------------------------------
        // offset: 30, size: 4, desc: compression type (0=none, 1=RLE-8, 2=RLE-4) | UNUSED DATA
        // --------------------------------------------------------------------------------
        // offset: 34, size: 4, desc: size of image data in bytes (including padding) | DATA_SIZE
        // --------------------------------------------------------------------------------
        // UNUSED DATA
        // offset: 38, size: 4, desc: horizontal resolution in pixels per meter (unreliable)
        // offset: 42, size: 4, desc: vertical resolution in pixels per meter (unreliable)
        // offset: 46, size: 4, desc: number of colors in image, or zero
        // offset: 50, size: 4, desc: number of important colors, or zero

        private void BtnSaveData_Click(object sender, EventArgs e)
        {
            saveFD.ShowDialog();
            bool saved = SaveXML(
                saveFD.FileName,
                txtName.Text,
                txtLastName.Text,
                txtNumber.Text,
                txtAddress.Text,
                txtEmail.Text
            );
            Console.WriteLine("File Saved: " + saved);
        }

        private bool SaveXML(string fileName, string name, string lastname, string tel, string address, string email)
        {
            if (fileName != "")
            {
                XML xml = new XML("contacto");
                xml.AddChild("nombre", name);
                xml.AddChild("apellido", lastname);
                xml.AddChild("telefono", tel);
                xml.AddChild("direccion", address);
                xml.AddChild("correo", email);

                FileStream stream = new FileStream(fileName, FileMode.Create);
                StreamWriter writer = new StreamWriter(stream);
                writer.Write(xml.ToString());
                writer.Close();
                stream.Close();
                return true;
            }
            else
                return false;
        }
        
    }

    class XML
    {
        private const string HEADER = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n";
        private XML next;
        private XML children;

        public string Tag { get; set; }
        public string Content { get; set; }
        public string Attribute { get; set; }
        public enum Props { Tag = 0, Attr, Value }

        public XML(string tag, string content = "", string attribute = "")
        {
            Tag = tag;
            Content = content;
            Attribute = attribute;
            next = null;
            children = null;
        }

        #region ADD_NODES
        public void AddNext(XML node)
        {
            if (next == null)
                next = node;
            else
            {
                XML temp = next;
                while (temp.next != null)
                    temp = temp.next;
                temp.next = node;
            }
        }

        public void AddNext(string tag, string content = "")
        {
            XML node = new XML(tag, content);
            AddNext(node);
        }

        public void AddChild(XML child)
        {
            if (children == null)
                children = child;
            else
                children.AddNext(child);
        }

        public void AddChild(string tag, string content = "")
        {
            XML child = new XML(tag, content);
            AddChild(child);
        }

        #endregion

        #region GET_NODES
        public XML GetChild(string tag)
        {
            XML child = children;
            while (child != null)
            {
                if (child.Tag == tag)
                    return child;
                child = child.next;
            }
            return null;
        }

        public XML GetChild(string value, XML.Props prop)
        {
            XML child = children;
            if (prop == XML.Props.Attr)
            {
                while (child != null)
                {
                    if (child.Attribute == value)
                        return child;
                    child = child.next;
                }    
            }
            else if (prop ==  XML.Props.Tag)
            {
                while (child != null)
                {
                    if (child.Tag == value)
                        return child;
                    child = child.next;
                }
            }
            else
            {
                while (child != null)
                {
                    if (child.Content == value)
                        return child;
                    child = child.next;
                }
            }
            return null;
        }

        public XML GetChild(string tag, string content)
        {
            XML child = children;
            while (child != null)
            {
                if (child.Tag == tag && child.Content == content)
                    return child;
                child = child.next;
            }
            return null;
        }

        public XML GetChild(string tag, string attribute, string content)
        {
            XML child = children;
            while (child != null)
            {
                if (child.Tag == tag &&
                    child.Content == content &&
                    child.Attribute == attribute)
                    return child;
                child = child.next;
            }
            return null;
        }

        #endregion

        public override string ToString()
        {
            if (this == null)
                return "Empty XML";
            else
            {
                string xmlData = "";

                XML node = this;
                while(node != null)
                {
                    xmlData += string.Format("<{0}>", node.Tag);
                    xmlData += node.Content;

                    if (node.children != null)
                        xmlData += (node.children.ToString());

                    xmlData += string.Format("</{0}>", node.Tag);
                    node = node.next;
                }
                return xmlData;
            }
        }
    }
}
