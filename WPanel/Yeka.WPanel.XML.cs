using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Yeka.WPanel.XML
{
    public class XML
    {
        XmlDocument doc;

        public XML(string filename)
        {
            doc = new XmlDocument();
            doc.Load(filename);
        }

        public Nodes this[string name]
        {
            get { return new Nodes(doc.SelectNodes("//" + name)); }
        }

    }

    public class Nodes
    {
        XmlNodeList nodes;

        public Nodes(XmlNodeList xml_node_list)
        {
            nodes = xml_node_list;
        }

        public Node this[int index]
        {
            get { return new Node(nodes[index]); }
        }

        public int Count
        {
            get { return nodes.Count; }
        }
    }

    public class Node
    {
        protected XmlNode node;

        public Node(XmlNode xml_node)
        {
            node = xml_node;
        }

        public Nodes this[string name]
        {
            get { return new Nodes(node.SelectNodes(name)); }
        }

        public string Value
        {
            get { return node.Value; }
        }

        public string attr(string attribute_name)
        {
            XmlAttribute attr = node.Attributes[attribute_name];
            return attr != null ? attr.Value : "";
        }
    }
}
