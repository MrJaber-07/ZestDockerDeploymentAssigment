using Application.DTOs.Item;
using AutoMapper;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Mapping
{
    public class ItemPofile : Profile
    {
        public ItemPofile()
        {
            CreateMap<Item,ItemResponse>();
            CreateMap<ItemRequest, Item>();
        }
    }
}
