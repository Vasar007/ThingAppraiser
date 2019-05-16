﻿using System;
using Microsoft.AspNetCore.Mvc;
using ThingAppraiser.Data.Configuration;
using ThingAppraiser.Data.Models;
using ThingAppraiser.Logging;
using ThingAppraiser.ConfigurationWebService.v1.Domain;

namespace ThingAppraiser.ConfigurationWebService.v1.Controllers
{
    [Route("api/v{version:apiVersion}/configuration")]
    [ApiController]
    public class ConfigurationController : ControllerBase
    {
        private static readonly LoggerAbstraction _logger =
            LoggerAbstraction.CreateLoggerInstanceFor<ConfigurationController>();

        private readonly IConfigCreator _configCreator;


        public ConfigurationController(IConfigCreator configCreator)
        {
            _configCreator = configCreator.ThrowIfNull(nameof(configCreator));
        }

        [HttpGet]
        public ActionResult<string> GetInfo()
        {
            return "Get proper configuration with POST request.";
        }

        [HttpPost]
        public ActionResult<ConfigurationXml> PostConfiguration(
            ConfigRequirements configRequirements)
        {
            try
            {
                ConfigurationXml configuration = _configCreator.CreateConfigBasedOnRequirements(
                    configRequirements
                );
                return configuration;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Exception occurred during configuration creating.");
            }
            return BadRequest(configRequirements);
        }
    }
}