using NLog;

namespace Lib
{
    public class VM
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        public VM()
        {
            log.Info("Start");
        }
    }
}