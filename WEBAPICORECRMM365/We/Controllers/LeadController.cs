using Microsoft.AspNetCore.Mvc;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WEBAPICORECRM.CRMHelper;
using WEBAPICORECRM.Models;

namespace WEBAPICORECRM.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeadController : ControllerBase
    {
        private readonly CrmServiceSingleton _serviceSingleton;
        private readonly ServiceClient _serviceClient;
        public LeadController()
        {
            _serviceSingleton = CrmServiceSingleton.GetService();
            _serviceClient = _serviceSingleton.OrganizationService;
        }

        [HttpGet("getlist")]
        public IActionResult Get()
        {
            if (_serviceClient == null)
                return BadRequest();

            QueryExpression expression = new QueryExpression
            {
                EntityName = LeadEntityKeys.EntityName,
                ColumnSet = new ColumnSet(LeadEntityKeys.Id,
                LeadEntityKeys.FirtName, LeadEntityKeys.LastName, LeadEntityKeys.Job, LeadEntityKeys.Phone, LeadEntityKeys.Email)
            };

            expression.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);

            var leads = _serviceClient.RetrieveMultiple(expression).Entities;

            var model = new List<LeadModel>();

            foreach (var item in leads)
            {
                var lead = new LeadModel();

                foreach (var key in item.Attributes.Keys)
                {
                    if (key == LeadEntityKeys.FirtName)
                    {
                        lead.FirstName = item[LeadEntityKeys.FirtName].ToString();
                    }

                    if (key == LeadEntityKeys.LastName)
                    {
                        lead.LastName = item[LeadEntityKeys.LastName].ToString();
                    }

                    if (key == LeadEntityKeys.Email)
                    {
                        lead.Email = item[LeadEntityKeys.Email].ToString();
                    }

                    if (key == LeadEntityKeys.Job)
                    {
                        lead.Job = item[LeadEntityKeys.Job].ToString();
                    }

                    if (key == LeadEntityKeys.Phone)
                    {
                        lead.Phone = item[LeadEntityKeys.Phone].ToString();
                    }

                    if (key == LeadEntityKeys.Id)
                    {
                        lead.Id = new Guid(item[LeadEntityKeys.Id].ToString());
                    }
                }

                model.Add(lead);
            }

            return Ok(model);
        }

        [HttpPost("createlead")]
        public IActionResult Create([FromBody()] LeadModel leadModel)
        {

            var keys = new AttributeCollection();

            if (!string.IsNullOrEmpty(leadModel.Email))
            {
                keys.Add(new KeyValuePair<string, object>(LeadEntityKeys.Email, leadModel.Email));
            }

            if (!string.IsNullOrEmpty(leadModel.FirstName))
            {
                keys.Add(new KeyValuePair<string, object>(LeadEntityKeys.FirtName, leadModel.FirstName));
            }

            if (!string.IsNullOrEmpty(leadModel.LastName))
            {
                keys.Add(new KeyValuePair<string, object>(LeadEntityKeys.LastName, leadModel.LastName));
            }

            if (!string.IsNullOrEmpty(leadModel.Job))
            {
                keys.Add(new KeyValuePair<string, object>(LeadEntityKeys.Job, leadModel.Job));
            }

            if (!string.IsNullOrEmpty(leadModel.Phone))
            {
                keys.Add(new KeyValuePair<string, object>(LeadEntityKeys.Phone, leadModel.Phone));
            }

            Entity ent = new Entity(entityName:LeadEntityKeys.EntityName);
            ent.Attributes = keys;

            Guid crmId = Guid.Empty;

            if (!ent.Contains(LeadEntityKeys.EntityName + "id") || (Guid)ent[LeadEntityKeys.EntityName + "id"] == null || (Guid)ent[LeadEntityKeys.EntityName + "id"] == Guid.Empty)
            {
                crmId = _serviceClient.Create(ent);
            }
            else
            {
                _serviceClient.Update(ent);
                crmId = (Guid)ent[LeadEntityKeys.EntityName + "id"];
            }

            return Ok(crmId);
        }
    }
}
