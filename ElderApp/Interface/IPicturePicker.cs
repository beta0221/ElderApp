using System;
using System.IO;
using System.Threading.Tasks;

namespace ElderApp.Interface
{
    public interface IPicturePicker
    {
        Task<Stream> GetImageStreamAsync();
    }
}
