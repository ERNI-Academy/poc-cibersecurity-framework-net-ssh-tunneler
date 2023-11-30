﻿using NetSSHTunneler.Domain.DTOs;
using NetSSHTunneler.Domain.Responses;
using NetSSHTunneler.Services.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NetSSHTunneler.Services.Interfaces
{
    public interface IChatHub
    {
        Task SendMessage(NewMessage message);
    }
}