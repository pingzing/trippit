using DigiTransit10.Helpers.FontLoading;
using SharpDX.DirectWrite;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigiTransit10.Services
{
    public interface ICustomFontService : IAsyncInitializable
    {
        IEnumerable<int> GetFontGlyphs(string fontName);
    }
    public class CustomFontService : ICustomFontService
    {
        private readonly IFileService _fileService;
        private readonly Factory _directWriteFactory;
        private CustomFontFileLoader _fontLoader;

        public Task Initialization { get; }

        public CustomFontService(IFileService fileService)
        {
            _fileService = fileService;
            _directWriteFactory = new Factory();
            Initialization = InitializeAsync();
        }

        private async Task InitializeAsync()
        {
            _fontLoader = new CustomFontFileLoader(_directWriteFactory, _fileService);
            await _fontLoader.Initialization;
        }

        public IEnumerable<int> GetFontGlyphs(string fontName)
        {
            Task.WaitAll(Initialization);           

            if (fontName.Contains("#"))
            {
                fontName = fontName.Substring(fontName.IndexOf("#") + 1);
            }

            FontCollection collection = new FontCollection(_directWriteFactory, _fontLoader, _fontLoader.Key);
            int familyIndex = -1;
            collection.FindFamilyName(fontName, out familyIndex);

            if (familyIndex == -1)
            {
                return null;
            }

            FontFamily fontFamily = collection.GetFontFamily(familyIndex);

            ConcurrentBag<int> glyphHexCodes = new ConcurrentBag<int>();
            var characterHexCodes = new List<int>();
            int count = UInt16.MaxValue; //Maybe?
            Font font = fontFamily.GetFont(0);
            
            //Parallel loop cuts us down from a several-minute runtime to a few seconds.
            Parallel.For(0, count + 1, (i, state) => 
            {
                if (font.HasCharacter(i))
                {
                    glyphHexCodes.Add(i);
                }
            });
            
            return glyphHexCodes;
        }
    }
}
