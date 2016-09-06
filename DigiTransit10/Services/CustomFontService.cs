using DigiTransit10.ExtensionMethods;
using DigiTransit10.Helpers;
using DigiTransit10.Helpers.FontLoading;
using Newtonsoft.Json;
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
        Task<IEnumerable<int>> GetFontGlyphsAsync(string fontName);
    }
    public class CustomFontService : ICustomFontService
    {
        private readonly IFileService _fileService;
        private readonly Factory _directWriteFactory;
        private CustomFontFileLoader _fontLoader;
        private Dictionary<string, List<int>> _fontGlyphCache = new Dictionary<string, List<int>>();

        public Task Initialization { get; }

        public CustomFontService(IFileService fileService)
        {
            _fileService = fileService;
            _directWriteFactory = new Factory();

            CacheFontGlyphs(Constants.HslPiktoFrameFontName, HslFontGlyphs.PiktoFrame);
            //CacheFontGlyphs(Constants.HslPiktoNormalFontName, HslFontGlyphs.PiktoNormal); //todo: get these values and put them in HslFontGlyphs

            Initialization = InitializeAsync();
        }

        private async Task InitializeAsync()
        {
            _fontLoader = new CustomFontFileLoader(_directWriteFactory, _fileService);
            await _fontLoader.Initialization;
        }

        private void CacheFontGlyphs(string name, IEnumerable<int> glyphs)
        {
            if(name.Contains("#"))
            {
                name = name.Substring(name.IndexOf("#") + 1);
            }

            _fontGlyphCache.AddOrUpdate(name, glyphs.ToList());            
        }

        public async Task<IEnumerable<int>> GetFontGlyphsAsync(string fontName)
        {
            if (fontName.Contains("#"))
            {
                fontName = fontName.Substring(fontName.IndexOf("#") + 1);
            }

            if(_fontGlyphCache.ContainsKey(fontName))
            {
                return _fontGlyphCache[fontName];
            }

            await Initialization;            

            FontCollection collection = new FontCollection(_directWriteFactory, _fontLoader, _fontLoader.Key);
            int familyIndex = -1;
            collection.FindFamilyName(fontName, out familyIndex);

            if (familyIndex == -1)
            {
                return null;
            }

            FontFamily fontFamily = collection.GetFontFamily(familyIndex);

            ConcurrentBag<int> glyphHexCodes = new ConcurrentBag<int>();            
            int count = UInt16.MaxValue; //Maybe? I suppose some fonts go past 65535, but for perf's sake, let's assume they don't.
            Font font = fontFamily.GetFont(0);
            
            //Parallel loop cuts us down from a several-minute runtime to a few seconds.
            Parallel.For(0, count + 1, (i, state) => 
            {
                if (font.HasCharacter(i))
                {
                    glyphHexCodes.Add(i);
                }
            });

            string lsit = JsonConvert.SerializeObject(glyphHexCodes);

            return glyphHexCodes;
        }
    }
}
