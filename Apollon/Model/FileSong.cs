using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.Audio;
using Windows.Storage;

namespace Apollon.Model
{
    class FileSong : ISong
    {
        private readonly IStorageFile file;

        public FileSong(IStorageFile file)
        {
            this.file = file;
        }

        public async Task<AudioFileInputNode> CreateNode(AudioGraph graph)
        {
            var result = await graph.CreateFileInputNodeAsync(file);
            if (result.Status != AudioFileNodeCreationStatus.Success)
                throw new Exception();
            return result.FileInputNode;
        }
    }
}
