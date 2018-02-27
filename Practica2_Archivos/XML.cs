namespace Practica2_Archivos
{
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
            else if (prop == XML.Props.Tag)
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

            string xmlData = "";
            XML node = this;

            while (node != null)
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
