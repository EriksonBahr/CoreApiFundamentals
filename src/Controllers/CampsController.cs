using AutoMapper;
using CoreCodeCamp.Data;
using CoreCodeCamp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreCodeCamp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CampsController : ControllerBase
    {
        private readonly ICampRepository campRepository;
        private readonly IMapper mapper;
        private readonly LinkGenerator linkGenerator;

        public CampsController(ICampRepository campRepository, IMapper mapper, LinkGenerator linkGenerator)
        {
            this.campRepository = campRepository;
            this.mapper = mapper;
            this.linkGenerator = linkGenerator;
        }
        [HttpGet]
        public async Task<ActionResult<CampModel[]>> GetCampsAsync(bool includeTalks = false)
        {
            try
            {
                var results = await campRepository.GetAllCampsAsync(includeTalks);
                CampModel[] model = mapper.Map<CampModel[]>(results);
                return model;
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "GetCampsAsync - " + e.Message);
            }

        }

        [HttpGet("{moniker}")]
        public async Task<ActionResult<CampModel>> GetCampByMonikerAsync(string moniker)
        {
            try
            {
                var result = await campRepository.GetCampAsync(moniker);
                if (result == null) return NotFound();
                return mapper.Map<CampModel>(result);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "GetCampByMonikerAsync - " + e.Message);
            }
        }

        [HttpGet("search")]
        public async Task<ActionResult<CampModel[]>> SearchCampsAsync(DateTime theDate, bool includeTalks = false)
        {
            try
            {
                var results = await campRepository.GetAllCampsByEventDate(theDate, includeTalks);
                if (!results.Any()) return NotFound();

                return mapper.Map<CampModel[]>(results);

            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "GetCampByMonikerAsync - " + e.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult<CampModel>> PostCamp(CampModel campModel)
        {
            try
            {
                var isDuplicatedCamp = (await campRepository.GetCampAsync(campModel.Moniker) != null);

                if (isDuplicatedCamp)
                    return BadRequest($"Moniker {campModel.Moniker} in use");

                var camp = mapper.Map<CampModel, Camp>(campModel);
                var uri = linkGenerator.GetPathByAction("GetCampByMonikerAsync",
                    "Camps",
                    new { moniker = campModel.Moniker });

                if (string.IsNullOrWhiteSpace(uri))
                    return BadRequest("Could not create a location using the provided moniker.");

                campRepository.Add(camp);
                if  (await campRepository.SaveChangesAsync())
                {
                    return Created(uri,
                        mapper.Map<Camp, CampModel>(camp));
                }

            }
            catch (Exception e)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, "PostCamp - " + e.Message);
            }

            return BadRequest();

        }

        [HttpPut("{moniker}")]
        //using action result because we're returning a body
        public async Task<ActionResult<CampModel>> PutCamp(string moniker, CampModel model)
        {

            try
            {
                var camp = await campRepository.GetCampAsync(moniker);
                if (camp == null)
                    return NotFound($"Could not put - the {moniker} does not exist");

                mapper.Map(model, camp);

                if (await campRepository.SaveChangesAsync())
                    return mapper.Map<CampModel>(camp);

            }
            catch (Exception e)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, "PutCamp - " + e.Message);
            }

            return BadRequest();


        }

        [HttpDelete("{moniker}")]
        public async Task<IActionResult> DeleteCamp(string moniker)
        {
            try
            {
                var camp = await campRepository.GetCampAsync(moniker);
                if (camp == null)
                    return NotFound();

                campRepository.Delete(camp);

                if (await campRepository.SaveChangesAsync())
                    return Ok();
            }
            catch (Exception e)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, "DeleteCamp - " + e.Message);
            }

            return BadRequest();
        }
    }
}
