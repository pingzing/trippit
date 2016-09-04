using DigiTransit10.Helpers.FontLoading;
using SharpDX.DirectWrite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigiTransit10.Services
{
    public interface ICustomFontService : IAsyncInitializable
    {
        Task<List<int>> GetFontGlyphsAsync(string fontName);
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

        public async Task<List<int>> GetFontGlyphsAsync(string fontName)
        {
            await Initialization;

            if(fontName.Contains("#"))
            {
                fontName = fontName.Substring(fontName.IndexOf("#") + 1);
            }

            FontCollection collection = new FontCollection(_directWriteFactory, _fontLoader, _fontLoader.Key);
            int familyIndex = -1;
            collection.FindFamilyName(fontName, out familyIndex);

            if(familyIndex == -1)
            {
                return null;
            }

            FontFamily fontFamily = collection.GetFontFamily(familyIndex);

            List<int> glyphHexCodes = new List<int>();
            var characterHexCodes = new List<int>();
            int count = UInt16.MaxValue; //Maybe?
            Font font = fontFamily.GetFont(0);
            //todo: this function is slow as molasses, so maybe it's better if we just run it on our own computer and record the values, hardcoded somewhere.
            await Task.Run(() =>
            {
                for (int i = 0; i < count; i++)
                {
                    System.Diagnostics.Debug.WriteLine("Searching at: " + i);
                    if (font.HasCharacter(i))
                    {
                        System.Diagnostics.Debug.WriteLine("Found! At: " + i);
                        glyphHexCodes.Add(i);
                    }
                }
            });         

            return glyphHexCodes;
        }
    }
}
