using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace oms.DataAccessLayer
{
    public static class PathDL
    {
        public static string GetOrderPath(Guid orderGuid, bool createIfdoesnotExists = false)
        {
            string path = ApplicationVariables.DataFolder + @$"\orders\{orderGuid}";

            if (createIfdoesnotExists && !Directory.Exists(path))
                Directory.CreateDirectory(path);

            return path;
        }

        public static string GetBatchPath(Guid batchGuid, bool createIfdoesnotExists = false)
        {
            string path = ApplicationVariables.DataFolder + @$"\batches\{batchGuid}";

            if (createIfdoesnotExists && !Directory.Exists(path))
                Directory.CreateDirectory(path);

            return path;
        }

        public static string GetPatientPath(Guid patientGuid, bool createIfdoesnotExists = false)
        {
            string path = ApplicationVariables.DataFolder + @$"\patients\{patientGuid}";

            if (createIfdoesnotExists && !Directory.Exists(path))
                Directory.CreateDirectory(path);

            return path;
        }

        public static string GetOrderTaskPath(Guid orderGuid, string taskCode, bool createIfdoesnotExists = false)
        {
            string path = GetOrderPath(orderGuid, createIfdoesnotExists);

            string statePath = Path.Combine(path, taskCode);

            if (!Directory.Exists(statePath) && createIfdoesnotExists)
                Directory.CreateDirectory(statePath);

            return statePath;
        }
    }
}
