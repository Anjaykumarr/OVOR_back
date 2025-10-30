using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OVOR.Repo.validations
{
    public class ErrorValidations
    {
        public static void ThrowException(string message, Exception ex = null)
        {
            if (ex != null)
                throw new Exception(message, ex);
            else
                throw new Exception(message);

        }
    }
}
