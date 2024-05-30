using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlas.Exceptions
{
    internal class ProcessExecutionException : Exception
    {
        public int ExitCode { get; }

        public ProcessExecutionException(int exitCode)
            : base($"Process exited with code: {exitCode}")
        {
            ExitCode = exitCode;
        }
    }
}
