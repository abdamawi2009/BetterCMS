﻿using System;

using BetterCms.Module.MediaManager.Models;

namespace BetterCms.Module.MediaManager.ViewModels.MediaManager
{
    [Serializable]
    public class MediaAudioViewModel : MediaFileViewModel
    {
        public MediaAudioViewModel()
        {
            Type = MediaType.Audio;            
        }
    }
}