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
    [ApiController]
    [Route("api/camps/{moniker}/{controller}")]
    public class TalksController : ControllerBase
    {
        private readonly ICampRepository campRepository;
        private readonly IMapper mapper;
        private readonly LinkGenerator linkGenerator;

        public TalksController(ICampRepository campRepository, IMapper mapper, LinkGenerator linkGenerator)
        {
            this.campRepository = campRepository;
            this.mapper = mapper;
            this.linkGenerator = linkGenerator;
        }

        [HttpGet]
        public async Task<ActionResult<TalkModel[]>> GetTalks(string moniker)
        {
            try
            {
                var talks = await campRepository.GetTalksByMonikerAsync(moniker, true);
                if (talks == null)
                    return NotFound();

                return mapper.Map<TalkModel[]>(talks);
            }
            catch (Exception e)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, "GetTalks - " + e.ToString());
            }
        }

        //this adds to the route. becoming: api/camps/{moniker}/{controller}/id
        [HttpGet("{id:int}")]

        //                                       this ACTION requires moniker and id \/
        public async Task<ActionResult<TalkModel>> GetTalkByID(string moniker, int id)
        {
            try
            {
                var talk = await campRepository.GetTalkByMonikerAsync(moniker, id, true);

                if (talk == null)
                    return NotFound();

                return mapper.Map<TalkModel>(talk);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "GetTalkByID - " + e.ToString());
            }
        }

        public async Task<ActionResult<TalkModel>> AddTalk(string moniker, TalkModel talkModel)
        {
            try
            {
                var entMoniker = await campRepository.GetCampAsync(moniker);
                if (entMoniker == null)
                    return BadRequest("The moniker provided does not exist");

                var entSpeaker = await campRepository.GetSpeakerAsync(talkModel.speaker.SpeakerId);
                if (entSpeaker == null)
                    return BadRequest("The speaker provided does not exist");

                var talk = mapper.Map<Talk>(talkModel);
                talk.Camp = entMoniker;
                talk.Speaker = entSpeaker;
                campRepository.Add(talk);


                if (await campRepository.SaveChangesAsync())
                {
                    var Url = linkGenerator.GetPathByAction(HttpContext,
                        "GetTalkByID",
                        values: new { moniker, id = talk.TalkId });

                    return Created(Url, mapper.Map<TalkModel>(talk));
                }

                return BadRequest();
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "AddTalk - " + e.ToString());
            }
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<TalkModel>> UpdateTalk(string moniker, int id, TalkModel talk)
        {
            try
            {
                var entTalk = await campRepository.GetTalkByMonikerAsync(moniker, id);
                if (entTalk == null)
                    return NotFound();

                mapper.Map(talk, entTalk);

                if (talk.speaker != null)
                {
                    var entSpeaker = await campRepository.GetSpeakerAsync(talk.speaker.SpeakerId);
                    if (entSpeaker != null)
                        entTalk.Speaker = entSpeaker;
                }

                if (await campRepository.SaveChangesAsync())
                {
                    return mapper.Map<TalkModel>(entTalk);
                }

                return BadRequest("Failed to update talk in the db");
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "AddTalk - " + e.ToString());
            }
        }

        [HttpDelete("{id:int}")]
        //iactionresult because we're only returning reponse codes, no BODY content
        public async Task<IActionResult> DeleteTalk(string moniker, int id)
        {
            try
            {
                var entTalk = await campRepository.GetTalkByMonikerAsync(moniker, id);
                if (entTalk == null)
                    NotFound("The provided talk was not found");

                campRepository.Delete(entTalk);

                if (await campRepository.SaveChangesAsync())
                {
                    return Ok();
                }

                return BadRequest();

            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "DeleteTalk - " + e.ToString());
            }
        }
    }
}
