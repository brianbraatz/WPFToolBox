using System;
namespace XPad
{
    interface IToolManager : IServiceProvider
    {
        CollectionBase<ITool> Tools { get; }
    }
}
