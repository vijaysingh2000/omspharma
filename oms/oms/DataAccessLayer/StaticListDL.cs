using DevExpress.Internal;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using oms.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace oms.DataAccessLayer
{
    public static class StaticListDL
    {
        private static Dictionary<string, List<BasicModel>> staticLists = new Dictionary<string, List<BasicModel>>();
        private static List<Tasks> tasksList = new List<Tasks>();       

        public static void BuildStaticList()
        {
            XElement staticListXml = XElement.Load(ApplicationVariables.StaticListFile);

            staticLists.Clear();
            IEnumerable<XElement> listElements = staticListXml.Elements("list");
            foreach (XElement listElement in listElements)
            {
                string listName = listElement.GetAttribute("name").Trim().ToUpper();
                List<BasicModel> basicModels;

                if(!staticLists.TryGetValue(listName, out basicModels))
                {
                    basicModels = new List<BasicModel>();
                    staticLists.Add(listName, basicModels);
                }

                IEnumerable<XElement> modelElements = listElement.Elements("model");
                foreach (XElement modelElement in modelElements)
                {
                    BasicModel basicModel = new BasicModel();
                    basicModel.Id = CommonFunctions.GetIntSafely(modelElement.GetAttribute("id"));
                    basicModel.Name = modelElement.GetAttribute("name");
                    basicModel.Description = modelElement.GetAttribute("description");

                    basicModels.Add(basicModel);
                }
            }

            
            XElement tasksListXml = XElement.Load(ApplicationVariables.TaskListFile);
            tasksList.Clear();

            IEnumerable<XElement> taskElements = tasksListXml.Elements("task");
            foreach (XElement taskElement in taskElements)
            {
                Tasks tasks = new Tasks();
                tasks.Code = taskElement.GetAttribute("id");
                tasks.Name = taskElement.GetAttribute("name");
                tasks.Steps = taskElement;
                tasksList.Add(tasks);
            }
        }

        public static int GetId(string tableName, string name)
        {
            BasicModel returnItem = null;
            if (staticLists.TryGetValue(tableName.Trim().ToUpper(), out List<BasicModel> returnList))
                returnItem = returnList.FirstOrDefault(x => x.Name.IsStringEqual(name));

            if (returnItem == null)
                return 0;

            return returnItem.Id;
        }

        public static string GetName(string tableName, int id)
        {
            BasicModel returnItem = Get(tableName, id);
            return returnItem.Name;
        }

        public static BasicModel Get(string tableName, int id)
        {
            BasicModel returnItem = null;   
            if (staticLists.TryGetValue(tableName.Trim().ToUpper(), out List<BasicModel> returnList))
                returnItem = returnList.FirstOrDefault(x => x.Id == id);

            if (returnItem == null)
                returnItem = new BasicModel() { Id = id, Name = "No Value", Description = "No Value" };

            return returnItem;
        }

        public static List<BasicModel> GetActive(string tableName)
        {
            if (staticLists.TryGetValue(tableName.Trim().ToUpper(), out List<BasicModel> returnList))
                return returnList;

            return new List<BasicModel>();
        }

        public static List<Tasks> GetAllTasks()
        {
            return tasksList;
        }
        public static XElement GetDefaultStepsForTask(string taskcode)
        {
            Tasks tasks = tasksList.FirstOrDefault(x=>x.Code.IsStringEqual(taskcode));
            if (tasks != null)
                return tasks.Steps;

            return new XElement("items");
        }
    }
}
