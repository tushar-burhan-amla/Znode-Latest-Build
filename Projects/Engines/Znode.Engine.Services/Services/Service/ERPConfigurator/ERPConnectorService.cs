using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using Znode.Engine.Api.Models;
using Znode.Engine.ERPConnector.Model;
using Znode.Libraries.Data;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.Framework.Business;
using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;

namespace Znode.Engine.Services
{
    public class ERPConnectorService : BaseService, IERPConnectorService
    {
        #region Private Variables
        private readonly IZnodeRepository<ZnodeERPConfigurator> _eRPConfiguratorRepository;
        private readonly IERPConfiguratorService _erpConfiguratorService;
        #endregion

        #region Constructor
        public ERPConnectorService()
        {
            _eRPConfiguratorRepository = new ZnodeRepository<ZnodeERPConfigurator>();
            _erpConfiguratorService = GetService<IERPConfiguratorService>();
        }
        #endregion
        #region Public Methods
        //Get ERPConnectorControls.
        public virtual ERPConnectorControlListModel GetERPConnectorControls(ERPConfiguratorModel erpConfiguratorModel)
        => GetERPConnectorControlListModel(GetXmlConfiguration(erpConfiguratorModel));
        
        public virtual ERPConnectorControlListModel SaveERPControlData(ERPConnectorControlListModel erpConnectorControlListModel)
       => SaveControlSettingsInDatabase(erpConnectorControlListModel);

        #endregion

        #region Private Methods
        //Get ERPConnectorControlListModel from ERPConnector .config file.  
        private ERPConnectorControlListModel GetERPConnectorControlListModel(XMLConfigurationModel xmlConfigurationModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Info);
            if (xmlConfigurationModel?.Controls.Count > 0)
            {
                XMLControlListModel controlListModel = xmlConfigurationModel?.Controls as XMLControlListModel;
                ERPConnectorControlListModel erpConnectorControlListModel = new ERPConnectorControlListModel();
                ERPConnectorControlListModel jsonData = LoadSettingsFromDatabase();
                foreach (var item in controlListModel)
                {
                    XMLControlModel controlModel = item as XMLControlModel;
                    erpConnectorControlListModel.ERPConnectorControlList.Add(new ERPConnectorControlModel
                    {
                        ControlLabel = controlModel.ControlLabel,
                        ControlType = controlModel.ControlType,
                        Id = controlModel.Id,
                        Name = controlModel.Name,
                        Value = GetControlValueByName(controlModel.Name, jsonData),
                        HelpText = controlModel.HelpText,
                        SelectOptions = GetSelectOptions(controlModel.DropDownValue),
                        htmlAttributes = GetHtmlAttributes(controlModel),
                        IsHeader = !string.IsNullOrEmpty(controlModel.IsHeader) ? Convert.ToBoolean(controlModel.IsHeader) : false
                    });
                }
                ZnodeLogging.LogMessage("ERPConnectorControlList count and ERPClassName: ", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Verbose, new object[] { erpConnectorControlListModel?.ERPConnectorControlList?.Count, erpConnectorControlListModel?.ERPClassName });
                return erpConnectorControlListModel;
            }
            return new ERPConnectorControlListModel();
        }

        //Get XMLConfigurationModel from Custom section.
        private XMLConfigurationModel GetXmlConfiguration(ERPConfiguratorModel erpConfiguratorModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Info);
            ExeConfigurationFileMap configFile = new ExeConfigurationFileMap();
            configFile.ExeConfigFilename = HttpContext.Current.Server.MapPath(string.Format(Convert.ToString(ConfigurationManager.AppSettings["ERPConfigurationPath"]), erpConfiguratorModel.ClassName, "config"));
            Configuration config = ConfigurationManager.OpenMappedExeConfiguration(configFile, ConfigurationUserLevel.None);
            ZnodeLogging.LogMessage("ClassName property of ERPConfiguratorModel: ", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Verbose, erpConfiguratorModel?.ClassName);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Info);
            return config.SectionGroups["ERPCustomConfig"]?.Sections[erpConfiguratorModel.ClassName] as XMLConfigurationModel;
        }
        
        //Get dropdown list option.
        private List<SelectListItem> GetSelectOptions(XMLSelectOptions xmlSelectOptions)
        {
            List<SelectListItem> list = new List<SelectListItem>();
            if (xmlSelectOptions?.Count > 0)
            {
                foreach (var item in xmlSelectOptions)
                {
                    XMLSelectOption _xmlSelectOption = item as XMLSelectOption;
                    list.Add(new SelectListItem { Text = _xmlSelectOption.ItemText, Value = _xmlSelectOption.ItemValue });
                }
            }
            return list;
        }

        // Get html attributes dictionary.
        private Dictionary<string, object> GetHtmlAttributes(XMLControlModel xmlControlModel)
        {
            XMLValidationModel xmlValidationModel = xmlControlModel.Validations;
            Dictionary<string, object> htmlAttributes = new Dictionary<string, object>();
            if (!Equals(xmlValidationModel, null))
            {
                htmlAttributes.Add("IsRequired", xmlValidationModel.IsRequired);
                htmlAttributes.Add("RegexPattern", xmlValidationModel.RegexPattern);
                if (xmlControlModel.MaxLength != "")
                {
                    htmlAttributes.Add("MaxLength", xmlControlModel?.MaxLength);
                }

                if (xmlControlModel.OnInput != "")
                {
                    htmlAttributes.Add("OnInput", xmlControlModel?.OnInput);
                }
            }
            return htmlAttributes;
        }

        //Save erp connector data in Json format in json file.
        private ERPConnectorControlListModel SaveControlSettingsInDatabase(ERPConnectorControlListModel erpConnectorControlListModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Info);
            ERPConfiguratorModel erpConfiguratorModel = new ERPConfiguratorModel();
            erpConfiguratorModel.ClassName = _erpConfiguratorService.GetActiveERPClassName();
            ZnodeLogging.LogMessage("ClassName property of ERPConfiguratorModel: ", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Verbose, erpConfiguratorModel?.ClassName);
            int index = 0;
            XMLConfigurationModel xmlString = GetXmlConfiguration(erpConfiguratorModel);
            foreach (ERPConnectorControlModel item in erpConnectorControlListModel?.ERPConnectorControlList)
            {
                if (string.Equals(xmlString.Controls[index].Name, item.Name, StringComparison.InvariantCultureIgnoreCase))
                    item.IsHeader = !string.IsNullOrEmpty(xmlString.Controls[index].IsHeader) ? Convert.ToBoolean(xmlString.Controls[index].IsHeader) : false;
                index++;
            }

            //save control Settings data in JSON format in database
            string ERPControlSettings = JsonConvert.SerializeObject(erpConnectorControlListModel, Newtonsoft.Json.Formatting.None);
            ZnodeERPConfigurator znodeERPConfigurator = _eRPConfiguratorRepository.Table.FirstOrDefault(x => x.IsActive);
            znodeERPConfigurator.JsonSetting = ERPControlSettings;
            ZnodeLogging.LogMessage("ERPConfigurator with Id to be updated: ", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Verbose, znodeERPConfigurator?.ERPConfiguratorId);
            bool isUpdated = _eRPConfiguratorRepository.Update(znodeERPConfigurator);
            return LoadSettingsFromDatabase();
        }

        //Read json data from json file.
        private ERPConnectorControlListModel LoadSettingsFromDatabase()
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Info);
            ERPConnectorControlListModel erpConnectorControlListModel = new ERPConnectorControlListModel();
            string ERPControlSettings = _eRPConfiguratorRepository.Table.FirstOrDefault(x => x.IsActive)?.JsonSetting;
            if (!string.IsNullOrEmpty(ERPControlSettings))
            {
                erpConnectorControlListModel = JsonConvert.DeserializeObject<ERPConnectorControlListModel>(ERPControlSettings);
            }
            ZnodeLogging.LogMessage("ERPConnectorControlList count: ", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Verbose, erpConnectorControlListModel?.ERPConnectorControlList?.Count);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Info);
            return erpConnectorControlListModel;
        }

        //Get control value by control name.
        private string GetControlValueByName(string name, ERPConnectorControlListModel erpConnectorControlListModel)
         => erpConnectorControlListModel?.ERPConnectorControlList?.Where(item => item.Name.Equals(name))?.FirstOrDefault()?.Value;

        #endregion
    }
}
