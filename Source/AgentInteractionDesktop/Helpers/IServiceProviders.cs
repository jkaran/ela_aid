using System.Runtime.InteropServices;
using System;
namespace Agent.Interaction.Desktop.Helpers
{
    [ComImport,
InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("6d5140c1-7436-11ce-8034-00aa006009fa")]
    internal interface IServiceProviders
    {
        #region Methods

        [return: MarshalAs(UnmanagedType.IUnknown)]
        object QueryService(ref Guid guidService, ref Guid riid);

        #endregion Methods
    }
}
