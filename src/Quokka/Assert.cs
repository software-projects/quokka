using System;
using System.Collections.Generic;
using System.Text;

namespace Quokka
{
    internal static class Assert
    {
        public static void ArgumentNotNull(object param, string paramName) {
            if (param == null) {
                throw new ArgumentNullException(paramName);
            }
        }
    }
}
