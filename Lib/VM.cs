using NLog;

namespace Lib
{
    public class VM
    {
        public static Logger log = LogManager.GetCurrentClassLogger();

        public VM()
        {
            log.Info("Start");
        }
    }
}