using System;

namespace Znode.Engine.Api.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public abstract class PageAttribute : Attribute
    {
        public abstract string Name { get; }
        public abstract string Description { get; }
    }
}