using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SilverConfig
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class CommentAttribute : Attribute
    {
        public string Description { get; }
        public bool InsideOfObject { get; }

        public CommentAttribute(string description, bool insideOfObject = false)
        {
            Description = description;
            InsideOfObject = insideOfObject;
        }
    }
}