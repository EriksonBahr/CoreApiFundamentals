using AutoMapper;
using CoreCodeCamp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreCodeCamp.Data
{
    public class CampProfile: Profile
    {
        public CampProfile()
        {
            CreateMap<Camp, CampModel>().
                ForMember(c => c.Venue, o => o.MapFrom(m => m.Location.VenueName)).
                ReverseMap(); // the reverse map acts just like doing CreateMap<CampModel, Camp>();

            CreateMap<Talk, TalkModel>().
                ReverseMap().
                //once this is after the reverse map, it means this rules applies to talk to talkmodel conversion (the opposit as the defined in the create map)
                ForMember(t => t.Camp, opt => opt.Ignore()).
                ForMember(t => t.Speaker, opt => opt.Ignore());

            CreateMap<Speaker, SpeakerModel>().
                ReverseMap();
        }
        
    }
}
