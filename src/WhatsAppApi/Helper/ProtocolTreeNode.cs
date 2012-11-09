﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WhatsAppApi.Helper
{
    public class ProtocolTreeNode
    {
        public string tag;
        public IEnumerable<KeyValue> attributeHash;
        public IEnumerable<ProtocolTreeNode> children;
        public byte[] data;

        //public ProtocolTreeNode(string tag, IEnumerable<KeyValue> attributeHash, IEnumerable<ProtocolTreeNode> children = null,
        //                    string data = "")
        public ProtocolTreeNode(string tag, IEnumerable<KeyValue> attributeHash, IEnumerable<ProtocolTreeNode> children = null,
                            byte[] data = null)
        {
            this.tag = tag ?? "";
            this.attributeHash = attributeHash ?? new KeyValue[0];
            this.children = children ?? new ProtocolTreeNode[0];
            //this.data = data ?? "";
            this.data = new byte[0];
            if (data != null)
                this.data = data;
        }

        //public ProtocolTreeNode(string tag, IEnumerable<KeyValue> attributeHash, ProtocolTreeNode children = null,
        //            string data = "")
        //{
        //    this.tag = tag ?? "";
        //    this.attributeHash = attributeHash ?? new KeyValue[0];
        //    this.children = children != null ? new ProtocolTreeNode[] { children } : new ProtocolTreeNode[0];
        //    this.data = data ?? "";
        //}
        public ProtocolTreeNode(string tag, IEnumerable<KeyValue> attributeHash, ProtocolTreeNode children = null)
        {
            this.tag = tag ?? "";
            this.attributeHash = attributeHash ?? new KeyValue[0];
            this.children = children != null ? new ProtocolTreeNode[] { children } : new ProtocolTreeNode[0];
            //this.data = data ?? "";
            this.data = new byte[0];
        }

        //public ProtocolTreeNode(string tag, IEnumerable<KeyValue> attributeHash, IEnumerable<ProtocolTreeNode> children,
        //                    byte[] data)
        //    : this(tag, attributeHash, children, Encoding.Default.GetString(data))
        //{ }

        //public ProtocolTreeNode(string tag, IEnumerable<KeyValue> attributeHash, string data = "")
        //    : this(tag, attributeHash, new ProtocolTreeNode[0], data)
        //{ }
        public ProtocolTreeNode(string tag, IEnumerable<KeyValue> attributeHash, byte[] data = null)
            : this(tag, attributeHash, new ProtocolTreeNode[0], data)
        { }

        //public ProtocolTreeNode(string tag, IEnumerable<KeyValue> attributeHash)
        //    : this(tag, attributeHash, new ProtocolTreeNode[0], "")
        //{
        //}
        public ProtocolTreeNode(string tag, IEnumerable<KeyValue> attributeHash)
            : this(tag, attributeHash, new ProtocolTreeNode[0], null)
        {
        }

        public string NodeString(string indent = "")
        {
            string ret = "\n" + indent + "<" + this.tag;
            if (this.attributeHash != null)
            {
                foreach (var item in this.attributeHash)
                {
                    ret += string.Format(" {0}=\"{1}\"", item.Key, item.Value);
                }
            }
            ret += ">";
            if (this.data.Length > 0)
            {
                //ret += this.data;
                ret += WhatsApp.SYSEncoding.GetString(this.data);
            }
            if (this.children != null && this.children.Count() > 0)
            {
                foreach (var item in this.children)
                {
                    ret += item.NodeString(indent + "  ");
                }
                ret += "\n" + indent;
            }
            ret += "</" + this.tag + ">";
            return ret;
        }

        public string GetAttribute(string attribute)
        {
            //string ret = null;
            //if (this.attributeHash.ContainsKey(attribute))
            //{
            //    ret = this.attributeHash[attribute];
            //}
            var ret = this.attributeHash.FirstOrDefault(x => x.Key.Equals(attribute));
            return (ret == null) ? null : ret.Value;
        }

        public ProtocolTreeNode GetChild(string tag)
        {
            if (this.children != null && this.children.Any())
            {
                foreach (var item in this.children)
                {
                    if (tag.Equals(item.tag, StringComparison.InvariantCultureIgnoreCase))
                    {
                        return item;
                    }
                    ProtocolTreeNode ret = item.GetChild(tag);
                    if (ret != null)
                    {
                        return ret;
                    }
                }
            }
            return null;
        }

        public IEnumerable<ProtocolTreeNode> GetAllChildren(string tag)
        {
            var tmpReturn = new List<ProtocolTreeNode>();
            if (this.children != null && this.children.Any())
            {
                foreach (var item in this.children)
                {
                    if (tag.Equals(item.tag, StringComparison.InvariantCultureIgnoreCase))
                    {
                        tmpReturn.Add(item);
                    }
                    tmpReturn.AddRange(item.GetAllChildren(tag));
                }
            }
            return tmpReturn.ToArray();
        }

        public IEnumerable<ProtocolTreeNode> GetAllChildren()
        {
            return this.children.ToArray();
        }

        //public string GetDataString()
        //{
        //    return this.data;
        //}
        public byte[] GetData()
        {
            return this.data;
        }

        public static bool TagEquals(ProtocolTreeNode node, string _string)
        {
            return (((node != null) && (node.tag != null)) && node.tag.Equals(_string, StringComparison.OrdinalIgnoreCase));
        }
    }
}
