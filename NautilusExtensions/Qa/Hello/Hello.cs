using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LSEXT;
using System.Runtime.InteropServices;

namespace NautilusExtensions.Qa
{
    [Guid("8CC125FB-D584-449C-9E30-E9B52BCEA713")]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    
    public interface _Hello : LSEXT.IGenericExtension
    {
    }

    [Guid("F0B81F50-B2B1-4D24-807D-46D6963ACD1B")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("NautilusExtensions.Qa.Hello")]
    
    public class Hello : _Hello
    {
        #region IGenericExtension Members

        void IGenericExtension.Execute(ref LSExtensionParameters Parameters)
        {
            System.Windows.Forms.MessageBox.Show(Parameters["USERNAME"].ToString());
        }

        #endregion
    }
}
