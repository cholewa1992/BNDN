﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ArtShare.Models;
using ShareItServices.TransferService;

namespace ArtShare.Logic
{
    public interface ITransferLogic
    {
        int UploadFile(IUploadModel model, UserDTO user, IDetailsModel details);
    }

}