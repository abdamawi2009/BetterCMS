﻿using System;
using System.IO;

using BetterCms.Core.Exceptions.Mvc;
using BetterCms.Core.Mvc.Commands;
using BetterCms.Core.Services.Storage;

using BetterCms.Module.Blog.Content.Resources;
using BetterCms.Module.Blog.Services;
using BetterCms.Module.Blog.ViewModels.Blog;
using BetterCms.Module.Root.Mvc;

using FluentNHibernate.Utils;

namespace BetterCms.Module.Blog.Commands.UploadBlogsImportFile
{
    public class UploadBlogsImportFileCommand : CommandBase, ICommand<UploadImportFileViewModel, UploadBlogsImportFileResponse>
    {
        private readonly IBlogMLService importService;
        
        private readonly IStorageService storageService;

        public UploadBlogsImportFileCommand(IBlogMLService importService, IStorageService storageService)
        {
            this.importService = importService;
            this.storageService = storageService;
        }

        public UploadBlogsImportFileResponse Execute(UploadImportFileViewModel request)
        {
            var fileId = Guid.NewGuid();
            var fileUri = importService.ConstructFilePath(fileId);

            try
            {
                var upload = new UploadRequest();
                upload.CreateDirectory = true;
                upload.Uri = fileUri;
                upload.InputStream = request.FileStream;
                upload.IgnoreAccessControl = true;

                storageService.UploadObject(upload);
            }
            catch (Exception exc)
            {
                throw new ValidationException(() => BlogGlobalization.ImportBlogPosts_FailedToSaveFileToStorage_Message,
                    "Failed to save blog posts import file to storage.", exc);
            }

            request.FileStream.Position = 0;
            var blogs = importService.DeserializeXMLStream(request.FileStream);
            var results = importService.ValidateImport(blogs, request.UseOriginalUrls);

            return new UploadBlogsImportFileResponse { Results = results, FileId = fileId };
        }
    }
}